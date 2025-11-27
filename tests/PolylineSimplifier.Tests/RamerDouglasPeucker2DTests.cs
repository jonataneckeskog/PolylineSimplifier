namespace PolylineSimplifier.Tests;

public class RamerDouglasPeucker2DTests
{
    // Simple point record for testing
    private record Point(float X, float Y);

    // Shared random instance for deterministic tests
    private readonly Random _random = new(42);

    // Helper functions for the generic API
    private static float GetX(Point p) => p.X;
    private static float GetY(Point p) => p.Y;

    // Helper to generate noisy diagonal line points
    private List<Point> GenerateNoisyDiagonalPoints(int count, float noiseAmplitude, int? spikeInterval = null, float spikeHeight = 0)
    {
        var points = new List<Point>();
        for (int i = 0; i < count; i++)
        {
            float x = i;
            float y = i + (float)(_random.NextDouble() - 0.5) * noiseAmplitude;

            if (spikeInterval.HasValue && i % spikeInterval.Value == spikeInterval.Value / 2)
                y += spikeHeight;

            points.Add(new Point(x, y));
        }
        return points;
    }

    #region Edge Cases

    [Fact]
    public void Simplify_EmptyList_ReturnsEmptyList()
    {
        var points = new List<Point>();

        var result = RamerDouglasPeucker2D.Simplify(points, 1.0f, GetX, GetY);

        Assert.Empty(result);
    }

    [Fact]
    public void Simplify_SinglePoint_ReturnsSinglePoint()
    {
        var points = new List<Point> { new(0, 0) };

        var result = RamerDouglasPeucker2D.Simplify(points, 1.0f, GetX, GetY);

        Assert.Single(result);
        Assert.Equal(new Point(0, 0), result[0]);
    }

    [Fact]
    public void Simplify_TwoPoints_ReturnsBothPoints()
    {
        var points = new List<Point> { new(0, 0), new(10, 10) };

        var result = RamerDouglasPeucker2D.Simplify(points, 1.0f, GetX, GetY);

        Assert.Equal(2, result.Count);
        Assert.Equal(new Point(0, 0), result[0]);
        Assert.Equal(new Point(10, 10), result[1]);
    }

    [Fact]
    public void Simplify_ReturnsNewList_DoesNotMutateInput()
    {
        var points = new List<Point> { new(0, 0), new(5, 5), new(10, 10) };
        var originalCount = points.Count;

        var result = RamerDouglasPeucker2D.Simplify(points, 1.0f, GetX, GetY);

        Assert.NotSame(points, result);
        Assert.Equal(originalCount, points.Count);
    }

    #endregion

    #region Invalid Epsilon Values

    [Fact]
    public void Simplify_NaNEpsilon_ReturnsOriginalPoints()
    {
        var points = new List<Point> { new(0, 0), new(5, 10), new(10, 0) };

        var result = RamerDouglasPeucker2D.Simplify(points, float.NaN, GetX, GetY);

        Assert.Equal(3, result.Count);
    }

    [Fact]
    public void Simplify_PositiveInfinityEpsilon_ReturnsOriginalPoints()
    {
        var points = new List<Point> { new(0, 0), new(5, 10), new(10, 0) };

        var result = RamerDouglasPeucker2D.Simplify(points, float.PositiveInfinity, GetX, GetY);

        Assert.Equal(3, result.Count);
    }

    [Fact]
    public void Simplify_NegativeInfinityEpsilon_ReturnsOriginalPoints()
    {
        var points = new List<Point> { new(0, 0), new(5, 10), new(10, 0) };

        var result = RamerDouglasPeucker2D.Simplify(points, float.NegativeInfinity, GetX, GetY);

        Assert.Equal(3, result.Count);
    }

    [Fact]
    public void Simplify_ZeroEpsilon_ReturnsOriginalPoints()
    {
        var points = new List<Point> { new(0, 0), new(5, 10), new(10, 0) };

        var result = RamerDouglasPeucker2D.Simplify(points, 0f, GetX, GetY);

        Assert.Equal(3, result.Count);
    }

    [Fact]
    public void Simplify_NegativeEpsilon_ReturnsOriginalPoints()
    {
        var points = new List<Point> { new(0, 0), new(5, 10), new(10, 0) };

        var result = RamerDouglasPeucker2D.Simplify(points, -1.0f, GetX, GetY);

        Assert.Equal(3, result.Count);
    }

    #endregion

    #region Collinear Points

