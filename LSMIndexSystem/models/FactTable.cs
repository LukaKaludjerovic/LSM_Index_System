using LSMIndexSystem.utils;

namespace LSMIndexSystem.models;

public class FactTable()
{
    public List<Row> Rows { get; set; } = [];
    public List<string> Columns { get; set; } = [];
    public Dictionary<string, LSMIndex> Indexes { get; set; } = [];

    public void AddRow(Row row)
    {
        Rows.Add(row);

        foreach (var index in Indexes.Values)
        {
            var key = row[index.ColumnName!];

            if (key != null)
            {
                index.Insert(key, row.Id);
            }
        }
    }

    public void AddRows(List<Row> rows)
    {
        foreach (var row in rows)
        {
            AddRow(row);
        }
    }

    public void CreateIndex(string columnName)
    {
        if (Columns.Contains(columnName) && !Indexes.ContainsKey(columnName))
        {
            Indexes[columnName] = new LSMIndex()
            {
                ColumnName = columnName
            };

            foreach (var row in Rows)
            {
                var key = row[columnName];

                if (key != null)
                {
                    Indexes[columnName].Insert(key, row.Id);
                }
            }
        }
    }

    public void PrintTable()
    {
        var rowsCount = Rows.Count;

        if (rowsCount > 10)
        {
            Console.WriteLine($"Table cannot be written because it contains {rowsCount} rows.");
            return;
        }

        Console.WriteLine("Table content:");
        Console.WriteLine(string.Join("\t\t", Columns));

        foreach (var row in Rows)
        {
            Console.WriteLine(row);
        }

        Console.WriteLine();
    }
    
    public void DeleteRow(int rowId)
    {
        var row = Rows.FirstOrDefault(r => r.Id == rowId);

        if (row == null)
        {
            Console.WriteLine($"Row with Id {rowId} does not exist.");
            return;
        }

        Rows.Remove(row);

        foreach (var entry in Indexes)
        {
            var columnName = entry.Key;
            var index = entry.Value;

            if (row.Data.ContainsKey(columnName))
            {
                var key = row.Data[columnName];

                if (key != null)
                {
                    index.Remove(key, rowId);
                }
            }
        }

        Console.WriteLine($"Row with Id {rowId} deleted.");
    }
}