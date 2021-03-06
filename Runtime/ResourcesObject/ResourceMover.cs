﻿#if UNITY_EDITOR

using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

/// <summary>
/// Used to move assets in resources folder during build
/// Author Paul Diac
/// </summary>
public class ResourceMover : IPreprocessBuildWithReport, IPostprocessBuildWithReport
{
    public int callbackOrder => int.MinValue;

    public static List<FileInfo> FileInfos = new List<FileInfo>();

    public class FileInfo
    {
        public string OriginalFile;
        public string MovedFile;
    }

    public static string MoveToResources(string assetPath, string guid)
    {
        //check if already in resources folder
        if (assetPath.StartsWith("Assets/Resources"))
        {
            var fileName = Path.GetFileNameWithoutExtension(Path.GetFileName(assetPath));

            if (fileName != guid)
            {
                Debug.LogError($"Asset {assetPath} is already in resources but it's referenced by a {nameof(ResourcesObject)} field, please move it out of Resources folder!");
            }

            return fileName;
        }

        CheckResourcesDirectory();

        string extension = Path.GetExtension(assetPath);

        var toMove = Path.Combine(Path.Combine("Assets", "Resources"), $"{guid}{extension}");

        var moveResult = AssetDatabase.MoveAsset(assetPath, toMove);


        if (string.IsNullOrEmpty(moveResult) == false)
        {
            Debug.Log(File.Exists(assetPath));
            Debug.LogError($"Failed to move asset {assetPath} Reason: {moveResult}");
            return string.Empty;
        }

        FileInfos.Add(new FileInfo
        {
            OriginalFile = assetPath,
            MovedFile = toMove
        });

        return Path.GetFileNameWithoutExtension(Path.GetFileName(toMove));
    }

    private static void CheckResourcesDirectory()
    {
        if (Directory.Exists("Assets/Resources"))
            Directory.CreateDirectory("Assets/Resources");
    }

    public void OnPostprocessBuild(BuildReport report)
    {
        Update();
    }

    private static List<string> GetAssetsGuids()
    {
        var paths = new List<string>();

        var guids = AssetDatabase.FindAssets($"t:{nameof(ScriptableObject)}");

        var checkedGuids = new HashSet<string>();

        var guidsLength = guids.Length;
        for (var index = 0; index < guidsLength; index++)
        {
            var guid = guids[index];

            if (checkedGuids.Add(guid) == false)
                continue;

            var result = AssetDatabase.GUIDToAssetPath(guid);
            if (string.IsNullOrEmpty(result))
            {
                Debug.LogError($"Could not get asset path {guid}");
                continue;
            }

            foreach (var o in AssetDatabase.LoadAllAssetsAtPath(result))
            {
                if (o is IHaveObjectsForResources haveObjectsForResources)
                {
                    paths.AddRange(haveObjectsForResources.GetResourcesObjectsGuids());
                }
            }
        }

        return paths;
    }

    public void OnPreprocessBuild(BuildReport report)
    {
        ResourcesObject.EditorIsBuilding = true;
        EditorApplication.update += Update;
        var guids = GetAssetsGuids();

        guids = guids.Distinct().ToList();

        var count = guids.Count;

        if (count == 0)
            return;
        Debug.Log($"Moving {count} assets to resources!");
        try
        {
            AssetDatabase.StartAssetEditing();
            int percent = count / 10;

            if (percent == 0)
                percent++;

            for (var index = 0; index < count; index++)
            {
                var guid = guids[index];
                var result = AssetDatabase.GUIDToAssetPath(guid);
                if (string.IsNullOrEmpty(result))
                {
                    Debug.LogError($"Could not get asset path {guid}");
                    continue;
                }

                if (index % percent == 0)
                    EditorUtility.DisplayProgressBar($"Moving files with tag:{ResourcesObject.MoveInResourcesTag} to resources", result, (float)(index + 1) / count);
                MoveToResources(result, guid);
            }
        }
        finally
        {
            EditorUtility.ClearProgressBar();
            AssetDatabase.StopAssetEditing();
            AssetDatabase.SaveAssets();
        }

    }

    private void Update()
    {
        ResourcesObject.EditorIsBuilding = false;
        EditorApplication.update -= Update;
        RestoreMovedAssets();
    }

    public static void RestoreMovedAssets()
    {
        if (FileInfos == null)
            return;

        int count = FileInfos.Count;
        int percent = count / 10;
        if (percent == 0)
            percent++;

        try
        {
            AssetDatabase.StartAssetEditing();

            for (int index = 0; index < count; index++)
            {
                var fileInfo = FileInfos[index];
                if (index % percent == 0)
                    EditorUtility.DisplayProgressBar($"Moving files with tag:{ResourcesObject.MoveInResourcesTag} from resources", fileInfo.OriginalFile, (float)(index + 1) / count);
                AssetDatabase.MoveAsset(fileInfo.MovedFile, fileInfo.OriginalFile);
            }

            Debug.Log($"Total of {count} moved assets.");
            FileInfos.Clear();
        }
        finally
        {
            EditorUtility.ClearProgressBar();
            AssetDatabase.StopAssetEditing();
            AssetDatabase.SaveAssets();
        }
    }
}

#endif