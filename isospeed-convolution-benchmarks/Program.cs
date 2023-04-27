// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using System.Globalization;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnostics.Windows;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Diagnostics.Windows.Configs;
using BenchmarkDotNet.Reports;
using isomorphism_convolutions_bench;
using Unipi.Nancy.MinPlusAlgebra;
using Unipi.Nancy.NetworkCalculus;
using Unipi.Nancy.Numerics;

#if SANITY_CHECK
SanityCheck(Globals.TEST_COUNT);
#endif

BenchmarkSwitcher
    .FromAssembly(typeof(Program).Assembly)
    .Run(args,
        DefaultConfig.Instance
            .WithSummaryStyle(SummaryStyle.Default.WithMaxParameterColumnWidth(10000))
            .WithCultureInfo(CultureInfo.InvariantCulture)
            #if PROFILE
            .AddDiagnoser(new EtwProfiler())
            #endif
    );

void SanityCheck(int n = 100)
{
    Console.WriteLine("Running sanity check");
    
    Console.WriteLine("IsoConvolutionBalancedStaircaseBenchmarks");
    var prevICBSBTestsCount = IsoConvolutionBalancedStaircaseBenchmarks.TestsCount;
    IsoConvolutionBalancedStaircaseBenchmarks.TestsCount = n;
    var icbsb = new IsoConvolutionBalancedStaircaseBenchmarks{ UseParallelism = true };
    foreach (var pair in IsoConvolutionBalancedStaircaseBenchmarks.StairCurvePairs)
    {
        icbsb.Pair = pair;
        try{
            var minp = icbsb.Direct();
            var iso = icbsb.Isospeed();
            var maxp = icbsb.Inversion();
            if(!Curve.Equivalent(minp, iso) || !Curve.Equivalent(maxp, iso))
            {
                Console.WriteLine();
                Console.WriteLine($"SANITY FAIL:");
                Console.WriteLine(pair.a.ToCodeString());
                Console.WriteLine(pair.b.ToCodeString());
            }
            else
            {
                Console.Write(".");
            }
        }
        catch(Exception ex)
        {
            Console.WriteLine();
            Console.WriteLine(pair.a.ToCodeString());
            Console.WriteLine(pair.b.ToCodeString());
            throw;
        }
    }
    IsoConvolutionBalancedStaircaseBenchmarks.TestsCount = prevICBSBTestsCount;
    Console.WriteLine();
    Console.WriteLine();

    Console.WriteLine("IsoConvolutionHorizontalStaircaseBenchmarks");
    var prevICHSBTestsCount = IsoConvolutionHorizontalStaircaseBenchmarks.TestsCount;
    IsoConvolutionHorizontalStaircaseBenchmarks.TestsCount = n;
    var ichsb = new IsoConvolutionHorizontalStaircaseBenchmarks{ UseParallelism = true };
    foreach (var pair in IsoConvolutionHorizontalStaircaseBenchmarks.StairCurvePairs)
    {
        ichsb.Pair = pair;
        try{
            var minp = ichsb.Direct();
            var iso = ichsb.Isospeed();
            var maxp = ichsb.Inversion();
            if(!Curve.Equivalent(minp, iso) || !Curve.Equivalent(maxp, iso))
            {
                Console.WriteLine();
                Console.WriteLine($"SANITY FAIL:");
                Console.WriteLine(pair.a.ToCodeString());
                Console.WriteLine(pair.b.ToCodeString());
            }
            else
            {
                Console.Write(".");
            }
        }
        catch(Exception ex)
        {
            Console.WriteLine();
            Console.WriteLine(pair.a.ToCodeString());
            Console.WriteLine(pair.b.ToCodeString());
            throw;
        }
    }
    IsoConvolutionHorizontalStaircaseBenchmarks.TestsCount = prevICHSBTestsCount;
    Console.WriteLine();
    Console.WriteLine();

    Console.WriteLine("IsoConvolutionVerticalStaircaseBenchmarks");
    var prevICVSBTestsCount = IsoConvolutionVerticalStaircaseBenchmarks.TestsCount;
    IsoConvolutionVerticalStaircaseBenchmarks.TestsCount = n;
    var icvsb = new IsoConvolutionVerticalStaircaseBenchmarks{ UseParallelism = true };
    foreach (var pair in IsoConvolutionVerticalStaircaseBenchmarks.StairCurvePairs)
    {
        icvsb.Pair = pair;
        try{
            var minp = icvsb.Direct();
            var iso = icvsb.Isospeed();
            var maxp = icvsb.Inversion();
            if(!Curve.Equivalent(minp, iso) || !Curve.Equivalent(maxp, iso))
            {
                Console.WriteLine();
                Console.WriteLine($"SANITY FAIL:");
                Console.WriteLine(pair.a.ToCodeString());
                Console.WriteLine(pair.b.ToCodeString());
            }
            else
            {
                Console.Write(".");
            }
        }
        catch(Exception ex)
        {
            Console.WriteLine();
            Console.WriteLine(pair.a.ToCodeString());
            Console.WriteLine(pair.b.ToCodeString());
            throw;
        }
    }
    IsoConvolutionVerticalStaircaseBenchmarks.TestsCount = prevICVSBTestsCount;
    Console.WriteLine();
    Console.WriteLine();
    
    Console.WriteLine("IsoConvolutionHorizontalKTradeoffStaircaseBenchmarks");
    var prevICHKTSBTestsCount = IsoConvolutionHorizontalKTradeoffStaircaseBenchmarks.TestsCount;
    IsoConvolutionHorizontalKTradeoffStaircaseBenchmarks.TestsCount = n;
    var ichktsb = new IsoConvolutionHorizontalKTradeoffStaircaseBenchmarks{ UseParallelism = true };
    foreach (var pair in IsoConvolutionHorizontalKTradeoffStaircaseBenchmarks.StairCurvePairs)
    {
        ichktsb.Pair = pair;
        try{
            var minp = ichktsb.Direct();
            var iso = ichktsb.Isospeed();
            var maxp = ichktsb.Inversion();
            if(!Curve.Equivalent(minp, iso) || !Curve.Equivalent(maxp, iso))
            {
                Console.WriteLine();
                Console.WriteLine($"SANITY FAIL:");
                Console.WriteLine(pair.a.ToCodeString());
                Console.WriteLine(pair.b.ToCodeString());
            }
            else
            {
                Console.Write(".");
            }
        }
        catch(Exception ex)
        {
            Console.WriteLine();
            Console.WriteLine(pair.a.ToCodeString());
            Console.WriteLine(pair.b.ToCodeString());
            throw;
        }
    }
    IsoConvolutionVerticalStaircaseBenchmarks.TestsCount = prevICVSBTestsCount;
    Console.WriteLine();
    Console.WriteLine();
}

