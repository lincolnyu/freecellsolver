using System;
using static Rubiks.RubikCube;
using static Rubiks.RubikCube.Operation;

namespace Rubiks.Helpers
{
    public static class RubikHelper
    {
        public static Operation QuickNegate(this Operation op)
        {
            switch (op.Angle)
            {
                case Angles.Mirror:
                    return op;
                case Angles.Forward:
                    return new Operation { Angle = Angles.Backward, DiscAxis = op.DiscAxis, DiscIndex = op.DiscIndex };
                case Angles.Backward:
                    return new Operation { Angle = Angles.Forward, DiscAxis = op.DiscAxis, DiscIndex = op.DiscIndex };
            }
            return null;
        }

        public static Operation AxisNext(this Operation op, int maxDiscIndex)
        {
            if (op.DiscIndex > maxDiscIndex)
            {
                op.DiscAxis++;
                if ((int)op.DiscAxis >= 3)
                {
                    return null;
                }
                op.DiscIndex = 0;
            }
            return op;
        }

        public static Operation AxisPrev(this Operation op, int maxDiscIndex)
        {
            if (op.DiscIndex < 0)
            {
                op.DiscAxis--;
                if (op.DiscAxis < 0)
                {
                    return null;
                }
                op.DiscIndex = maxDiscIndex;
            }
            return op;
        }

        public static Operation AvoidExcludeNext(this Operation op, int maxDiscIndex, DiscAxes excludedAxis, int excludedDisc)
        {
            if (op.DiscIndex == excludedDisc && op.DiscAxis == excludedAxis)
            {
                op.DiscIndex++;
                if (op.AxisNext(maxDiscIndex) == null)
                {
                    return null;
                }
                op.Angle = 0;
            }
            return op;
        }

        public static Operation AvoidExcludePrev(this Operation op, int maxDiscIndex, DiscAxes excludedAxis, int excludedDisc)
        {
            if (op.DiscIndex == excludedDisc && op.DiscAxis == excludedAxis)
            {
                op.DiscIndex--;
                if (op.AxisPrev(maxDiscIndex) == null)
                {
                    return null;
                }
            }
            return op;
        }

        public static Operation GetNextOperation(this Operation op, int maxDiscIndex, Tuple<DiscAxes, int> excluded = null)
        {
            op.Angle++;
            if ((int)op.Angle >= 3)
            {
                op.Angle = 0;
                op.DiscIndex++;
                if (op.AxisNext(maxDiscIndex) == null)
                {
                    return null;
                }
            }
            return excluded != null ? op.AvoidExcludeNext(maxDiscIndex, excluded.Item1, excluded.Item2) : op;
        }

        public static Operation GetPrevOperation(this Operation op, int maxDiscIndex, Tuple<DiscAxes, int> excluded = null)
        {
            op.Angle--;
            if (op.Angle < 0)
            {
                op.Angle = Angles.Backward;
                op.DiscIndex--;
                if (op.AxisPrev(maxDiscIndex) == null)
                {
                    return null;
                }
            }
            return excluded != null ? op.AvoidExcludePrev(maxDiscIndex, excluded.Item1, excluded.Item2) : op;
        }

        public static Operation GetPrevOperation(this Operation op, int maxDiscIndex)
        {
            op.Angle--;
            if (op.Angle >= 0)
            {
                return op;
            }
            op.Angle = (Angles)2;
            op.DiscIndex--;
            return op.AxisPrev(maxDiscIndex);
        }
    }
}
