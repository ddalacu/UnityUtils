using System;
using Framework.Utility;
using UnityEngine;

///<summary>
/// Author Paul Diac
/// Version 0.0a
///</summary>
[Serializable, CreateAssetMenu(menuName = "ScriptableValue/LocalizedString", fileName = "LocalizedString")]
public class LocalizedScriptableString : ScriptableStringValue
{
    public ValueEntry[] Translations;

    [Serializable]
    public class ValueEntry
    {
        public SystemLanguage Language = SystemLanguage.English;
        public string ValueToSet;
    }

    public bool GetTranslation(SystemLanguage language, out string value)
    {
        var translationsLength = Translations.Length;
        for (int i = 0; i < translationsLength; i++)
        {
            var valueEntry = Translations[i];
            if (valueEntry.Language == language)
            {
                value = valueEntry.ValueToSet;
                return true;
            }
        }

        value = default;
        return false;
    }

    public void UpdateTranslation(ILocalizationSystem localizationSystem)
    {
        if (GetTranslation(localizationSystem.Language, out var language))
        {
            Value = language;
        }
        else
        {
            Value = DefaultValue;
        }
    }

}
