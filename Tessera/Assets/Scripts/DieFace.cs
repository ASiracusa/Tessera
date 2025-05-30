using UnityEngine;
using static Constants;

public class DieFace
{
    public Constants.DieColor faceColor;
    public string faceText;
    public int faceValue;

    public DieFace(Constants.DieColor faceColor, string faceText, int faceValue)
    {
        this.faceColor = faceColor;
        this.faceText = faceText;
        this.faceValue = faceValue;
    }
}
