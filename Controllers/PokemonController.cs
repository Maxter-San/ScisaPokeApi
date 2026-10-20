using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ScisaPokeApi.Models;
using ScisaPokeApi.Services;

namespace ScisaPokeApi.Controllers;

public class PokemonController : Controller
{
    private readonly ILogger<PokemonController> _logger;

    private readonly PokeApiService _pokeApiService;

    public PokemonController(ILogger<PokemonController> logger, PokeApiService pokeApiService)
    {
        _logger = logger;
        _pokeApiService = pokeApiService;
    }

    public async Task<IActionResult> Index(string? searchTerm, string? selectedSpecie, int? page)
    {
        int pageSize = 10;
        int pageNumber = page ?? 1;

        var speciesOptions = await _pokeApiService.GetSpeciesOptionsAsync();

        var pagedResult = await _pokeApiService.GetPagedPokemonsAsync(searchTerm, selectedSpecie, pageNumber, pageSize);

        var viewModel = new PokemonIndexViewModel
        {
            SearchTerm = searchTerm ?? string.Empty,
            SelectedSpecie = selectedSpecie ?? string.Empty,
            AvailableSpecies = speciesOptions,
            Pokemons = pagedResult.Items,
            PageSize = pageSize,
            PageNumber = pageNumber,
            TotalPages = pagedResult.TotalPages
        };

        return View(viewModel);
    }

    public IActionResult Info()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
