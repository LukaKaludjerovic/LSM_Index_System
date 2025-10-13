namespace LSMIndexSystem.models;

public class Row()
{
    public int Id { get; set; }
    public Dictionary<string, string> Data { get; set; } = [];

    public string this[string columnName]
    {
        get => Data.ContainsKey(columnName) ? Data[columnName] : "";
        set => Data[columnName] = value;
    }

    public override string ToString()
    {
        string rowString = $"{Id}" + "\t\t";
        
        foreach (var entry in Data)
        {
            if (entry.Value == null)
            {
                rowString += "NULL" + "\t\t";
            }
            else
            {
                rowString += $"{entry.Value}" + "\t\t";
            }
        }

        return rowString;
    }
}