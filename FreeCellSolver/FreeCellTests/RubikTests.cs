using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Diagnostics;
using Rubiks;
using Rubiks.Helpers;
using static Rubiks.RubikCube;
using static Rubiks.RubikCube.Operation;

namespace FreeCellTests
{
    [TestClass]
    public class RubikTests
    {
        [TestMethod]
        public void TestAvoidExcludeNext()
        {
            var op = new Operation { DiscAxis = DiscAxes.X, DiscIndex = 0, Angle = Angles.Forward };
            var expected = new Operation { DiscAxis = DiscAxes.X, DiscIndex = 1, Angle = Angles.Forward };
            var actual = op.AvoidExcludeNext(1, DiscAxes.X, 0);
            Assert.IsTrue(ReferenceEquals(op, actual));
            Assert.AreEqual(expected, actual);

            op = new Operation { DiscAxis = DiscAxes.X, DiscIndex = 0, Angle = Angles.Forward };
            expected = new Operation { DiscAxis = DiscAxes.X, DiscIndex = 0, Angle = Angles.Forward };
            actual = op.AvoidExcludeNext(1, DiscAxes.X, 1);
            Assert.IsTrue(ReferenceEquals(op, actual));
            Assert.AreEqual(expected, actual);

            op = new Operation { DiscAxis = DiscAxes.X, DiscIndex = 1, Angle = Angles.Mirror };
            expected = new Operation { DiscAxis = DiscAxes.Y, DiscIndex = 0, Angle = Angles.Forward };
            actual = op.AvoidExcludeNext(1, DiscAxes.X, 1);
            Assert.IsTrue(ReferenceEquals(op, actual));
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestGetNextOperation()
        {
            var op = new Operation { DiscAxis = DiscAxes.X, DiscIndex = 0, Angle = Angles.Forward };
            var expected = new Operation { DiscAxis = DiscAxes.X, DiscIndex = 0, Angle = Angles.Mirror};
            var actual = op.GetNextOperation(1);
            Assert.IsTrue(ReferenceEquals(op, actual));
            Assert.AreEqual(expected, actual);

            expected = new Operation { DiscAxis = DiscAxes.X, DiscIndex = 0, Angle = Angles.Backward };
            actual = op.GetNextOperation(1);
            Assert.IsTrue(ReferenceEquals(op, actual));
            Assert.AreEqual(expected, actual);

            expected = new Operation { DiscAxis = DiscAxes.X, DiscIndex = 1, Angle = Angles.Forward };
            actual = op.GetNextOperation(1);
            Assert.IsTrue(ReferenceEquals(op, actual));
            Assert.AreEqual(expected, actual);

            op = new Operation { DiscAxis = DiscAxes.X, DiscIndex = 1, Angle = Angles.Backward };
            expected = new Operation { DiscAxis = DiscAxes.Y, DiscIndex = 0, Angle = Angles.Forward };
            actual = op.GetNextOperation(1);
            Assert.IsTrue(ReferenceEquals(op, actual));
            Assert.AreEqual(expected, actual);

            op = new Operation { DiscAxis = DiscAxes.Z, DiscIndex = 1, Angle = Angles.Backward };
            actual = op.GetNextOperation(1);
            Assert.IsTrue(ReferenceEquals(null, actual));
            Assert.AreEqual(null, actual);
        }

        [TestMethod]
        public void TestGetNextOperationExclude()
        {
            var op = new Operation { DiscAxis = DiscAxes.X, DiscIndex = 0, Angle = Angles.Forward };
            var expected = new Operation { DiscAxis = DiscAxes.X, DiscIndex = 0, Angle = Angles.Mirror };
            var actual = op.GetNextOperation(1, new Tuple<DiscAxes, int>(DiscAxes.X, 1));
            Assert.IsTrue(ReferenceEquals(op, actual));
            Assert.AreEqual(expected, actual);

            op = new Operation { DiscAxis = DiscAxes.X, DiscIndex = 0, Angle = Angles.Forward };
            expected = new Operation { DiscAxis = DiscAxes.X, DiscIndex = 1, Angle = Angles.Forward };
            actual = op.GetNextOperation(1, new Tuple<DiscAxes, int>(DiscAxes.X, 0));
            Assert.IsTrue(ReferenceEquals(op, actual));
            Assert.AreEqual(expected, actual);

            op = new Operation { DiscAxis = DiscAxes.X, DiscIndex = 0, Angle = Angles.Backward };
            expected = new Operation { DiscAxis = DiscAxes.Y, DiscIndex = 0, Angle = Angles.Forward };
            actual = op.GetNextOperation(1, new Tuple<DiscAxes, int>(DiscAxes.X, 1));
            Assert.IsTrue(ReferenceEquals(op, actual));
            Assert.AreEqual(expected, actual);

            op = new Operation { DiscAxis = DiscAxes.Z, DiscIndex = 0, Angle = Angles.Backward };
            actual = op.GetNextOperation(1, new Tuple<DiscAxes, int>(DiscAxes.Z, 1));
            Assert.IsTrue(ReferenceEquals(null, actual));
            Assert.AreEqual(null, actual);
        }

        [TestMethod]
        public void TestGetPrevOperation()
        {
            var op = new Operation { DiscAxis = DiscAxes.Z, DiscIndex = 1, Angle = Angles.Backward };
            var expected = new Operation { DiscAxis = DiscAxes.Z, DiscIndex = 1, Angle = Angles.Mirror };
            var actual = op.GetPrevOperation(1);
            Assert.IsTrue(ReferenceEquals(op, actual));
            Assert.AreEqual(expected, actual);
            
            expected = new Operation { DiscAxis = DiscAxes.Z, DiscIndex = 1, Angle = Angles.Forward };
            actual = op.GetPrevOperation(1);
            Assert.IsTrue(ReferenceEquals(op, actual));
            Assert.AreEqual(expected, actual);

            expected = new Operation { DiscAxis = DiscAxes.Z, DiscIndex = 0, Angle = Angles.Backward };
            actual = op.GetPrevOperation(1);
            Assert.IsTrue(ReferenceEquals(op, actual));
            Assert.AreEqual(expected, actual);

            op = new Operation { DiscAxis = DiscAxes.Z, DiscIndex = 0, Angle = Angles.Forward };
            expected = new Operation { DiscAxis = DiscAxes.Y, DiscIndex = 1, Angle = Angles.Backward };
            actual = op.GetPrevOperation(1);
            Assert.IsTrue(ReferenceEquals(op, actual));
            Assert.AreEqual(expected, actual);

            op = new Operation { DiscAxis = DiscAxes.X, DiscIndex = 0, Angle = Angles.Forward };
            actual = op.GetPrevOperation(1);
            Assert.IsTrue(ReferenceEquals(null, actual));
            Assert.AreEqual(null, actual);
        }

        private RubikCube GetResetRubik()
        {
            var rubikstr = "444";
            rubikstr += "   444";
            rubikstr += "   444";
            rubikstr += "333000111222";
            rubikstr += "333000111222";
            rubikstr += "333000111222";
            rubikstr += "   555";
            rubikstr += "   555";
            rubikstr += "   555";

            var rubik = new RubikCube(3);
            rubik.FromString(rubikstr);

            return rubik;
        }

        [TestMethod]
        public void TestSolve()
        {
            var rubik = GetResetRubik();
            var op1 = new Operation { DiscAxis = DiscAxes.X, DiscIndex = 0, Angle = Angles.Forward };
            var op2 = new Operation { DiscAxis = DiscAxes.Y, DiscIndex = 1, Angle = Angles.Mirror };
            Debug.WriteLine("------------------------------");
            Debug.WriteLine("Init");
            Debug.WriteLine(rubik);
            rubik.OperateSelf(op1);
            Debug.WriteLine("------------------------------");
            Debug.WriteLine(op1);
            Debug.WriteLine(rubik);
            rubik.OperateSelf(op2);
            Debug.WriteLine("------------------------------");
            Debug.WriteLine(op2);
            Debug.WriteLine(rubik);
            var solver1 = new RubikCubeSolver(rubik.Clone(), 1);
            var result1 = solver1.SolveFirst();
            var solver2 = new RubikCubeSolver(rubik.Clone(), 2);
            solver2.SolveStep += (dfs, state, type) =>
            {
                Debug.WriteLine("------------------------------");
                Debug.WriteLine(type);
                Debug.WriteLine(dfs.LastOperation);
                Debug.WriteLine(state);
            };
            var result2 = solver2.SolveFirst();
            Assert.IsTrue(result1 == null);
            Assert.IsTrue(result2 != null);
            Assert.AreEqual(2, result2.Count);
            Assert.AreEqual(op2.QuickNegate(), result2[0]);
            Assert.AreEqual(op1.QuickNegate(), result2[1]);
        }
        
        // depth first dp is stupid
        /*[TestMethod]*/
        public void TestSolveDP()
        {
            var rubik = GetResetRubik();
            var op1 = new Operation { DiscAxis = DiscAxes.X, DiscIndex = 0, Angle = Angles.Forward };
            var op2 = new Operation { DiscAxis = DiscAxes.Y, DiscIndex = 1, Angle = Angles.Mirror };
            Debug.WriteLine("------------------------------");
            Debug.WriteLine("Init");
            Debug.WriteLine(rubik);
            rubik.OperateSelf(op1);
            Debug.WriteLine("------------------------------");
            Debug.WriteLine(op1);
            Debug.WriteLine(rubik);
            rubik.OperateSelf(op2);
            Debug.WriteLine("------------------------------");
            Debug.WriteLine(op2);
            Debug.WriteLine(rubik);
            var solver = new RubikCubeSolverDP(rubik.Clone());
            solver.SolveStep += (dfs, state, type) =>
            {
                Debug.WriteLine("------------------------------");
                Debug.WriteLine(type);
                Debug.WriteLine(dfs.LastOperation);
                Debug.WriteLine(state);
            };
            var result = solver.SolveFirst();
            Assert.IsTrue(result != null);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(op2.QuickNegate(), result[0]);
            Assert.AreEqual(op1.QuickNegate(), result[1]);
        }

        [TestMethod]
        public void TestSolveRandom()
        {
            var rand = new Random(13);
            var orig = GetResetRubik();

            for (var t = 0; t < 1; t++)
            {
                var rubik = orig.Clone();

                Assert.IsTrue(rubik.IsDone());

                var n = 3;// rand.Next(2, 15);
                var ops = new Operation[n];
                DiscAxes dav = 0;
                int div = 0;
                Angles av = 0;
                for (var i = 0; i < n; i++)
                {
                    var da = rand.Next(3);
                    var di = rand.Next(2);
                    var a = rand.Next(3);
                    if (i != 0 && da == 0 && di == 0)
                    {
                        di = 1;
                    }
                    dav += da;
                    div += di;
                    av += a;
                    if ((int)dav >= 3) dav -= 3;
                    if (div >= 2) div -= 2;
                    if ((int)av >= 3) av -= 3;
                    ops[i] = new Operation { DiscAxis = dav, DiscIndex = div, Angle = av };
                    rubik.OperateSelf(ops[i]);
                }

                System.Diagnostics.Debug.WriteLine(rubik.ToString());

                var rcopy = rubik.Clone();
                foreach (var op in ops.Reverse())
                {
                    rcopy.OperateSelf(op.QuickNegate());
                }
                Assert.AreEqual(orig, rcopy);

                var solver = new RubikCubeSolver(rubik.Clone(), n);
#if false
                var c = 0;
                solver.Stepped += (v, k, op) =>
                {
                    if (k != null && op != null)
                    {
                        c++;
                    }
#if false
                    if (k == null) return;
                    var path = v.GetSolution();
                    var kk = k.Clone();
                    foreach (var o in path.Reverse())
                    {
                        kk.Operate(o.QuickNegate());
                    }
                    Assert.AreEqual(rubik, kk);
#endif
                };
#endif
                var result = solver.SolveFirst();

#if true
                Assert.IsTrue(result != null);

                foreach (var op in result)
                {
                    rubik.OperateSelf(op);
                }
                Assert.IsTrue(rubik.IsDone());
#endif
            }
        }
    }
}

