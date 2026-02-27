# PolylineSimplifier

An efficient .NET library for simplifying 2D polylines using the Ramer-Douglas-Peucker algorithm. It reduces the number of points in a curve while preserving its visual shape.

## Usage

### Installation

```bash
dotnet add package PolylineSimplifier
```

### Example

```csharp
using PolylineSimplifier;

public record Point(float X, float Y);

List<Point> points = new List<Point>
{
    new(0, 0), new(1, 0.1f), new(2, -0.1f), new(3, 5), new(4, 6),
    new(5, 7), new(6, 8.1f), new(7, 9), new(8, 9), new(9, 9)
};

List<Point> simplified = RamerDouglasPeucker2D.Simplify(
    points,
    epsilon: 1.0f,
    getX: p => p.X,
    getY: p => p.Y);
```

### Parameters

| Parameter | Description                                                                   |
| --------- | ----------------------------------------------------------------------------- |
| `points`  | The input polyline as a list of points                                        |
| `epsilon` | Maximum perpendicular distance tolerance. Larger values = more simplification |
| `getX`    | Function to extract the X coordinate from a point                             |
| `getY`    | Function to extract the Y coordinate from a point                             |

## Benchmarks

Measured on an Intel Core i5-9400F Processor:

| Method        |     Mean | Allocated |
| ------------- | -------: | --------: |
| 100 Points    |  2.80 μs |   2.11 KB |
| 1000 Points   |  65.6 μs |   15,6 KB |
| 10000 Points  |  1260 μs |    143 KB |
| 100000 Points | 26500 μs |   1440 KB |
<small>Note: Epsilon = 1.0 for all benchmarks. A larger epsilon generally decreases the execution time an allocates less memory.</small>

### Running Benchmarks

```bash
git clone git@github.com:jonataneckeskog/PolylineSimplifier.git
cd PolylineSimplifier
dotnet run -c Release --project tests/PolylineSimplifier.Benchmarks
```

## License

MIT License – see [LICENSE](LICENSE) for details.

