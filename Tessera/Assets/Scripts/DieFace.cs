using UnityEngine;
using static Constants;

public class DieFace
{
    public Constants.DieColor letterColor;
    public string faceText;
    public int faceValue;

    public DieFace(Constants.DieColor letterColor, string faceText, int faceValue)
    {
        this.letterColor = letterColor;
        this.faceText = faceText;
        this.faceValue = faceValue;
    }
}
