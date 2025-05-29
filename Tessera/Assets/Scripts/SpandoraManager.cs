using UnityEngine;

public class SpandoraManager : MonoBehaviour
{

    public GameObject diePrefab;

    private void Start()
    {
        for (int r = -2; r <= 2; r++)
        {
            for (int c = -2; c <= 2; c++)
            {
                Instantiate(diePrefab, new Vector3(20 * c, 20 * r, 0), Quaternion.identity);
            }
        }
    }

    void Update()
    {
        
    }
}
