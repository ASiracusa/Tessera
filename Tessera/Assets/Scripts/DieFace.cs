using UnityEngine;
using static Constants;

public class DieFace
{
    public Constants.DieColor letterColor;
    public string faceText;

    public DieFace(Constants.DieColor letterColor, string faceText)
    {
        this.letterColor = letterColor;
        this.faceText = faceText;
    }
}
