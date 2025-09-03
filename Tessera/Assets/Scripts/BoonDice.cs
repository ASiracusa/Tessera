using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using static Constants;

public static class BoonDice
{
    public enum BoonTrigger
    {
        CheckWord
    }

    public enum BonusField
    {
        Base,
        Mult,
        Time,
        Gold
    }

    public static readonly BoonDiePreset[] BOON_DICE = new BoonDiePreset[] {
        new(
            "LEO",
            BoonTrigger.CheckWord,
            BonusField.Base,
            (word, diePoses, rank) => { return 3 * rank; },
            (word, diePoses) => { return word.Length == 3; }
        ),
        new(
            "AQUARIUS",
            BoonTrigger.CheckWord,
            BonusField.Mult,
            (word, diePoses, rank) => { return rank; },
            (word, diePoses) => { return word.Length == 4; }
        ),
        new(
            "ARIES",
            BoonTrigger.CheckWord,
            BonusField.Mult,
            (word, diePoses, rank) => { return 2 * rank; },
            (word, diePoses) => { return word.Length == 5; }
        ),
        new(
            "PISCES",
            BoonTrigger.CheckWord,
            BonusField.Mult,
            (word, diePoses, rank) => { return 4 * rank; },
            (word, diePoses) => { return word.Length == 6; }
        ),
        new(
            "SAGITTARIUS",
            BoonTrigger.CheckWord,
            BonusField.Mult,
            (word, diePoses, rank) => { return 8 * rank; },
            (word, diePoses) => { return word.Length > 6; }
        ),
        new(
            "CANCER",
            BoonTrigger.CheckWord,
            BonusField.Mult,
            (word, diePoses, rank) => { return rank; },
            (word, diePoses) => { return word.Length % 2 == 1; }
        ),
        new(
            "LIBRA",
            BoonTrigger.CheckWord,
            BonusField.Base,
            (word, diePoses, rank) => { return 3 * rank; },
            (word, diePoses) => { return word.Length % 2 == 0; }
        ),
        new(
            "VIRGO",
            BoonTrigger.CheckWord,
            BonusField.Base,
            (word, diePoses, rank) => { return rank * word.Length; },
            (word, diePoses) => { return word.Distinct().Count() == word.Length; }
        ),
        new(
            "GEMINI",
            BoonTrigger.CheckWord,
            BonusField.Mult,
            (word, diePoses, rank) => { return 2 * rank * CountDoubleLetters(word, diePoses); },
            (word, diePoses) => { return CountDoubleLetters(word, diePoses) > 0; }
        ),
        new(
            "TAURUS",
            BoonTrigger.CheckWord,
            BonusField.Mult,
            (word, diePoses, rank) => { return 2 * rank * LENGTH_MULTS[word.Length]; },
            (word, diePoses) => { return CountStraights(word, diePoses) > word.Length - 2; }
        ),
        new(
            "SCORPIO",
            BoonTrigger.CheckWord,
            BonusField.Mult,
            (word, diePoses, rank) => { return rank * LENGTH_MULTS[word.Length]; },
            (word, diePoses) => { return CountStraights(word, diePoses) == 0; }
        ),
        new(
            "CAPRICORN",
            BoonTrigger.CheckWord,
            BonusField.Mult,
            (word, diePoses, rank) => { return 3 * rank * LENGTH_MULTS[word.Length]; },
            (word, diePoses) => { return CountCrosses(word, diePoses) > 0; }
        ),
    };

    private static int CountDoubleLetters(string word, List<int> diePoses)
    {
        int value = 0;
        for (int j = 0; j < word.Length - 1; j++)
        {
            if (word[j] == word[j + 1])
            {
                value += 1;
            }
        }
        return value;
    }

    private static int CountStraights(string word, List<int> diePoses)
    {
        int value = 0;
        for (int j = 0; j < word.Length - 2; j++)
        {
            if (diePoses[j] / 5 - diePoses[j + 1] / 5 == diePoses[j + 1] / 5 - diePoses[j + 2] / 5 &&
                diePoses[j] % 5 - diePoses[j + 1] % 5 == diePoses[j + 1] % 5 - diePoses[j + 2] % 5)
            {
                value += 1;
            }
        }
        return value;
    }

    private static int CountCrosses(string word, List<int> diePoses)
    {
        int value = 0;
        List<int> intersections = new();
        for (int j = 0; j < word.Length - 1; j++)
        {
            if (Mathf.Abs(diePoses[j] / 5 - diePoses[j + 1] / 5) == 1 && Mathf.Abs(diePoses[j] % 5 - diePoses[j + 1] % 5) == 1)
            {
                int intersection = diePoses[j] + diePoses[j + 1];
                if (intersections.Contains(intersection))
                {
                    value += 1;
                }
                else
                {
                    intersections.Add(intersection);
                }
            }
        }
        return value;
    }

}