using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MultiTouchScrollRect : ScrollRect
{
    private readonly List<(int, Vector2)> _pointers = new List<(int, Vector2)>();

    private Vector2 _initialPos;

    private RaycastResult _raycast;

    private bool IsRemote
    {
        get
        {
#if UNITY_EDITOR
            return UnityEditor.EditorApplication.isRemoteConnected;
#else
            return false;
#endif
        }
    }


    public override void OnBeginDrag(PointerEventData eventData)
    {
        if (SystemInfo.deviceType == DeviceType.Handheld || IsRemote)
        {
            _pointers.Add((eventData.pointerId, Vector2.zero));

            if (_pointers.Count == 1)
            {
                _initialPos = eventData.position;
                base.OnBeginDrag(eventData);
            }

            Debug.Log("Begin!");
        }
        else if (SystemInfo.deviceType == DeviceType.Desktop)
        {
            base.OnBeginDrag(eventData);
        }
    }


    private void Update()
    {
        if (_pointers.Count > 0)
        {
            var totalDelta = new Vector2(0, 0);
            var pointersCount = _pointers.Count;
            for (var index = 0; index < pointersCount; index++)
            {
                var pointer = _pointers[index];
                totalDelta += pointer.Item2;

                _pointers[index] = (pointer.Item1, Vector2.zero);
            }

            totalDelta /= pointersCount;

            _initialPos += totalDelta;

            base.OnDrag(new PointerEventData(EventSystem.current)
            {
                position = _initialPos,
                pointerPressRaycast = _raycast
            });
        }
    }

    public override void OnDrag(PointerEventData eventData)
    {
        if (SystemInfo.deviceType == DeviceType.Handheld || IsRemote)
        {
            _raycast = eventData.pointerCurrentRaycast;
            for (int i = 0; i < _pointers.Count; i++)
            {
                if (_pointers[i].Item1 == eventData.pointerId)
                {
                    _pointers[i] = (eventData.pointerId, eventData.delta);
                    break;
                }
            }
        }
        else if (SystemInfo.deviceType == DeviceType.Desktop)
        {
            base.OnDrag(eventData);
        }
    }


    public override void OnEndDrag(PointerEventData eventData)
    {
        if (SystemInfo.deviceType == DeviceType.Handheld || IsRemote)
        {
            for (int i = 0; i < _pointers.Count; i++)
            {
                if (_pointers[i].Item1 == eventData.pointerId)
                {
                    _pointers.RemoveAt(i);
                    break;
                }
            }

            if (_pointers.Count == 0)
            {
                base.OnEndDrag(eventData);
            }
        }
        else if (SystemInfo.deviceType == DeviceType.Desktop)
        {
            base.OnEndDrag(eventData);
        }
    }
}