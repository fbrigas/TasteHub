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
        List<RecipeResponseDto> dtos = new List<RecipeResponseDto>();
        foreach (Recipe recipe in recipes)
        {
            RecipeResponseDto dto = new RecipeResponseDto
            {
                Id = recipe.Id,
                Title = recipe.Title,
                Description = recipe.Description,
                Instructions = recipe.Instructions,
                DurationInMinutes = recipe.DurationInMinutes,
                Difficulty = recipe.Difficulty,
                Category = recipe.Category,
                Status = recipe.Status,
                UserId = recipe.UserId
            };

            dtos.Add(dto);
        }

        return Ok(dtos);
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var recipe = _recipeService.GetRecipeById(id);

        if (recipe == null)
        {
            return NotFound();
        }

        RecipeResponseDto dto = new RecipeResponseDto
        {
            Id = recipe.Id,
            Title = recipe.Title,
            Description = recipe.Description,
            Instructions = recipe.Instructions,
            DurationInMinutes = recipe.DurationInMinutes,
            Difficulty = recipe.Difficulty,
            Category = recipe.Category,
            Status = recipe.Status,
            UserId = recipe.UserId
        };

        return Ok(dto);
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

        RecipeResponseDto responseDto = new RecipeResponseDto
        {
            Id = recipe.Id,
            Title = recipe.Title,
            Description = recipe.Description,
            Instructions = recipe.Instructions,
            DurationInMinutes = recipe.DurationInMinutes,
            Difficulty = recipe.Difficulty,
            Category = recipe.Category,
            Status = recipe.Status,
            UserId = recipe.UserId
        };

        return CreatedAtAction(
            nameof(GetById),
            new { id = recipe.Id },
            responseDto
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