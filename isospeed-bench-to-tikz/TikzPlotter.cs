using System.Text;

namespace iso_bench_to_tikz;

public static class TikzPlotter
{
    public static string PlotTikzComparison(
        IReadOnlyList<(decimal x, decimal y)> values,
        string xLabel,
        string yLabel
    )
    {
        var sb = new StringBuilder();

        sb.Append(PlotTikzComparisonHeader(values, xLabel, yLabel));
        sb.Append(PlotTikzComparisonValues(values, xLabel, yLabel));
        sb.Append(PlotTikzComparisonBisector(values, xLabel, yLabel));
        sb.Append(PlotTikzComparisonFooter(values, xLabel, yLabel));
        
        return sb.ToString();
    }

    private static string PlotTikzComparisonHeader(
        IReadOnlyList<(decimal x, decimal y)> values,
        string xLabel,
        string yLabel
    )
    {
        var sb = new StringBuilder();

        sb.AppendLine("\\begin{tikzpicture}");
        sb.AppendLine("\\begin{axis}[");
        sb.AppendLine($"{tabs(1)}font = \\small,");
        sb.AppendLine($"{tabs(1)}axis lines = left,");
        sb.AppendLine($"{tabs(1)}axis lines = left,");
        sb.AppendLine($"{tabs(1)}grid = major,");
        sb.AppendLine($"{tabs(1)}grid style = {{draw=gray!30}},");
        sb.AppendLine($"{tabs(1)}xmode = log,");
        sb.AppendLine($"{tabs(1)}ymode = log,");
        
        var yMin = getLowerBoundary(values.Select(p => p.y));
        var yMax = getUpperBoundary(values.Select(p => p.y));
        var xMin = getLowerBoundary(values.Select(p => p.x));
        var xMax = getUpperBoundary(values.Select(p => p.x));
        sb.AppendLine($"{tabs(1)}ymin = {yMin},");
        sb.AppendLine($"{tabs(1)}ymax = {yMax},");
        sb.AppendLine($"{tabs(1)}xmin = {xMin},");
        sb.AppendLine($"{tabs(1)}xmax = {xMax},");
        
        sb.AppendLine($"{tabs(1)}xlabel = {{{xLabel}}},");
        sb.AppendLine($"{tabs(1)}ylabel = {{{yLabel}}},");
        
        sb.AppendLine($"{tabs(1)}% log ticks with fixed point,");
        sb.AppendLine($"{tabs(1)}xtick = {{{0.1}, {1}, {10}, {100}, {1_000}, {10_000}, {100_000}, {1_000_000}, {10_000_000}}},");
        sb.AppendLine($"{tabs(1)}ytick = {{{0.1}, {1}, {10}, {100}, {1_000}, {10_000}, {100_000}, {1_000_000}, {10_000_000}}},");
        sb.AppendLine($"{tabs(1)}xticklabels = \\empty,");
        sb.AppendLine($"{tabs(1)}yticklabels = \\empty,");
        sb.AppendLine($"{tabs(1)}extra x ticks = {{{0.1}, {1}, {10}, {100}, {1_000}, {10_000}, {60_000}, {600_000}, {1800_000}, {3600_000}}},");
        sb.AppendLine($"{tabs(1)}extra x tick labels= {{$100 {{\\mu}}s$, $1 ms$, $10 ms$, $100 ms$, $1 s$, $10 s$, $1 m.$, $10 m.$, $30 m.$, $1 h$}},");
        sb.AppendLine($"{tabs(1)}extra y ticks = {{{0.1}, {1}, {10}, {100}, {1_000}, {10_000}, {60_000}, {600_000}, {1800_000}, {3600_000}}},");
        sb.AppendLine($"{tabs(1)}extra y tick labels= {{$100 {{\\mu}}s$, $1 ms$, $10 ms$, $100 ms$, $1 s$, $10 s$, $1 m.$, $10 m.$, $30 m.$, $1 h$}},");
        
        sb.AppendLine($"]");
        
        return sb.ToString();
    }
    
    private static string PlotTikzComparisonValues(
        IReadOnlyList<(decimal x, decimal y)> values,
        string xLabel,
        string yLabel
    )
    {
        var sb = new StringBuilder();

        sb.AppendLine($"{tabs(1)}\\addplot [only marks, mark size=1pt] coordinates {{");
        foreach (var pair in values)
        {
            sb.AppendLine($"{tabs(2)}({pair.x}, {pair.y})");
        }
        sb.AppendLine($"{tabs(1)}}};");
        
        return sb.ToString();
    }
    
    private static string PlotTikzComparisonBisector(
        IReadOnlyList<(decimal x, decimal y)> values,
        string xLabel,
        string yLabel
    )
    {
        var sb = new StringBuilder();

        var yMin = getLowerBoundary(values.Select(p => p.y));
        var yMax = getUpperBoundary(values.Select(p => p.y));
        var xMin = getLowerBoundary(values.Select(p => p.x));
        var xMax = getUpperBoundary(values.Select(p => p.x));
        var domainMin = Math.Min(xMin, yMin);
        var domainMax = Math.Max(xMax, yMax);
        sb.AppendLine($"{tabs(1)}\\addplot+ [mark=none, black, dashed, domain={domainMin}:{domainMax}] {{x}};");
        
        return sb.ToString();
    }
    
    private static string PlotTikzComparisonFooter(
        IReadOnlyList<(decimal x, decimal y)> values,
        string xLabel,
        string yLabel
    )
    {
        var sb = new StringBuilder();
        
        sb.AppendLine("\\end{axis}");
        sb.AppendLine("\\end{tikzpicture}");

        
        return sb.ToString();
    }

    private static string tabs(int n)
    {
        var sbt = new StringBuilder();
        for (int i = 0; i < n; i++)
            sbt.Append("\t");
        return sbt.ToString();
    }

    private static decimal getLowerBoundary(IEnumerable<decimal> values)
    {
        var min = values.Min();
        if (min <= 0.5m)
            return 0.1m;
        if (min <= 1)
            return 0.5m;
        if (min <= 10)
            return 1;
        if (min <= 100)
            return 10;
        if (min <= 1_000)
            return 100;
        if (min <= 10_000)
            return 1_000;
        if (min <= 60_000)
            return 10_000;
        if (min <= 600_000)
            return 60_000;
        if (min <= 1800_000)
            return 600_000;
        if (min <= 3600_000)
            return 1800_000;
        return 3600_000;
    }
    
    private static decimal getUpperBoundary(IEnumerable<decimal> values)
    {
        var max = values.Max();
        if (max <= 0.1m)
            return 0.1m;
        if (max <= 0.5m)
            return 0.5m;
        if (max <= 1)
            return 1;
        if (max <= 10)
            return 10;
        if (max <= 100)
            return 100;
        if (max <= 1_000)
            return 1_000;
        if (max <= 10_000)
            return 10_000;
        if (max <= 60_000)
            return 60_000;
        if (max <= 600_000)
            return 600_000;
        if (max <= 1800_000)
            return 1800_000;
        return 3600_000;
    }
}