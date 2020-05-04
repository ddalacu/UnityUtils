using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[DefaultExecutionOrder(99)]
public class UIZoom : MonoBehaviour, IScrollHandler, IPointerDownHandler, IPointerUpHandler
{

    [SerializeField]
    private float _zoomSpeed = 0.1f;

    [SerializeField]
    private float _maxZoom = 10f;

    [SerializeField]
    private float _minZoom = 0.1f;

    [SerializeField]
    private ScrollRect _scrollRect = default;

    [SerializeField]
    private RectTransform _rectTransform = default;

    private readonly List<int> _pointers = new List<int>();

    private float? _lastDistance;

    public event Action ScaleChanged; 

    private Touch GetTouchFromId(int pointerId)
    {
        for (int i = 0; i < Input.touchCount; i++)
        {
            var touch = Input.GetTouch(i);
            if (touch.fingerId == pointerId)
                return touch;
        }

        return default;
    }

    private Camera _camera;


    private void Awake()
    {
        var comps = GetComponentsInParent<Canvas>();
        if (comps.Length > 0)
        {
            var canvas = comps[0];
            Debug.Assert(canvas.isRootCanvas);
            if (canvas.renderMode == RenderMode.ScreenSpaceCamera || canvas.renderMode == RenderMode.WorldSpace)
                _camera = canvas.worldCamera;
        }
    }

    private void Update()
    {
        if (_pointers.Count < 2)
        {
            _lastDistance = null;
            return;
        }

        var firstTouch = GetTouchFromId(_pointers[0]);
        var secondTouch = GetTouchFromId(_pointers[1]);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(_rectTransform, firstTouch.position, _camera, out var firstPoint);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_rectTransform, secondTouch.position, _camera, out var secondPoint);

        var dist = Vector2.Distance(firstPoint, secondPoint);

        if (_lastDistance.HasValue)
        {
            var delta = dist - _lastDistance.Value;

            if (Mathf.Abs(delta) > 0.001f)
            {
                var percent = delta / _lastDistance.Value;
                HandeZoom(percent * _rectTransform.localScale.x, (firstPoint + secondPoint) / 2);

                RectTransformUtility.ScreenPointToLocalPointInRectangle(_rectTransform, firstTouch.position, _camera, out firstPoint);
                RectTransformUtility.ScreenPointToLocalPointInRectangle(_rectTransform, secondTouch.position, _camera, out secondPoint);
                dist = Vector2.Distance(firstPoint, secondPoint);
                _lastDistance = dist;
            }
        }
        else
        {
            _lastDistance = dist;
        }




    }

    public void OnScroll(PointerEventData eventData)
    {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_rectTransform, eventData.position, eventData.enterEventCamera,
            out var firstPoint))
        {
            var delta = (eventData.scrollDelta.y * _zoomSpeed);
            HandeZoom(delta, firstPoint);
        }
    }

    public bool HandeZoom(float delta, Vector2 screenPoint)
    {
        var desiredScale = Mathf.Clamp(_rectTransform.localScale.x + delta, _minZoom, _maxZoom);

        delta = desiredScale - _rectTransform.localScale.x;

        if (Mathf.Abs(delta) < 0.001f)
            return false;

        _rectTransform.localScale = new Vector3(desiredScale, desiredScale, desiredScale);


        var rect = _rectTransform.rect;
        var percent = new Vector2(Mathf.InverseLerp(rect.xMin, rect.xMax, screenPoint.x), Mathf.InverseLerp(rect.yMin, rect.yMax, screenPoint.y));

        var shiftX = -delta * rect.width * (percent.x - 0.5f);
        var shiftY = -delta * rect.height * (percent.y - 0.5f);
        var currPos = _scrollRect.content.localPosition;

        _rectTransform.localPosition = new Vector3(currPos.x + shiftX, currPos.y + shiftY, currPos.z);

        ScaleChanged?.Invoke();

        return true;
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        _pointers.Add(eventData.pointerId);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _pointers.Remove(eventData.pointerId);
    }
}