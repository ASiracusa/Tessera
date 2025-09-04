using System;
using System.Collections.Generic;
using static BoonDice;

public class AttemptSpellBoonDiePreset : BoonDiePreset
{
    public BonusField? bonusTo;
    public Func<string, List<int>, int, int> bonusFormula;
    public Func<string, List<int>, bool> bonusCond;

    public AttemptSpellBoonDiePreset(string boonName, BonusField? bonusTo, Func<string, List<int>, int, int> bonusFormula, Func<string, List<int>, bool> bonusCond) : base(boonName)
    {
        this.bonusTo = bonusTo;
        this.bonusFormula = bonusFormula;
        this.bonusCond = bonusCond;
    }
}
