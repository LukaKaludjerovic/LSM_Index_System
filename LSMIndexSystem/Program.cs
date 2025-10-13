using LSMIndexSystem.models;
using LSMIndexSystem.utils;

// creating output file
var folder = "./outputs";
Directory.CreateDirectory(folder);
var outputFileName = $"output_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
var relativePath = Path.Combine(folder, outputFileName);
using var writer = new StreamWriter(relativePath);
Console.SetOut(writer);

// generating random data and creating table
var numD = 6;
var numFact = 6;
var numRows = 1000;

var tableFile = RandomGenerator.GenerateRandomData(numD, numFact, numRows);
var table = FactTableFileLoader.FromCSV(tableFile);

if (table == null)
{
    return;
}

// creating indexes for all Di columns
for (var i = 1; i <= numD; i++)
{
    table.CreateIndex($"D{i}");
}

// generating random conditions
var randomConditions = new Random();

Dictionary<string, string> conditions = [];

for (var i = 1; i <= numD; i++)
{
    var generateCondition = randomConditions.Next();

    if (generateCondition % 2 == 0)
    {
        if (i % 2 == 0)
            conditions[$"D{i}"] = randomConditions.Next(1, 11).ToString();
        else
            conditions[$"D{i}"] = ((char)('A' + randomConditions.Next(0, 5))).ToString();
    }
}

// generating random logical operator
var randomLogicalOperators = new Random();
string logicalOperator;
var randomLogicalOperator = randomLogicalOperators.Next();

if (randomLogicalOperator % 2 == 0)
    logicalOperator = LogicalOperator.AND;
else
    logicalOperator = LogicalOperator.OR;

// generating random aggregations
var randomAggregations = new Random();

Dictionary<string, string> aggregations = [];

for (var i = 1; i <= numFact; i++)
{
    var randomAggregation = randomAggregations.Next();

    if (randomAggregation % 5 == 0)
    {
        aggregations[$"Fact{i}"] = AggregateFunction.SUM;
    }
    else if (randomAggregation % 5 == 1)
    {
        aggregations[$"Fact{i}"] = AggregateFunction.AVG;
    }
    else if (randomAggregation % 5 == 2)
    {
        aggregations[$"Fact{i}"] = AggregateFunction.MIN;
    }
    else if (randomAggregation % 5 == 3)
    {
        aggregations[$"Fact{i}"] = AggregateFunction.MAX;
    }
    else
    {
        aggregations[$"Fact{i}"] = AggregateFunction.COUNT;
    }
}

// printing random generated conditions and aggregations
Console.WriteLine("Generated conditions:");
foreach (var condition in conditions)
{
    Console.WriteLine($"{condition.Key} = {condition.Value}");
}
Console.WriteLine();

Console.WriteLine($"Generated logical operator: {logicalOperator}");
Console.WriteLine();

Console.WriteLine("Generated aggregations:");
foreach (var aggregation in aggregations)
{
    Console.WriteLine($"{aggregation.Value}({aggregation.Key})");
}
Console.WriteLine();
Console.WriteLine("------------------------------------------------------------------------------------------------------------------------");
Console.WriteLine();

// warming up, console disabled during warm up process
var originalOut = Console.Out;
Console.SetOut(TextWriter.Null);

Query.SearchWithoutIndex(table, conditions, logicalOperator, aggregations);
Query.SearchWithIndex(table, conditions, logicalOperator, aggregations);

Console.SetOut(originalOut);

// system warmed up, console enabled
Query.SearchWithoutIndex(table, conditions, logicalOperator, aggregations);
Console.WriteLine("------------------------------------------------------------------------------------------------------------------------");
Console.WriteLine();
Query.SearchWithIndex(table, conditions, logicalOperator, aggregations);