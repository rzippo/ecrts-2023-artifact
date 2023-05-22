# *Isospeed*: Improving (min,+) Convolution by Exploiting (min,+)/(max,+) Isomorphism - **Artifact**
*Raffaele Zippo*, *Paul Nikolaus* and *Giovanni Stea*

## Contents

This artifact provides the necessary information for a third party to repeat the experiments described in Section 4 of [1].
This artifact is composed of 

* the source code of the Nancy library [2], extended with the (min,+) isospeed algorithm discussed in the paper
* a benchmark program that runs the experiments whose results are discussed in the paper
* a parser program that makes TikZ figures from the results produced by the benchmarks

The algorithmic implementation provided here is the same used to generate the results discussed in the paper.
This will slightly differ from what we will soon publish on https://github.com/rzippo/nancy - we found some useful tweaks in the last two months.

## Claims

In [1], we present a new method for (min,+) convolution called isospeed, which leverages the isomorphism between (min,+) and (max,+) algebras to reduce the computation times of said operation, under very general hypotheses on the operands. 
Our expriments compare our isospeed algorithm against two baselines:

* the direct algorithm, i.e., the standard algorithm for (min,+) convolution found in [3];
* the inverse algorithm, i.e., the algorithm that uses pseudoinversion of the operands, a (max,+) convolution, and pseudoinversion of the results. This algorithm is described in [4].

These experiments measure the runtime of (min,+) convolution of randomly generated operands, using the three algorithms (isospeed, direct, and inverse). 
They show that isospeed is (almost always) as fast as, and often much faster than, the best between the direct algorithm and the inverse one. 
There are few cases when isospeed is not as fast as the best baselines. These are due to different causes:

* a particular (min,+) convolution is computationally inexpensive (i.e., in the order of milliseconds), and unoptimizable (i.e., either the direct or the inverse algorithms cannot be optimized further). 
In this case, there is little to do, and checking the isospeed requirements only adds a modicum of overhead;

* the heuristic that selects the fastest between the direct and inverse by-sequence convolution (by counting horizontal segments and discontinuities) fails to identify the correct choice. 
This happens mostly when operands are staircase curves, hence have an equal number of horizontal segments and discontinuities. 

We ran our experiments on a cloud Virtual Machine (Intel Xeon Processors (CascadeLake) cores @2.2 GHz, 32 GB of DRAM, Ubuntu 22.04), using randomly generated parameters for the shapes discussed above. 
We run all algorithms in serial mode (rather than parallel, which is the default in Nancy). 
To make the comparison more challenging, horizontal filtering is included in the baseline algorithms as well, since it does not depend on the results of this paper, whereas vertical filtering – which is a consequence of isomorphism – is used only in the isospeed algorithm. 
Moreover, we include the cost of testing operand properties in the isospeed and inverse algorithms (there is nothing to test for the direct one). 
We measured the time to compute the convolution using the three methods.

Note that, since we measure runtime, the raw results are *expected* to vary, as they depend on the hardware setup.
The comparison between different methods, instead, should match the observations in [1].

## To reproduce the results

### Requirements

Compiling and running this code requires .NET 6.0 ([here](https://dotnet.microsoft.com/en-us/download)), as well as network connectivity to download the other dependencies during compilation.

### Run the experiment

```
dotnet build -c Release
dotnet run -c Release --project ./isospeed-convolution-benchmarks/isospeed-convolution-benchmarks.csproj -- --filter "*"
```

### Gather the results

The benchmark results can be then found in `BenchmarkDotNet.Artifacts/results`, in .md, .html, and .csv formats.
"IsoConvolution**Balanced**StaircaseBenchmarks-report" will contain the results using *balanced* curves, and so on for *horizontal* and *vertical* ones.

The `Method` column will report whether direct, inverse or isospeed was used.
The `Pair` column will contain (as C# code) the parameters of the operands.
The `Mean` column will contain the runtime of a single (min,+) convolution.

Grouping these results by `Pair` and comparing the runtime for each method, one should observe similar runtime improvements as shown in [1].

### Automated plots

To produce the plots in the paper, we used the C# program `isospeed-bench-to-tikz`, that parses the .csv files and generates TikZ plots with our desired layout.
TikZ plots are then compiled with LaTeX as part of the paper.

```
dotnet run -c Release --project ./isospeed-bench-to-tikz/isospeed-bench-to-tikz.csproj
```

The program will leave the .tikz files produced in `BenchmarkDotNet.Artifacts/results`.
The table below shows the tikz files produced and the corresponding figures in the paper.

| Tikz file | Figure |
| - | - |
| IsoConvolutionHorizontalStaircaseBenchmarks-minp-iso | Figure 6a |
| IsoConvolutionHorizontalStaircaseBenchmarks-maxp-iso | Figure 6b |
| IsoConvolutionHorizontalStaircaseBenchmarks-best-iso | Figure 6c |
| IsoConvolutionVerticalStaircaseBenchmarks-minp-iso | Figure 7a |
| IsoConvolutionVerticalStaircaseBenchmarks-maxp-iso | Figure 7b |
| IsoConvolutionVerticalStaircaseBenchmarks-best-iso | Figure 7c |
| IsoConvolutionBalancedStaircaseBenchmarks-minp-iso | Figure 8a |
| IsoConvolutionBalancedStaircaseBenchmarks-maxp-iso | Figure 8b |
| IsoConvolutionBalancedStaircaseBenchmarks-best-iso | Figure 8c |
| IsoConvolutionHorizontalKTradeoffStaircaseBenchmarks-minp-iso | Figure 9a |
| IsoConvolutionHorizontalKTradeoffStaircaseBenchmarks-maxp-iso | Figure 9b |
| IsoConvolutionHorizontalKTradeoffStaircaseBenchmarks-best-iso | Figure 9c |

## Expected runtime and tweaks

The parameters of this benchmark suite are found in the `Globals` static class, in `Program.cs` starting at line 142.
This suite runs for about 24 hours on our hardware configuration.

For a smaller functional test, one may reduce the number of test run (`TEST_COUNT`) or (heuristically) reduce the size of the computation (`RNG_MAX`, `LARGE_EXTENSION_LCM_THRESHOLD`).

## Documentation

The documentation of the Nancy library can be found at [nancy.unipi.it](https://nancy.unipi.it).
Note that the above will be about the public version of the library -- which we will soon update with the algorithms discussed here.

## References

[1] Raffaele Zippo, Paul Nikolaus and Giovanni Stea. Isospeed: Improving (min,+) Convolution by Exploiting (min,+)/(max,+) Isomorphism

[2] Raffaele Zippo and Giovanni Stea. Nancy: An efficient parallel Network Calculus library

[3] Anne Bouillard, Marc Boyer, and Euriell Le Corronc. Deterministic Network Calculus: From Theory to Practical Implementation

[4] Victor Pollex, Henrik Lipskoch, Frank Slomka, and Steffen Kollmann. Runtime Improved Computation of Path Latencies with the Real-Time Calculus