public static class Globals {
    public const int TEST_COUNT = 100;

    public const int RNG_SEED = 4321;

    public const int RNG_MAX = 100;

    public const FILTER_VARIATION_ENUM FILTER_VARIATION = FILTER_VARIATION_ENUM.NO_FILTER;
    
    public enum FILTER_VARIATION_ENUM {
        NO_FILTER,
        FILTER_LARGER,
        FILTER_SMALLER
    };

    public const int LARGE_VARIATION_MULTIPLIER = 100;

    public const FILTER_EXTENSION_ENUM FILTER_EXTENSION = FILTER_EXTENSION_ENUM.FILTER_SMALLER;
    
    public enum FILTER_EXTENSION_ENUM {
        NO_FILTER,
        FILTER_LARGER,
        FILTER_SMALLER
    };

    public const int LARGE_EXTENSION_LCM_THRESHOLD = 500;

    public static bool FilterByVariation(Curve f, Curve g)
    {
        if(FILTER_VARIATION == FILTER_VARIATION_ENUM.NO_FILTER)
            return true;

        var d_f = f.PseudoPeriodLength;
        var d_g = g.PseudoPeriodLength;
        var lcm_d = Rational.LeastCommonMultiple(d_f, d_g);
        var d_growth = Rational.Max(lcm_d / d_f, lcm_d / d_g);

        var c_f = f.PseudoPeriodHeight;
        var c_g = g.PseudoPeriodHeight;
        var lcm_c = Rational.LeastCommonMultiple(c_f, c_g);
        var c_growth = Rational.Max(lcm_c / c_f, lcm_c / c_g);

        if(FILTER_VARIATION == FILTER_VARIATION_ENUM.FILTER_LARGER)
            return d_growth >= LARGE_VARIATION_MULTIPLIER * c_growth || c_growth >= LARGE_VARIATION_MULTIPLIER * d_growth;
        else
            return d_growth <= LARGE_VARIATION_MULTIPLIER * c_growth || c_growth <= LARGE_VARIATION_MULTIPLIER * d_growth;
    }

