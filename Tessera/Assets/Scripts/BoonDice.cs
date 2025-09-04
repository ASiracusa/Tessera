using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using static Constants;

public static class BoonDice
{

    public enum BonusField
    {
        Base,
        Mult,
        Time,
        Gold
    }

    public static readonly BoonDiePreset[] BOON_DICE = new BoonDiePreset[] {
        new AttemptSpellBoonDiePreset(
            "LEO",
            BonusField.Base,
            (word, diePoses, rank) => { return 3 * rank; },
            (word, diePoses) => { return word.Length == 3; }
        ),
        new AttemptSpellBoonDiePreset(
            "AQUARIUS",
            BonusField.Mult,
            (word, diePoses, rank) => { return rank; },
            (word, diePoses) => { return word.Length == 4; }
        ),
        new AttemptSpellBoonDiePreset(
            "ARIES",
            BonusField.Mult,
            (word, diePoses, rank) => { return 2 * rank; },
            (word, diePoses) => { return word.Length == 5; }
        ),
        new AttemptSpellBoonDiePreset(
            "PISCES",
            BonusField.Mult,
            (word, diePoses, rank) => { return 4 * rank; },
            (word, diePoses) => { return word.Length == 6; }
        ),
        new AttemptSpellBoonDiePreset(
            "SAGITTARIUS",
            BonusField.Mult,
            (word, diePoses, rank) => { return 8 * rank; },
            (word, diePoses) => { return word.Length > 6; }
        ),
        new AttemptSpellBoonDiePreset(
            "CANCER",
            BonusField.Mult,
            (word, diePoses, rank) => { return rank; },
            (word, diePoses) => { return word.Length % 2 == 1; }
        ),
        new AttemptSpellBoonDiePreset(
            "LIBRA",
            BonusField.Base,
            (word, diePoses, rank) => { return 3 * rank; },
            (word, diePoses) => { return word.Length % 2 == 0; }
        ),
        new AttemptSpellBoonDiePreset(
            "VIRGO",
            BonusField.Base,
            (word, diePoses, rank) => { return rank * word.Length; },
            (word, diePoses) => { return word.Distinct().Count() == word.Length; }
        ),
        new AttemptSpellBoonDiePreset(
            "GEMINI",
            BonusField.Mult,
            (word, diePoses, rank) => { return 2 * rank * CountDoubleLetters(word, diePoses); },
            (word, diePoses) => { return CountDoubleLetters(word, diePoses) > 0; }
        ),
        new AttemptSpellBoonDiePreset(
            "TAURUS",
            BonusField.Mult,
            (word, diePoses, rank) => { return 2 * rank * LENGTH_MULTS[word.Length]; },
            (word, diePoses) => { return CountStraights(word, diePoses) > word.Length - 2; }
        ),
        new AttemptSpellBoonDiePreset(
            "SCORPIO",
            BonusField.Mult,
            (word, diePoses, rank) => { return rank * LENGTH_MULTS[word.Length]; },
            (word, diePoses) => { return CountStraights(word, diePoses) == 0; }
        ),
        new AttemptSpellBoonDiePreset(
            "CAPRICORN",
            BonusField.Mult,
            (word, diePoses, rank) => { return 3 * rank * LENGTH_MULTS[word.Length]; },
            (word, diePoses) => { return CountCrosses(word, diePoses) > 0; }
        ),
        new SpellableBoonDiePreset(
            "UNDIQUE",
            false,
            false,
            (diceData, diePoses, currPos) => { return true; },
            (diceData, diePoses, currPos) => { return true; },
            (diceData, diePoses, i) => { return ""; },
            (diceData, diePoses, rank) => { return 0; }
        ),
        new SpellableBoonDiePreset(
            "RECTUS",
            false,
            false,
            (diceData, diePoses, currPos) => { return true; },
            (diceData, diePoses, currPos) => { return currPos / 5 - diePoses[^1] / 5 == diePoses[^1] / 5 - diePoses[^2] / 5 && currPos % 5 - diePoses[^1] % 5 == diePoses[^1] % 5 - diePoses[^2] % 5; },
            (diceData, diePoses, i) => { return ""; },
            (diceData, diePoses, rank) => { return 3 * rank; }
        ),
        new SpellableBoonDiePreset(
            "CARDINALIS",
            false,
            false,
            (diceData, diePoses, currPos) => { return Mathf.Abs(currPos / 5 - diePoses[^1] / 5) + Mathf.Abs(currPos % 5 - diePoses[^1] % 5) == 1; },
            (diceData, diePoses, currPos) => { return Mathf.Abs(currPos / 5 - diePoses[^1] / 5) + Mathf.Abs(currPos % 5 - diePoses[^1] % 5) == 1; },
            (diceData, diePoses, i) => { return ""; },
            (diceData, diePoses, rank) => { return 5 * rank; }
        ),
        new SpellableBoonDiePreset(
            "OBLIQUUS",
            false,
            false,
            (diceData, diePoses, currPos) => { return Mathf.Abs(currPos / 5 - diePoses[^1] / 5) + Mathf.Abs(currPos % 5 - diePoses[^1] % 5) == 2; },
            (diceData, diePoses, currPos) => { return Mathf.Abs(currPos / 5 - diePoses[^1] / 5) + Mathf.Abs(currPos % 5 - diePoses[^1] % 5) == 2; },
            (diceData, diePoses, i) => { return ""; },
            (diceData, diePoses, rank) => { return 2 * rank; }
        ),
        new SpellableBoonDiePreset(
            "CLAUSULA",
            false,
            true,
            (diceData, diePoses, currPos) => { return true; },
            (diceData, diePoses, currPos) => { return false; },
            (diceData, diePoses, i) => { return ""; },
            (diceData, diePoses, rank) => { return rank; }
        ),
        new SpellableBoonDiePreset(
            "SIMULACRUM",
            false,
            true,
            (diceData, diePoses, currPos) => { return diceData[diePoses[^1]] is LetterDie; },
            (diceData, diePoses, currPos) => { return true; },
            (diceData, diePoses, i) => { LetterDie die = (LetterDie)diceData[diePoses[i-1]]; return die.faces[die.currFace].faceText; },
            (diceData, diePoses, rank) => { return 2 * rank; }
        ),
        new SpellableBoonDiePreset(
            "PROPHETIA",
            true,
            false,
            (diceData, diePoses, currPos) => { return true; },
            (diceData, diePoses, currPos) => { return diceData[currPos] is LetterDie; },
            (diceData, diePoses, i) => { if (i == diePoses.Count-1) return ""; else { LetterDie die = (LetterDie)diceData[diePoses[i+1]]; return die.faces[die.currFace].faceText; } },
            (diceData, diePoses, rank) => { return 2 * rank; }
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