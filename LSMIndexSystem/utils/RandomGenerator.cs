namespace LSMIndexSystem.utils;

public class RandomGenerator
{
    public static string GenerateRandomData(int numD, int numFact, int numRows)
    {
        var folder = "./data";
        Directory.CreateDirectory(folder);
        var fileName = $"data_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
        var relativePath = Path.Combine(folder, fileName);
        var fullPath = Path.GetFullPath(relativePath);
    
        var random = new Random();

        using var writer = new StreamWriter(relativePath);

        var headers = new List<string>
        {
            "Id"
        };
        for (var i = 1; i <= numD; i++)
        {
            headers.Add($"D{i}");
        }
        for (var i = 1; i <= numFact; i++)
        {
            headers.Add($"Fact{i}");
        }
        writer.WriteLine(string.Join(",", headers));

        for (var row = 1; row <= numRows; row++)
        {
            var values = new List<string>
            {
                row.ToString()
            };

            for (int i = 1; i <= numD; i++)
            {
                if (i % 2 == 0)
                    values.Add(random.Next(1, 11).ToString());
                else
                    values.Add(((char)('A' + random.Next(0, 5))).ToString());
            }

            for (int i = 1; i <= numFact; i++)
            {
                values.Add(random.Next(100, 301).ToString());
            }

            writer.WriteLine(string.Join(",", values));
        }

        return fullPath;
    }
}