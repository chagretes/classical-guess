using TMPro;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(RectTransform)), RequireComponent(typeof(TextMeshProUGUI))]
public class TextExtension : MonoBehaviour
{
    [SerializeField]
    public RectTransform RectTransform { get; private set; }
    [SerializeField]
    public TextMeshProUGUI Text { get; private set; }

    void Awake()
    {
        RectTransform = GetComponent<RectTransform>();
        Text = GetComponent<TextMeshProUGUI>();
    }

    public void SetText(string text)
    {
        Text.text = text;
    }
}
