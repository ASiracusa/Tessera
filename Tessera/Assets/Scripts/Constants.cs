using UnityEngine;
using System.Collections.Generic;

public static class Constants
{
    public static readonly Dictionary<char, int> TILE_VALUES = new Dictionary<char, int> {
        { 'A', 1 },
        { 'B', 3 },
        { 'C', 3 },
        { 'D', 2 },
        { 'E', 1 },
        { 'F', 4 },
        { 'G', 2 },
        { 'H', 4 },
        { 'I', 1 },
        { 'J', 8 },
        { 'K', 5 },
        { 'L', 1 },
        { 'M', 3 },
        { 'N', 1 },
        { 'O', 1 },
        { 'P', 3 },
        { 'Q', 10 },
        { 'R', 1 },
        { 'S', 1 },
        { 'T', 1 },
        { 'U', 1 },
        { 'V', 4 },
        { 'W', 4 },
        { 'X', 8 },
        { 'Y', 4 },
        { 'Z', 10 }
    };

    public static readonly int[] LENGTH_MULTS = new int[] {
        0,
        0,
        0,
        1,
        2,
        3,
        5,
        7,
        10,
        15,
        20
    };

    public enum DieColor
    {
        White,
        Gray,
        Black,
        Red,
        Yellow,
        Blue,
        Green,
        Purple,
        Orange
    }

    public static readonly Dictionary<DieColor, byte[]> ColorBytes = new Dictionary<DieColor, byte[]> {
        { DieColor.White, new [] { (byte)0xFF, (byte)0xFF, (byte)0xFF }},
        { DieColor.Gray, new [] { (byte)0x80, (byte)0x80, (byte)0x80 }},
        { DieColor.Black, new [] { (byte)0x00, (byte)0x00, (byte)0x00 }},
        { DieColor.Red, new [] { (byte)0xE0, (byte)0x0D, (byte)0x3B }},
        { DieColor.Yellow, new [] { (byte)0xDE, (byte)0xAA, (byte)0x11 }},
        { DieColor.Blue, new [] { (byte)0x0D, (byte)0x5E, (byte)0xE0 }},
        { DieColor.Green, new [] { (byte)0x24, (byte)0xB0, (byte)0x43 }},
        { DieColor.Purple, new [] { (byte)0x8C, (byte)0x0D, (byte)0xE0 }},
        { DieColor.Orange, new [] { (byte)0xE0, (byte)0x69, (byte)0x0D }}
    };

    public enum WordValidity
    {
        Invalid,
        Found,
        New
    }

    public enum RepetitionType
    {
        AllUnique,
        AllSame,
        Random
    }

    public enum LetterDiePresetType
    {
        CommonVowels,
        LessCommonVowels,
        CommonVowelsOneColor,
        OneVowelThreeColors,
        ThreeVowelsOneColor,
        CommonConsonants,
        LessCommonConsonants,
        CommonConsonantsOneColor,
        OneConsonantThreeColors,
        ThreeConsonantsOneColor,
        TwoThreePointLetters,
        ThreeFourPointLetters,
        FourFivePointLetters,
        MidValueOneColor,
        RareLetters,
        RandomLetters
    }

