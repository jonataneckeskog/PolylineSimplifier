using BenchmarkDotNet.Attributes;
using PolylineSimplifier;

namespace PolylineSimplifier.Benchmarks;

[MemoryDiagnoser]
public class RamerDouglasPeucker2DBenchmarks
{
    private List<(float X, float Y)> _smallPolyline = null!;
    private List<(float X, float Y)> _mediumPolyline = null!;
    private List<(float X, float Y)> _largePolyline = null!;

    [GlobalSetup]
    public void Setup()
    {
        _smallPolyline = GeneratePolyline(100);
        _mediumPolyline = GeneratePolyline(1_000);
        _largePolyline = GeneratePolyline(10_000);
    }

    private static List<(float X, float Y)> GeneratePolyline(int count)
    {
        var random = new Random(42); // Fixed seed for reproducibility
        return Enumerable.Range(0, count)
            .Select(i => ((float)i, (float)(random.NextDouble() * 100)))
            .ToList();
    }

    [Benchmark]
    public List<(float X, float Y)> Simplify_Small_100Points()
    {
        return RamerDouglasPeucker2D.Simplify(
            _smallPolyline,
            epsilon: 1.0f,
            p => p.X,
            p => p.Y);
    }

    [Benchmark]
    public List<(float X, float Y)> Simplify_Medium_1000Points()
    {
        return RamerDouglasPeucker2D.Simplify(
            _mediumPolyline,
            epsilon: 1.0f,
            p => p.X,
            p => p.Y);
    }

    [Benchmark]
    public List<(float X, float Y)> Simplify_Large_10000Points()
    {
        return RamerDouglasPeucker2D.Simplify(
            _largePolyline,
            epsilon: 1.0f,
            p => p.X,
            p => p.Y);
    }

    [Benchmark]
    [Arguments(0.1f)]
    [Arguments(1.0f)]
    [Arguments(10.0f)]
    public List<(float X, float Y)> Simplify_VaryingEpsilon(float epsilon)
    {
        return RamerDouglasPeucker2D.Simplify(
            _mediumPolyline,
            epsilon,
            p => p.X,
            p => p.Y);
    }
}

