using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using Unipi.Nancy.Numerics;

namespace Unipi.Nancy.MinPlusAlgebra;

// todo: document these, see if it needs adjustments

/// <summary>
/// This class provides methods to generate plots with Tikz. 
/// </summary>
public static class ToTikzPlotExtension
{
    public static List<string> DefaultColorList = new ()
    {
        "green!60!black",
        "blue",
        "red!60!black"
    };

    public static List<string> GetDefaultColors(int n)
    {
        var result = new List<string>();
        for (int i = 0; i < n; i++)
        {
            result.Add(DefaultColorList[i % DefaultColorList.Count]);
        }

        return result;
    }

    public static List<string> GetDefaultNames(int n)
    {
        var result = new List<string>();
        for (int i = 0; i < n; i++)
        {
            var round = i / 27;
            var index = i % 27;
            var letter = (char) ('f' + index);
            if(round > 0)
                result.Add($"{letter}{round}");
            else
                result.Add(letter.ToString());
        }

        return result;
    }

    public static List<string> GetDefaultLineStyles(int n)
    {
        var result = new List<string>();
        for (int i = 0; i < n; i++)
        {
            result.Add("solid");
        }

        return result;
    }

    public static string ToTikzPlot(
        this IReadOnlyList<Curve> curves, 
        IReadOnlyList<string>? names = null, 
        IReadOnlyList<string>? colors = null, 
        IReadOnlyList<string>? lineStyles = null,
        Rational? upTo = null)
    {
        Rational t;
        if(upTo is not null)
            t = (Rational) upTo;
        else
            t = curves.Max(c => c.SecondPseudoPeriodEnd);
        t = t == 0 ? 10 : t;

        var sequences = curves
            .Select(c => c.Cut(0, t, isEndIncluded: true))
            .ToList();

        var xmax = sequences.Max(s => s.DefinedUntil);
        var ymax = sequences.Max(s => s.IsRightClosed ? s.ValueAt(s.DefinedUntil) : s.LeftLimitAt(s.DefinedUntil));

        var xmarks = sequences
            .SelectMany(s => s
                .EnumerateBreakpoints()
                .Select(bp => bp.center.Time))
            .Where(x => x.IsFinite)
            .OrderBy(x => x)
            .Distinct()
            .ToList();

        var ymarks = sequences
            .SelectMany(s => s
                .EnumerateBreakpoints()
                .GetBreakpointsValues())
            .Where(y => y.IsFinite)
            .OrderBy(y => y)
            .Distinct()
            .ToList();

        var sb = new StringBuilder();
        sb.AppendLines(GetTikzHeader(xmax, ymax, xmarks, ymarks));

        names ??= GetDefaultNames(curves.Count);
        colors ??= GetDefaultColors(curves.Count);
        lineStyles ??= GetDefaultLineStyles(curves.Count);

        sb.AppendLines(ToTikzPlot(sequences, names, colors, lineStyles));

        if (curves.Count == 1 && curves.Single() is { IsUltimatelyInfinite: false } f)
        {
            sb.AppendLines(GetUppMarks(f, names[0]));
        }

        sb.AppendLines(GetTikzFooter());

        return sb.ToString();
    }

    public static string ToTikzPlot(
        this Curve curve,
        string? name = null,
        string? color = null,
        string? lineStyle = null,
        Rational? upTo = null
    )
    {
        var names = name != null ? new List<string>{name} : GetDefaultNames(1);
        var colors = color != null ? new List<string>{color} : GetDefaultColors(1);
        var lineStyles = lineStyle != null ? new List<string>{lineStyle} : null;
        return ToTikzPlot(new[] { curve }, names, colors, lineStyles, upTo);
    }

    public static string ToTikzPlot(params Curve[] curves)
    {
        var names = GetDefaultNames(curves.Length);
        var colors = GetDefaultColors(curves.Length);
        var lineStyles = GetDefaultLineStyles(curves.Length);
        return ToTikzPlot(curves, names, colors, lineStyles);
    }

    private static string tabs(int n)
    {
        var sbt = new StringBuilder();
        for (int i = 0; i < n; i++)
            sbt.Append("\t");
        return sbt.ToString();
    }

