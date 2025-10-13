namespace LSMIndexSystem.models;

public class Level
{
    public int Capacity { get; set; }
    public Dictionary<string, List<int>> Data { get; set; } = [];

    public void Insert(string key, int rowId)
    {
        if (!Data.ContainsKey(key))
        {
            Data[key] = [];
        }

        Data[key].Add(rowId);
    }

    public bool IsFull()
    {
        return Data.Count >= Capacity;
    }

    public void Merge(Level lowerLevel)
    {
        foreach (var entry in lowerLevel.Data)
        {
            var lowerKey = entry.Key;
            var lowerValue = entry.Value;
            
            if (!Data.ContainsKey(lowerKey))
            {
                Data[lowerKey] = [];
            }

            Data[lowerKey].AddRange(lowerValue);
        }
    }

    public void Clear()
    {
        Data.Clear();
    }
}