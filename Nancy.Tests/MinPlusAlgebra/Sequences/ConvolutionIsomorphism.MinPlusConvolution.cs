using System;
using System.Collections.Generic;
using System.Linq;
using Unipi.Nancy.MinPlusAlgebra;
using Unipi.Nancy.Numerics;
using Xunit;
using Xunit.Abstractions;

namespace Unipi.Nancy.Tests.MinPlusAlgebra.Sequences;

public partial class ConvolutionIsomorphism
{
    public static IEnumerable<(Sequence f, Sequence g)> LeftContinuousConvolutions()
    {
        var sequences = LeftContinuousExamples().Concat(ContinuousExamples());

        var pairs = sequences.SelectMany(
            f => sequences.Select(
                g => (f, g)
            )
        );
        return pairs;
    }

    public static IEnumerable<object[]> LeftContinuousConvolutionTestcases()
    {
        foreach (var (f, g) in LeftContinuousConvolutions())
        {
            yield return new object[] {f, g};
        }
    }

    [Theory]
    [MemberData(nameof(LeftContinuousConvolutionTestcases))]
    public void MinPlusConvolution_Equivalence(Sequence f, Sequence g)
    {
        output.WriteLine($"var f = {f.ToCodeString()};");
        output.WriteLine($"var g = {g.ToCodeString()};");

        // for simplicity, we only support this case for now
        Assert.True(f.IsLeftClosed);
        Assert.True(g.IsLeftClosed);
        Assert.True(f.IsRightOpen);
        Assert.True(g.IsRightOpen);

        var ta_f = f.DefinedFrom;
        var ta_g = g.DefinedFrom;
        var tb_f = f.DefinedUntil;
        var tb_g = g.DefinedUntil;

        // conjecture: the result of by-sequence convolution is valid, in the general case, only for the smaller of the lengths
        // in isospeed, we use the mix-and-match th. instead
        var lf = tb_f - ta_f;
        var lg = tb_g - ta_g;
        var length = Rational.Min(lf, lg);
        var cutStart = ta_f + ta_g;
        var cutEnd = ta_f + ta_g + length;

        var direct = Sequence.Convolution(f, g).Cut(cutStart, cutEnd);

        var ta_f_prime = f.IsRightContinuousAt(ta_f) && f.GetSegmentAfter(ta_f).IsConstant
            ? f.GetSegmentAfter(ta_f).EndTime
            : ta_f;
        var ta_g_prime = g.IsRightContinuousAt(ta_g) && g.GetSegmentAfter(ta_g).IsConstant
            ? g.GetSegmentAfter(ta_g).EndTime
            : ta_g;

        var f_upi = f.UpperPseudoInverse(false);
        var g_upi = g.UpperPseudoInverse(false);

        var maxp = Sequence.MaxPlusConvolution(f_upi, g_upi); //.Cut(vcutStart, vcutEnd, isEndIncluded: true);
        var inversion_raw = maxp.LowerPseudoInverse(false);

        output.WriteLine($"var direct = {direct.ToCodeString()};");
        output.WriteLine($"var inversion_raw = {inversion_raw.ToCodeString()};");

        if (ta_f_prime == ta_f && ta_g_prime == ta_g)
        {
            var inversion = inversion_raw.Cut(cutStart, cutEnd);
            Assert.True(Sequence.Equivalent(direct, inversion));
        }
        else
        {
            // todo: does not handle left-open sequences
            var ext = Sequence.Constant(
                f.ValueAt(ta_f) + g.ValueAt(ta_g), 
                ta_f + ta_g,
                ta_f_prime + ta_g_prime
            );
            var reconstructed = Sequence.Minimum(ext, inversion_raw, false)
                .Cut(cutStart, cutEnd);
            Assert.True(Sequence.Equivalent(direct, reconstructed));
        }
    }

