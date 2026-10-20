namespace ScisaPokeApi.Models;

public class PokemonIndexViewModel
{
    public string SearchTerm { get; set; } = string.Empty;
    public string SelectedSpecie { get; set; } = string.Empty;
    public List<SelectOptionDto> AvailableSpecies { get; set; } = new();
    public List<PokemonDto> Pokemons { get; set; } = new();
}

public class PokemonDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
}

public class SelectOptionDto
{
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
}