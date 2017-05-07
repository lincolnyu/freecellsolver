using NextUp.Helpers;
using System;
using System.Text;
using static QSharp.Classical.Algorithms.DepthFirstSolverCommon;
using QSharp.Classical.Algorithms;

namespace Rubiks
{
    public partial class RubikCube : IEquatable<RubikCube>, IState
    {
        public enum Colors
        {
            Color0 = 0,
            Color1 = 1,
            Color2 = 2,
            Color3 = 3,
            Color4 = 4,
            Color5 = 5
        }

        /// <summary>
        ///  Arrangement of surfaces
        /// </summary>
        /// <remarks>
        ///      4
        ///    3 0 1 2
        ///      5
        ///  
        ///            0 1 2
        ///            3 4 5
        ///            6 7 8
        ///   -------------------------------
        ///    0 1 2 | 0 1 2 | 0 1 2 | 0 1 2
        ///    3 4 5 | 3 4 5 | 3 4 5 | 3 4 5
        ///    6 7 8 | 6 7 8 | 6 7 8 | 6 7 8
        ///   -------------------------------
        ///            0 1 2
        ///            3 4 5
        ///            6 7 8
        ///    
        /// </remarks>
        private readonly Colors[,,] _surfaces;

        public RubikCube(int n, bool reset = true)
        {
            Size = n;
            _surfaces = new Colors[6, n, n];
            if (reset)
            {
                ResetToGood();
            }
        }

        public int Size { get; }

        public bool Solved => IsDone();

        public override string ToString()
        {
            var sb = new StringBuilder();
            for (var i = 0; i < 3; i++)
            {
                SurfaceRowToString(4, i, 4, sb);
                sb.AppendLine();
            }
            for (var i = 0; i < 3; i++)
            {
                SurfaceRowToString(3, i, 0, sb);
                SurfaceRowToString(0, i, 1, sb);
                SurfaceRowToString(1, i, 1, sb);
                SurfaceRowToString(2, i, 1, sb);
                sb.AppendLine();
            }
            for (var i = 0; i < 3; i++)
            {
                SurfaceRowToString(5, i, 4, sb);
                sb.AppendLine();
            }
            return sb.ToString();
        }

        public override bool Equals(object obj)
            => (obj as RubikCube)?.Equals(this)?? false;
        
        public RubikCube Clone()
        {
            var clone = new RubikCube(Size, false);
            CopyTo(clone);
            return clone;
        }

        public void CopyTo(RubikCube cube)
        {
            for (var s = 0; s < 6; s++)
            {
                for (var i = 0; i < Size; i++)
                {
                    for (var j = 0; j < Size; j++)
                    {
                        cube._surfaces[s, i, j] = _surfaces[s, i, j];
                    }
                }
            }
        }

        public override int GetHashCode()
            => HashingHelper.GetHashCodeForItems(_surfaces[0, 0, 0], _surfaces[0, 0, Size - 1], _surfaces[0, Size - 1, 0],
                _surfaces[1, 0, 1], _surfaces[1, 1, Size - 1], _surfaces[1, Size - 1, 1],
                _surfaces[4, 0, 1], _surfaces[1, 1, Size - 1], _surfaces[1, Size - 1, 1]);

        public bool Equals(RubikCube other)
            => Size == other.Size && SurfaceEqual(other, 0) && SurfaceEqual(other, 1) && SurfaceEqual(other, 4);

        public bool FromString(string str)
        {
            var surfaceOrder = new [] { 4, 3, 0, 1, 2, 5 };
            var indices = new Tuple<int, int>[Size * 6];
            var c = 0;
            for (var i = 0; i < Size; i++)
            {
                indices[c++] = new Tuple<int, int>(4, i);
            }
            for (var i = 0; i < Size; i++)
            {
                indices[c++] = new Tuple<int, int>(3, i);
                indices[c++] = new Tuple<int, int>(0, i);
                indices[c++] = new Tuple<int, int>(1, i);
                indices[c++] = new Tuple<int, int>(2, i);
            }
            for (var i = 0; i < Size; i++)
            {
                indices[c++] = new Tuple<int, int>(5, i);
            }

            c = 0;
            var col = 0;
            foreach (var ch in str)
            {
                if (char.IsWhiteSpace(ch))
                {
                    continue;
                }
                var ci = ch - '0';
                if (ci < 0 || ci >= 6)
                {
                    ResetToGood();
                    return false;
                }
                var t = indices[c];
                _surfaces[t.Item1, t.Item2, col++] = (Colors)ci;
                if (col >= Size)
                {
                    col = 0;
                    c++;
                    if (c >= indices.Length)
                    {
                        break;
                    }
                }
            }
            return true;
        }

