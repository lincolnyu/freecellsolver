using GoBasedGameSolvers.Gobang;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FreeCellTests
{
    [TestClass]
    public class GobangTieTests
    {
        [TestMethod]
        public void TieTest()
        {
            var state = new GobangState(3, 3, 3);
            Assert.IsTrue(state.AliveCount == 0 && state.Count == 0);
            state[0, 0] = GoBased.GoSnapshot.PointStates.Black;
            Assert.IsTrue(state.AliveCount == 3);
            state[1, 1] = GoBased.GoSnapshot.PointStates.White;
            Assert.IsTrue(state.AliveCount == 5);
            state[0, 2] = GoBased.GoSnapshot.PointStates.Black;
            Assert.IsTrue(state.AliveCount == 6);
            state[0, 1] = GoBased.GoSnapshot.PointStates.White;
            Assert.IsTrue(state.AliveCount == 5);
            state[2, 1] = GoBased.GoSnapshot.PointStates.Black;
            Assert.IsTrue(state.AliveCount == 4);
            state[1, 2] = GoBased.GoSnapshot.PointStates.White;
            Assert.IsTrue(state.AliveCount == 4);
            state[1, 0] = GoBased.GoSnapshot.PointStates.Black;
            Assert.IsTrue(state.AliveCount == 3);
            Assert.IsFalse(state.IsTie);
            state[2, 0] = GoBased.GoSnapshot.PointStates.White;
            Assert.IsTrue(state.AliveCount == 0);
            Assert.IsTrue(state.IsTie);

            state[2, 0] = GoBased.GoSnapshot.PointStates.None;
            Assert.IsTrue(state.AliveCount == 3);
            Assert.IsFalse(state.IsTie);
            state[1, 0] = GoBased.GoSnapshot.PointStates.None;
            Assert.IsTrue(state.AliveCount == 4);
            state[1, 2] = GoBased.GoSnapshot.PointStates.None;
            Assert.IsTrue(state.AliveCount == 4);
            state[2, 1] = GoBased.GoSnapshot.PointStates.None;
            Assert.IsTrue(state.AliveCount == 5);
            state[0, 1] = GoBased.GoSnapshot.PointStates.None;
            Assert.IsTrue(state.AliveCount == 6);
            state[0, 2] = GoBased.GoSnapshot.PointStates.None;
            Assert.IsTrue(state.AliveCount == 5);
            state[1, 1] = GoBased.GoSnapshot.PointStates.None;
            Assert.IsTrue(state.AliveCount == 3);
            state[0, 0] = GoBased.GoSnapshot.PointStates.None;
            Assert.IsTrue(state.AliveCount == 0 && state.Count == 0);
        }
    }
}
