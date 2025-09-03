using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

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
        "/"
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
        new BoonDiePreset("LEO", BoonTrigger.CheckWord, BonusField.Base, "3 * rank", "length == 3")
    };

    public static bool CheckBoonCond (string boonCond, string word)
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
        int total1 = CalculateBoonBonus(sides[0], word, 0);
        int total2 = CalculateBoonBonus(sides[1], word, 0);

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

    public static int CalculateBoonBonus (string boonFormula, string word, int rank)
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