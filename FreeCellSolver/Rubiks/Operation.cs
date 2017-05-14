using NextUp.Helpers;
using System;
using static QSharp.Classical.Algorithms.DepthFirstSolverCommon;
using QSharp.Classical.Algorithms;
using Rubiks.Helpers;

namespace Rubiks
{
    public partial class RubikCube : IEquatable<RubikCube>
    {
        public class Operation : IEquatable<Operation>, IOperation
        {
            public enum Singmaster
            {
                F,
                B,
                U,
                D,  
                L,
                R,
                F2,
                B2,
                U2,
                D2,
                L2,
                R2,
                Fp,
                Bp,
                Up,
                Dp,
                Lp,
                Rp
            }

            public enum DiscAxes
            {
                X = 0,
                Y,
                Z
            }

            public enum Angles
            {
                Forward = 0,
                Mirror,
                Backward,
            }

            public DiscAxes DiscAxis;

            public int DiscIndex;  // 0, 1, ..., n/2-1, n/2+1, ... n
            public Angles Angle;

            public Operation()
            {
            }

            public Operation(Singmaster sm)
                => FromSingmaster(sm);
            
            public override string ToString()
                => $"{DiscAxis},{DiscIndex},{Angle}";

            public override bool Equals(object obj)
                => (obj as Operation)?.Equals(this) ?? false;

            public override int GetHashCode()
                => HashingHelper.GetHashCodeForItems(DiscAxis, DiscIndex, Angle);

            public Operation Clone()
                => new Operation { DiscAxis = DiscAxis, DiscIndex = DiscIndex, Angle = Angle };

            public bool Equals(Operation other)
                => DiscAxis == other.DiscAxis && DiscIndex == other.DiscIndex && Angle == other.Angle;

            public static implicit operator Singmaster(Operation op)
                => op.ToSingmaster();

            public static implicit operator Operation(Singmaster sm)
                => new Operation(sm);   

            public Singmaster ToSingmaster()
            {
                switch (DiscAxis)
                {
                    case DiscAxes.X:
                        switch (Angle)
                        {
                            case Angles.Forward:
                                return DiscIndex == 1 ? Singmaster.L : Singmaster.Rp;
                            case Angles.Backward:
                                return DiscIndex == 1 ? Singmaster.Lp : Singmaster.R;
                            case Angles.Mirror:
                                return DiscIndex == 1 ? Singmaster.L2 : Singmaster.R2;
                        }
                        break;
                    case DiscAxes.Y:
                        switch (Angle)
                        {
                            case Angles.Forward:
                                return DiscIndex == 1 ? Singmaster.F : Singmaster.Bp;
                            case Angles.Backward:
                                return DiscIndex == 1 ? Singmaster.Fp : Singmaster.B;
                            case Angles.Mirror:
                                return DiscIndex == 1 ? Singmaster.F2 : Singmaster.B2;
                        }
                        break;
                    case DiscAxes.Z:
                        switch (Angle)
                        {
                            case Angles.Forward:
                                return DiscIndex == 1 ? Singmaster.D : Singmaster.Up;
                            case Angles.Backward:
                                return DiscIndex == 1 ? Singmaster.Dp : Singmaster.U;
                            case Angles.Mirror:
                                return DiscIndex == 1 ? Singmaster.D2 : Singmaster.U2;
                        }
                        break;
                }
                throw new ArgumentException("Unexpected operation");
            }

            public void FromSingmaster(Singmaster sm)
            {
                switch (sm)
                {
                    case Singmaster.F:
                        DiscAxis = DiscAxes.Y;
                        DiscIndex = 1;
                        Angle = Angles.Forward;
                        break;
                    case Singmaster.B:
                        DiscAxis = DiscAxes.Y;
                        DiscIndex = 0;
                        Angle = Angles.Backward;
                        break;
                    case Singmaster.U:
                        DiscAxis = DiscAxes.Z;
                        DiscIndex = 0;
                        Angle = Angles.Backward;
                        break;
                    case Singmaster.D:
                        DiscAxis = DiscAxes.Z;
                        DiscIndex = 1;
                        Angle = Angles.Forward;
                        break;
                    case Singmaster.L:
                        DiscAxis = DiscAxes.X;
                        DiscIndex = 1;
                        Angle = Angles.Forward;
                        break;
                    case Singmaster.R:
                        DiscAxis = DiscAxes.X;
                        DiscIndex = 0;
                        Angle = Angles.Backward;
                        break;
                    case Singmaster.F2:
                        DiscAxis = DiscAxes.Y;
                        DiscIndex = 1;
                        Angle = Angles.Mirror;
                        break;
                    case Singmaster.B2:
                        DiscAxis = DiscAxes.Y;
                        DiscIndex = 0;
                        Angle = Angles.Mirror;
                        break;
                    case Singmaster.U2:
                        DiscAxis = DiscAxes.Z;
                        DiscIndex = 0;
                        Angle = Angles.Mirror;
                        break;
                    case Singmaster.D2:
                        DiscAxis = DiscAxes.Z;
                        DiscIndex = 1;
                        Angle = Angles.Mirror;
                        break;
                    case Singmaster.L2:
                        DiscAxis = DiscAxes.X;
                        DiscIndex = 1;
                        Angle = Angles.Mirror;
                        break;
                    case Singmaster.R2:
                        DiscAxis = DiscAxes.X;
                        DiscIndex = 0;
                        Angle = Angles.Mirror;
                        break;
                    case Singmaster.Fp:
                        DiscAxis = DiscAxes.Y;
                        DiscIndex = 1;
                        Angle = Angles.Backward;
                        break;
                    case Singmaster.Bp:
                        DiscAxis = DiscAxes.Y;
                        DiscIndex = 0;
                        Angle = Angles.Forward;
                        break;
                    case Singmaster.Up:
                        DiscAxis = DiscAxes.Z;
                        DiscIndex = 0;
                        Angle = Angles.Forward;
                        break;
                    case Singmaster.Dp:
                        DiscAxis = DiscAxes.Z;
                        DiscIndex = 1;
                        Angle = Angles.Backward;
                        break;
                    case Singmaster.Lp:
                        DiscAxis = DiscAxes.X;
                        DiscIndex = 1;
                        Angle = Angles.Backward;
                        break;
                    case Singmaster.Rp:
                        DiscAxis = DiscAxes.X;
                        DiscIndex = 0;
                        Angle = Angles.Forward;
                        break;
                }
            }

            #region IOperation members

            public IOperation GetFirst(DepthFirstSolverCommon dfs)
            {
                var initRubik = (RubikCube)dfs.InitialState;
                return new Operation().AvoidExcludeNext(initRubik.Size - 2, DiscAxis, DiscIndex);
            }

            public IOperation GetNext(DepthFirstSolverCommon dfs)
            {
                var initRubik = (RubikCube)dfs.InitialState;
                var op = Clone();
                if (dfs.OperationStack.Count > 0)
                {
                    var topOp = (Operation)dfs.OperationStack.Peek();
                    return op.GetNextOperation(initRubik.Size - 2, new Tuple<DiscAxes, int>(topOp.DiscAxis, topOp.DiscIndex));
                }
                else
                {
                    return op.GetNextOperation(initRubik.Size - 2);
                }
            }

            #endregion
        }
    }
}
