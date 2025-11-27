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

        if (points.Count < 3)
            return [.. points];

        if (!float.IsFinite(epsilon) || epsilon <= 0)
            return [.. points];

        // Use squared epsilon to avoid sqrt in distance calculations
        float epsilonSquared = epsilon * epsilon;

        // Use a boolean array to mark which points to keep (avoids allocating lists in recursion)
        bool[] keepPoint = new bool[points.Count];
        keepPoint[0] = true;
        keepPoint[points.Count - 1] = true;

        SimplifySection(points, 0, points.Count - 1, epsilonSquared, keepPoint, getX, getY);

        List<T> result = new();
        for (int i = 0; i < points.Count; i++)
        {
            if (keepPoint[i])
                result.Add(points[i]);
        }

        return result;
    }

    /// <summary>
    /// Recursively processes a section of the polyline, marking points to keep.
    /// </summary>
    private static void SimplifySection<T>(
        List<T> points,
        int startIndex,
        int endIndex,
        float epsilonSquared,
        bool[] keepPoint,
        Func<T, float> getX,
        Func<T, float> getY)
    {
        if (endIndex - startIndex < 2)
            return;

        T startPoint = points[startIndex];
        T endPoint = points[endIndex];

        float x1 = getX(startPoint);
        float y1 = getY(startPoint);
        float x2 = getX(endPoint);
        float y2 = getY(endPoint);

        float dx = x2 - x1;
        float dy = y2 - y1;
        float lineLengthSquared = dx * dx + dy * dy;

        float maxDistanceSquared = 0;
        int maxIndex = startIndex;

        if (lineLengthSquared < float.Epsilon)
        {
            for (int i = startIndex + 1; i < endIndex; i++)
            {
                float px = getX(points[i]) - x1;
                float py = getY(points[i]) - y1;
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
                float px = getX(points[i]);
                float py = getY(points[i]);

                // Perpendicular distance squared: (crossTerm + dx*py - dy*px)^2 / lineLengthSquared
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
            SimplifySection(points, startIndex, maxIndex, epsilonSquared, keepPoint, getX, getY);
            SimplifySection(points, maxIndex, endIndex, epsilonSquared, keepPoint, getX, getY);
        }
    }
}
