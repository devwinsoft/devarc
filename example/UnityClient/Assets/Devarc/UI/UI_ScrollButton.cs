using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(EventTrigger))]
public class UI_ScrollButton : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    Vector3 mInitScale = Vector3.one;
    EventTrigger mTrigger = null;
    RectTransform mRectTransform = null;
    ScrollRect mScroll = null;

    private void Awake()
    {
        mTrigger = GetComponent<EventTrigger>();
        mRectTransform = GetComponent<RectTransform>();
        mInitScale = mRectTransform.localScale;

        var entry_down = new EventTrigger.Entry();
        entry_down.eventID = EventTriggerType.PointerDown;
        entry_down.callback.AddListener((evt) =>
        {
            mRectTransform.localScale = 0.875f * mInitScale;
        });
        mTrigger.triggers.Add(entry_down);

        var entry_up = new EventTrigger.Entry();
        entry_up.eventID = EventTriggerType.PointerUp;
        entry_up.callback.AddListener((evt) =>
        {
            mRectTransform.localScale = mInitScale;
        });
        mTrigger.triggers.Add(entry_up);

        mScroll =  GetComponentInParent<ScrollRect>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        mScroll?.OnBeginDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        mScroll?.OnDrag(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        mScroll?.OnEndDrag(eventData);
    }
}
