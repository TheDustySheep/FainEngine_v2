using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FainEngine_v2.UI
{
    public class Layout
    {
        public struct Size(SizeMode mode, float value)
        {
            public SizeMode Mode = mode;
            public float Value = value;
        }

        public enum SizeMode
        {
            Fit = 0,
            Fixed,
        }

        public enum Direction
        {
            Horizontal = 0,
            Vertical,
        }
    }

}
