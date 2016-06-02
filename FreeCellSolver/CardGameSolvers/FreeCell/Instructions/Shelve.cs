namespace CardGameSolvers.FreeCell.Instructions
{
    public class Shelve : Instruction
    {
        public int SourceCol;
        public int ShelvedIndex;

        public Shelve(int sourceCol, int shelvedIndex)
        {
            SourceCol = sourceCol;
            ShelvedIndex = shelvedIndex;
        }

        public override void Redo(FreeCellState state)
        {
            state.Shelve(SourceCol, ShelvedIndex);
        }

        public override void Undo(FreeCellState state)
        {
            state.Unshelve(SourceCol, ShelvedIndex);
        }

        public override string ToString()
        {
            return $"Shelve {SourceCol} on {ShelvedIndex}";
        }
    }
}
