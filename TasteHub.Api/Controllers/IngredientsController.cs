using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TasteHub.Services;
using TasteHub.Entities;
using TasteHub.Api.DTOs;

namespace TasteHub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IngredientsController : ControllerBase
{
    private readonly IngredientService _ingredientService;

    public IngredientsController(IngredientService ingredientService)
    {
        _ingredientService = ingredientService;
    }

    // GET: api/ingredients
    [HttpGet]
    public IActionResult GetAll()
    {
        List<Ingredient> ingredients =
            _ingredientService.GetApprovedIngredients();

        return Ok(ingredients.Select(i => new
        {
            i.Id,
            i.Name
        }));
    }

    // POST: api/ingredients
    [Authorize]
    [HttpPost]
    public IActionResult Create(CreateIngredientDto dto)
    {
        Ingredient ingredient = new Ingredient
        {
            Name = dto.Name
        };

        try
        {
            _ingredientService.AddIngredient(ingredient);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }

        return StatusCode(201, new
        {
            ingredient.Id,
            ingredient.Name,
            ingredient.Status
        });
    }
}