using System.Text.Json;
using System.Text.Json.Serialization;
using ScisaPokeApi.Models;

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