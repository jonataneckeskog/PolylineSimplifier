# PolylineSimplifier

An efficient .NET library for simplifying 2D polylines using the Ramer-Douglas-Peucker algorithm.

## Usage

### Installation

```bash
dotnet add package PolylineSimplifier
```

### Example

```csharp
using PolylineSimplifier;

public record Point(float X, float Y);

var points = new List<Point>
{
    new(0, 0), new(1, 0.1f), new(2, -0.1f), new(3, 5), new(4, 6),
    new(5, 7), new(6, 8.1f), new(7, 9), new(8, 9), new(9, 9)
};

var simplified = RamerDouglasPeucker2D.Simplify(
    points,
    epsilon: 1.0f,
    getX: p => p.X,
    getY: p => p.Y);
```

### Parameters

| Parameter | Description |
|-----------|-------------|
| `points` | The input polyline as a list of points |
| `epsilon` | Maximum perpendicular distance tolerance. Larger values = more simplification |
| `getX` | Function to extract the X coordinate from a point |
| `getY` | Function to extract the Y coordinate from a point |

## Benchmarks

Measured on AMD Ryzen 5 7600 Processor:

| Method                           | epsilon | Mean          | Allocated |
|--------------------------------- |--------:|--------------:|----------:|
| Simplify_Small_100Points         | 1       |      1.912 μs |   2.27 KB |
| Simplify_Medium_1000Points       | 1       |     32.363 μs |  17.21 KB |
| Simplify_Large_10000Points       | 1       |    962.689 μs | 138.07 KB |
| Simplify_ExtraLarge_100000Points | 1       | 23,547.090 μs | 2146.2 KB |

### Running Benchmarks

```bash
git clone git@github.com:jonataneckeskog/PolylineSimplifier.git
cd PolylineSimplifier
dotnet run -c Release --project tests/PolylineSimplifier.Benchmarks
```

## License

MIT License – see [LICENSE](LICENSE) for details.

