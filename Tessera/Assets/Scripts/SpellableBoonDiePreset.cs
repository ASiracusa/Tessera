using System;
using System.Collections.Generic;

public class SpellableBoonDiePreset : BoonDiePreset
{
    public bool canStartWord;
    public bool canEndWord;
    public Func<List<Die>, List<int>, int, bool> enterDie;
    public Func<List<Die>, List<int>, int, bool> exitDie;
    public Func<List<Die>, List<int>, int, string> dieText;
    public Func<List<Die>, List<int>, int, int> dieValue;

    public SpellableBoonDiePreset(string boonName, bool canStartWord, bool canEndWord, Func<List<Die>, List<int>, int, bool> enterDie, Func<List<Die>, List<int>, int, bool> exitDie, Func<List<Die>, List<int>, int, string> dieText, Func<List<Die>, List<int>, int, int> dieValue) : base(boonName)
    {
        this.canEndWord = canEndWord;
        this.canStartWord = canStartWord;
        this.enterDie = enterDie;
        this.exitDie = exitDie;
        this.dieText = dieText;
        this.dieValue = dieValue;
    }
}
