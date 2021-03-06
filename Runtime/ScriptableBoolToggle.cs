﻿using Framework.Utility;
using UnityEngine;
using UnityEngine.UI;

public class ScriptableBoolToggle : MonoBehaviour
{
    [SerializeField]
    private ScriptableBoolValue _scriptableBool = default;

    [SerializeField]
    private Toggle _toggle = default;

    private void Start()
    {
        _toggle.isOn = _scriptableBool.Value;
        _toggle.onValueChanged.AddListener(Toggled);
        _scriptableBool.ValueChanged += ValChanged;
    }

    private void OnDestroy()
    {
        if (_toggle)
            _toggle.onValueChanged.RemoveListener(Toggled);
        _scriptableBool.ValueChanged -= ValChanged;
    }

    private void ValChanged(IObservableValue<bool> observable)
    {
        _toggle.onValueChanged.RemoveListener(Toggled);
        _toggle.isOn = observable.Value;
        _toggle.onValueChanged.AddListener(Toggled);
    }

    private void Toggled(bool value)
    {
        _scriptableBool.ValueChanged -= ValChanged;
        _scriptableBool.Value = value;
        _scriptableBool.ValueChanged += ValChanged;
    }
}