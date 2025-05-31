using UnityEngine;
using static Constants;

public class Die
{
    public Constants.DieColor dieColor;
    public int currFace;
    public DieFace[] faces;
    public int rank;

    public Die (Constants.DieColor dieColor, int currFace, DieFace[] faces, int rank)
    {
        this.dieColor = dieColor;
        this.currFace = currFace;
        this.faces = faces;
        this.rank = rank;
    }
}
