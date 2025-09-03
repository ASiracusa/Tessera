using UnityEngine;
using System;
using static BoonDice;
using System.Collections.Generic;

public class BoonDiePreset
{
    public string boonName;
    public BoonTrigger boonTrigger;
    public BonusField bonusTo;
    public Func<string, List<int>, int, int> bonusFormula;
    public Func<string, List<int>, bool> bonusCond;

    public BoonDiePreset (string boonName, BoonTrigger boonTrigger, BonusField bonusTo, Func<string, List<int>, int, int> bonusFormula, Func<string, List<int>, bool> bonusCond)
    {
        this.boonName = boonName;
        this.boonTrigger = boonTrigger;
        this.bonusTo = bonusTo;
        this.bonusFormula = bonusFormula;
        this.bonusCond = bonusCond;
    }
}
