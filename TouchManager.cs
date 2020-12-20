using UnityEngine;
using System.Collections;
 
public class TouchManager : MonoBehaviour
{
   #region singaltonStuff
    private static TouchManager _instance;
    public static TouchManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = GameObject.FindObjectOfType<TouchManager>();
            return _instance;
        }
    }
    #endregion
 
    public delegate void TouchDelegate(Touch eventData);
    public static event TouchDelegate OnTouchDown;
    public static event TouchDelegate OnTouchUp;
    public static event TouchDelegate OnTouchDrag;
#if UNITY_EDITOR
    public delegate void TouchDelegateEditor(TouchPhase eventData);
    public static event TouchDelegateEditor OnTouchDownEditor;
    public static event TouchDelegateEditor OnTouchUpEditor;
    public static event TouchDelegateEditor OnTouchDragEditor;
#endif
    
    private void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                if (OnTouchDown != null)
                    OnTouchDown(touch);
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                if (OnTouchUp != null)
                    OnTouchUp(touch);
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                if (OnTouchDrag != null)
                    OnTouchDrag(touch);
            }
        }

#if UNITY_EDITOR
        // Simulate touch events from mouse events
        if (true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (OnTouchDownEditor != null)
                    OnTouchDownEditor(TouchPhase.Began);
                //HandleTouch(10, Camera.main.ScreenToWorldPoint(Input.mousePosition), TouchPhase.Began);
            }
            if (Input.GetMouseButton(0))
            {
                if (OnTouchDragEditor != null)
                    OnTouchDragEditor(TouchPhase.Moved);
                //HandleTouch(10, Camera.main.ScreenToWorldPoint(Input.mousePosition), TouchPhase.Moved);
            }
            if (Input.GetMouseButtonUp(0))
            {
                if (OnTouchUpEditor != null)
                    OnTouchUpEditor(TouchPhase.Ended);
                //HandleTouch(10, Camera.main.ScreenToWorldPoint(Input.mousePosition), TouchPhase.Ended);
            }
        }
#endif
    }
}