    public static bool FilterByLargestExtension(Curve f, Curve g)
    {
        if(FILTER_EXTENSION == FILTER_EXTENSION_ENUM.NO_FILTER)
            return true;

        var d_f = f.PseudoPeriodLength;
        var d_g = g.PseudoPeriodLength;
        var lcm_d = Rational.LeastCommonMultiple(d_f, d_g);
        var d_growth = Rational.Max(lcm_d / d_f, lcm_d / d_g);

        var c_f = f.PseudoPeriodHeight;
        var c_g = g.PseudoPeriodHeight;
        var lcm_c = Rational.LeastCommonMultiple(c_f, c_g);
        var c_growth = Rational.Max(lcm_c / c_f, lcm_c / c_g);

        if(FILTER_EXTENSION == FILTER_EXTENSION_ENUM.FILTER_LARGER)
            return Rational.Max(d_growth, c_growth) >= LARGE_EXTENSION_LCM_THRESHOLD;
        else
            return Rational.Max(d_growth, c_growth) <= LARGE_EXTENSION_LCM_THRESHOLD;
    }
    
    public static ComputationSettings NoIsospeedSettings = ComputationSettings.Default() with
    {
        UseParallelism = false,
        UseConvolutionIsomorphismOptimization = false,
        UseBySequenceConvolutionIsomorphismOptimization = false,
    };
    
    public static ComputationSettings IsospeedSettings = ComputationSettings.Default() with
    {
        UseParallelism = false,
        UseConvolutionIsomorphismOptimization = true,
        UseBySequenceConvolutionIsomorphismOptimization = true,
    };
}

#if PROFILE
[EtwProfiler]
#endif
[SimpleJob(warmupCount: 0, targetCount: 1)]
public class IsoConvolutionBalancedStaircaseBenchmarks
{
    public static int TestsCount = Globals.TEST_COUNT;
    public static int MaxNumeratorDenominator = Globals.RNG_MAX;
    public static int RngSeed = Globals.RNG_SEED;

    [ParamsSource(nameof(StairCurvePairs))]
    public (Curve, Curve) Pair { get; set; }
    
    public bool UseParallelism { get; set; } = false;
    
    [Benchmark]
    public Curve Direct()
    {
        var (a, b) = Pair;
        var result = Curve.Convolution(a, b, Globals.NoIsospeedSettings with { UseParallelism = UseParallelism});
        return result;
    }
    
    // [Benchmark(Baseline = true)]
    [Benchmark]
    public Curve Isospeed()
    {
        var (a, b) = Pair;
        var result = Curve.Convolution(a, b, Globals.IsospeedSettings with { UseParallelism = UseParallelism});
        return result;
    }
    
    [Benchmark]
    public Curve Inversion()
    {
        var (a, b) = Pair;
        var a_upi = a.UpperPseudoInverse();
        var b_upi = b.UpperPseudoInverse();
        var result = Curve.MaxPlusConvolution(a_upi, b_upi, Globals.NoIsospeedSettings with { UseParallelism = UseParallelism}).LowerPseudoInverse();
        return result;
    }
    
    public static IEnumerable<(Curve a, Curve b)> StairCurvePairs => 
        RngStairCurves(RngSeed, MaxNumeratorDenominator)
            .Chunk(2)
            .Select(c => (f: (Curve) c[0], g: (Curve) c[1]))
            .Where(p => Globals.FilterByLargestExtension(p.f, p.g))
            .Where(p => Globals.FilterByVariation(p.f, p.g))
            .Take(TestsCount);

    public static IEnumerable<Curve> RngStairCurves(int seed, int max = 1000)
    {
        using var rngRationals = RngRationals(seed, max).GetEnumerator();
        rngRationals.MoveNext();
        while (true)
        {
            var a = rngRationals.Current;
            rngRationals.MoveNext();
            var b = rngRationals.Current;
            rngRationals.MoveNext();
            // yield return new StairCurve(a, b);
            yield return new ConstantJumpsStaircaseCurve(
                new List<Rational>(),
                new List<Rational>{a, b}
            );
        }
    }