    public static List<(Sequence a, Sequence b, Rational cutEnd)> SingleCutConvolutions()
    {
        var testcases = new List<(Sequence a, Sequence b, Rational cutEnd)>()
        {
            (
                new Sequence(new List<Element>{ new Point(2,1), new Segment(2,3,1,0), new Point(3,1), new Segment(3,4,1,1), new Point(4,2), new Segment(4,5,2,0), new Point(5,2), new Segment(5,6,2,1), new Point(6,3), new Segment(6,7,3,0), new Point(7,3), new Segment(7,8,3,1), new Point(8,4), new Segment(8,9,4,0), new Point(9,4), new Segment(9,10,4,1), new Point(10,5), new Segment(10,11,5,0), new Point(11,5), new Segment(11,12,5,1), new Point(12,6), new Segment(12,13,6,0), new Point(13,6), new Segment(13,14,6,1) }),
                new Sequence(new List<Element>{ new Point(0,2), new Segment(0,1,2,1), new Point(1,3), new Segment(1,3,3,0), new Point(3,3), new Segment(3,4,3,1), new Point(4,4), new Segment(4,6,4,0), new Point(6,4), new Segment(6,7,4,1), new Point(7,5), new Segment(7,9,5,0), new Point(9,5), new Segment(9,10,5,1), new Point(10,6), new Segment(10,12,6,0) }),
                14
            )
        };
        return testcases;
    }

    public static IEnumerable<object[]> SingleCutConvolutionTestcases()
    {
        foreach (var (a, b, cutEnd) in SingleCutConvolutions())
        {
            yield return new object[] {a, b, cutEnd};
        }
    }

    [Theory]
    [MemberData(nameof(SingleCutConvolutionTestcases))]
    public void MinPlusConvolution_SingleCut_Equivalence(Sequence f, Sequence g, Rational cutEnd)
    {
        output.WriteLine($"var f = {f.ToCodeString()};");
        output.WriteLine($"var g = {g.ToCodeString()};");

        var settings = ComputationSettings.Default() with {UseBySequenceConvolutionIsomorphismOptimization = false};

        // for simplicity, we only support this case for now
        Assert.True(f.IsLeftClosed);
        Assert.True(g.IsLeftClosed);
        Assert.True(f.IsRightOpen);
        Assert.True(g.IsRightOpen);

        var ta_f = f.DefinedFrom;
        var ta_g = g.DefinedFrom;
        var tb_f = f.DefinedUntil;
        var tb_g = g.DefinedUntil;

        // conjecture: the result of by-sequence convolution is valid, in the general case, only for the smaller of the lengths
        // in isospeed, we use the mix-and-match th. instead
        var lf = tb_f - ta_f;
        var lg = tb_g - ta_g;
        var length = Rational.Min(lf, lg);
        var cutStart = ta_f + ta_g;
        var equivCutEnd = ta_f + ta_g + length;
        var cutCeiling = Rational.PlusInfinity;

        if (cutEnd > equivCutEnd)
            throw new InvalidOperationException();

        var direct = Sequence.Convolution(f, g, settings).Cut(cutStart, cutEnd);

        var ta_f_prime = f.IsRightContinuousAt(ta_f) && f.GetSegmentAfter(ta_f).IsConstant
            ? f.GetSegmentAfter(ta_f).EndTime
            : ta_f;
        var ta_g_prime = g.IsRightContinuousAt(ta_g) && g.GetSegmentAfter(ta_g).IsConstant
            ? g.GetSegmentAfter(ta_g).EndTime
            : ta_g;

        var f_upi = f.UpperPseudoInverse(false);
        var g_upi = g.UpperPseudoInverse(false);

        var maxp = Sequence.MaxPlusConvolution(
            f_upi, g_upi,
            cutEnd: cutCeiling, cutCeiling: cutEnd,
            isEndIncluded: true, isCeilingIncluded: true,
            settings: settings);
        var inversion_raw = maxp.LowerPseudoInverse(false);

        output.WriteLine($"var direct = {direct.ToCodeString()};");
        output.WriteLine($"var inversion_raw = {inversion_raw.ToCodeString()};");

        if (ta_f_prime == ta_f && ta_g_prime == ta_g)
        {
            var inversion = inversion_raw.Cut(cutStart, cutEnd);
            Assert.True(Sequence.Equivalent(direct, inversion));
        }
        else
        {
            // todo: does not handle left-open sequences
            var ext = Sequence.Constant(
                f.ValueAt(ta_f) + g.ValueAt(ta_g), 
                ta_f + ta_g,
                ta_f_prime + ta_g_prime
            );
            var reconstructed = Sequence.Minimum(ext, inversion_raw, false)
                .Cut(cutStart, cutEnd);
            Assert.True(Sequence.Equivalent(direct, reconstructed));
        }
    }
}