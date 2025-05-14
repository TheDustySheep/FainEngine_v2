using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FainEngine_v2.UI
{
    public class Layout
    {
        public enum SizeMode
        {
            Fit      = 0,
            Fixed    = 1,
            Grow     = 2,
            Shrink   = 3,
            Flexible = 4,
        }

        public enum Axis
        {
            X = 0,
            Y = 1,
        }

        public enum Justify
        {
            Start,
            Center,
            End,
            SpaceBetween,
            SpaceEvenly
        }

        public enum Align
        {
            Start,
            Center,
            End,
        }

        public enum OverflowMode
        {
            Wrap,
            Clip,
            Ellipsis
        }
    }
}