    public static IEnumerable<Rational> RngRationals(int seed, int max = 1000)
    {
        var rng = new Random(seed);
        while (true)
        {
            var n = rng.Next(max) + 1;
            // var d = rng.Next(max) + 1;
            yield return new Rational(n, 1);
        }
    }
}

#if PROFILE
[EtwProfiler]
#endif
[SimpleJob(warmupCount: 0, targetCount: 1)]
public class IsoConvolutionHorizontalStaircaseBenchmarks
{
    public static int TestsCount = Globals.TEST_COUNT;
    public static int MaxNumeratorDenominator = Globals.RNG_MAX;
    public static int RngSeed = Globals.RNG_SEED;

    [ParamsSource(nameof(StairCurvePairs))]
    public (Curve, Curve) Pair { get; set; }
    
    public bool UseParallelism { get; set; } = false;
    
    [Benchmark]
    public Curve Direct()
    {
        var (a, b) = Pair;
        var result = Curve.Convolution(a, b, Globals.NoIsospeedSettings with { UseParallelism = UseParallelism});
        return result;
    }
    
    // [Benchmark(Baseline = true)]
    [Benchmark]
    public Curve Isospeed()
    {
        var (a, b) = Pair;
        var result = Curve.Convolution(a, b, Globals.IsospeedSettings with { UseParallelism = UseParallelism});
        return result;
    }
    
    [Benchmark]
    public Curve Inversion()
    {
        var (a, b) = Pair;
        var a_upi = a.UpperPseudoInverse();
        var b_upi = b.UpperPseudoInverse();
        var result = Curve.MaxPlusConvolution(a_upi, b_upi, Globals.NoIsospeedSettings with { UseParallelism = UseParallelism}).LowerPseudoInverse();
        return result;
    }
   
    public static IEnumerable<(Curve a, Curve b)> StairCurvePairs => 
        RngStairCurves(RngSeed, MaxNumeratorDenominator)
            .Chunk(2)
            .Select(c => (f: (Curve) c[0], g: (Curve) c[1]))
            .Where(p => Globals.FilterByLargestExtension(p.f, p.g))
            .Where(p => Globals.FilterByVariation(p.f, p.g))
            .Take(TestsCount);

    public static IEnumerable<Curve> RngStairCurves(int seed, int max = 1000)
    {
        using var rngRationals = RngRationals(seed, max).GetEnumerator();
        rngRationals.MoveNext();
        while (true)
        {
            #if true
            var parameters = rngRationals.TakeNext(6);
            yield return new ManyConstantsStaircaseCurve(
                new List<Rational>(),
                parameters
            );
            #else
            var a = rngRationals.Current;
            rngRationals.MoveNext();
            var b = rngRationals.Current;
            rngRationals.MoveNext();
            var c = rngRationals.Current;
            rngRationals.MoveNext();
            var d = rngRationals.Current;
            rngRationals.MoveNext();
            var e = rngRationals.Current;
            rngRationals.MoveNext();
            var f = rngRationals.Current;
            rngRationals.MoveNext();
            yield return GetHorizontalStairCurve(a, b, c, d, e, f);
            #endif
        }
    }

    public static Curve GetHorizontalStairCurve(Rational a, Rational b, Rational c, Rational d, Rational e, Rational f)
    {
        var v1 = a * b;
        var v2 = v1 + d * e;

        var curve = new Curve(
            new Sequence(
                new List<Element>{
                    Point.Origin(),
                    new Segment(0, a, 0, b),
                    new Point(a, v1),
                    new Segment(a, a + c, v1, 0),
                    new Point(a + c, v1),
                    new Segment(a + c, a + c + d, v1, e),
                    new Point(a + c + d, v2),
                    new Segment(a + c + d, a + c + d + f, v2, 0)
                }
            ),
            0,
            a + c + d + f,
            v2
        );
        return curve;
    }

    public static IEnumerable<Rational> RngRationals(int seed, int max = 1000)
    {
        var rng = new Random(seed);
        while (true)
        {
            var n = rng.Next(max) + 1;
            // var d = rng.Next(max) + 1;
            yield return new Rational(n, 1);
        }
    }
}

