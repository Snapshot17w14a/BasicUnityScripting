using UnityEngine;

public class FillBar : MonoBehaviour
{
    private float width;
    private float height;
    private RectTransform fillObjectRectTransform;
    private enum Orientation { Horizontal, Vertical }

    [SerializeField] private Orientation orientation = Orientation.Horizontal;

    private void Awake()
    {
        width = GetComponent<RectTransform>().sizeDelta.x;
        height = GetComponent<RectTransform>().sizeDelta.y;
        fillObjectRectTransform = transform.Find("Bar Fill").GetComponent<RectTransform>();
    }

    public void SetFill(float fill)
    {
        fill = Mathf.Clamp(fill, 0f, 1f);
        if (orientation == Orientation.Horizontal) fillObjectRectTransform.sizeDelta = new Vector2(width * fill, fillObjectRectTransform.sizeDelta.y);
        else fillObjectRectTransform.sizeDelta = new Vector2(fillObjectRectTransform.sizeDelta.x, height * fill);
    }
}