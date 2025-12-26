using UnityEngine;

public class KeyboardAdjuster : MonoBehaviour
{
    public RectTransform panelToAdjust;
    private RectTransform canvasRect;
    public float originalPanelY;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Get the RectTransform of the Canvas
        var parentCanvas = GetComponentInParent<Canvas>();
        canvasRect = parentCanvas.GetComponent<RectTransform>();

        // Store the panel's original Y position
        originalPanelY = panelToAdjust.anchoredPosition.y;
    }

    // Update is called once per frame
    void Update()
    {
        bool keyboardVisible = TouchScreenKeyboard.visible;
        Rect keyboardArea = TouchScreenKeyboard.area;

        if (canvasRect == null || panelToAdjust == null)
            return;

        if (keyboardVisible == true)
        {
            var parentCanvas = canvasRect.GetComponent<Canvas>();
            Camera cam = parentCanvas != null && parentCanvas.renderMode == RenderMode.ScreenSpaceCamera ? parentCanvas.worldCamera : null;

            Vector2 screenPoint = new Vector2(0, keyboardArea.y + keyboardArea.height);
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPoint, cam, out localPoint);
            float keyboardHeightInCanvas = Mathf.Abs(localPoint.y);
            float empiricalDeviation = 58f;
            panelToAdjust.anchoredPosition = new Vector2(panelToAdjust.anchoredPosition.x, originalPanelY + keyboardHeightInCanvas - empiricalDeviation);
        }
        else
        {
            panelToAdjust.anchoredPosition = new Vector2(panelToAdjust.anchoredPosition.x, originalPanelY);
        }
    }
}
