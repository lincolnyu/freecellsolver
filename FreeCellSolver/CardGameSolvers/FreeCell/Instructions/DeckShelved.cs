namespace CardGameSolvers.FreeCell.Instructions
{
    public class DeckShelved : Instruction
    {
        public int ShelvedIndex;
        private int _deckIdx;

        public DeckShelved(int shelvedIndex)
        {
            ShelvedIndex = shelvedIndex;
        }

        public override void Redo(FreeCellState state)
        {
            _deckIdx = state.DeckShelved(ShelvedIndex);
        }

        public override void Undo(FreeCellState state)
        {
            state.UndeckShelved(ShelvedIndex, _deckIdx);
        }

        public override string ToString()
        {
            return $"Deck shelved {ShelvedIndex} to {_deckIdx}";
        }
    }
}
