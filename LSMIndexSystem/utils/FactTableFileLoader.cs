namespace LSMIndexSystem.models;

public class FactTableFileLoader
{
    public static FactTable? FromCSV(string filePath)
    {
        string[] lines = [];

        try
        {
            lines = File.ReadAllLines(filePath);
        }
        catch (Exception)
        {
            Console.WriteLine("File does not exist, could not create table.");
            return null;
        }

        if (lines.Length == 0)
        {
            Console.WriteLine("File is empty, could not create table.");
            return null;
        }

        var columns = lines[0]
            .Split(",")
            .Select(c => c.Trim())
            .ToList();

        var table = new FactTable()
        {
            Columns = columns
        };

        foreach (var line in lines.Skip(1))
        {
            var parts = line
                .Split(",")
                .Select(p => p.Trim())
                .ToList();

            if (parts.Count != columns.Count)
            {
                continue;
            }

            var row = new Row()
            {
                Id = int.Parse(parts[0])
            };

            for (var i = 1; i < columns.Count; i++)
            {
                var columnName = columns[i];
                var value = parts[i];

                row[columnName] = value;
            }

            table.AddRow(row);
        }

        return table;
    }
}