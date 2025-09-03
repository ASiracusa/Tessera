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

    public static readonly BoonDiePreset[] BOON_DICE = new BoonDiePreset[] {
        new BoonDiePreset("SAGITTARIUS", BoonTrigger.CheckWord, BonusField.Base, "3 * rank", "length = 3")
    };

    public static int CalculateBoonBonus (string boonFormula, string word, int rank)
    {
        string[] tokens = boonFormula.Split(" ");
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