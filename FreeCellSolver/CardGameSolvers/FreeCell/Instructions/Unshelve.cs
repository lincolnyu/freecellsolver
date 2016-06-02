namespace CardGameSolvers.FreeCell.Instructions
{
    public class Unshelve : Instruction
    {
        public int ShelvedIndex;
        public int TargetCol;

        public Unshelve(int targetCol, int shelvedIndex)
        {
            TargetCol = targetCol;
            ShelvedIndex = shelvedIndex;
        }

        public override void Redo(FreeCellState state)
        {
            state.Unshelve(TargetCol, ShelvedIndex);
        }

        public override void Undo(FreeCellState state)
        {
            state.Shelve(TargetCol, ShelvedIndex);
        }

        public override string ToString()
        {
            return $"Unshelve {ShelvedIndex} to {TargetCol}";
        }
    }
}
