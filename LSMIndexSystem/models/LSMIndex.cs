namespace LSMIndexSystem.models;

public class LSMIndex
{
    public string? ColumnName { get; set; }
    public List<Level> Levels { get; set; } = [new()
    {
        Capacity = 1000
    }];

    public void Insert(string key, int rowId)
    {
        Levels[0].Insert(key, rowId);

        for (var i = 0; i < Levels.Count; i++)
        {
            if (Levels[i].IsFull())
            {
                if (i + 1 >= Levels.Count)
                {
                    Levels.Add(new()
                    {
                        Capacity = Levels[i].Capacity * 3
                    });
                }

                Levels[i + 1].Merge(Levels[i]);
                Levels[i].Clear();
            }
        }
    }

    public List<int> Search(string key)
    {
        var result = new List<int>();

        foreach (var level in Levels)
        {
            if (level.Data.ContainsKey(key))
            {
                result.AddRange(level.Data[key]);
            }
        }

        return result;
    }

    public void Remove(string key, int rowId)
    {
        foreach (var level in Levels)
        {
            if (level.Data.ContainsKey(key))
            {
                level.Data[key].Remove(rowId);

                if (level.Data[key].Count == 0)
                {
                    level.Data.Remove(key);
                }
            }
        }
    }
}