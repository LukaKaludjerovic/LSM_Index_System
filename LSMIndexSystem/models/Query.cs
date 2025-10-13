using System.Diagnostics;

namespace LSMIndexSystem.models;

public class Query
{
    public static void SearchWithoutIndex(FactTable table, Dictionary<string, string> conditions, string? logicalOperator = null, Dictionary<string, string>? aggregations = null)
    {
        if (conditions.Count == 0)
        {
            Console.WriteLine("Search conditions not passed.");
            return;
        }

        if (conditions.Count > 1 && logicalOperator == null)
        {
            Console.WriteLine("Logical operator must be passed if searching with multiple conditions.");
            return;
        }

        var sw = Stopwatch.StartNew();
        var columns = table.Columns;
        var rows = table.Rows;

        var resultSets = FindMatchedIdsWithoutIndex(rows, conditions);
        var result = ComputeLogicalOperator(rows, resultSets, conditions, logicalOperator);

        PrintSearchResults(columns, result);
        PrintAggregationResults(aggregations, result);

        sw.Stop();
        Console.WriteLine($"Search without indexes completed in {sw.Elapsed.TotalMilliseconds} milliseconds.");
        Console.WriteLine();
    }

    public static void SearchWithIndex(FactTable table, Dictionary<string, string> conditions, string? logicalOperator = null, Dictionary<string, string>? aggregations = null)
    {
        if (conditions.Count == 0)
        {
            Console.WriteLine("Search conditions not passed.");
            return;
        }

        if (conditions.Count > 1 && logicalOperator == null)
        {
            Console.WriteLine("Logical operator must be passed if searching with multiple conditions.");
            return;
        }

        var sw = Stopwatch.StartNew();
        var columns = table.Columns;
        var rows = table.Rows;
        var indexes = table.Indexes;

        var resultSets = FindMatchedIdsWithIndex(rows, indexes, conditions);
        var result = ComputeLogicalOperator(rows, resultSets, conditions, logicalOperator);

        PrintSearchResults(columns, result, true);
        PrintAggregationResults(aggregations, result);

        sw.Stop();
        Console.WriteLine($"Search with indexes completed in {sw.Elapsed.TotalMilliseconds} milliseconds.");
        Console.WriteLine();
    }

    private static List<HashSet<int>> FindMatchedIdsWithoutIndex(List<Row> rows, Dictionary<string, string> conditions)
    {
        List<HashSet<int>> resultSets = [];

        foreach (var condition in conditions)
        {
            string column = condition.Key;
            string value = condition.Value ?? "NULL";

            HashSet<int> matchedIds = [];

            foreach (var row in rows)
            {
                if (Equals(row[column], value))
                {
                    matchedIds.Add(row.Id);
                }
            }

            resultSets.Add(matchedIds);
        }

        return resultSets;
    }
    
    private static List<HashSet<int>> FindMatchedIdsWithIndex(List<Row> rows, Dictionary<string, LSMIndex> indexes, Dictionary<string, string> conditions)
    {
        List<HashSet<int>> resultSets = [];

        foreach (var condition in conditions)
        {
            string column = condition.Key;
            string value = condition.Value ?? "NULL";

            HashSet<int> matchedIds = [];

            if (indexes.ContainsKey(column))
            {
                var rowIds = indexes[column].Search(value);
                matchedIds.UnionWith(rowIds);
            }
            else
            {
                foreach (var row in rows)
                {
                    if (Equals(row[column], value))
                    {
                        matchedIds.Add(row.Id);
                    }
                }
            }

            resultSets.Add(matchedIds);
        }

        return resultSets;
    }

    private static List<Row> ComputeLogicalOperator(List<Row> rows, List<HashSet<int>> resultSets, Dictionary<string, string> conditions, string? logicalOperator)
    {
        HashSet<int> finalIds = [];

        var firstSet = resultSets.FirstOrDefault();

        if (firstSet == null)
        {
            return [];
        }

        finalIds.UnionWith(firstSet);

        if (conditions.Count > 1 && logicalOperator != null)
        {
            switch (logicalOperator)
            {
                case "AND":
                    foreach (var set in resultSets.Skip(1))
                    {
                        finalIds.IntersectWith(set);
                    }
                    break;

                case "OR":
                    foreach (var set in resultSets.Skip(1))
                    {
                        finalIds.UnionWith(set);
                    }
                    break;

                default:
                    Console.WriteLine("Invalid logical operator passed.");
                    break;
            }
        }

        var result = rows.Where(r => finalIds.Contains(r.Id)).ToList();

        return result;
    }

    private static void PrintSearchResults(List<string> columns, List<Row> rows, bool withIndex = false)
    {
        if (!withIndex)
        {
            Console.WriteLine("Search without index results:");
        }
        else
        {
            Console.WriteLine("Search with index results:");
        }

        if (rows.Count > 0)
        {
            foreach (var column in columns)
            {
                Console.Write(column);
                for (var i = 0; i < 8 - column.Length; i++)
                {
                    Console.Write(" ");
                }
            }
            Console.WriteLine();
            foreach (var row in rows)
            {
                Console.WriteLine(row);
            }
            Console.WriteLine();
        }
        else
        {
            Console.WriteLine("No rows found.");
            Console.WriteLine();
        }
    }

    private static void PrintAggregationResults(Dictionary<string, string>? aggregations, List<Row> rows)
    {
        if (aggregations != null && rows.Count > 0)
        {
            Console.WriteLine("Aggregation results:");

            foreach (var aggregation in aggregations)
            {
                var aggregateColumn = aggregation.Key;
                var aggegateFunction = aggregation.Value;

                if (aggregateColumn != "" && aggegateFunction != "")
                {
                    var aggregationResult = ComputeAggregation(rows, aggregateColumn, aggegateFunction);

                    Console.WriteLine($"{aggegateFunction}({aggregateColumn}) = {aggregationResult}");
                }
            }
            Console.WriteLine();
        }
    }
    
    private static double ComputeAggregation(List<Row> rows, string factColumn, string aggegateFunction)
    {
        var values = rows
            .Select(r => r[factColumn])
            .Where(v => v != null)
            .Select(Convert.ToDouble)
            .ToList();

        if (values.Count == 0)
        {
            return 0;
        }

        return aggegateFunction switch
        {
            "SUM" => values.Sum(),
            "AVG" => values.Average(),
            "MIN" => values.Min(),
            "MAX" => values.Max(),
            "COUNT" => values.Count,
            _ => 0,
        };
    }
}