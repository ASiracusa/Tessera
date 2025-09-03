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

    public static readonly string[] ARITHMETIC_OPS = new string[] {
        "+",
        "-",
        "*",
        "/",
        "%"
    };

    public static readonly string[] COMPARISON_OPS = new string[] {
        "==",
        "!=",
        "<",
        ">",
        "<=",
        ">="
    };

    public static readonly BoonDiePreset[] BOON_DICE = new BoonDiePreset[] {
        new("LEO", BoonTrigger.CheckWord, BonusField.Base, "3 * rank", "length == 3"),
        new("AQUARIUS", BoonTrigger.CheckWord, BonusField.Mult, "rank", "length == 4"),
        new("ARIES", BoonTrigger.CheckWord, BonusField.Mult, "2 * rank", "length == 5"),
        new("PISCES", BoonTrigger.CheckWord, BonusField.Mult, "4 * rank", "length == 6"),
        new("SAGITTARIUS", BoonTrigger.CheckWord, BonusField.Mult, "8 * rank", "length > 6"),
        new("CANCER", BoonTrigger.CheckWord, BonusField.Mult, "rank", "length % 2 == 1"),
        new("VIRGO", BoonTrigger.CheckWord, BonusField.Base, "rank * length", "uniqueletters == length"),
        new("GEMINI", BoonTrigger.CheckWord, BonusField.Mult, "2 * rank * doubleletters", "doubleletters > 0"),
        new("LIBRA", BoonTrigger.CheckWord, BonusField.Base, "3 * rank", "length % 2 == 0"),
        new("TAURUS", BoonTrigger.CheckWord, BonusField.Mult, "2 * rank * lenmult", "straights == length - 2"),
        new("SCORPIO", BoonTrigger.CheckWord, BonusField.Mult, "rank * lenmult", "straights == 0"),
        new("CAPRICORN", BoonTrigger.CheckWord, BonusField.Mult, "3 * rank * lenmult", "crosses > 0"),
    };

    public static bool CheckBoonCond (string boonCond, string word, List<int> diePoses)
    {
        // Find which condition is being used
        string comparator = null;
        foreach (string compOper in COMPARISON_OPS)
        {
            if (boonCond.Contains(compOper))
            {
                comparator = compOper;
                break;
            }
        }
        if (comparator == null)
        {
            throw new ArgumentException("No valid comparator.");
        }

        // Calculate values of both sides of the comparison
        string[] sides = boonCond.Split(comparator);
        int total1 = CalculateBoonBonus(sides[0], word, diePoses, 0);
        int total2 = CalculateBoonBonus(sides[1], word, diePoses, 0);

        // Return evaluation based on totals and comparator
        return comparator switch
        {
            "==" => total1 == total2,
            "!=" => total1 != total2,
            "<" => total1 < total2,
            ">" => total1 > total2,
            "<=" => total1 <= total2,
            ">=" => total1 >= total2,
            _ => false,
        };
    }

    public static int CalculateBoonBonus (string boonFormula, string word, List<int> diePoses, int rank)
    {
        string[] tokens = boonFormula.Trim().Split(" ");
        int total = 0;
        string oper = "+";
        int value;

        for (int i = 0; i < tokens.Length; i++)
        {
            string token = tokens[i];
            if (i % 2 == 0)
            {
                switch (token)
                {
                    case "rank":
                        value = rank;
                        break;
                    case "length":
                        value = word.Length;
                        break;
                    case "lenmult":
                        value = LENGTH_MULTS[word.Length];
                        break;
                    case "uniqueletters":
                        value = word.Distinct().Count();
                        break;
                    case "doubleletters":
                        value = 0;
                        for (int j = 0; j < word.Length - 1; j++)
                        {
                            if (word[j] == word[j + 1])
                            {
                                value += 1;
                            }
                        }
                        break;
                    case "straights":
                        value = 0;
                        for (int j = 0; j < word.Length - 2; j++)
                        {
                            if (diePoses[j] / 5 - diePoses[j + 1] / 5 == diePoses[j + 1] / 5 - diePoses[j + 2] / 5 &&
                                diePoses[j] % 5 - diePoses[j + 1] % 5 == diePoses[j + 1] % 5 - diePoses[j + 2] % 5)
                            {
                                value += 1;
                            }
                        }
                        break;
                    case "crosses":
                        value = 0;
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
                        break;
                    default:
                        bool isNumeric = int.TryParse(token, out value);
                        if (!isNumeric)
                        {
                            throw new ArgumentException("Invalid parameter in formula.");
                        }
                        break;
                }

                switch (oper)
                {
                    case "+":
                        total += value;
                        break;
                    case "-":
                        total -= value;
                        break;
                    case "*":
                        total *= value;
                        break;
                    case "/":
                        total /= value;
                        break;
                    case "%":
                        total %= value;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                if (ARITHMETIC_OPS.Contains(token))
                {
                    oper = token;
                }
                else
                {
                    throw new ArgumentException("Invalid operator in formula.");
                }
            }
        }

        return total;
    }

}