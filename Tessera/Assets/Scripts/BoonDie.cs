using UnityEngine;
using static Constants;

public class BoonDie : Die
{
    public int boonId;

    public BoonDie (Constants.DieColor dieColor, int rank, int boonId) : base(dieColor, rank)
    {
        this.boonId = boonId;
    }
}
