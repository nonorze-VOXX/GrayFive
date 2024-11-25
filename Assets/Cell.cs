using TMPro;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public static readonly Color bgColor = new(0.5f, 0.5f, 0.5f, 0.5f);
    private TMP_Text text;

    private void Awake()
    {
        text = GetComponentInChildren<TMP_Text>();
        GetComponentInChildren<SpriteRenderer>().color = bgColor;
    }

    public void SetNumber(int number)
    {
        text.text = number.ToString();
    }

    public void SetTextColor(Color color)
    {
        text.color = color;
    }

    public void SetView((int, Color) valueTuple)
    {
        SetNumber(valueTuple.Item1);
        SetTextColor(valueTuple.Item2);
    }
}