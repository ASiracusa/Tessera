using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using static Constants;
using static BoonDice;

public class SpandoraManager : MonoBehaviour
{

    public GameObject diePrefab;
    public GameObject tutorialText;
    public GameObject roundCompleteText;
    public GameObject gameOverText;
    public GameObject replayText;

    private GameObject bottomRoot;
    private GameObject dieRoot;
    private GameObject basePointsTextObject;
    private GameObject multPointsTextObject;

    private List<int> spanPoses;
    private string currWordText;
    private int currWordScore;
    private int currWordTimeGain;
    private List<string> spelledWords;
    private WordValidity validWord;

    private List<GameObject> diceObjects;
    private List<Die> diceData;
    private List<int> boonPoses;

    private string[] fullDictionary;

    private bool inCutscene;
    private int spandoraRound;
    private float roundTimescale;
    private int pointThreshold;
    private int remainingTime;
    private IEnumerator roundCountdownCoroutine;

    private void Start ()
    {
        bottomRoot = GameObject.Find("BottomAnchor");
        dieRoot = GameObject.Find("BottomAnchor/DieRoot");

        spanPoses = new List<int>();
        spelledWords = new List<string>();
        diceObjects = new List<GameObject>();
        diceData = new List<Die>();
        boonPoses = new List<int>();

        GenerateDictionary();
        GenerateStartingDice();

        StartCoroutine(StartGame());
    }

    void Update ()
    {
        if (!inCutscene)
        {
            DrawWord();

            if (Input.GetKeyDown(KeyCode.Space) && spanPoses.Count == 0)
            {
                GenerateBoard(true);
                roundTimescale += 0.5f;
            }
        }
    }

    static public bool IsAdjacent (int pos1, int pos2)
    {
        int row1 = pos1 / 5;
        int col1 = pos1 % 5;
        int row2 = pos2 / 5;
        int col2 = pos2 % 5;
        return Mathf.Abs(row1 - row2) == 1 || Mathf.Abs(col1 - col2) == 1;
    }

