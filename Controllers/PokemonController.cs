using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ScisaPokeApi.Models;
using ScisaPokeApi.Services;

namespace ScisaPokeApi.Controllers;

public class PokemonController : Controller
{
    private readonly ILogger<PokemonController> _logger;

    private readonly PokeApiService _pokeApiService;
    private readonly IEmailService _emailService;

    public PokemonController(ILogger<PokemonController> logger, PokeApiService pokeApiService, IEmailService emailService)
    {
        _logger = logger;
        _pokeApiService = pokeApiService;
        _emailService = emailService;
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

    [HttpPost]
    public async Task<IActionResult> EnviarCorreo(string destinatario, string tipoEnvio, int? pokemonId, string? searchTerm, string? selectedSpecie)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(destinatario))
            {
                TempData["ErrorMessage"] = "Debe proporcionar un correo electrónico válido.";
                return RedirectToAction(nameof(Index), new { searchTerm, selectedSpecie });
            }

            string asunto = "";
            string cuerpoHtml = "";

            if (tipoEnvio == "individual" && pokemonId.HasValue)
            {
                var pokemon = await _pokeApiService.GetPokemonDetailAsync(pokemonId.Value);

                if (pokemon == null || pokemon.Id == 0)
                {
                    TempData["ErrorMessage"] = "No se encontró el Pokémon seleccionado.";
                    return RedirectToAction(nameof(Index), new { searchTerm, selectedSpecie });
                }

                asunto = $"Detalles del Pokémon {pokemon.Name}";

                cuerpoHtml = $"""
                    <h2>Información</h2>
                    <p><strong>Nombre:</strong> {pokemon.Name}</p>
                    <p><strong># Pokédex:</strong> {pokemon.Id}</p>
                    <p><strong>Altura:</strong> {pokemon.Height} m</p>
                    <p><strong>Peso:</strong> {pokemon.Weight} kg</p>
                    <p><strong>Tipos:</strong> {string.Join(", ", pokemon.Types)}</p>
                    <img src="{pokemon.ImageUrl}" alt="{pokemon.Name}" width="150" />
                """;
            }
            else
            {
                var pokemons = await _pokeApiService.GetPokemonsAsync(searchTerm, selectedSpecie);

                if (pokemons.Count == 0)
                {
                    TempData["ErrorMessage"] = "No se encontraron Pokémons.";
                    return RedirectToAction(nameof(Index), new { searchTerm, selectedSpecie });
                }

                asunto = "Catálogo de Pokémons";
                cuerpoHtml = "<h2>Listado de Pokémon</h2><ul>";
                foreach (var p in pokemons)
                {
                    cuerpoHtml += $"<li>#{p.Id} - {p.Name}</li>";
                }
                cuerpoHtml += "</ul>";
            }

            await _emailService.SendEmailAsync(destinatario, asunto, cuerpoHtml);

            TempData["SuccessMessage"] = "¡Correo enviado exitosamente!";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al enviar el correo electrónico a {Destinatario}", destinatario);
            TempData["ErrorMessage"] = "Ocurrió un error al intentar enviar el correo. Inténtalo más tarde.";
        }

        return RedirectToAction(nameof(Index), new { searchTerm, selectedSpecie });
    }
}
