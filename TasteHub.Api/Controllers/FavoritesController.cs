
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TasteHub.Services;

namespace TasteHub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FavoritesController : ControllerBase
{
    private readonly FavoriteService _favoriteService;

    public FavoritesController(FavoriteService favoriteService)
    {
        _favoriteService = favoriteService;
    }

    private int? GetCurrentUserId()
    {
        string? claim = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        return int.TryParse(claim, out int userId)
            ? userId
            : null;
    }

    // GET: api/favorites
    [HttpGet]
    public IActionResult GetMyFavorites()
    {
        int? userId = GetCurrentUserId();

        if (userId == null)
            return Unauthorized();

        var recipes = _favoriteService.GetUserFavorites(
            userId.Value);

        var result = recipes.Select(recipe => new
        {
            recipe.Id,
            recipe.Title,
            recipe.Description,
            recipe.Instructions,
            recipe.DurationInMinutes,
            recipe.Difficulty,
            recipe.Category,
            recipe.Status,
            recipe.UserId,

            Ingredients = recipe.Ingredients.Select(ri => new
            {
                ri.IngredientId,
                Name = ri.Ingredient.Name,
                ri.Quantity,
                ri.Unit
            }).ToList()
        }).ToList();

        return Ok(result);
    }

    // GET: api/favorites/{recipeId}
    [HttpGet("{recipeId}")]
    public IActionResult CheckFavorite(int recipeId)
    {
        int? userId = GetCurrentUserId();

        if (userId == null)
            return Unauthorized();

        bool isFavorite = _favoriteService.IsFavorite(
            userId.Value, recipeId);

        return Ok(new { isFavorite });
    }

    // POST: api/favorites/{recipeId}
    [HttpPost("{recipeId}")]
    public IActionResult AddFavorite(int recipeId)
    {
        int? userId = GetCurrentUserId();

        if (userId == null)
            return Unauthorized();

        try
        {
            _favoriteService.AddFavorite(
                userId.Value, recipeId);

            return NoContent();
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

    // DELETE: api/favorites/{recipeId}
    [HttpDelete("{recipeId}")]
    public IActionResult RemoveFavorite(int recipeId)
    {
        int? userId = GetCurrentUserId();

        if (userId == null)
            return Unauthorized();

        try
        {
            _favoriteService.RemoveFavorite(
                userId.Value, recipeId);

            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
