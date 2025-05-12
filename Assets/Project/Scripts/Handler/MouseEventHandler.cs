using System;
using UnityEngine;

public class MouseEventHandler : MonoBehaviour
{
    public event Action onMouseDown = null;
    public event Action onMouseUp = null;
    
    private void OnMouseDown()
    {
        onMouseDown?.Invoke();
    }

    private void OnMouseUp()
    {
        onMouseUp?.Invoke();
    }

    public void Reset()
    {
        onMouseDown = null;
        onMouseUp = null;
    }
}
