using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using static Constants;

public class SpandoraManager : MonoBehaviour
{

    public GameObject diePrefab;

    private GameObject bottomRoot;
    private GameObject dieRoot;
    private GameObject basePointsTextObject;
    private GameObject multPointsTextObject;

    private List<int> spanPoses;
    private string currWordText;
    private int currWordScore;
    private List<string> spelledWords;
    private WordValidity validWord;

    private List<GameObject> diceObjects;
    private List<Die> diceData;

    private string[] fullDictionary;

    private void Start()
    {
        bottomRoot = GameObject.Find("BottomAnchor");
        dieRoot = GameObject.Find("BottomAnchor/DieRoot");

        spanPoses = new List<int>();
        spelledWords = new List<string>();
        diceObjects = new List<GameObject>();
        diceData = new List<Die>();

        GenerateDictionary();

        GenerateStartingDice();
        GenerateBoard();
    }

    void Update()
    {
        DrawWord();

        if (Input.GetKeyDown("space") && spanPoses.Count == 0)
        {
            GenerateBoard();
        }
    }

    static public bool IsAdjacent (int pos1, int pos2)
    {
        int row1 = pos1 / 5;
        int col1 = pos1 % 5;
        int row2 = pos2 / 5;
        int col2 = pos2 % 5;
        return Mathf.Abs(row1 - row2) <= 1 && Mathf.Abs(col1 - col2) <= 1;
    }

    private void DrawWord ()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;

