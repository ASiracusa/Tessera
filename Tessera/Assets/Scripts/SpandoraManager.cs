using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using TMPro;
using static Constants;

public class SpandoraManager : MonoBehaviour
{

    public GameObject diePrefab;

    private GameObject bottomRoot;
    private GameObject currWordTextObject;

    private List<int> spanPoses;
    private string currWordText;
    private List<string> spelledWords;
    private WordValidity validWord;

    private List<GameObject> diceObjects;
    private List<Die> diceData;

    private string[] fullDictionary;

    private void Start()
    {
        GenerateDictionary();

        bottomRoot = GameObject.Find("BottomAnchor");
        currWordTextObject = GameObject.Find("BottomAnchor/CurrWordCanvas/CurrWordText");

        spanPoses = new List<int>();
        spelledWords = new List<string>();
        diceObjects = new List<GameObject>();
        diceData = new List<Die>();

        int diePos = 0;
        for (int r = 2; r >= -2; r--)
        {
            for (int c = 2; c >= -2; c--)
            {
                int letterInd = UnityEngine.Random.Range(0,26);
                char letter = (char) (65 + letterInd);
                int letterVal = Constants.TILE_VALUES[letter];
                DieFace dieFace = new DieFace(DieColor.White, letter.ToString(), letterVal);
                DieFace[] dieFaces = new DieFace[]{dieFace};
                Die die = new Die(DieColor.Gray, 0, dieFaces);
                diceData.Add(die);

                GameObject letterDie = Instantiate(diePrefab, Vector3.zero, Quaternion.identity, bottomRoot.transform);
                letterDie.transform.localPosition = new Vector3(20 * c, 20 * r, 0);
                diceObjects.Add(letterDie);

                GameObject textCenter = letterDie.transform.Find("DieCanvas/TextCenter").gameObject;
                textCenter.GetComponent<TMP_Text>().text = dieFace.faceText;
                GameObject textBottom = letterDie.transform.Find("DieCanvas/TextBottom").gameObject;
                textBottom.GetComponent<TMP_Text>().text = letterVal.ToString();
                GameObject hitbox = letterDie.transform.Find("DieFaceCollider").gameObject;
                hitbox.GetComponent<DieFaceData>().diePos = diePos;

                diePos++;
            }
        }
    }

    void Update()
    {
        DrawWord();
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
                Die currDie = diceData[diePos];
                currWordText = currDie.faces[currDie.currFace].faceText;
                currWordTextObject.GetComponent<TMP_Text>().text = currWordText;
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
                            Die currDie = diceData[diePos];
                            currWordText = currWordText + currDie.faces[currDie.currFace].faceText;
                            currWordTextObject.GetComponent<TMP_Text>().text = currWordText;
                        }
                        else if (spanPoses.Count > 1 && diePos == spanPoses[^2])
                        {
                            spanPoses.RemoveAt(spanPoses.Count - 1);
                            currWordText = currWordText.Substring(0, currWordText.Length - 1);
                            currWordTextObject.GetComponent<TMP_Text>().text = currWordText;
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
        CheckWord();
        if (validWord == WordValidity.New)
        {
            spelledWords.Add(currWordText);
        }

        spanPoses.Clear();
        currWordText = "";
        currWordTextObject.GetComponent<TMP_Text>().text = "";
        validWord = WordValidity.Invalid;
    }

    private void CheckWord ()
    {
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
        
        byte wordOpacity = validWord == WordValidity.Invalid ? (byte) 0x30 : 
            validWord == WordValidity.Found ? (byte) 0xA0 : 
            (byte) 0xFF;
        currWordTextObject.GetComponent<TMP_Text>().faceColor = new Color32(0xFF, 0xFF, 0xFF, wordOpacity);
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
}