        private void SurfaceRowToString(int surface, int row, int indent, StringBuilder sb)
        {
            sb.Append(' ', indent);
            for (var j = 0; j < Size; j++)
            {
                sb.Append($"{(int)_surfaces[surface, row, j]}");
            }
        }

        public int TrueIndex(Operation op) => (op.DiscIndex < Size / 2) ? op.DiscIndex : op.DiscIndex + 1;

        public bool IsDone() => CheckSurface(0) && CheckSurface(1) && CheckSurface(4);

        public void ResetToGood()
        {
            for (var s = 0; s < 6; s++)
            {
                for (var i = 0; i < Size; i++)
                {
                    for (var j = 0; j < Size; j++)
                    {
                        _surfaces[s, i, j] = (Colors)s;
                    }
                }
            }
        }

        public Colors Get(int surface, int row, int col) => _surfaces[surface, row, col];
        public void Set(int surface, int row, int col, Colors color) => _surfaces[surface, row, col] = color; 

        private bool CheckSurface(int s)
        {
            var firstColor = _surfaces[s, 0, 0];
            var i = 0;
            var j = 1;
            for (; i < Size; i++)
            {
                for (; j < Size; j++)
                {
                    if (_surfaces[s, i, j] != firstColor)
                    {
                        return false;
                    }
                }
                j = 0;
            }
            return true;
        }

