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

    [HttpGet]
    public async Task<IActionResult> GetDetail(int id)
    {
        try
        {
            var pokemonDetail = await _pokeApiService.GetPokemonDetailAsync(id);

            if (pokemonDetail == null || pokemonDetail.Id == 0)
            {
                return NotFound(new { success = false, message = "Pokémon no encontrado." });
            }

            return Json(pokemonDetail);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = "Ocurrió un error al consultar el detalle." });
        }
    }

    [HttpGet]
    public async Task<IActionResult> ExportToExcel(string? searchTerm, string? selectedSpecie)
    {
        try
        {
            var result = await _pokeApiService.GetPokemonsAsync(searchTerm, selectedSpecie);

            if (result == null || !result.Any())
            {
                TempData["ErrorMessage"] = "No se encontraron Pokémons para exportar a excel.";
                return RedirectToAction(nameof(Index), new { searchTerm, selectedSpecie });
            }

            byte[] excelBytes = _pokeApiService.ExportPokemonsToExcel(result);

            string fileName = $"Pokemons_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            return File(excelBytes, contentType, fileName);
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "Ocurrió un problema al generar el archivo Excel. Inténtalo nuevamente.";
            return RedirectToAction(nameof(Index), new { searchTerm, selectedSpecie });
        }
    }
}
