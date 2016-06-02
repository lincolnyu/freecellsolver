using System.Collections.Generic;
using CardGameSolvers.FreeCell.Instructions;

namespace CardGameSolvers.FreeCell
{
    public class Step
    {
        public int SelectedInstruction;

        public List<Instruction> PossibleMoves = new List<Instruction>();
    }
}
