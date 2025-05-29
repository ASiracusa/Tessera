using UnityEngine;
using TMPro;
using static Constants;

public class SpandoraManager : MonoBehaviour
{

    public GameObject diePrefab;

    private void Start()
    {
        for (int r = -2; r <= 2; r++)
        {
            for (int c = -2; c <= 2; c++)
            {
                int letterInd = Random.Range(0,26);
                char letter = (char) (65 + letterInd);
                GameObject letterDie = Instantiate(diePrefab, new Vector3(20 * c, 20 * r, 0), Quaternion.identity);
                GameObject textCenter = letterDie.transform.Find("DieCanvas/TextCenter").gameObject;
                textCenter.GetComponent<TMP_Text>().text = letter.ToString();
                GameObject textBottom = letterDie.transform.Find("DieCanvas/TextBottom").gameObject;
                textBottom.GetComponent<TMP_Text>().text = Constants.TILE_VALUES[letter].ToString();
            }
        }
    }

    void Update()
    {
        
    }
}
