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

            public override string ToString()
                =>$"{DiscAxis},{DiscIndex},{Angle}";

            public override bool Equals(object obj)
                => (obj as Operation)?.Equals(this) ?? false;

            public override int GetHashCode()
             => HashingHelper.GetHashCodeForItems(DiscAxis, DiscIndex, Angle);

            public Operation Clone()
                => new Operation { DiscAxis = DiscAxis, DiscIndex = DiscIndex, Angle = Angle };

            public bool Equals(Operation other)
                => DiscAxis == other.DiscAxis && DiscIndex == other.DiscIndex && Angle == other.Angle;

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
