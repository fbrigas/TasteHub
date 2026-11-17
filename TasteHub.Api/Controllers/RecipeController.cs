
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
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

    // GET: api/recipes
    [HttpGet]
    public IActionResult GetAll()
    {
        var recipes = User.IsInRole("Admin")
            ? _recipeService.GetAllRecipes()
            : _recipeService.GetApprovedRecipes();

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

    // GET: api/recipes/my
    [Authorize]
    [HttpGet("my")]
    public IActionResult GetMyRecipes()
    {
        string? userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized();
        }

        List<Recipe> recipes = _recipeService.GetRecipesByUserId(userId);

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

    // GET: api/recipes/{id}
    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        Recipe? recipe = _recipeService.GetRecipeById(id);

        if (recipe == null)
        {
            return NotFound();
        }

        bool isApproved = recipe.Status == RecipeStatus.Approved;
        bool isAdmin = User.IsInRole("Admin");

        string? userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        bool isAuthor = int.TryParse(userIdClaim, out int userId)
            && recipe.UserId == userId;

        if (!isApproved && !isAdmin && !isAuthor)
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

    // POST: api/recipes
    [Authorize]
    [HttpPost]
    public IActionResult Create(CreateRecipeDto dto)
    {
        string? userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized(new
            {
                message = "Não foi possível identificar o utilizador."
            });
        }

        Recipe recipe = new Recipe
        {
            Title = dto.Title,
            Description = dto.Description,
            Instructions = dto.Instructions,
            DurationInMinutes = dto.DurationInMinutes,
            Difficulty = dto.Difficulty,
            Category = dto.Category,
            UserId = userId,
            Status = RecipeStatus.Draft
        };

        try
        {
            _recipeService.AddRecipe(recipe);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }

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

    // POST: api/recipes/{id}/submit
    [Authorize]
    [HttpPost("{id}/submit")]
    public IActionResult Submit(int id)
    {
        Recipe? recipe = _recipeService.GetRecipeById(id);

        if (recipe == null)
        {
            return NotFound("Receita não encontrada.");
        }

        string? userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized();
        }

        if (recipe.UserId != userId)
        {
            return Forbid();
        }

        try
        {
            _recipeService.SubmitRecipe(id);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }

        return NoContent();
    }

    // PUT: api/recipes/{id}
    [Authorize]
    [HttpPut("{id}")]
    public IActionResult Update(int id, UpdateRecipeDto dto)
    {
        Recipe? recipe = _recipeService.GetRecipeById(id);

        if (recipe == null)
        {
            return NotFound();
        }

        string? userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized();
        }

        bool isAuthor = recipe.UserId == userId;
        bool isAdmin = User.IsInRole("Admin");

        if (!isAuthor && !isAdmin)
        {
            return Forbid();
        }

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
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }

        return NoContent();
    }

    // DELETE: api/recipes/{id}
    [Authorize]
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        Recipe? recipe = _recipeService.GetRecipeById(id);

        if (recipe == null)
        {
            return NotFound();
        }

        string? userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized();
        }

        bool isAuthor = recipe.UserId == userId;
        bool isAdmin = User.IsInRole("Admin");

        if (!isAuthor && !isAdmin)
        {
            return Forbid();
        }

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

    // POST: api/recipes/{id}/approve
    [Authorize(Roles = "Admin")]
    [HttpPost("{id}/approve")]
    public IActionResult Approve(int id)
    {
        try
        {
            _recipeService.ApproveRecipe(id);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }

        return NoContent();
    }

    // POST: api/recipes/{id}/reject
    [Authorize(Roles = "Admin")]
    [HttpPost("{id}/reject")]
    public IActionResult Reject(int id)
    {
        try
        {
            _recipeService.RejectRecipe(id);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }

        return NoContent();
    }
}
