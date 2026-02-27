namespace PolylineSimplifier;

/// <summary>
/// Simplifies a 2D polyline using the Ramer-Douglas-Peucker algorithm.
/// </summary>
/// <remarks>
/// The algorithm finds a similar curve with fewer points by keeping points
/// that are further than epsilon from the line between endpoints.
/// Uses squared distances internally to avoid sqrt calculations.
/// </remarks>
public static class RamerDouglasPeucker2D
{
    /// <summary>
    /// Simplifies a polyline using the Ramer-Douglas-Peucker algorithm.
    /// </summary>
    /// <typeparam name="T">The type of the points.</typeparam>
    /// <param name="points">The points to simplify.</param>
    /// <param name="epsilon">The maximum perpendicular distance tolerance.</param>
    /// <param name="getX">Function to extract the X coordinate from a point.</param>
    /// <param name="getY">Function to extract the Y coordinate from a point.</param>
    /// <returns>A new list containing the simplified points.</returns>
    /// <remarks>
    /// If the input list has fewer than 3 points, the method returns a copy of the original list.
    /// </remarks>
    public static List<T> Simplify<T>(
        List<T> points,
        float epsilon,
        Func<T, float> getX,
        Func<T, float> getY)
    {
        ArgumentNullException.ThrowIfNull(points);
        ArgumentNullException.ThrowIfNull(getX);
        ArgumentNullException.ThrowIfNull(getY);

        int count = points.Count;

        if (count < 3)
            return [.. points];

        if (!float.IsFinite(epsilon) || epsilon <= 0)
            return [.. points];

        // Use squared epsilon to avoid sqrt in distance 
        float epsilonSquared = epsilon * epsilon;

        // Cache coordinates to eliminate delegate overhead
        var coords = new (float x, float y)[count];
        for (int i = 0; i < count; i++)
        {
            coords[i] = (getX(points[i]), getY(points[i]));
        }

        bool[] keepPoint = new bool[count];
        keepPoint[0] = true;
        keepPoint[count - 1] = true;

        // Iterative approach using a Stack
        var stack = new Stack<(int startIndex, int endIndex)>();
        stack.Push((0, count - 1));

        while (stack.Count > 0)
        {
            var (startIndex, endIndex) = stack.Pop();
            if (endIndex - startIndex < 2)
                continue;

            float x1 = coords[startIndex].x;
            float y1 = coords[startIndex].y;
            float x2 = coords[endIndex].x;
            float y2 = coords[endIndex].y;

            float dx = x2 - x1;
            float dy = y2 - y1;
            float lineLengthSquared = dx * dx + dy * dy;

            float maxDistanceSquared = -1f;
            int maxIndex = startIndex;

            if (lineLengthSquared < float.Epsilon)
            {
                for (int i = startIndex + 1; i < endIndex; i++)
                {
                    float px = coords[i].x - x1;
                    float py = coords[i].y - y1;
                    float distSquared = px * px + py * py;

                    if (distSquared > maxDistanceSquared)
                    {
                        maxDistanceSquared = distSquared;
                        maxIndex = i;
                    }
                }
            }
            else
            {
                float crossTerm = x1 * y2 - x2 * y1;

                for (int i = startIndex + 1; i < endIndex; i++)
                {
                    float px = coords[i].x;
                    float py = coords[i].y;

                    float numerator = crossTerm + dx * py - dy * px;
                    float distSquared = numerator * numerator / lineLengthSquared;

                    if (distSquared > maxDistanceSquared)
                    {
                        maxDistanceSquared = distSquared;
                        maxIndex = i;
                    }
                }
            }

            if (maxDistanceSquared > epsilonSquared)
            {
                keepPoint[maxIndex] = true;

                // Push right side first, then left side, so the left side is processed first 
                stack.Push((maxIndex, endIndex));
                stack.Push((startIndex, maxIndex));
            }
        }

        // Pre-calculate exact capacity needed for the result list to avoid allocations
        int keptCount = 0;
        for (int i = 0; i < count; i++)
        {
            if (keepPoint[i]) keptCount++;
        }

        List<T> result = new(keptCount);
        for (int i = 0; i < count; i++)
        {
            if (keepPoint[i])
                result.Add(points[i]);
        }

        return result;
    }
}
