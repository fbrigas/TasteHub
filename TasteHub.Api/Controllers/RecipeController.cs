using Microsoft.AspNetCore.Mvc;
using TasteHub.Services;
using TasteHub.Entities;
using TasteHub.Api.DTOs;

namespace TasteHub.Api.Controllers;


[ApiController]
[Route("api/[controller]")]
public class RecipesController : ControllerBase
{
    private readonly RecipeService _recipeService;

    public RecipesController(RecipeService recipeService)
    {
        _recipeService = recipeService;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var recipes = _recipeService.GetAllRecipes();

        return Ok(recipes);
    }
    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var recipe = _recipeService.GetRecipeById(id);

        if (recipe == null)
        {
            return NotFound();
        }

        return Ok(recipe);
    }
    [HttpPost]
    public IActionResult Create(CreateRecipeDto dto)
    {
        Recipe recipe = new Recipe
        {
            Title = dto.Title,
            Description = dto.Description,
            Instructions = dto.Instructions,
            DurationInMinutes = dto.DurationInMinutes,
            Difficulty = dto.Difficulty,
            Category = dto.Category,
            UserId = dto.UserId,
            Status = RecipeStatus.Draft
        };

        _recipeService.AddRecipe(recipe);

        return CreatedAtAction(
            nameof(GetById),
            new { id = recipe.Id },
            recipe
        );
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, UpdateRecipeDto dto)
    {
        Recipe updatedRecipe = new Recipe
        {
            Title = dto.Title,
            Description = dto.Description,
            Instructions = dto.Instructions,
            DurationInMinutes = dto.DurationInMinutes,
            Difficulty = dto.Difficulty,
            Category = dto.Category
        };

        try
        {
            _recipeService.UpdateRecipe(id, updatedRecipe);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        try
        {
            _recipeService.DeleteRecipe(id);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }

        return NoContent();
    }
}