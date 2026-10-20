namespace ScisaPokeApi.Models;

public class PokemonIndexViewModel
{
    public List<PokemonDto> Pokemons { get; set; } = new();
    public List<SelectOptionDto> AvailableSpecies { get; set; } = new();

    //Filtro
    public string SearchTerm { get; set; } = string.Empty;
    public string SelectedSpecie { get; set; } = string.Empty;

    //Paginación
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int TotalPages { get; set; }
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
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