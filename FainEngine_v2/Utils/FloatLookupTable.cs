using System.Runtime.CompilerServices;

namespace FainEngine_v2.Utils
{
    public sealed class FloatLookupTable
    {
        private readonly float[] _x;
        private readonly float[] _y;
        private readonly int _count;

        public FloatLookupTable((float x, float y)[] points)
        {
            // MUST have at least two points
            if (points is null || points.Length < 2)
                throw new ArgumentException("Need ≥2 points", nameof(points));

            // 1) sort the tuple array by x ascending
            Array.Sort(points, (a, b) => a.x.CompareTo(b.x));

            // 2) fling them into two raw float arrays
            _count = points.Length;
            _x = new float[_count];
            _y = new float[_count];
            for (int i = 0; i < _count; i++)
            {
                _x[i] = points[i].x;
                _y[i] = points[i].y;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Interpolate(float input)
        {
            // clamp
            if (input <= _x[0]) return _y[0];
            if (input >= _x[_count - 1]) return _y[_count - 1];

            // manual binary search: find hi = insertion point, lo = hi-1
            int lo = 0, hi = _count - 1;
            while (lo <= hi)
            {
                int mid = (lo + hi) >> 1;
                float xm = _x[mid];
                if (xm == input) return _y[mid];
                else if (xm < input) lo = mid + 1;
                else hi = mid - 1;
            }

            int i0 = hi;    // last below
            int i1 = lo;    // first above

            // interpolate
            float x0 = _x[i0], x1 = _x[i1];
            float y0 = _y[i0], y1 = _y[i1];
            return y0 + (input - x0) * (y1 - y0) / (x1 - x0);
        }
    }
}
