using System;
using UnityEngine;

public class MouseEventHandler : MonoBehaviour
{
    // 마우스 다운 이벤트
    public event Action onMouseDown = null;
    // 마우스 업 이벤트
    public event Action onMouseUp = null;
    
    // 이벤트 발생
    private void OnMouseDown()
    {
        onMouseDown?.Invoke();
    }
    
    // 이벤트 발생
    private void OnMouseUp()
    {
        onMouseUp?.Invoke();
    }

    // 초기화
    public void Reset()
    {
        onMouseDown = null;
        onMouseUp = null;
    }
}
