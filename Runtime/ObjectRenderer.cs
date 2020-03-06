using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A newer better version of <see cref="CharacterToUiRenderer"/>
/// </summary>
public class ObjectRenderer
{
    private static readonly List<Camera> RenderCameras = new List<Camera>();

    private static readonly Vector3 IndexOffset = new Vector3(0f, -15f, 0f);

    private GameObject _target;

    private Vector3 _originalPosition;

    private Quaternion _originalRotation;

    public bool IsValid => RenderingCamera != null;

    public RenderTexture Texture { get; private set; }

    public Camera RenderingCamera { get; private set; }

    public void ResetTransform()
    {
        if (_target == null) return;
        _target.transform.position = _originalPosition;
        _target.transform.rotation = _originalRotation;
    }

    public void StopRendering()
    {
        if (IsValid == false)
            return;

        ResetTransform();
        var texture = RenderingCamera.targetTexture;
        RenderingCamera.targetTexture = null;
        Object.DestroyImmediate(texture);

        RenderingCamera.enabled = false;
        RenderingCamera = null;
    }

    public void PauseRendering()
    {
        if (IsValid == false)
            return;
        RenderingCamera.enabled = false;
    }

    public void ResumeRendering()
    {
        if (IsValid == false)
            return;
        RenderingCamera.enabled = true;
    }

    public class RenderSettings
    {
        public Vector3 ToCameraOffset = new Vector3(0f, -1.2f, 1.2f);
        public int TextureWidth = 300;
        public int TextureHeight = 480;
        public int TextureDepth = 16;
    }

    public static ObjectRenderer StartRendering(GameObject characterInstance, RenderSettings renderSettings)
    {
        var camera = GetPooledCamera(out var cameraIndex);
        var cameraTransform = camera.transform;

        cameraTransform.position = (new Vector3(0, -100, 0) + IndexOffset * cameraIndex);

        var targetData = new ObjectRenderer
        {
            _target = characterInstance,
            RenderingCamera = camera,
            _originalPosition = characterInstance.transform.position,
            _originalRotation = characterInstance.transform.rotation
        };

        var renderTexture = new RenderTexture(renderSettings.TextureWidth, renderSettings.TextureHeight, renderSettings.TextureDepth, RenderTextureFormat.ARGB32)
        {
            useMipMap = false,
            filterMode = FilterMode.Trilinear,
            name = "ObjectRenderer"
        };

        characterInstance.SetActive(true);

        characterInstance.transform.position = cameraTransform.TransformPoint(renderSettings.ToCameraOffset);
        Quaternion look = Quaternion.LookRotation(cameraTransform.position - characterInstance.transform.position);
        characterInstance.transform.rotation = Quaternion.Euler(0, look.eulerAngles.y, 0);

        camera.enabled = true;
        camera.targetTexture = renderTexture;
        camera.RenderWithoutUIUpdate();
        targetData.Texture = renderTexture;

        return targetData;
    }

    public static RenderTexture Render(GameObject characterInstance, RenderSettings renderSettings)
    {
        var camera = GetPooledCamera(out var cameraIndex);
        var cameraTransform = camera.transform;

        cameraTransform.position = (new Vector3(0, -100, 0) + IndexOffset * cameraIndex);

        var originalPosition = characterInstance.transform.position;
        var originalRotation = characterInstance.transform.rotation;

        var renderTexture = new RenderTexture(renderSettings.TextureWidth, renderSettings.TextureHeight, renderSettings.TextureDepth, RenderTextureFormat.ARGB32)
        {
            useMipMap = false,
            filterMode = FilterMode.Trilinear,
            name = "ObjectRenderer"
        };

        characterInstance.SetActive(true);

        characterInstance.transform.position = cameraTransform.TransformPoint(renderSettings.ToCameraOffset);
        Quaternion look = Quaternion.LookRotation(cameraTransform.position - characterInstance.transform.position);
        characterInstance.transform.rotation = Quaternion.Euler(0, look.eulerAngles.y, 0);

        camera.enabled = true;
        camera.targetTexture = renderTexture;

        camera.RenderWithoutUIUpdate();

        camera.targetTexture = null;
        camera.enabled = false;

        characterInstance.transform.position = originalPosition;
        characterInstance.transform.rotation = originalRotation;

        return renderTexture;
    }


    /// <summary>
    /// Creates a game object with a camera used for rendering characters
    /// </summary>
    /// <returns></returns>
    public static Camera CreateNewCamera()
    {
        GameObject renderCameraGameObject = new GameObject("RenderCamera")
        {
            //hideFlags = HideFlags.NotEditable
        };
        Object.DontDestroyOnLoad(renderCameraGameObject);

        Camera setupCamera = renderCameraGameObject.AddComponent<Camera>();
        setupCamera.clearFlags = CameraClearFlags.Color;
        setupCamera.backgroundColor = Color.clear;
        setupCamera.farClipPlane = 10;
        setupCamera.enabled = false;
        return setupCamera;
    }

    private static Camera GetPooledCamera(out int index)
    {
        for (int i = RenderCameras.Count - 1; i >= 0; --i)
        {
            var cam = RenderCameras[i];
            if (cam == null)
            {
                RenderCameras.RemoveAt(i);
                continue;
            }

            if (cam.targetTexture == null)
            {
                index = i;
                return cam;
            }
        }

        var setupRenderToTextureCamera = CreateNewCamera();

        index = RenderCameras.Count;

        RenderCameras.Add(setupRenderToTextureCamera);

        return setupRenderToTextureCamera;
    }
}