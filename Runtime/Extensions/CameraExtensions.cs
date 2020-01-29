using System.Reflection;
using UnityEngine;

public static class CameraExtensions
{
    private static FieldInfo _field;

    static CameraExtensions()
    {
        _field = typeof(Canvas).GetField("willRenderCanvases", BindingFlags.NonPublic | BindingFlags.Static);
    }

    public static void RenderWithoutUIUpdate(this Camera camera)
    {
        var canvasHackObject = _field.GetValue(null);
        _field.SetValue(null, null);
        camera.Render();
        _field.SetValue(null, canvasHackObject);
    }
}