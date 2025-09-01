using UnityEngine;
using static Constants;

public class BoonDie : Die
{
    public string boonName;
    public BoonTrigger boonTrigger;
    public BonusField bonusTo;
    public string bonusFormula;
    public string bonusCond;

    public BoonDie (Constants.DieColor dieColor, int rank, string boonName, Constants.BoonTrigger boonTrigger, Constants.BonusField bonusTo, string bonusFormula, string bonusCond) : base(dieColor, rank)
    {
        this.boonName = boonName;
        this.boonTrigger = boonTrigger;
        this.bonusTo = bonusTo;
        this.bonusFormula = bonusFormula;
        this.bonusCond = bonusCond;
    }
}
