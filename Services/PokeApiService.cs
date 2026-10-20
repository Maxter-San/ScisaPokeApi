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

    public async Task<List<SelectOptionDto>> GetSpeciesOptionsAsync(int limit = 50)
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
}

public class PokeApiSpeciesListResult
{
    [JsonPropertyName("results")]
    public List<PokeApiSpeciesResource> Results { get; set; } = new();
}

public class PokeApiSpeciesResource
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;
}