using System;
using GameStore.Api.Data;
using GameStore.Api.Dtos;
using GameStore.Api.Entities;
using GameStore.Api.Mapping;
using GameStore.Api.Helpers;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Endpoints;

public static class GamesEndpoints
{
    const string GetGameEndpointName = "GetGame";

    public static RouteGroupBuilder MapGamesEndpoints(this WebApplication app)
    {

        var group = app.MapGroup("games")
                       .WithParameterValidation();

        // GET /games
        group.MapGet("/", async (GameStoreContext dbContext) =>
          await dbContext.Games
                   .Include(game => game.Genre)
                   .Select(game => game.ToGameSummaryDto())
                   .AsNoTracking()
                   .ToListAsync()
        );

        // GET /games/1
        group.MapGet("/{id}", async (int id, GameStoreContext dbContext) =>
        {
            Game? game = await dbContext.Games.FindAsync(id);

            return game is null ? Results.NotFound() : Results.Ok(game.ToGameDetailsDto());
        })
        .WithName(GetGameEndpointName);

        /*Make GET where show games by release date from latest to earliest and depending on query parameters earliest to latest.
        Depending on query parameters do this for, specific genres, and groups of genres of game, but defaults to all genres
        */
        /*Make GET where show price of games from greatest to least and depending on query parameters least to greatest.
        Depending on query parameters do this for, specific genres, and groups of genres of game, but defaults to all genres
        */

        /*See if you can combine them into one method*/

        group.MapGet("/orderby", async (HttpContext context, GameStoreContext dbContext) =>
{
    var queryParams = Helper.GetAllQueryParameters(context);
    return await Helper.GetGames(dbContext, queryParams).ToListAsync();

});



        // POST /games
        group.MapPost("/", async (CreateGameDto newGame, GameStoreContext dbContext) =>
        {


            Game game = newGame.ToEntity();

            dbContext.Games.Add(game);
            await dbContext.SaveChangesAsync();

            return Results.CreatedAtRoute(GetGameEndpointName, new { id = game.Id }, game.ToGameDetailsDto());
        });

        // PUT /games/1
        group.MapPut("/{id}", async (int id, UpdateGameDto updateGame, GameStoreContext dbContext) =>
        {
            var existingGame = await dbContext.Games.FindAsync(id);

            if (existingGame is null)
            {
                return Results.NotFound();
            }

            dbContext.Entry(existingGame).CurrentValues.SetValues(updateGame.ToEntity(id));

            await dbContext.SaveChangesAsync();

            return Results.NoContent();
        });

        // DELETE /games/1
        group.MapDelete("/{id}", async (int id, GameStoreContext dbContext) =>
        {
            await dbContext.Games
                 .Where(game => game.Id == id)
                 .ExecuteDeleteAsync();

            return Results.NoContent();
        });

        return group;
    }

}