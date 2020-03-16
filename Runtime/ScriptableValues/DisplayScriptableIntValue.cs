using Framework.Utility;
using TMPro;
using UnityEngine;

public class DisplayScriptableIntValue : MonoBehaviour
{
    public TMP_Text DisplayingText;

    public ScriptableIntValue ValueToDisplay;

    private void OnEnable()
    {
        ValueToDisplay.ValueChanged += CurrencyValueChanged;
        CurrencyValueChanged(ValueToDisplay);
    }

    private void OnDisable()
    {
        ValueToDisplay.ValueChanged -= CurrencyValueChanged;
    }

    private void CurrencyValueChanged(IObservableValue<int> observable)
    {
        DisplayingText.text = observable.Value.ToString();
    }
}