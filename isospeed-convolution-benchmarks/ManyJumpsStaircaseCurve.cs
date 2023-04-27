using Unipi.Nancy.MinPlusAlgebra;
using Unipi.Nancy.Numerics;

namespace isomorphism_convolutions_bench;

public class ManyJumpsStaircaseCurve : Curve
{
    private List<Rational> TransientParams { get; }
    private List<Rational> PeriodicParams { get; }

    /// <summary>
    /// Builds a left-continuous staircase curve with jumps at each step.
    /// </summary>
    /// <param name="transientParams">Must contain either 0 or 2 + n*3 parameters (n >= 0), each > 0.</param>
    /// <param name="periodicParams">Must contain n*3 parameters (n > 0), each > 0.</param>
    public ManyJumpsStaircaseCurve(List<Rational> transientParams, List<Rational> periodicParams)
        : base(BuildCurve(transientParams, periodicParams))
    {
        TransientParams = transientParams;
        PeriodicParams = periodicParams;
    }
    
    private static Curve BuildCurve(List<Rational> transientParams, List<Rational> periodicParams)
    {
        if (transientParams.Count != 0 && (transientParams.Count - 2) % 3 != 0)
            throw new ArgumentException($"Invalid number of transientParams: {transientParams.Count}");
        if (periodicParams.Count % 3 != 0)
            throw new ArgumentException($"Invalid number of periodicParams: {transientParams.Count}");
        if (transientParams.Any(p => p <= 0) || periodicParams.Any(p => p <= 0))
            throw new ArgumentException($"All params must be strictly positive");
        
        var elements = new List<Element>(); 
        
        var prevTime = Rational.Zero;
        var prevValue = Rational.Zero;
        var tParamIndex = 0;
        while (tParamIndex < transientParams.Count)
        {
            if (tParamIndex > 0)
            {
                var jumpHeight = NextTransientParam();
                var segmentLenght = NextTransientParam();
                var segmentSlope = NextTransientParam();

                var point = new Point(prevTime, prevValue);
                var segment = new Segment(prevTime, prevTime + segmentLenght, prevValue + jumpHeight, segmentSlope);
                elements.Add(point);
                elements.Add(segment);
                prevTime = segment.EndTime;
                prevValue = segment.LeftLimitAtEndTime;
            }
            else
            {
                var segmentLenght = NextTransientParam();
                var segmentSlope = NextTransientParam();
                
                var point = new Point(prevTime, prevValue);
                var segment = new Segment(prevTime, prevTime + segmentLenght, prevValue, segmentSlope);
                elements.Add(point);
                elements.Add(segment);
                prevTime = segment.EndTime;
                prevValue = segment.LeftLimitAtEndTime;
            }
        }

        var pStartTime = prevTime;
        var pStartValue = prevValue;
        var pParamIndex = 0;

        while (pParamIndex < periodicParams.Count)
        {
            var jumpHeight = NextPeriodicParam();
            var segmentLenght = NextPeriodicParam();
            var segmentSlope = NextPeriodicParam();

            var point = new Point(prevTime, prevValue);
            var segment = new Segment(prevTime, prevTime + segmentLenght, prevValue + jumpHeight, segmentSlope);
            elements.Add(point);
            elements.Add(segment);
            prevTime = segment.EndTime;
            prevValue = segment.LeftLimitAtEndTime;
        }

        var T = pStartTime;
        var d = prevTime - pStartTime;
        var c = prevValue - pStartValue;
        var sequence = elements.ToSequence();

        return new Curve(sequence, T, d, c);
        
        Rational NextTransientParam()
        {
            var v = transientParams[tParamIndex];
            tParamIndex++;
            return v;
        }
        
        Rational NextPeriodicParam()
        {
            var v = periodicParams[pParamIndex];
            pParamIndex++;
            return v;
        }
    }

    public override string ToString()
    {
        return ToCodeString();
    }

    public override string ToCodeString(bool formatted = false, int indentation = 0)
    {
        return $"new ManyJumpsStaircaseCurve({TransientParams.ToCodeString()}, {PeriodicParams.ToCodeString()})";
    }
}