#if PROFILE
[EtwProfiler]
#endif
[SimpleJob(warmupCount: 0, targetCount: 1)]
public class IsoConvolutionVerticalStaircaseBenchmarks
{
    public static int TestsCount = Globals.TEST_COUNT;
    public static int MaxNumeratorDenominator = Globals.RNG_MAX;
    public static int RngSeed = Globals.RNG_SEED;

    [ParamsSource(nameof(StairCurvePairs))]
    public (Curve, Curve) Pair { get; set; }
    
    public bool UseParallelism { get; set; } = false;
    
    [Benchmark]
    public Curve Direct()
    {
        var (a, b) = Pair;
        var result = Curve.Convolution(a, b, Globals.NoIsospeedSettings with { UseParallelism = UseParallelism});
        return result;
    }
    
    // [Benchmark(Baseline = true)]
    [Benchmark]
    public Curve Isospeed()
    {
        var (a, b) = Pair;
        var result = Curve.Convolution(a, b, Globals.IsospeedSettings with { UseParallelism = UseParallelism});
        return result;
    }
    
    [Benchmark]
    public Curve Inversion()
    {
        var (a, b) = Pair;
        var a_upi = a.UpperPseudoInverse();
        var b_upi = b.UpperPseudoInverse();
        var result = Curve.MaxPlusConvolution(a_upi, b_upi, Globals.NoIsospeedSettings with { UseParallelism = UseParallelism}).LowerPseudoInverse();
        return result;
    }
   
    public static IEnumerable<(Curve a, Curve b)> StairCurvePairs => 
        RngStairCurves(RngSeed, MaxNumeratorDenominator)
            .Chunk(2)
            .Select(c => (f: (Curve) c[0], g: (Curve) c[1]))
            .Where(p => Globals.FilterByLargestExtension(p.f, p.g))
            .Where(p => Globals.FilterByVariation(p.f, p.g))
            .Take(TestsCount);

    public static IEnumerable<Curve> RngStairCurves(int seed, int max = 1000)
    {
        using var rngRationals = RngRationals(seed, max).GetEnumerator();
        rngRationals.MoveNext();
        while (true)
        {
            #if true
            var tParams = rngRationals.TakeNext(2);
            var pParams = rngRationals.TakeNext(6);
            yield return new ManyJumpsStaircaseCurve(
                tParams,
                pParams
            );
            #else
            var a = rngRationals.Current;
            rngRationals.MoveNext();
            var b = rngRationals.Current;
            rngRationals.MoveNext();
            var c = rngRationals.Current;
            rngRationals.MoveNext();
            var d = rngRationals.Current;
            rngRationals.MoveNext();
            var e = rngRationals.Current;
            rngRationals.MoveNext();
            var f = rngRationals.Current;
            rngRationals.MoveNext();
            var g = rngRationals.Current;
            rngRationals.MoveNext();
            var h = rngRationals.Current;
            rngRationals.MoveNext();
            yield return GetVerticalStairCurve(a, b, c, d, e, f, g, h);
            #endif
        }
    }

    public static Curve GetVerticalStairCurve(Rational a, Rational b, Rational c, Rational d, Rational e, Rational f, Rational g, Rational h)
    {
        var v1 = a * b;
        var v2 = v1 + c;
        var v3 = v2 + d * e;
        var v4 = v3 + f;

        var curve = new Curve(
            new Sequence(
                new List<Element>{
                    Point.Origin(),
                    new Segment(0, a, 0, b),
                    new Point(a, v1),
                    new Segment(a, a + d, v2, e),
                    new Point(a + d, v3),
                    new Segment(a + d, a + d + g, v4, h)
                }
            ),
            a,
            d + g,
            c + d * e + f + g * h
        );
        return curve;
    }

    public static IEnumerable<Rational> RngRationals(int seed, int max = 1000)
    {
        var rng = new Random(seed);
        while (true)
        {
            var n = rng.Next(max) + 1;
            // var d = rng.Next(max) + 1;
            yield return new Rational(n, 1);
        }
    }
}

#if PROFILE
[EtwProfiler]
#endif
[SimpleJob(warmupCount: 0, targetCount: 1)]
public class IsoConvolutionHorizontalKTradeoffStaircaseBenchmarks
{
    public static int TestsCount = Globals.TEST_COUNT;
    public static int MaxNumeratorDenominator = Globals.RNG_MAX;
    public static int RngSeed = Globals.RNG_SEED;

