// See https://aka.ms/new-console-template for more information

using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using iso_bench_to_tikz;

CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

ParseMinPlusIsoBenchmark("./BenchmarkDotNet.Artifacts/results");

void ParseMinPlusIsoBenchmark(string resultsPath)
{
    var DirectMethod = "Direct";
    var IsospeedMethod = "Isospeed";
    var InversionMethod = "Inversion";

    var DirectMethodLabel = "direct";
    var IsospeedMethodLabel = "isospeed";
    var InversionMethodLabel = "inverse";
    var BestMethodLabel = "best";

    ComparisonByCurveType("IsoConvolutionBalancedStaircaseBenchmarks");
    ComparisonByCurveType("IsoConvolutionHorizontalStaircaseBenchmarks");
    ComparisonByCurveType("IsoConvolutionVerticalStaircaseBenchmarks");
    ComparisonByCurveType("IsoConvolutionHorizontalKTradeoffStaircaseBenchmarks");

    void ComparisonByCurveType(string type)
    {
        var csvPath = Path.Combine(resultsPath, $"{type}-report.csv");
        using var reader = new StreamReader(csvPath);

        var delimiter = File.ReadLines(csvPath).First().StartsWith("Method,") ? "," : ";";
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = delimiter
        };
        using var csv = new CsvReader(reader, config);

        var samples = csv.GetRecords<BenchmarkResult>().ToList();
        var minpVsIsoValues = GetComparisonValues(samples, DirectMethod, IsospeedMethod);
        var minpVsIsoTikz = TikzPlotter.PlotTikzComparison(
            minpVsIsoValues, DirectMethodLabel, IsospeedMethodLabel);
        var minpVsIsoOutTikz = Path.Combine(resultsPath, $"{type}-minp-iso.tikz");
        File.WriteAllText(minpVsIsoOutTikz, minpVsIsoTikz);

        var maxpVsIsoValues = GetComparisonValues(samples, InversionMethod, IsospeedMethod);
        var maxpVsIsoTikz = TikzPlotter.PlotTikzComparison(
            maxpVsIsoValues, InversionMethodLabel, IsospeedMethodLabel);
        var maxpVsIsoOutTikz = Path.Combine(resultsPath, $"{type}-maxp-iso.tikz");
        File.WriteAllText(maxpVsIsoOutTikz, maxpVsIsoTikz);

        var maxpVsMinpValues = GetComparisonValues(samples, InversionMethod, DirectMethod);
        var maxpVsMinpTikz = TikzPlotter.PlotTikzComparison(
            maxpVsMinpValues, InversionMethodLabel, DirectMethodLabel);
        var maxpVsMinpOutTikz = Path.Combine(resultsPath, $"{type}-maxp-minp.tikz");
        File.WriteAllText(maxpVsMinpOutTikz, maxpVsMinpTikz);

        var isoVsBestValues = GetComparisonValues_Best(samples, IsospeedMethod, new List<string>{ DirectMethod, InversionMethod });
        var isoVsBestTikz = TikzPlotter.PlotTikzComparison(
            isoVsBestValues, BestMethodLabel, IsospeedMethodLabel);
        var isoVsBestOutTikz = Path.Combine(resultsPath, $"{type}-best-iso.tikz");
        File.WriteAllText(isoVsBestOutTikz, isoVsBestTikz);
    }
}

List<(decimal x, decimal y)> GetComparisonValues(List<BenchmarkResult> samples, string method1, string method2)
{
    var samplesByTestCase = samples.GroupBy(br => br.Pair);
    var comparisonValues = samplesByTestCase
        .Select(g =>
        {
            var method1Sample = g.First(br => br.Method == method1)!;
            var m1TimeString = method1Sample.Median ?? method1Sample.Mean;
            var m1Time = timeStringToDecimal(m1TimeString);

            var method2Sample = g.First(br => br.Method == method2)!;
            var m2TimeString = method2Sample.Median ?? method2Sample.Mean;
            var m2Time = timeStringToDecimal(m2TimeString);
            
            return (x: m1Time, y: m2Time);
        })
        .ToList();
    return comparisonValues;

    decimal timeStringToDecimal(string s)
    {
        var raw = decimal.Parse(s.Split(" ")[0]);
        return s.EndsWith(" μs") ? raw / 1000:
            s.EndsWith(" ms") ? raw :
            s.EndsWith(" s") ? raw * 1_000 :
            throw new InvalidOperationException($"Unrecognized unit: {s}");
    }
}

List<(decimal x, decimal y)> GetComparisonValues_Best(List<BenchmarkResult> samples, string method1, List<string> otherMethods)
{
    var samplesByTestCase = samples.GroupBy(br => br.Pair);
    var comparisonValues = samplesByTestCase
        .Select(g =>
        {
            var method1Sample = g.First(br => br.Method == method1)!;
            var m1TimeString = method1Sample.Median ?? method1Sample.Mean;
            var m1Time = timeStringToDecimal(m1TimeString);

            var otherSamples = otherMethods.Select(m => {
                var mSample = g.First(br => br.Method == m)!;
                var mTimeString = mSample.Median ?? mSample.Mean;
                var mTime = timeStringToDecimal(mTimeString);
                return mTime;
            });
            return (x: otherSamples.Min(), y: m1Time);
        })
        .ToList();
    return comparisonValues;

    decimal timeStringToDecimal(string s)
    {
        var raw = decimal.Parse(s.Split(" ")[0]);
        return s.EndsWith(" μs") ? raw / 1000:
            s.EndsWith(" ms") ? raw :
            s.EndsWith(" s") ? raw * 1_000 :
            throw new InvalidOperationException($"Unrecognized unit: {s}");
    }
}