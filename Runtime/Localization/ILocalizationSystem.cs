using System;
using System.Collections.Generic;
using UnityEngine;

public interface ILocalizationSystem : ISaveState
{
    SystemLanguage Language { get; set; }

    IEnumerable<SystemLanguage> GetAvailableLanguages();

    event Action<ILocalizationSystem> LanguageChanged;
}