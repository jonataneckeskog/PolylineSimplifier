namespace PolylineSimplifier
{
    /// <summary>
    /// Simplifies a 2D polyline using the Ramer-Douglas-Peucker algorithm.
    /// </summary>
    public static class RamerDouglasPeucker2D
    {
        /// <summary>
        /// Simplifies a polyline using the Ramer-Douglas-Peucker algorithm.
        /// </summary>
        /// <typeparam name="T">The type of the points.</typeparam>
        /// <param name="points">The points to simplify.</param>
        /// <param name="epsilon">The epsilon value.</param>
        /// <param name="getX">Function to extract the X coordinate from a point. Used for calculations.</param>
        /// <param name="getY">Function to extract the Y coordinate from a point. Used for calculations.</param>
        /// <param name="createPoint">Function to create a point from X and Y coordinates. Used to construct simplified points.</param>
        /// <returns>The simplified points.</returns>
        /// /// <remarks>
        /// If the input list has fewer than 3 points, the method returns a copy of the original list.
        /// </remarks>
        public static List<T> Simplify<T>(
            List<T> points,
            float epsilon,
            Func<T, float> getX,
            Func<T, float> getY,
            Func<float, float, T> createPoint)
        {
            if (points.Count < 3)
                return new List<T>(points);

            // TODO: Implement Ramer-Douglas-Peucker algorithm
            return new List<T>(points);
        }
    }
}