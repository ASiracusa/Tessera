using UnityEngine;
using static Constants;

public class LetterDie : Die
{
    public int currFace;
    public DieFace[] faces;

    public LetterDie (Constants.DieColor dieColor, int rank, int currFace, DieFace[] faces) : base(dieColor, rank)
    {
        this.currFace = currFace;
        this.faces = faces;
    }
}
