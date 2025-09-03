using UnityEngine;
using static Constants;

public class Die
{
    public DieColor dieColor;
    public int rank;

    public Die (DieColor dieColor, int rank)
    {
        this.dieColor = dieColor;
        this.rank = rank;
    }
}
