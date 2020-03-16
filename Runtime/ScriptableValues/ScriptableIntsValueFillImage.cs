using Framework.Utility;
using UnityEngine;
using UnityEngine.UI;

class ScriptableIntsValueFillImage : MonoBehaviour
{
    public Image Image;

    public ScriptableIntValue A;

    public ScriptableIntValue B;

    private void OnEnable()
    {
        A.ValueChanged += CurrencyValueChanged;
        B.ValueChanged += CurrencyValueChanged;

        CurrencyValueChanged(A);
    }

    private void OnDisable()
    {
        A.ValueChanged -= CurrencyValueChanged;
        B.ValueChanged -= CurrencyValueChanged;
    }

    private void CurrencyValueChanged(IObservableValue<int> observable)
    {
        if (B.Value != 0)
            Image.fillAmount = A.Value / (float) B.Value;
        else
            Image.fillAmount = 1;
    }
}