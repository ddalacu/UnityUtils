using UnityEngine;

public class PlayerPrefsSaveManager : ISaveSystem
{
#if UNITY_EDITOR
    [UnityEditor.MenuItem("Tools/DeletePlayerPrefs")]
    public static void DeleteSave()
    {
        PlayerPrefs.DeleteAll();
    }
#endif

    public void SaveState(ISaveState obj)
    {
        var id = obj.SaveIdentifier;
        var state = obj.GetState();

        PlayerPrefs.SetString(id, state);
    }

    public void LoadState(ISaveState obj, out bool isDefault)
    {
        var id = obj.SaveIdentifier;
        if (PlayerPrefs.HasKey(id))
        {
            obj.LoadState(PlayerPrefs.GetString(id));
            isDefault = false;
            return;
        }

        obj.LoadDefaultState();
        isDefault = true;
    }

    public void LoadState(ISaveState obj)
    {
        LoadState(obj, out _);
    }
}