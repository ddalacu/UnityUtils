using System;
using Framework.Utility;
using TMPro;
using UnityEngine;

class DisplayScriptableSpanValue : MonoBehaviour
{
    public TMP_Text DisplayingText;

    public ScriptableSpanValue ValueToDisplay;

    public bool DisplaySimple = true;

    [SerializeField]
    private string _textFormat= "{0}:{1}:{2}:{3}";

    private void OnEnable()
    {
        ValueToDisplay.ValueChanged += CurrencyValueChanged;
        CurrencyValueChanged(ValueToDisplay);
    }

    private void OnDisable()
    {
        ValueToDisplay.ValueChanged -= CurrencyValueChanged;
    }

    protected virtual void CurrencyValueChanged(IObservableValue<TimeSpan> observable)
    {
        var val = observable.Value;

        if (_textFormat != null)
            DisplayingText.text = string.Format(_textFormat, val.Days, val.Hours, val.Minutes, val.Seconds);
        else
        if (DisplaySimple)
            DisplayingText.text = val.ToString();
        else
            DisplayingText.text = val.ToReadableString();
    }
}