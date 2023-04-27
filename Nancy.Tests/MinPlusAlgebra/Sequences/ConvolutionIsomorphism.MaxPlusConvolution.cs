using System.Collections.Generic;
using System.Linq;
using Unipi.Nancy.MinPlusAlgebra;
using Unipi.Nancy.Numerics;
using Xunit;
using Xunit.Abstractions;

namespace Unipi.Nancy.Tests.MinPlusAlgebra.Sequences;

public partial class ConvolutionIsomorphism
{
    public static IEnumerable<(Sequence f, Sequence g)> RightContinuousConvolutions()
    {
        var sequences = RightContinuousExamples().Concat(ContinuousExamples());

        var pairs = sequences.SelectMany(
            f => sequences.Select(
                g => (f, g)
            )
        );
        return pairs;
    }

    public static IEnumerable<object[]> RightContinuousConvolutionTestcases()
    {
        foreach (var (f, g) in RightContinuousConvolutions())
        {
            yield return new object[] {f, g};
        }
    }

    [Theory]
    [MemberData(nameof(RightContinuousConvolutionTestcases))]
    public void MaxPlusConvolution_Equivalence(Sequence f, Sequence g)
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

        // conjecture: the result of by-sequence convolution is valid, in the general case, only for the smallest of the lengths
        // in isospeed, we use the mix-and-match th. instead
        var lf = tb_f - ta_f;
        var lg = tb_g - ta_g;
        var length = Rational.Min(lf, lg);
        var cutStart = ta_f + ta_g;
        var cutEnd = ta_f + ta_g + length;

        var direct = Sequence.MaxPlusConvolution(f, g).Cut(cutStart, cutEnd);

        var tb_f_prime = f.GetSegmentBefore(tb_f).IsConstant
            ? f.GetSegmentBefore(tb_f).StartTime
            : tb_f;
        var tb_g_prime = g.GetSegmentBefore(tb_g).IsConstant
            ? g.GetSegmentBefore(tb_g).StartTime
            : tb_g;

        var f_lpi = f.LowerPseudoInverse(false);
        var g_lpi = g.LowerPseudoInverse(false);

        var minp = Sequence.Convolution(f_lpi, g_lpi); // .Cut(vcutStart, vcutEnd, isEndIncluded: true);
        var inversion_raw = minp.UpperPseudoInverse(false);

        output.WriteLine($"var direct = {direct.ToCodeString()};");
        output.WriteLine($"var inversion_raw = {inversion_raw.ToCodeString()};");

        if (tb_f_prime == tb_f && tb_g_prime == tb_g)
        {
            var inversion = inversion_raw.Cut(cutStart, cutEnd);
            Assert.True(Sequence.Equivalent(direct, inversion));
        }
        else
        {
            // todo: does not handle left-open sequences
            var missingElements = new List<Element> { };
            if (tb_f_prime < tb_f)
            {
                var pf = f.GetElementAt(tb_f_prime);
                var sf = f.GetSegmentAfter(tb_f_prime);
                foreach (var eg in g.Elements)
                {
                    missingElements.AddRange(Element.MaxPlusConvolution(pf, eg));
                    missingElements.AddRange(Element.MaxPlusConvolution(sf, eg));
                }
            }

            if (tb_g_prime < tb_g)
            {
                var pg = g.GetElementAt(tb_g_prime);
                var sg = g.GetSegmentAfter(tb_g_prime);
                foreach (var ef in f.Elements)
                {
                    missingElements.AddRange(Element.MaxPlusConvolution(ef, pg));
                    missingElements.AddRange(Element.MaxPlusConvolution(ef, sg));
                }
            }

            var upperEnvelope = missingElements.UpperEnvelope();
            var ext = upperEnvelope.ToSequence(
                fillFrom: upperEnvelope.First().StartTime, 
                fillTo: upperEnvelope.Last().EndTime, 
                fillWith: Rational.MinusInfinity
            );
            var reconstructed = Sequence.Maximum(ext, inversion_raw, false)
                .Cut(cutStart, cutEnd);
            Assert.True(Sequence.Equivalent(direct, reconstructed));
        }
    }

}