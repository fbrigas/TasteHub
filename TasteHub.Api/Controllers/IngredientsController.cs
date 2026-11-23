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

    [Authorize(Roles = "Admin")]
    [HttpGet("pending")]
    public IActionResult GetPending()
    {
        List<Ingredient> ingredients = _ingredientService.GetPendingIngredients();

        return Ok(ingredients.Select(i => new
        {
            i.Id,
            i.Name,
            i.Status
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

    [Authorize(Roles = "Admin")]
    [HttpPost("{id}/approve")]
    public IActionResult Approve(int id)
    {
        try
        {
            _ingredientService.ApproveIngredient(id);

            return Ok(new
            {
                message = "Ingrediente aprovado com sucesso."
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("{id}/reject")]
    public IActionResult Reject(int id)
    {
        try
        {
            _ingredientService.RejectIngredient(id);

            return Ok(new
            {
                message = "Ingrediente rejeitado com sucesso."
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

}