    private bool CheckIfSpellable(int diePos)
    {
        if (spanPoses.Count > 0 && diceData[spanPoses[^1]] is BoonDie boonDie1 && BOON_DICE[boonDie1.boonId] is SpellableBoonDiePreset spellableBoonDiePreset1)
        {
            if (!spellableBoonDiePreset1.exitDie(diceData, spanPoses, diePos)) return false;
        }
        if (spanPoses.Count == 0 && diceData[diePos] is BoonDie boonDie2 && BOON_DICE[boonDie2.boonId] is SpellableBoonDiePreset spellableBoonDiePreset2)
        {
            if (!spellableBoonDiePreset2.canStartWord) return false;
        }
        if (spanPoses.Count > 0 && diceData[diePos] is BoonDie boonDie3 && BOON_DICE[boonDie3.boonId] is SpellableBoonDiePreset spellableBoonDiePreset3)
        {
            if (!spellableBoonDiePreset3.enterDie(diceData, spanPoses, diePos)) return false;
        }
        return true;
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
                if (CheckIfSpellable(diePos))
                {
                    spanPoses.Add(diePos);
                    CheckWord();
                }
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
                    if (IsAdjacent(diePos, spanPoses[^1]) && CheckIfSpellable(diePos))
                    {
                        if (!spanPoses.Contains(diePos))
                        {
                            spanPoses.Add(diePos);
                        }
                        else if (spanPoses.Count > 1 && diePos == spanPoses[^2])
                        {
                            spanPoses.RemoveAt(spanPoses.Count - 1);
                        }
                        CheckWord();
                    }
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

            if (currWordTimeGain > 0)
            {
                remainingTime += currWordTimeGain;
                GameObject.Find("TopAnchor/LidCanvas/TimerText").GetComponent<TMP_Text>().text = remainingTime.ToString();
            }

            pointThreshold = Mathf.Max(pointThreshold - currWordScore, 0);
            GameObject.Find("TopAnchor/LidCanvas/ScoreThresholdText").GetComponent<TMP_Text>().text = pointThreshold.ToString();
            if (pointThreshold == 0)
            {
                StopCoroutine(roundCountdownCoroutine);
                StartCoroutine(RoundComplete());
            }
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
        int multPoints = LENGTH_MULTS[Mathf.Min(10, spanPoses.Count)];
        currWordTimeGain = 0;
        for (int i = 0; i < spanPoses.Count; i++)
        {
            int diePos = spanPoses[i];
            if (diceData[diePos] is LetterDie die)
            {
                DieFace dieFace = die.faces[die.currFace];
                currWordText += dieFace.faceText;
                basePoints += die.rank * TILE_VALUES[dieFace.faceText[0]];
                if (dieFace.letterColor == DieColor.Red)
                {
                    multPoints += TILE_VALUES[dieFace.faceText[0]];
                }
                else if (dieFace.letterColor == DieColor.Blue)
                {
                    currWordTimeGain += TILE_VALUES[dieFace.faceText[0]];
                }
            }
            else if (diceData[diePos] is BoonDie boonDie && BOON_DICE[boonDie.boonId] is SpellableBoonDiePreset spellableBoonDiePreset)
            {
                currWordText += spellableBoonDiePreset.dieText(diceData, spanPoses, i);
                basePoints += spellableBoonDiePreset.dieValue(diceData, spanPoses, boonDie.rank);
            }
        }
        foreach (int boonIndex in boonPoses)
        {
            BoonDie boonDie = (BoonDie)diceData[boonIndex];
            if (BOON_DICE[boonDie.boonId] is AttemptSpellBoonDiePreset boonDiePreset)
            {
                if (!boonDiePreset.bonusCond(currWordText, spanPoses))
                {
                    continue;
                }
                int bonus = boonDiePreset.bonusFormula(currWordText, spanPoses, boonDie.rank);
                if (boonDiePreset.bonusTo == BonusField.Base)
                {
                    basePoints += bonus;
                }
                else if (boonDiePreset.bonusTo == BonusField.Mult)
                {
                    multPoints += bonus;
                }
            }
            else if (BOON_DICE[boonDie.boonId] is SpellableBoonDiePreset)
            {
                
            }
        }
        currWordScore = basePoints * multPoints;
        GameObject.Find("BottomAnchor/CurrWordCanvas/CurrWordText").GetComponent<TMP_Text>().text = currWordText;
        GameObject.Find("BottomAnchor/CurrWordCanvas/CurrScoreRoot/ScoreBaseText").GetComponent<TMP_Text>().text = basePoints.ToString();
        GameObject.Find("BottomAnchor/CurrWordCanvas/CurrScoreRoot/ScoreMultText").GetComponent<TMP_Text>().text = multPoints.ToString();

        // Check word validity
        if (currWordText.Length < 3 || (diceData[spanPoses[^1]] is BoonDie boonDie2 && BOON_DICE[boonDie2.boonId] is SpellableBoonDiePreset spellableBoonDiePreset2 && !spellableBoonDiePreset2.canEndWord))
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
            string textFile = Resources.Load("FullDictionary").ToString();
            fullDictionary = new List<string>(textFile.Split(new char[] {'\r','\n'},StringSplitOptions.RemoveEmptyEntries)).ToArray();
        }
        catch (Exception e)
        {
            Debug.Log("The file could not be read:");
            Debug.Log(e.Message);
        }
    }

    private void GenerateStartingDice ()
    {
        foreach (LetterDiePresetType diePresetType in INITIAL_DICE_PRESET_TYPES)
        {
            LetterDiePreset letterDiePreset = LETTER_DIE_PRESETS[diePresetType];
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
                new (letterDiePreset.letterColors[0], chosenLetters[0]),
                new (letterDiePreset.letterColors[1], chosenLetters[1]),
                new (letterDiePreset.letterColors[2], chosenLetters[2])
            };
            LetterDie die = new (DieColor.Gray, 1, 0, dieFaces);
            diceData.Add(die);
        }

        for (int i = 0; i < 25-diceData.Count; i++)
        {
            BoonDie die = new(DieColor.Black, 1, UnityEngine.Random.Range(0, BOON_DICE.Length));
            diceData.Add(die);
        }
    }

    private void GenerateBoard (bool keepInPlace)
    {
        // Shuffle diceData
        ShuffleBoard(keepInPlace);

        // Remove existing dice GameObjects
        if (remainingTime > 0)
        {
            foreach (Transform child in dieRoot.transform)
            {
                Destroy(child.gameObject);
            }
        }

        // Create new dice GameObjects
        int diePos = 0;
        for (int r = 2; r >= -2; r--)
        {
            for (int c = 2; c >= -2; c--)
            {
                GameObject dieObject = Instantiate(diePrefab, Vector3.zero, Quaternion.identity, dieRoot.transform);
                dieObject.transform.localPosition = new Vector3(20 * c, 20 * r, 0);
                diceObjects.Add(dieObject);

                Die die = diceData[diePos];

                GameObject textCenter = dieObject.transform.Find("DieCanvas/TextCenter").gameObject;
                GameObject textBottom = dieObject.transform.Find("DieCanvas/TextBottom").gameObject;

                if (die is LetterDie letterDie)
                {
                    DieFace dieFace = letterDie.faces[letterDie.currFace];

                    textCenter.GetComponent<TMP_Text>().text = dieFace.faceText;
                    Color32 dieLetterColor = new(
                        ColorBytes[dieFace.letterColor][0],
                        ColorBytes[dieFace.letterColor][1],
                        ColorBytes[dieFace.letterColor][2],
                        0xc0
                    );
                    textCenter.GetComponent<TMP_Text>().faceColor = dieLetterColor;

                    textBottom.GetComponent<TMP_Text>().text = (die.rank * TILE_VALUES[dieFace.faceText[0]]).ToString();
                }
                else
                {
                    BoonDie boonDie = (BoonDie)die;

                    textCenter.GetComponent<TMP_Text>().text = BOON_DICE[boonDie.boonId].boonName;
                    textCenter.GetComponent<TMP_Text>().fontSize = 2f;

                    textBottom.GetComponent<TMP_Text>().text = "";
                }

                Color32 dieBaseColor = new(
                    ColorBytes[die.dieColor][0],
                    ColorBytes[die.dieColor][1],
                    ColorBytes[die.dieColor][2],
                    0xFF
                );
                Renderer renderer = dieObject.GetComponent<Renderer>();
                MaterialPropertyBlock block = new();
                renderer.GetPropertyBlock(block); // Get existing properties if any
                block.SetColor("_BaseColor", dieBaseColor);
                renderer.SetPropertyBlock(block);

                GameObject hitbox = dieObject.transform.Find("DieFaceCollider").gameObject;
                hitbox.GetComponent<DieFaceData>().diePos = diePos;

                diePos++;
            }
        }
    }

    private void ShuffleBoard (bool keepInPlace)
    {
        if (keepInPlace)
        {
            for (int i = 0; i < diceData.Count; i++)
            {
                if (diceData[i] is LetterDie)
                {
                    LetterDie letterDie = (LetterDie)diceData[i];
                    letterDie.currFace = UnityEngine.Random.Range(0, 3);
                }
            }
        }
        else
        {
            for (int i = diceData.Count - 1; i > 0; i--)
            {
                int j = UnityEngine.Random.Range(0, i + 1);
                if (diceData[j] is LetterDie)
                {
                    LetterDie letterDie = (LetterDie)diceData[j];
                    letterDie.currFace = UnityEngine.Random.Range(0, 3);
                }
                Die temp = diceData[i];
                diceData[i] = diceData[j];
                diceData[j] = temp;
            }
        }
    }

    private void GenerateBoonPoses ()
    {
        boonPoses.Clear();
        for (int i = 0; i < diceData.Count; i++)
        {
            if (diceData[i] is BoonDie)
            {
                boonPoses.Add(i);
            }
        }
    }

    private void BeginRound ()
    {
        spandoraRound++;
        roundTimescale = 1.0f;
        pointThreshold = 100 * (int)Mathf.Pow(2, (spandoraRound - 1) / 2) * (spandoraRound % 2 == 0 ? 3 : 2) / 2;

        GameObject.Find("TopAnchor/LidCanvas/RoundText").GetComponent<TMP_Text>().text = "ROUND " + spandoraRound.ToString();
        GameObject.Find("TopAnchor/LidCanvas/ScoreThresholdText").GetComponent<TMP_Text>().text = pointThreshold.ToString();

        spelledWords = new List<string>();
        GenerateBoard(false);
        GenerateBoonPoses();

        roundCountdownCoroutine = RoundCountdown(60);
        StartCoroutine(roundCountdownCoroutine);
    }

    private void EndRound ()
    {
        GameObject.Find("TopAnchor/LidCanvas/RoundText").GetComponent<TMP_Text>().text = "";
        GameObject.Find("TopAnchor/LidCanvas/TimerText").GetComponent<TMP_Text>().text = "";
        GameObject.Find("TopAnchor/LidCanvas/ScoreThresholdText").GetComponent<TMP_Text>().text = "";

        // Remove existing dice GameObjects
        foreach (Transform child in dieRoot.transform) {
            Destroy(child.gameObject);
        }
    }

    private IEnumerator StartGame ()
    {
        inCutscene = true;

        tutorialText.SetActive(true);
        yield return new WaitForSeconds(3.0f);

        tutorialText.SetActive(false);
        spandoraRound = 0;
        BeginRound();

        inCutscene = false;
    }

    private IEnumerator RoundComplete ()
    {
        inCutscene = true;

        EndRound();
        roundCompleteText.SetActive(true);
        yield return new WaitForSeconds(3.0f);

        roundCompleteText.SetActive(false);
        BeginRound();

        inCutscene = false;
    }

    private IEnumerator GameOver ()
    {
        inCutscene = true;
        StopCoroutine(roundCountdownCoroutine);
        EndRound();

        gameOverText.SetActive(true);
        replayText.SetActive(true);

        while (!Input.GetKeyDown(KeyCode.Space) && !Input.GetKeyDown(KeyCode.Escape))
        {
            yield return null;
        }
        if (Input.GetKey(KeyCode.Escape))
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #endif
            Application.Quit();
        }
        else
        {
            gameOverText.SetActive(false);
            replayText.SetActive(false);
            StartCoroutine(StartGame());
        }
    }

    private IEnumerator RoundCountdown (int startingTime)
    {
        remainingTime = startingTime;
        GameObject.Find("TopAnchor/LidCanvas/TimerText").GetComponent<TMP_Text>().text = remainingTime.ToString();
        while (remainingTime > 0)
        {
            yield return new WaitForSeconds(1.0f / roundTimescale);
            remainingTime--;
            GameObject.Find("TopAnchor/LidCanvas/TimerText").GetComponent<TMP_Text>().text = remainingTime.ToString();
        }

        StartCoroutine(GameOver());
    }
}
