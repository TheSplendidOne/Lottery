using System.Globalization;
using Lottery;

var inputPath = "C:\\Code\\input.txt";

var lines = await File.ReadAllLinesAsync(inputPath);

var games = lines
    .Where(line => !String.IsNullOrWhiteSpace(line))
    .Select(line => line.Split(' '))
    .Select(items => new Game()
    {
        LosingCellsCount = Int32.Parse(items[0], NumberFormatInfo.InvariantInfo),
        WinningCellsCount = Int32.Parse(items[1], NumberFormatInfo.InvariantInfo),
        Multiplier = Decimal.Parse(items[2], NumberFormatInfo.InvariantInfo)
    });

using var writer = new StreamWriter(GetOutputPath(inputPath));

foreach (var game in games)
{
    var coefficients = CalculateCoefficients(game);

    var outputLine = String.Join(' ', coefficients.Select(c => Math.Round(c, 3).ToString("0.###", CultureInfo.InvariantCulture)));

    writer.WriteLine(outputLine);
}

List<decimal> CalculateCoefficients(Game game)
{
    var cellsTotal = game.WinningCellsCount + game.LosingCellsCount;

    var coefficients = new List<decimal>(game.WinningCellsCount);

    for(var cellsOpened = 0; cellsOpened < game.WinningCellsCount; cellsOpened++)
    {
        var coefficient = coefficients.LastOrDefault(game.Multiplier)
            * (cellsTotal - cellsOpened)
            / (game.WinningCellsCount - cellsOpened);

        coefficients.Add(coefficient);
    }

    return coefficients;
}

string GetOutputPath(string inputPath)
{
    const string outputFileName = "output.txt";

    var inputFileDirectory = Directory.GetParent(inputPath)!.FullName;

    var outputPath = Path.Combine(inputFileDirectory, outputFileName);

    return outputPath;
}