    [Fact]
    public void Simplify_CollinearPoints_ReturnsOnlyEndpoints()
    {
        // Points on a straight horizontal line
        var points = new List<Point>
        {
            new(0, 0),
            new(2, 0),
            new(4, 0),
            new(6, 0),
            new(8, 0),
            new(10, 0)
        };

        var result = RamerDouglasPeucker2D.Simplify(points, 0.1f, GetX, GetY);

        Assert.Equal(2, result.Count);
        Assert.Equal(new Point(0, 0), result[0]);
        Assert.Equal(new Point(10, 0), result[1]);
    }

    [Fact]
    public void Simplify_CollinearDiagonalPoints_ReturnsOnlyEndpoints()
    {
        // Points on a diagonal line y = x
        var points = new List<Point>
        {
            new(0, 0),
            new(2, 2),
            new(4, 4),
            new(6, 6),
            new(10, 10)
        };

        var result = RamerDouglasPeucker2D.Simplify(points, 0.1f, GetX, GetY);

        Assert.Equal(2, result.Count);
        Assert.Equal(new Point(0, 0), result[0]);
        Assert.Equal(new Point(10, 10), result[1]);
    }

    [Fact]
    public void Simplify_CollinearVerticalPoints_ReturnsOnlyEndpoints()
    {
        // Points on a vertical line x = 5
        var points = new List<Point>
        {
            new(5, 0),
            new(5, 2),
            new(5, 4),
            new(5, 6),
            new(5, 10)
        };

        var result = RamerDouglasPeucker2D.Simplify(points, 0.1f, GetX, GetY);

        Assert.Equal(2, result.Count);
        Assert.Equal(new Point(5, 0), result[0]);
        Assert.Equal(new Point(5, 10), result[1]);
    }

    #endregion

    #region Triangle Cases

    [Fact]
    public void Simplify_Triangle_SmallEpsilon_KeepsAllPoints()
    {
        // Triangle with apex at (5, 10)
        var points = new List<Point>
        {
            new(0, 0),
            new(5, 10),
            new(10, 0)
        };

        // Epsilon smaller than perpendicular distance to apex
        var result = RamerDouglasPeucker2D.Simplify(points, 1.0f, GetX, GetY);

        Assert.Equal(3, result.Count);
    }

    [Fact]
    public void Simplify_Triangle_LargeEpsilon_ReturnsOnlyEndpoints()
    {
        // Triangle with apex at (5, 1) - small deviation
        var points = new List<Point>
        {
            new(0, 0),
            new(5, 1),
            new(10, 0)
        };

        // Epsilon larger than perpendicular distance
        var result = RamerDouglasPeucker2D.Simplify(points, 2.0f, GetX, GetY);

        Assert.Equal(2, result.Count);
        Assert.Equal(new Point(0, 0), result[0]);
        Assert.Equal(new Point(10, 0), result[1]);
    }

    #endregion

    #region Complex Polylines

    [Fact]
    public void Simplify_Zigzag_PreservesSignificantPoints()
    {
        // Zigzag pattern
        var points = new List<Point>
        {
            new(0, 0),
            new(2, 5),
            new(4, 0),
            new(6, 5),
            new(8, 0),
            new(10, 5)
        };

        var result = RamerDouglasPeucker2D.Simplify(points, 1.0f, GetX, GetY);

        // Should keep all points since deviations are significant
        Assert.Equal(6, result.Count);
    }

    [Fact]
    public void Simplify_SineWaveApproximation_ReducesPoints()
    {
        // Approximate sine wave with many points
        var points = new List<Point>();
        for (int i = 0; i <= 100; i++)
        {
            float x = i;
            float y = (float)Math.Sin(i * Math.PI / 50) * 10;
            points.Add(new Point(x, y));
        }

        var result = RamerDouglasPeucker2D.Simplify(points, 0.5f, GetX, GetY);

        // Result should have fewer points than original
        Assert.True(result.Count < points.Count);
        // But should keep more than just endpoints
        Assert.True(result.Count > 2);
        // First and last points should be preserved
        Assert.Equal(points[0], result[0]);
        Assert.Equal(points[^1], result[^1]);
    }

    [Fact]
    public void Simplify_SquareShape_PreservesCorners()
    {
        // Square: corners should be preserved
        var points = new List<Point>
        {
            new(0, 0),
            new(5, 0),   // midpoint on bottom edge (should be removed)
            new(10, 0),  // corner
            new(10, 5),  // midpoint on right edge (should be removed)
            new(10, 10), // corner
            new(5, 10),  // midpoint on top edge (should be removed)
            new(0, 10),  // corner
            new(0, 5),   // midpoint on left edge (should be removed)
            new(0, 0)    // back to start
        };

        var result = RamerDouglasPeucker2D.Simplify(points, 0.1f, GetX, GetY);

        // Should keep corners: (0,0), (10,0), (10,10), (0,10), (0,0)
        Assert.Equal(5, result.Count);
    }

