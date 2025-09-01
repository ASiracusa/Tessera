using UnityEngine;
using static Constants;

public class Die
{
    public Constants.DieColor dieColor;
    public int rank;

    public Die (Constants.DieColor dieColor, int rank)
    {
        this.dieColor = dieColor;
        this.rank = rank;
    }
}
