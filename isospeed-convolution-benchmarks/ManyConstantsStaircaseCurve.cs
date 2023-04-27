using Unipi.Nancy.MinPlusAlgebra;
using Unipi.Nancy.Numerics;

namespace isomorphism_convolutions_bench;

public class ManyConstantsStaircaseCurve : Curve
{
    private List<Rational> TransientParams { get; }
    private List<Rational> PeriodicParams { get; }

    /// <summary>
    /// Builds a left-continuous staircase curve with jumps at each step.
    /// </summary>
    /// <param name="transientParams">Must contain either 0 or n*3 parameters (n >= 0), each > 0.</param>
    /// <param name="periodicParams">Must contain n*3 parameters (n > 0), each > 0.</param>
    public ManyConstantsStaircaseCurve(List<Rational> transientParams, List<Rational> periodicParams)
        : base(BuildCurve(transientParams, periodicParams))
    {
        TransientParams = transientParams;
        PeriodicParams = periodicParams;
    }
    
    private static Curve BuildCurve(List<Rational> transientParams, List<Rational> periodicParams)
    {
        if (transientParams.Count != 0 && transientParams.Count % 3 != 0)
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
            var riseSegmentLenght = NextTransientParam();
            var riseSegmentSlope = NextTransientParam();
            var constantSegmentLenght = NextTransientParam();

            var point1 = new Point(prevTime, prevValue);
            var riseSegment = new Segment(prevTime, prevTime + riseSegmentLenght, prevValue, riseSegmentSlope);
            var point2 = new Point(prevTime + riseSegmentLenght, riseSegment.LeftLimitAtEndTime);
            var constantSegment = Segment.Constant(
                prevTime + riseSegmentLenght,
                prevTime + riseSegmentLenght + constantSegmentLenght, 
                riseSegment.LeftLimitAtEndTime);
            elements.Add(point1);
            elements.Add(riseSegment);
            elements.Add(point2);
            elements.Add(constantSegment);
            prevTime = constantSegment.EndTime;
            prevValue = constantSegment.LeftLimitAtEndTime;
        }

        var pStartTime = prevTime;
        var pStartValue = prevValue;
        var pParamIndex = 0;

        while (pParamIndex < periodicParams.Count)
        {
            var riseSegmentLenght = NextPeriodicParam();
            var riseSegmentSlope = NextPeriodicParam();
            var constantSegmentLenght = NextPeriodicParam();

            var point1 = new Point(prevTime, prevValue);
            var riseSegment = new Segment(prevTime, prevTime + riseSegmentLenght, prevValue, riseSegmentSlope);
            var point2 = new Point(prevTime + riseSegmentLenght, riseSegment.LeftLimitAtEndTime);
            var constantSegment = Segment.Constant(
                prevTime + riseSegmentLenght,
                prevTime + riseSegmentLenght + constantSegmentLenght, 
                riseSegment.LeftLimitAtEndTime);
            elements.Add(point1);
            elements.Add(riseSegment);
            elements.Add(point2);
            elements.Add(constantSegment);
            prevTime = constantSegment.EndTime;
            prevValue = constantSegment.LeftLimitAtEndTime;
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
        return $"new ManyConstantsStaircaseCurve({TransientParams.ToCodeString()}, {PeriodicParams.ToCodeString()})";
    }
}