            if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, LayerMask.GetMask("TileFace")))
            {
                int diePos = hitInfo.collider.gameObject.GetComponent<DieFaceData>().diePos;
                spanPoses.Add(diePos);
                CheckWord();
            }
        }
        else if (Input.GetMouseButton(0))
        {
            if (spanPoses.Count != 0) 
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hitInfo;

                if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, LayerMask.GetMask("TileFace")))
                {
                    int diePos = hitInfo.collider.gameObject.GetComponent<DieFaceData>().diePos;
                    if (IsAdjacent(diePos, spanPoses[^1]))
                    {
                        if (!spanPoses.Contains(diePos))
                        {
                            spanPoses.Add(diePos);
                        }
                        else if (spanPoses.Count > 1 && diePos == spanPoses[^2])
                        {
                            spanPoses.RemoveAt(spanPoses.Count - 1);
                        }
                    }
                    CheckWord();
                }
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (spanPoses.Count != 0) 
            {
                AttemptWord();
            }
        }
    }

    private void AttemptWord ()
    {
        
        if (validWord == WordValidity.New)
        {
            spelledWords.Add(currWordText);
        }

        spanPoses.Clear();
        currWordText = "";
        GameObject.Find("BottomAnchor/CurrWordCanvas/CurrWordText").GetComponent<TMP_Text>().text = "";
        GameObject.Find("BottomAnchor/CurrWordCanvas/CurrScoreRoot").SetActive(false);
        validWord = WordValidity.Invalid;
    }

    private void CheckWord ()
    {
        // Concatenate word and evaluate score
        currWordText = "";
        int basePoints = 0;
        int multPoints = Constants.LENGTH_MULTS[Mathf.Min(10, spanPoses.Count)];
        foreach (int diePos in spanPoses)
        {
            Die die = diceData[diePos];
            DieFace dieFace = die.faces[die.currFace];
            currWordText = currWordText + dieFace.faceText;
            basePoints += die.rank * Constants.TILE_VALUES[dieFace.faceText[0]];
            if (dieFace.letterColor == DieColor.Red)
            {
                multPoints += Constants.TILE_VALUES[dieFace.faceText[0]];
            }
        }
        currWordScore = basePoints * multPoints;
        GameObject.Find("BottomAnchor/CurrWordCanvas/CurrWordText").GetComponent<TMP_Text>().text = currWordText;
        GameObject.Find("BottomAnchor/CurrWordCanvas/CurrScoreRoot/ScoreBaseText").GetComponent<TMP_Text>().text = basePoints.ToString();
        GameObject.Find("BottomAnchor/CurrWordCanvas/CurrScoreRoot/ScoreMultText").GetComponent<TMP_Text>().text = multPoints.ToString();

        // Check word validity
        if (currWordText.Length < 3)
        {
            validWord = WordValidity.Invalid;
        }
        else
        {
            int wordIndex = Array.BinarySearch(fullDictionary, currWordText);
            validWord = wordIndex < 0 ? WordValidity.Invalid : 
                spelledWords.Contains(currWordText) ? WordValidity.Found : 
                WordValidity.New;
        }
        
        // Color word based on validity
        byte wordOpacity = validWord == WordValidity.Invalid ? (byte) 0x30 : 
            validWord == WordValidity.Found ? (byte) 0xA0 : 
            (byte) 0xFF;
        GameObject.Find("BottomAnchor/CurrWordCanvas/CurrWordText").GetComponent<TMP_Text>().faceColor = new Color32(0xFF, 0xFF, 0xFF, wordOpacity);

        // Hide/show score based on validity
        GameObject.Find("BottomAnchor/CurrWordCanvas/CurrScoreRoot").SetActive(validWord == WordValidity.New);
    }

    private void GenerateDictionary ()
    {
        try
        {
            fullDictionary = new string[279496];
            string dictPath = "Assets/Resources/FullDictionary.txt";
            using (StreamReader sr = new StreamReader(dictPath))
            {
                string line;
                int index = 0;
                while ((line = sr.ReadLine()) != null)
                {
                    fullDictionary[index] = line.ToUpper();
                    index++;
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log("The file could not be read:");
            Debug.Log(e.Message);
        }
    }

    private void GenerateStartingDice ()
    {
        foreach (LetterDiePresetType diePresetType in Constants.INITIAL_DICE_PRESET_TYPES)
        {
            LetterDiePreset letterDiePreset = Constants.LETTER_DIE_PRESETS[diePresetType];
            string[] chosenLetters = new string[3];
            if (letterDiePreset.repetitionType == RepetitionType.AllUnique)
            {
                int i = 0;
                while (i < 3)
                {
                    string chosenLetter = letterDiePreset.pool.Substring(UnityEngine.Random.Range(0, letterDiePreset.pool.Length), 1);
                    if (!chosenLetters.Contains(chosenLetter))
                    {
                        chosenLetters[i] = chosenLetter;
                        i++;
                    }
                }
            }
            else if (letterDiePreset.repetitionType == RepetitionType.AllSame)
            {
                string chosenLetter = letterDiePreset.pool.Substring(UnityEngine.Random.Range(0, letterDiePreset.pool.Length), 1);
                chosenLetters = new string[]{chosenLetter, chosenLetter, chosenLetter};
            }
            else
            {
                for (int i = 0; i < 3; i++)
                {
                    string chosenLetter = letterDiePreset.pool.Substring(UnityEngine.Random.Range(0, letterDiePreset.pool.Length), 1);
                    chosenLetters[i] = chosenLetter;
                }
            }
            
            DieFace[] dieFaces = new DieFace[] {
                new DieFace(letterDiePreset.letterColors[0], chosenLetters[0]),
                new DieFace(letterDiePreset.letterColors[1], chosenLetters[1]),
                new DieFace(letterDiePreset.letterColors[2], chosenLetters[2])
            };
            Die die = new Die(DieColor.Gray, 0, dieFaces, 1);
            diceData.Add(die);
        }
    }

    private void GenerateBoard ()
    {
        // Remove existing dice GameObjects
        foreach (Transform child in dieRoot.transform) {
            GameObject.Destroy(child.gameObject);
        }

        // Shuffle diceData
        for (int i = diceData.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            Die temp = diceData[i];
            diceData[i] = diceData[j];
            diceData[j] = temp;
        }

        // Create new dice GameObjects
        int diePos = 0;
        for (int r = 2; r >= -2; r--)
        {
            for (int c = 2; c >= -2; c--)
            {
                GameObject letterDie = Instantiate(diePrefab, Vector3.zero, Quaternion.identity, dieRoot.transform);
                letterDie.transform.localPosition = new Vector3(20 * c, 20 * r, 0);
                diceObjects.Add(letterDie);

                Die die = diceData[diePos];
                die.currFace = UnityEngine.Random.Range(0, 3);
                DieFace dieFace = die.faces[die.currFace];

                GameObject textCenter = letterDie.transform.Find("DieCanvas/TextCenter").gameObject;
                textCenter.GetComponent<TMP_Text>().text = dieFace.faceText;
                Color32 dieLetterColor = new Color32(
                    Constants.ColorBytes[dieFace.letterColor][0],
                    Constants.ColorBytes[dieFace.letterColor][1],
                    Constants.ColorBytes[dieFace.letterColor][2],
                    0x96
                );
                textCenter.GetComponent<TMP_Text>().faceColor = dieLetterColor;
                GameObject textBottom = letterDie.transform.Find("DieCanvas/TextBottom").gameObject;
                textBottom.GetComponent<TMP_Text>().text = (die.rank * Constants.TILE_VALUES[dieFace.faceText[0]]).ToString();
                GameObject hitbox = letterDie.transform.Find("DieFaceCollider").gameObject;
                hitbox.GetComponent<DieFaceData>().diePos = diePos;

                diePos++;
            }
        }
    }
}