    [Fact]
    public void Simplify_NoisyLine_RemovesMidPoints()
    {
        var points = GenerateNoisyDiagonalPoints(count: 1000, noiseAmplitude: 2); // y ≈ x with ±1 noise

        // Epsilon of 5 should tolerate the ±1 noise and simplify to a straight line
        var result = RamerDouglasPeucker2D.Simplify(points, 5.0f, GetX, GetY);

        Assert.True(result.Count <= 100, $"Expected ≤100 points, got {result.Count}");
        Assert.True(result.Count >= 2, "Should keep at least endpoints");

        Assert.Equal(points[0], result[0]);
        Assert.Equal(points[^1], result[^1]);
    }

    [Fact]
    public void Simplify_NoisyLineWithSpikes_KeepsSomeMidpoints()
    {
        // y ≈ x with ±1 noise, plus significant spikes every 100 points
        var points = GenerateNoisyDiagonalPoints(count: 1000, noiseAmplitude: 2, spikeInterval: 100, spikeHeight: 20);

        var result = RamerDouglasPeucker2D.Simplify(points, 5.0f, GetX, GetY);

        // Should keep endpoints + the ~10 spike points (roughly)
        Assert.True(result.Count >= 10, $"Expected ≥10 points for spikes, got {result.Count}");
        Assert.True(result.Count <= 50, $"Expected ≤50 points, got {result.Count}");
        Assert.Equal(points[0], result[0]);
        Assert.Equal(points[^1], result[^1]);
    }

    #endregion

    #region Degenerate Cases

    [Fact]
    public void Simplify_AllSamePoints_ReturnsEndpoints()
    {
        // All points at the same location
        var points = new List<Point>
        {
            new(5, 5),
            new(5, 5),
            new(5, 5),
            new(5, 5)
        };

        var result = RamerDouglasPeucker2D.Simplify(points, 1.0f, GetX, GetY);

        Assert.Equal(2, result.Count);
        Assert.Equal(new Point(5, 5), result[0]);
        Assert.Equal(new Point(5, 5), result[1]);
    }

    [Fact]
    public void Simplify_StartAndEndSame_KeepsMiddlePoint()
    {
        // Closed polyline (start == end) with interior point
        var points = new List<Point>
        {
            new(0, 0),
            new(5, 10),
            new(0, 0)
        };

        var result = RamerDouglasPeucker2D.Simplify(points, 1.0f, GetX, GetY);

        // Should keep all three since (5,10) is far from the degenerate line
        Assert.Equal(3, result.Count);
    }

    #endregion

    #region Epsilon Boundary Tests

    [Fact]
    public void Simplify_PointBelowEpsilon_IsRemoved()
    {
        // Point with perpendicular distance less than epsilon should be removed
        var points = new List<Point>
        {
            new(0, 0),
            new(5, 5),  // perpendicular distance from line (0,0)-(10,0) is 5
            new(10, 0)
        };

        // Use epsilon larger than perpendicular distance (5)
        var result = RamerDouglasPeucker2D.Simplify(points, 5.1f, GetX, GetY);

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void Simplify_PointAboveEpsilon_IsKept()
    {
        // Point with perpendicular distance greater than epsilon should be kept
        var points = new List<Point>
        {
            new(0, 0),
            new(5, 5),  // perpendicular distance from line (0,0)-(10,0) is 5
            new(10, 0)
        };

        // Use epsilon smaller than perpendicular distance (5)
        var result = RamerDouglasPeucker2D.Simplify(points, 4.9f, GetX, GetY);

        Assert.Equal(3, result.Count);
    }

    #endregion

    #region Order Preservation

    [Fact]
    public void Simplify_PreservesPointOrder()
    {
        var points = new List<Point>
        {
            new(0, 0),
            new(3, 10),
            new(5, 0),
            new(7, 10),
            new(10, 0)
        };

        var result = RamerDouglasPeucker2D.Simplify(points, 1.0f, GetX, GetY);

        // Verify order is preserved
        for (int i = 0; i < result.Count - 1; i++)
        {
            int indexInOriginal = points.IndexOf(result[i]);
            int nextIndexInOriginal = points.IndexOf(result[i + 1]);
            Assert.True(indexInOriginal < nextIndexInOriginal, "Points should maintain original order");
        }
    }

    #endregion
}
