using UnityEngine;

public class LetterDiePreset
{
    public string pool;
    public Constants.RepetitionType repetitionType;
    public Constants.DieColor[] letterColors;

    public LetterDiePreset (string pool, Constants.RepetitionType repetitionType, Constants.DieColor[] letterColors)
    {
        this.pool = pool;
        this.repetitionType = repetitionType;
        this.letterColors = letterColors;
    }
}
