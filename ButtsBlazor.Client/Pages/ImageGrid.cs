using ButtsBlazor.Api.Model;
using ButtsBlazor.Client.Components;

namespace ButtsBlazor.Client.Pages;

public struct GridSquare
{
    public WebPath? Entity { get; set; }
    public bool Full => Entity != null || Item.HasValue;
    public GridItemSize? Item { get; set; }

    public void Clear()
    {
        Entity = null;
        Item = null;
    }
}

public class ImageGrid(Random random, int columns, int rows)
{
    public int Columns { get; } = columns;
    public int Rows { get; } = rows;
    private GridSquare[,] grid = new GridSquare[columns, rows];

    public void Clear()
    {
        for (int i = 0; i < Columns; i++)
        for (var j = 0; j < Rows; j++)
            grid[i, j].Clear();
    }

    public GridSquare[] Place(WebPath[] entities)
    {
        Clear();
        var extra = (Rows * Columns) - entities.Length;
        int x = 0;
        return (extra switch
        {
            6 => PlaceImages(entities, 1, 3, 4),
            _ => throw new NotImplementedException()
        }).ToArray();
    }



    private IEnumerable<GridSquare> PlaceImages(WebPath[] entities, int numLarge, int numMedium, int rowSize)
    {
        Clear();
        var hashSet = new List<WebPath>(entities);
        var horizontal = random.Next(0, numMedium + 1);
        var vertical = numMedium - horizontal;
        while (numLarge > 0)
        {
            PlaceNext(hashSet, GridItemSize.TwoByTwo);
            numLarge--;
        }
        while (horizontal > 0)
        {
            PlaceNext(hashSet, GridItemSize.OneByTwo);
            horizontal--;
        }
        while (vertical > 0)
        {
            PlaceNext(hashSet, GridItemSize.TwoByOne);
            vertical--;
        }
        while (hashSet.Count > 0)
        {
            PlaceNext(hashSet, GridItemSize.OneByOne);
        }

        for (var j = 0; j < Rows; j++)
        for (var i = 0; i < Columns; i++)
        {
            if (grid[i, j].Entity != null)
            {
                Console.WriteLine($"grid[]: {grid[i, j].Item} {i},{j}");
                yield return grid[i, j];
            }
        }
    }

    private void PlaceNext(List<WebPath> hashSet, GridItemSize size)
    {
        var img = hashSet[random.Next(hashSet.Count)];
        hashSet.Remove(img);
        Place(img, size);
    }


    private void Place(WebPath entity, GridItemSize size)
    {
        var locations = new List<(int x, int y)>();
        for (int i = 0; i < Columns; i++)
        {
            for (var j = 0; j < Rows; j++)
            {
                if (!IsValid(grid, i, j, size))
                    continue;
                locations.Add((i, j));
            }
        }
        if (locations.Count > 0)
        {
            var loc = locations[random.Next(locations.Count)];
            Place(entity, loc.x, loc.y, size);
        }
    }

    private void Place(WebPath entity, int locX, int locY, GridItemSize size)
    {
        grid[locX, locY].Entity = entity;
        grid[locX, locY].Item = size;
        if (size.HasFlag(GridItemSize.TwoCols))
            grid[locX + 1, locY].Item = size;
        if (size.HasFlag(GridItemSize.TwoRows))
            grid[locX, locY + 1].Item = size;
        if (size == GridItemSize.TwoByTwo)
            grid[locX + 1, locY + 1].Item = size;
    }

    private bool IsValid(GridSquare[,] grid, int i, int j, GridItemSize size)
    {
        var sq = grid[i, j];
        if (sq.Full)
            return false;
        if (size.HasFlag(GridItemSize.TwoCols) && (i + 1 >= Columns || grid[i + 1, j].Full))
            return false;
        if (size.HasFlag(GridItemSize.TwoRows) && (j + 1 >= Rows || grid[i, j + 1].Full))
            return false;
        if (size == GridItemSize.TwoByTwo && grid[i + 1, j + 1].Full)
            return false;
        return true;
    }


}