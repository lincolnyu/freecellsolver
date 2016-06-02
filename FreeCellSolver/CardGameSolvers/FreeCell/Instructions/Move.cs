namespace CardGameSolvers.FreeCell.Instructions
{
    public class Move : Instruction
    {
        public int SourceCol;
        public int MoveCount;
        public int TargetCol;

        public Move(int sourceCol, int moveCount, int targetCol)
        {
            SourceCol = sourceCol;
            MoveCount = moveCount;
            TargetCol = targetCol;
        }

        public override void Redo(FreeCellState state)
        {
            state.Move(SourceCol, TargetCol, MoveCount);
        }

        public override void Undo(FreeCellState state)
        {
            state.Move(TargetCol, SourceCol, MoveCount);
        }

        public override string ToString()
        {
            return $"Move {MoveCount} of col {SourceCol} to {TargetCol}";
        }

    }
}
