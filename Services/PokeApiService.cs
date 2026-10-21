using System.Text.Json;
using System.Text.Json.Serialization;
using ScisaPokeApi.Models;
using ClosedXML.Excel;

namespace ScisaPokeApi.Services;

public class PokeApiService
{
    private readonly HttpClient _httpClient;

    public PokeApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri("https://pokeapi.co/api/v2/");
    }

    public async Task<List<SelectOptionDto>> GetSpeciesOptionsAsync(int limit = 151)
    {
        var response = await _httpClient.GetFromJsonAsync<PokeApiSpeciesListResult>($"pokemon-species?limit={limit}");
        var options = new List<SelectOptionDto>();

        if (response?.Results != null)
        {
            foreach (var item in response.Results)
            {
                options.Add(new SelectOptionDto
                {
                    Name = item.Name,
                    DisplayName = item.Name
                });
            }
        }

        return options;
    }

    public async Task<List<PokemonDto>> GetPokemonsAsync(string? searchTerm, string? selectedSpecie, int limit = 151)
    {
        var response = await _httpClient.GetFromJsonAsync<PokeApiPokemonsListResult>($"pokemon?limit={limit}");
        var pokeList = new List<PokemonDto>();

        if (response?.Results != null)
        {
            foreach (var item in response.Results)
            {
                // Sacando el ID del pokemon desde la URL
                var segments = item.Url.TrimEnd('/').Split('/');
                int.TryParse(segments.Last(), out int pokemonId);

                pokeList.Add(new PokemonDto
                {
                    Id = pokemonId,
                    Name = item.Name,
                    ImageUrl = $"https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/versions/generation-iv/heartgold-soulsilver/{pokemonId}.png"
                });

                //https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/{pokemonId}.png
                //https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/versions/generation-iv/heartgold-soulsilver/{pokemonId}.png
            }
        }

        var query = pokeList.AsEnumerable();

        // Filtro por nombre Y especie
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(p => p.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(selectedSpecie) && selectedSpecie != "0")
        {
            query = query.Where(p => p.Name.Equals(selectedSpecie, StringComparison.OrdinalIgnoreCase));
        }

        return query.ToList();
    }

    public async Task<Paginacion<PokemonDto>> GetPagedPokemonsAsync(string? searchTerm, string? selectedSpecie, int pageNumber, int pageSize)
    {
        var allPokemons = await GetPokemonsAsync(searchTerm, selectedSpecie);

        int totalItems = allPokemons.Count;

        var pagedItems = allPokemons
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new Paginacion<PokemonDto>
        {
            Items = pagedItems,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalItems = totalItems
        };
    }

    public async Task<PokemonDetailDto> GetPokemonDetailAsync(int id)
    {
        var response = await _httpClient.GetFromJsonAsync<PokeApiPokemonDetailResource>($"pokemon/{id}");
        var pokeDetail = new PokemonDetailDto();

        if (response?.Id != 0)
        {
            pokeDetail = new PokemonDetailDto
            {
                Id = response.Id,
                Name = response.Name,
                ImageUrl = $"https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/versions/generation-iv/heartgold-soulsilver/{response.Id}.png",
                Height = response.Height / 10.0,
                Weight = response.Weight / 10.0,
                Types = response.Types.Select(t => t.Type.Name).ToArray()
            };
        }

        return pokeDetail;
    }

    public byte[] ExportPokemonsToExcel(List<PokemonDto> pokemons)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Pokemons Visibles");

        worksheet.Cell(1, 1).Value = "# Pokédex";
        worksheet.Cell(1, 2).Value = "Nombre";
        worksheet.Cell(1, 3).Value = "URL Imagen";

        var headerRow = worksheet.Row(1);
        headerRow.Style.Font.Bold = true;

        int row = 2;
        foreach (var pokemon in pokemons)
        {
            worksheet.Cell(row, 1).Value = pokemon.Id;
            worksheet.Cell(row, 2).Value = char.ToUpper(pokemon.Name[0]) + pokemon.Name.Substring(1);
            worksheet.Cell(row, 3).Value = pokemon.ImageUrl;
            row++;
        }

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}

public class PokeApiSpeciesListResult
{
    [JsonPropertyName("results")]
    public List<PokeApiSpecieResource> Results { get; set; } = new();
}

public class PokeApiSpecieResource
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;
}

public class PokeApiPokemonsListResult
{
    [JsonPropertyName("results")]
    public List<PokeApiPokemonResource> Results { get; set; } = new();
}

public class PokeApiPokemonResource
{
    [JsonPropertyName("id")]
    public int Id { get; set; } = 0;
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;
}

public class PokeApiPokemonDetailResource
{
    [JsonPropertyName("id")]
    public int Id { get; set; } = 0;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("weight")]
    public int Weight { get; set; }

    [JsonPropertyName("height")]
    public int Height { get; set; }

    [JsonPropertyName("types")]
    public List<PokemonTypeEntryResource> Types { get; set; } = new();
}

public class PokemonTypeEntryResource
{
    [JsonPropertyName("slot")]
    public int Slot { get; set; }

    [JsonPropertyName("type")]
    public PokeApiSpecieResource Type { get; set; } = new(); //reutilizando formato
}