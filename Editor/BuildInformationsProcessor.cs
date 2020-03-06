using System;
using System.Globalization;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Framework.Utility
{
    /// <summary>
    /// Will fill certain datas in the <see cref="BuildInformations"/>
    /// </summary>
    public class BuildInformationsProcessor : IPreprocessBuildWithReport
    {
        public int callbackOrder => int.MinValue;

        public void OnPreprocessBuild(BuildReport report)
        {
            var buildInformations = BuildInformations.LoadDefault();
            buildInformations.BuildTimeAsString = DateTime.Now.ToString(CultureInfo.InvariantCulture);


            EditorUtility.SetDirty(buildInformations);
            Debug.Log("BuildData:\n" + EditorJsonUtility.ToJson(buildInformations, true));
        }
    }
}