    internal static IEnumerable<string> GetTikzHeader(
        Rational xmax, 
        Rational ymax,
        List<Rational>? xmarks = null,
        List<Rational>? ymarks = null,
        string? legendPos = null)
    {
        legendPos ??= "south east";

        yield return $"\\begin{{tikzpicture}}";
        yield return $"{tabs(1)}\\begin{{axis}}[";
        yield return $"{tabs(2)}clip = true,";
        yield return $"{tabs(2)}grid = both,";
        yield return $"{tabs(2)}grid style = {{draw=gray!30}},";
        yield return $"{tabs(2)}axis lines = left,";
        yield return $"{tabs(2)}axis equal image,";
        yield return $"{tabs(2)}minor tick num = 1,";
        yield return $"{tabs(2)}xlabel = time,";
        yield return $"{tabs(2)}ylabel = data,";
        yield return $"{tabs(2)}x label style = {{at={{(axis description cs:1,0)}},anchor=north}},";
        yield return $"{tabs(2)}y label style = {{at={{(axis description cs:0,1)}},rotate=-90,anchor=south}},";
        yield return $"{tabs(2)}xmin = 0,";
        yield return $"{tabs(2)}ymin = 0,";
        yield return FormattableString.Invariant($"{tabs(2)}xmax = {(decimal) xmax + 1},");
        yield return FormattableString.Invariant($"{tabs(2)}ymax = {(decimal) ymax + 1},");
        yield return $"{tabs(2)}xticklabels = \\empty,";
        yield return $"{tabs(2)}yticklabels = \\empty,";

        if (xmarks != null)
        {   
            var sb = new StringBuilder();
            sb.Append($"{tabs(2)}extra x ticks = {{ ");
            foreach (var xmark in xmarks)
            {
                sb.Append((decimal) xmark);
                sb.Append(", ");
            }
            sb.Remove(sb.Length - 2, 2);
            sb.Append(" },");
            yield return sb.ToString();
        }

        if (ymarks != null)
        {
            var sb = new StringBuilder();
            sb.Append($"{tabs(2)}extra y ticks = {{ ");
            foreach (var ymark in ymarks)
            {
                sb.Append((decimal) ymark);
                sb.Append(", ");
            }
            sb.Remove(sb.Length - 2, 2);
            sb.Append(" },");
            yield return sb.ToString();
        }

        yield return $"{tabs(2)}legend pos = {legendPos}";
        yield return $"{tabs(1)}]";
    }

    internal static IEnumerable<string> GetTikzFooter()
    {
        yield return $"{tabs(1)}\\end{{axis}}";
        yield return "\\end{tikzpicture}";
    }

    public static IEnumerable<string> GetUppMarks(Curve f, string name)
    {
        return _getUppMarks().Select(FormattableString.Invariant);

        IEnumerable<FormattableString> _getUppMarks()
        {
            var marksColor = "black!60";
            var marksStyle = "thick, densely dashed";
            var arrowStyle = "thick, <->";

            var tf = (decimal)f.PseudoPeriodStart;
            var ftf = (decimal)f.ValueAt(f.PseudoPeriodStart);

            yield return $"{tabs(2)}\\addplot [ color = {marksColor}, {marksStyle} ] coordinates {{ ({tf}, 0) ({tf}, {ftf}) }};";
            yield return $"{tabs(2)}\\node [ anchor = south west ] at (axis cs:{tf}, 0) {{$T_{{{name}}}$}};";
            yield return $"";

            var tfdf = (decimal)f.FirstPseudoPeriodEnd;
            var ftfdf = (decimal)f.ValueAt(f.FirstPseudoPeriodEnd);
            var ftf2df = (decimal)f.ValueAt(f.SecondPseudoPeriodEnd);

            yield return
                $"{tabs(2)}\\addplot [ color = {marksColor}, {marksStyle} ] coordinates {{ ({tfdf}, {ftfdf}) ({tfdf}, {(ftfdf + ftf2df) / 2}) }};";
            yield return
                $"{tabs(2)}\\addplot [ color = {marksColor}, {marksStyle} ] coordinates {{ ({tf}, {ftf}) ({tf}, {(ftfdf + ftf2df) / 2}) }};";
            yield return
                $"{tabs(2)}\\addplot [ color = {marksColor}, {arrowStyle} ] coordinates {{ ({tf}, {(ftfdf + ftf2df) / 2}) ({tfdf}, {(ftfdf + ftf2df) / 2}) }};";
            yield return $"{tabs(2)}\\node [ anchor = south ] at (axis cs:{(tf + tfdf) / 2}, {(ftfdf + ftf2df) / 2}) {{$d_{{{name}}}$}};";
            yield return $"";

            var tf2df = (decimal)f.SecondPseudoPeriodEnd;

            yield return
                $"{tabs(2)}\\addplot [ color = {marksColor}, {marksStyle} ] coordinates {{ ({tf}, {ftf}) ({(tfdf + tf2df) / 2}, {ftf}) }};";
            yield return
                $"{tabs(2)}\\addplot [ color = {marksColor}, {marksStyle} ] coordinates {{ ({tfdf}, {ftfdf}) ({(tfdf + tf2df) / 2}, {ftfdf}) }};";
            yield return
                $"{tabs(2)}\\addplot [ color = {marksColor}, {arrowStyle} ] coordinates {{ ({(tfdf + tf2df) / 2}, {ftf}) ({(tfdf + tf2df) / 2}, {ftfdf}) }};";
            yield return $"{tabs(2)}\\node [ anchor = west ] at (axis cs:{(tfdf + tf2df) / 2}, {(ftf + ftfdf) / 2}) {{$c_{{{name}}}$}};";
            yield return $"";
        }
    }