    [ParamsSource(nameof(StairCurvePairs))]
    public (Curve, Curve) Pair { get; set; }
    
    public bool UseParallelism { get; set; } = false;

    [Benchmark]
    public Curve Direct()
    {
        var (a, b) = Pair;
        var result = Curve.Convolution(a, b, Globals.NoIsospeedSettings with { UseParallelism = UseParallelism});
        return result;
    }

    // [Benchmark(Baseline = true)]
    [Benchmark]
    public Curve Isospeed()
    {
        var (a, b) = Pair;
        var result = Curve.Convolution(a, b, Globals.IsospeedSettings with { UseParallelism = UseParallelism});
        return result;
    }

    [Benchmark]
    public Curve Inversion()
    {
        var (a, b) = Pair;
        var a_upi = a.UpperPseudoInverse();
        var b_upi = b.UpperPseudoInverse();
        var result = Curve.MaxPlusConvolution(a_upi, b_upi, Globals.NoIsospeedSettings with { UseParallelism = UseParallelism}).LowerPseudoInverse();
        return result;
    }

    public static IEnumerable<(Curve a, Curve b)> StairCurvePairs =>
        RngStairCurves(RngSeed, MaxNumeratorDenominator)
            // .Chunk(2)
            // .Select(c => (f: (Curve) c[0], g: (Curve) c[1]))
            .Where(p => Globals.FilterByLargestExtension(p.f, p.g))
            .Where(p => Globals.FilterByVariation(p.f, p.g))
            .Take(TestsCount);

    public static IEnumerable<(Curve f, Curve g)> RngStairCurves(int seed, int max = 1000)
    {
        using var rngRationals = RngRationals(seed, max).GetEnumerator();
        rngRationals.MoveNext();
        while (true)
        {
            var fs1l = rngRationals.TakeNext();
            var fs1s = rngRationals.TakeNext();
            var fc1l = rngRationals.TakeNext();
            var fs2l = rngRationals.TakeNext();
            var fs2s = rngRationals.TakeNext();
            var fc2l = rngRationals.TakeNext();

            var d_f = fs1l + fc1l + fs2l + fc2l;
            var c_f = fs1l * fs1s + fs2l * fs2s;

            var kc1 = rngRationals.TakeNext().Numerator;
            var kc2 = rngRationals.TakeNext().Numerator;
            var kcf = kc1 < kc2 ? kc1 : kc2;
            var kcg = kc1 < kc2 ? kc2 : kc1;

            var c_g = (c_f * kcf) / kcg;

            var kd1 = rngRationals.TakeNext().Numerator;
            var kd2 = rngRationals.TakeNext().Numerator;
            var kdf = kd1 < kd2 ? kd2 : kd1;
            var kdg = kd1 < kd2 ? kd1 : kd2;

            var d_g = (d_f * kdf) / kdg;

            var gs1l = 1 * d_g / 10;
            var gs1s = (c_g / 3) / gs1l;
            var gc1l = 2 * d_g / 10;
            var gs2l = 4 * d_g / 10;
            var gs2s = (2 * c_g / 3) / gs2l;
            var gc2l = 3 * d_g / 10;

            var f = new ManyConstantsStaircaseCurve(
                new List<Rational>(),
                new List<Rational> { fs1l, fs1s, fc1l, fs2l, fs2s, fc2l }
            );
            var g = new ManyConstantsStaircaseCurve(
                new List<Rational>(),
                new List<Rational> { gs1l, gs1s, gc1l, gs2l, gs2s, gc2l }
            );
            yield return (f, g);
        }
    }
    
    public static IEnumerable<Rational> RngRationals(int seed, int max = 1000)
    {
        var rng = new Random(seed);
        while (true)
        {
            var n = rng.Next(max) + 1;
            // var d = rng.Next(max) + 1;
            yield return new Rational(n, 1);
        }
    }
}

public static class RngRationalsExtensions
{
    public static List<Rational> TakeNext(this IEnumerator<Rational> source, int n)
    {
        var rationals = new List<Rational>();
        for (int i = 0; i < n; i++)
        {
            if(source.MoveNext())
                rationals.Add(source.Current);
        }

        return rationals;
    }
    
    public static Rational TakeNext(this IEnumerator<Rational> source)
    {
        if(source.MoveNext())
            return source.Current;
        else
            throw new InvalidOperationException();
    }
}