    public static readonly Dictionary<LetterDiePresetType, LetterDiePreset> LETTER_DIE_PRESETS = new Dictionary<LetterDiePresetType, LetterDiePreset> {
        {LetterDiePresetType.CommonVowels, new LetterDiePreset("EEEEAAAIIIOOU", RepetitionType.AllUnique, new []{DieColor.White, DieColor.White, DieColor.White})},
        {LetterDiePresetType.LessCommonVowels, new LetterDiePreset("UUUUOOOAAIIE", RepetitionType.AllUnique, new []{DieColor.White, DieColor.White, DieColor.White})},
        {LetterDiePresetType.CommonVowelsOneColor, new LetterDiePreset("EEEEAAAIIIOOU", RepetitionType.AllUnique, new []{DieColor.Red, DieColor.White, DieColor.White})},
        {LetterDiePresetType.OneVowelThreeColors, new LetterDiePreset("EEEEAAAIIIOOU", RepetitionType.AllSame, new []{DieColor.Red, DieColor.Yellow, DieColor.Blue})},
        {LetterDiePresetType.ThreeVowelsOneColor, new LetterDiePreset("EEEEAAAIIIOOU", RepetitionType.AllUnique, new []{DieColor.Red, DieColor.White, DieColor.White})},
        {LetterDiePresetType.CommonConsonants, new LetterDiePreset("TTTTNNNSSSRRLL", RepetitionType.AllUnique, new []{DieColor.White, DieColor.White, DieColor.White})},
        {LetterDiePresetType.LessCommonConsonants, new LetterDiePreset("DDDDGGGGLLLRRRSSNNT", RepetitionType.AllUnique, new []{DieColor.White, DieColor.White, DieColor.White})},
        {LetterDiePresetType.CommonConsonantsOneColor, new LetterDiePreset("TTTTNNNSSSRRLL", RepetitionType.AllUnique, new []{DieColor.Red, DieColor.White, DieColor.White})},
        {LetterDiePresetType.OneConsonantThreeColors, new LetterDiePreset("TTTTNNNSSSRRLL", RepetitionType.AllSame, new []{DieColor.Red, DieColor.Yellow, DieColor.Blue})},
        {LetterDiePresetType.ThreeConsonantsOneColor, new LetterDiePreset("TTTTNNNSSSRRLL", RepetitionType.AllUnique, new []{DieColor.Red, DieColor.White, DieColor.White})},
        {LetterDiePresetType.TwoThreePointLetters, new LetterDiePreset("DDDDGGGGBBCCMMPP", RepetitionType.AllUnique, new []{DieColor.White, DieColor.White, DieColor.White})},
        {LetterDiePresetType.ThreeFourPointLetters, new LetterDiePreset("BBBBBCCCCCMMMMMPPPPPFFFFHHHHVVVVWWWWYYYY", RepetitionType.AllUnique, new []{DieColor.White, DieColor.White, DieColor.White})},
        {LetterDiePresetType.FourFivePointLetters, new LetterDiePreset("KKKKKFFFHHHVVVWWWYYY", RepetitionType.AllUnique, new []{DieColor.White, DieColor.White, DieColor.White})},
        {LetterDiePresetType.MidValueOneColor, new LetterDiePreset("TNSRLDGBCMPFHVWYK", RepetitionType.AllUnique, new []{DieColor.Red, DieColor.White, DieColor.White})},
        {LetterDiePresetType.RareLetters, new LetterDiePreset("XXXXXXJJJJQQQQZZZZ", RepetitionType.AllUnique, new []{DieColor.White, DieColor.White, DieColor.White})},
        {LetterDiePresetType.RandomLetters, new LetterDiePreset("QWERTYUIOPASDFGHJKLZXCVBNM", RepetitionType.Random, new []{DieColor.White, DieColor.White, DieColor.White})},
    };

    public static readonly LetterDiePresetType[] INITIAL_DICE_PRESET_TYPES = new LetterDiePresetType[] {
        LetterDiePresetType.CommonVowels,
        LetterDiePresetType.CommonVowels,
        LetterDiePresetType.CommonVowels,
        LetterDiePresetType.LessCommonVowels,
        LetterDiePresetType.OneVowelThreeColors,
        LetterDiePresetType.ThreeVowelsOneColor,
        LetterDiePresetType.CommonConsonants,
        LetterDiePresetType.CommonConsonants,
        LetterDiePresetType.CommonConsonants,
        LetterDiePresetType.LessCommonConsonants,
        LetterDiePresetType.CommonConsonantsOneColor,
        LetterDiePresetType.OneConsonantThreeColors,
        LetterDiePresetType.ThreeConsonantsOneColor,
        LetterDiePresetType.TwoThreePointLetters,
        LetterDiePresetType.TwoThreePointLetters,
        LetterDiePresetType.TwoThreePointLetters,
        LetterDiePresetType.ThreeFourPointLetters,
        LetterDiePresetType.ThreeFourPointLetters,
        LetterDiePresetType.ThreeFourPointLetters,
        LetterDiePresetType.FourFivePointLetters,
        LetterDiePresetType.FourFivePointLetters,
        LetterDiePresetType.MidValueOneColor,
        LetterDiePresetType.MidValueOneColor,
        LetterDiePresetType.RareLetters,
        LetterDiePresetType.RareLetters
    };
}