        private bool SurfaceEqual(RubikCube other, int s)
        {
            for (var i = 0; i < Size; i++)
            {
                for (var j = 0; j < Size; j++)
                {
                    if (_surfaces[s, i, j] != other._surfaces[s, i, j])
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public void OperateSelf(Operation op)
        {
            RotateSurface(op);
            switch (op.DiscAxis)
            {
                case Operation.DiscAxes.X:
                    RotateX(op);
                    break;
                case Operation.DiscAxes.Y:
                    RotateY(op);
                    break;
                case Operation.DiscAxes.Z:
                    RotateZ(op);
                    break;
            }
        }

        private void RotateX(Operation op)
        {
            var trueIndex = TrueIndex(op);
            switch (op.Angle)
            {
                case Operation.Angles.Backward:
                    RotateBackwardX(trueIndex);
                    break;
                case Operation.Angles.Forward:
                    RotateForwardX(trueIndex);
                    break;
                case Operation.Angles.Mirror:
                    MirrorX(trueIndex);
                    break;
            }
        }

        private void RotateY(Operation op)
        {
            var trueIndex = TrueIndex(op);
            switch (op.Angle)
            {
                case Operation.Angles.Backward:
                    RotateBackwardY(trueIndex);
                    break;
                case Operation.Angles.Forward:
                    RotateForwardY(trueIndex);
                    break;
                case Operation.Angles.Mirror:
                    MirrorY(trueIndex);
                    break;
            }
        }

        private void RotateZ(Operation op)
        {
            var trueIndex = TrueIndex(op);
            switch (op.Angle)
            {
                case Operation.Angles.Backward:
                    RotateBackwardZ(trueIndex);
                    break;
                case Operation.Angles.Forward:
                    RotateForwardZ(trueIndex);
                    break;
                case Operation.Angles.Mirror:
                    MirrorZ(trueIndex);
                    break;
            }
        }

        private void RotateSurface(Operation op)
        {
            int s = 0;
            if (op.DiscIndex == 0)
            {
                switch (op.DiscAxis)
                {
                    case Operation.DiscAxes.X:
                        s = 1;
                        break;
                    case Operation.DiscAxes.Y:
                        s = 4;
                        break;
                    case Operation.DiscAxes.Z:
                        s = 0;
                        break;
                }
                switch (op.Angle)
                {
                    case Operation.Angles.Backward:
                        RotateSurfaceBackward(s);
                        break;
                    case Operation.Angles.Forward:
                        RotateSurfaceForward(s);
                        break;
                    case Operation.Angles.Mirror:
                        MirrorSurface(s);
                        break;
                }
            }
            else if (op.DiscIndex == Size - 1)
            {
                switch (op.DiscAxis)
                {
                    case Operation.DiscAxes.X:
                        s = 3;
                        break;
                    case Operation.DiscAxes.Y:
                        s = 5;
                        break;
                    case Operation.DiscAxes.Z:
                        s = 2;
                        break;
                }
                switch (op.Angle)
                {
                    case Operation.Angles.Backward:
                        RotateSurfaceForward(s);
                        break;
                    case Operation.Angles.Forward:
                        RotateSurfaceBackward(s);
                        break;
                    case Operation.Angles.Mirror:
                        MirrorSurface(s);
                        break;
                }
            }
        }

        private void RotateSurfaceForward(int s)
        {
            Permutate(ref _surfaces[s, 0, 0], ref _surfaces[s, 2, 0], ref _surfaces[s, 2, 2], ref _surfaces[s, 0, 2]);
            Permutate(ref _surfaces[s, 0, 1], ref _surfaces[s, 1, 0], ref _surfaces[s, 2, 1], ref _surfaces[s, 1, 2]);
        }

        private void RotateSurfaceBackward(int s)
        {
            Permutate(ref _surfaces[s, 0, 0], ref _surfaces[s, 0, 2], ref _surfaces[s, 2, 2], ref _surfaces[s, 2, 0]);
            Permutate(ref _surfaces[s, 0, 1], ref _surfaces[s, 1, 2], ref _surfaces[s, 2, 1], ref _surfaces[s, 1, 0]);
        }

        private void MirrorSurface(int s)
        {
            Swap(ref _surfaces[s, 0, 0], ref _surfaces[s, 2, 2]);
            Swap(ref _surfaces[s, 0, 1], ref _surfaces[s, 2, 1]);
            Swap(ref _surfaces[s, 0, 2], ref _surfaces[s, 2, 0]);
            Swap(ref _surfaces[s, 1, 0], ref _surfaces[s, 1, 2]);
        }

        private void RotateForwardX(int n)
        {
            for (var i = 0; i < 3; i++)
            {
                Permutate(ref _surfaces[0, i, 2 - n], ref _surfaces[4, i, 2 - n], ref _surfaces[2, 2 - i, n], ref _surfaces[5, i, 2- n]);
            }
        }

        private void RotateBackwardX(int n)
        {
            for (var i = 0; i < 3; i++)
            {
                Permutate(ref _surfaces[0, i, 2 - n], ref _surfaces[5, i, 2 - n], ref _surfaces[2, 2 - i, n], ref _surfaces[4, i, 2 - n]);
            }
        }

        private void MirrorX(int n)
        {
            for (var i = 0; i < 3; i++)
            {
                Swap(ref _surfaces[0, i, 2 - n], ref _surfaces[2, 2 - i, n]);
                Swap(ref _surfaces[4, i, 2 - n], ref _surfaces[5, i, 2 - n]);
            }
        }

        private void RotateForwardY(int n)
        {
            for (var i = 0; i < 3; i++)
            {
                Permutate(ref _surfaces[0, n, i], ref _surfaces[3, n, i], ref _surfaces[2, n, i], ref _surfaces[1, n, i]);
            }
        }

        private void RotateBackwardY(int n)
        {
            for (var i = 0; i < 3; i++)
            {
                Permutate(ref _surfaces[0, n, i], ref _surfaces[1, n, i], ref _surfaces[2, n, i], ref _surfaces[3, n, i]);
            }
        }

        private void MirrorY(int n)
        {
            for (var i = 0; i < 3; i++)
            {
                Swap(ref _surfaces[0, n, i], ref _surfaces[2, n, i]);
                Swap(ref _surfaces[3, n, i], ref _surfaces[1, n, i]);
            }
        }

        private void RotateForwardZ(int n)
        {
            for (var i = 0; i < 3; i++)
            {
                Permutate(ref _surfaces[5, n, i], ref _surfaces[3, i, 2 - n], ref _surfaces[4, 2 - n, 2 - i], ref _surfaces[1, 2 - i, n]);
            }
        }

        private void RotateBackwardZ(int n)
        {
            for (var i = 0; i < 3; i++)
            {
                Permutate(ref _surfaces[5, n, i], ref _surfaces[1, 2 - i, n], ref _surfaces[4, 2 - n, 2 - i], ref _surfaces[3, i, 2 - n]);
            }
        }

        private void MirrorZ(int n)
        {
            for (var i = 0; i < 3; i++)
            {
                Swap(ref _surfaces[5, n, i], ref _surfaces[4, 2 - n, 2 - i]);
                Swap(ref _surfaces[3, i, 2 - n], ref _surfaces[1, 2 - i, n]);
            }
        }

        private void Swap<T>(ref T a, ref T b)
        {
            var t = a;
            a = b;
            b = t;
        }

        /// <summary>
        ///  a from b from c from d from a
        /// </summary>
        /// <typeparam name="T">Type of arguments</typeparam>
        /// <param name="a">1st argment</param>
        /// <param name="b">2nd argument</param>
        /// <param name="c">3rd argument</param>
        /// <param name="d">4th argument</param>
        private void Permutate<T>(ref T a, ref T b, ref T c, ref T d)
        {
            var t = a;
            a = b;
            b = c;
            c = d;
            d = t;
        }

        #region IState members

        public IState Operate(IOperation op)
        {
            var clone = Clone();
            clone.OperateSelf((Operation)op);
            return clone;
        }

        public IOperation GetFirstOperation(DepthFirstSolverCommon dfs)
            => new Operation();

        #endregion
    }
}