    public static IEnumerable<string> ToTikzPlot(
        this IReadOnlyList<Sequence> sequences, 
        IReadOnlyList<string> names, 
        IReadOnlyList<string> colors,
        IReadOnlyList<string> lineStyles
    )
    {
        if (sequences.Count != names.Count || sequences.Count != colors.Count)
            throw new InvalidEnumArgumentException("The arguments must be of the same length");
        if (sequences.Any(s => s.FirstFiniteTime.IsPlusInfinite))
            throw new InvalidEnumArgumentException("Cannot plot infinite-only sequences");

        yield return $"{tabs(2)}% copies for legend";

        foreach (var (sequence,i) in sequences.WithIndex())
        {
            var firstSegment = sequence.Elements.FirstOrDefault(e => e is Segment && e.IsFinite);
            var legendElement = firstSegment ?? sequence.Elements.First(e => e.IsFinite);
            var legendLine = legendElement.ToTikzLine(colors[i], lineStyles[i]);
            yield return $"{tabs(2)}{legendLine}";
        }
        foreach (var name in names)
            yield return $"{tabs(2)}\\addlegendentry{{$ {name} $}};";
        yield return "";

        var plots = sequences
            .Select((s, i) => s.ToTikzLines(colors[i], lineStyles[i])
            )
            .ToList();

        foreach (var (plot, i) in plots.WithIndex())
        {
            yield return $"{tabs(2)}% {names[i]}";
            foreach (var line in plot)
                yield return $"{tabs(2)}{line}";
            yield return "";
        }
    }

    internal static IEnumerable<string> ToTikzLines(
        this Sequence sequence, 
        string color, 
        string? lineStyle = null
    )
    {
        if (lineStyle != null)
        {
            if (lineStyle.EndsWith(","))
                lineStyle += " ";
            else if (!lineStyle.EndsWith(", "))
                lineStyle += ", ";
        }
        else
        {
            lineStyle = "";
        }

        foreach (var element in sequence.Elements.Where(e => e.IsFinite))
            yield return element.ToTikzLine(color, lineStyle);
    }

    internal static string ToTikzLine(this Element element, string color, string? lineStyle = null)
    {
        if (element.IsInfinite)
            throw new InvalidOperationException("Cannot plot infinities.");

        if (lineStyle != null)
        {
            if (lineStyle.EndsWith(","))
                lineStyle += " ";
            else if (!lineStyle.EndsWith(", "))
                lineStyle += ", ";
        }
        else
        {
            lineStyle = "";
        }

        FormattableString line;
        if(element is Point p)
        {
            var x = (decimal)p.Time;
            var y = (decimal)p.Value;
            line = $"\\addplot [ color = {color}, thick, only marks, mark size = 1pt ] coordinates {{ ({x},{y}) }};";
        }
        else if (element is Segment s)
        {
            var x1 = (decimal) s.StartTime;
            var y1 = (decimal) s.RightLimitAtStartTime;
            var x2 = (decimal) s.EndTime;
            var y2 = (decimal) s.LeftLimitAtEndTime;
            line = $"\\addplot [ color = {color}, thick, )-(, {lineStyle}shorten >=1pt, shorten <=1pt ] coordinates {{ ({x1},{y1}) ({x2},{y2}) }};";
        }
        else
        {
            throw new InvalidCastException();
        }

        return FormattableString.Invariant(line);
    }
}