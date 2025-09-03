using UnityEngine;
using static BoonDice;

public class BoonDiePreset
{
    public string boonName;
    public BoonTrigger boonTrigger;
    public BonusField bonusTo;
    public string bonusFormula;
    public string bonusCond;

    public BoonDiePreset (string boonName, BoonTrigger boonTrigger, BonusField bonusTo, string bonusFormula, string bonusCond)
    {
        this.boonName = boonName;
        this.boonTrigger = boonTrigger;
        this.bonusTo = bonusTo;
        this.bonusFormula = bonusFormula;
        this.bonusCond = bonusCond;
    }
}
