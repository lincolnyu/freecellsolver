namespace CardGameSolvers.FreeCell.Instructions
{
    public class DeckActive : Instruction
    {
        public int SourceCol;
        private int _deckIdx;

        public DeckActive(int sourceCol)
        {
            SourceCol = sourceCol;
        }

        public override void Redo(FreeCellState state)
        {
            _deckIdx = state.DeckActive(SourceCol);
        }

        public override void Undo(FreeCellState state)
        {
            state.UndeckActive(SourceCol, _deckIdx);
        }

        public override string ToString()
        {
            return $"Deck col {SourceCol} to {_deckIdx}";
        }
    }
}
