using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableAset/ScriptableLocalizationSystem", fileName = "ScriptableLocalizationSystem")]
public class ScriptableLocalizationSystem : ScriptableObject, ILocalizationSystem
{
    public string SaveIdentifier => nameof(ScriptableLocalizationSystem);

    [SerializeField]
    private LocalizedScriptableString[] _localizedScriptableStrings;

    [SerializeField]
    private SystemLanguage _language = SystemLanguage.Unknown;

    public event Action<ILocalizationSystem> LanguageChanged;

    public SystemLanguage[] AvailableLanguages;

    public ISaveSystem SaveSystem { get; set; }

    public static ScriptableLocalizationSystem Load()
    {
        return Resources.Load<ScriptableLocalizationSystem>(nameof(ScriptableLocalizationSystem));
    }

    public void LoadFromSaveSystem()
    {
        SaveSystem?.LoadState(this);
    }

    public SystemLanguage Language
    {
        get => _language;
        set
        {
            if (_language != value)
            {
                _language = value;
                UpdateCurrentTranslations();
                LanguageChanged?.Invoke(this);
                SaveSystem.SaveState(this);
            }
        }
    }

    private void OnEnable()
    {
        UpdateCurrentTranslations();
    }

    [Button]
    public void UpdateCurrentTranslations()
    {
        if (_localizedScriptableStrings == null)
            return;

        foreach (var localizedScriptableString in _localizedScriptableStrings)
        {
            if (localizedScriptableString == null)
                continue;
            localizedScriptableString.UpdateTranslation(this);
        }
    }

    public IEnumerable<SystemLanguage> GetAvailableLanguages()
    {
        return AvailableLanguages;
    }

#if UNITY_EDITOR
    [Button]
    private void FindAllTranslations()
    {
        var guids = UnityEditor.AssetDatabase.FindAssets($"t:{nameof(LocalizedScriptableString)}");
        _localizedScriptableStrings = new LocalizedScriptableString[guids.Length];

        for (int i = 0; i < guids.Length; i++)
        {
            string guid = guids[i];
            var assetpath = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
            _localizedScriptableStrings[i] = UnityEditor.AssetDatabase.LoadAssetAtPath<LocalizedScriptableString>(assetpath);
        }
    }

    [Button]
    private void UpdateUsedLanguages()
    {
        FindAllTranslations();

        var list = new HashSet<SystemLanguage>();
        foreach (var str in _localizedScriptableStrings)
            foreach (var item in str.Translations)
                list.Add(item.Language);

        Debug.Log(list.Count);

        AvailableLanguages = new SystemLanguage[list.Count];
        list.CopyTo(AvailableLanguages);
    }

#endif

    public string GetState()
    {
        return JsonConvert.SerializeObject(Language);
    }

    public void LoadState(string state)
    {
        var activeLanguage = JsonConvert.DeserializeObject<SystemLanguage>(state);
        Language = activeLanguage;
    }

    public void LoadDefaultState()
    {
        Language = SystemLanguage.Unknown;
    }
}
