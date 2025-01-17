using GameStore.Api.Data;
using GameStore.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Helpers;

public class Helper
{

    public static Dictionary<string, string> GetAllQueryParameters(HttpContext context)
    {
        var queryParams = new Dictionary<string, string>();
        foreach (var key in context.Request.Query.Keys)
        {

            queryParams[key] = context.Request.Query[key];
        }
        return queryParams;
    }


    public static IQueryable<Game> GetGames(GameStoreContext dbContext, Dictionary<string, string> queryParams)
    {
        var query = dbContext.Games.Include(g => g.Genre).AsNoTracking().AsQueryable();
        var ascending = ExtractAscending(queryParams);
        //var releaseDateBool = ExtractReleaseDateBool(queryParams);
        var priceBool = ExtractPriceBool(queryParams);
        var genreIds = ExtractGenreIds(queryParams);

        if (priceBool)
        {
            return GetGamesByPrice(dbContext, ascending, genreIds);
        }
        else
        {
            return GetGamesByReleaseDate(dbContext, ascending, genreIds);
        }


    }

    public static IQueryable<Game> GetGamesByReleaseDate(GameStoreContext dbContext, bool ascending = false, List<int> genreIds = null)
    {
        var query = dbContext.Games.Include(g => g.Genre).AsNoTracking().AsQueryable();
        if (genreIds != null && genreIds.Any())
        {
            query = query.Where(g => genreIds.Contains(g.GenreId));
        }
        if (ascending)
        {
            query = query.OrderBy(g => g.ReleaseDate);
        }
        else
        {
            query = query.OrderByDescending(g => g.ReleaseDate);
        }
        return query;
    }

    public static IQueryable<Game> GetGamesByPrice(GameStoreContext dbContext, bool ascending = false, List<int> genreIds = null)
    {
        var query = dbContext.Games.Include(g => g.Genre).AsNoTracking().AsQueryable();
        var sqlQuery = query.ToQueryString();
        if (genreIds != null && genreIds.Any())
        {
            query = query.Where(g => genreIds.Contains(g.GenreId));
        }
        if (ascending)
        {
            query = query.OrderBy(g => (double)g.Price);
        }
        else
        {
            query = query.OrderByDescending(g => (double)g.Price);
        }
        return query;
    }

    private static bool ExtractAscending(Dictionary<string, string> queryParams)
    {
        if (queryParams.TryGetValue("ascending", out var ascendingValue) && bool.TryParse(ascendingValue, out var ascending))
        {
            return ascending;
        }
        return false;
    }
    /*private static bool ExtractReleaseDateBool(Dictionary<string, string> queryParams)
    {
        if (queryParams.TryGetValue("releaseDateBool", out var releaseDateBoolValue) && bool.TryParse(releaseDateBoolValue, out var releaseDateBool))
        {
            return releaseDateBool;
        }
        return false;
    } */
    private static bool ExtractPriceBool(Dictionary<string, string> queryParams)
    {
        if (queryParams.TryGetValue("priceBool", out var priceBoolValue) && bool.TryParse(priceBoolValue, out var priceBool))
        {
            return priceBool;
        }
        return false; // Default value
    }

    private static List<int> ExtractGenreIds(Dictionary<string, string> queryParams)
    {
        if (queryParams.TryGetValue("genreIds", out var genreIdsValue))
        {
            return genreIdsValue.Split(',').Select(int.Parse).ToList();
        }

        return new List<int>(); // Default value
    }

}

