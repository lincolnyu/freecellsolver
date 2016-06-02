namespace CardGameSolvers.FreeCell.Instructions
{
    public abstract class Instruction
    {
        public abstract void Redo(FreeCellState state);
        public abstract void Undo(FreeCellState state);
    }
}
