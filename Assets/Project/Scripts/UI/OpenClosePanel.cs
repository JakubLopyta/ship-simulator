using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;

public class OpenClosePanel : MonoBehaviour
{
    [Header("General Settings")]
    [SerializeField] private bool disableAnimation = false;
    [SerializeField] private bool closeOnStartup = true;

    [Header("Window Setup")]
    [SerializeField] private GameObject window;
    [SerializeField] private CanvasGroup windowCanvasGroup;
    [SerializeField] private Button windowButton;

    private Color translucentButtonColor = new Color32(0, 0, 0, 0);
    private Color selectedButtonColor = new Color32(78, 101, 192, 190);

    public enum AnimateToDirection
    {
        Top, Bottom, Left, Right
    }

    [Header("Animation Setup")]
    [SerializeField] private AnimateToDirection openDirection = AnimateToDirection.Top;
    [SerializeField] private AnimateToDirection closeDirection = AnimateToDirection.Bottom;
    [SerializeField] private Vector2 distanceToAnimate = new Vector2(100, 100);
    [SerializeField] private AnimationCurve easingCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [Range(0, 1f)][SerializeField] public float animationDuration = 0.2f;

    private bool isOpen;
    private Vector2 initialPosition;
    private Vector2 currentPosition;

    private Vector2 upOffset;
    private Vector2 downOffset;
    private Vector2 leftOffset;
    private Vector2 rightOffset;

    private Coroutine animateWindowCoroutine;

    private void OnValidate()
    {
        if (window != null)
        {
            windowCanvasGroup = window.GetComponent<CanvasGroup>();
        }

        distanceToAnimate.x = Mathf.Max(0, distanceToAnimate.x);
        distanceToAnimate.y = Mathf.Max(0, distanceToAnimate.y);
    }

    private void Start()
    {
        initialPosition = window.transform.position;
        InitializeOffsetPositions();

        if (closeOnStartup)
        {
            window.SetActive(false);
        }

        disableAnimation = PlayerPrefs.GetInt("animations", 0) == 0;
    }

    private void InitializeOffsetPositions()
    {
        upOffset = new Vector2(0, distanceToAnimate.y);
        downOffset = new Vector2(0, -distanceToAnimate.y);
        rightOffset = new Vector2(distanceToAnimate.x, 0);
        leftOffset = new Vector2(-distanceToAnimate.x, 0);
    }

    public void ToggleOpenClose()
    {
        if (isOpen) CloseWindow();
        else OpenWindow();
    }

    public void OpenWindow()
    {
        if (isOpen) return;
        isOpen = true;

        if (windowButton != null)
        {
            ColorBlock colorBlock = windowButton.colors;
            colorBlock.normalColor = isOpen ? selectedButtonColor : translucentButtonColor;
            windowButton.colors = colorBlock;
        }

        if (animateWindowCoroutine != null)
            StopCoroutine(animateWindowCoroutine);

        if (disableAnimation)
        {
            window.gameObject.SetActive(true);
            window.transform.position = initialPosition;
        }
        else
            animateWindowCoroutine = StartCoroutine(AnimateWindow(true));
    }

    public void CloseWindow()
    {
        if (!isOpen) return;
        isOpen = false;

        if (windowButton != null)
        {
            ColorBlock colorBlock = windowButton.colors;
            colorBlock.normalColor = isOpen ? selectedButtonColor : translucentButtonColor;
            windowButton.colors = colorBlock;
        }

        if (animateWindowCoroutine != null)
            StopCoroutine(animateWindowCoroutine);

        if (disableAnimation)
        {
            window.transform.position = initialPosition;
            window.gameObject.SetActive(false);
        }
        else
            animateWindowCoroutine = StartCoroutine(AnimateWindow(false));
    }

    private Vector2 GetOffset(AnimateToDirection direction)
    {
        switch (direction)
        {
            case AnimateToDirection.Top:    return upOffset;
            case AnimateToDirection.Bottom: return downOffset;
            case AnimateToDirection.Left:   return leftOffset;
            case AnimateToDirection.Right:  return rightOffset;
            default:                        return Vector2.zero;
        }
    }

    private IEnumerator AnimateWindow(bool open)
    {
        if (open)
        {
            window.gameObject.SetActive(true);
            window.transform.position = initialPosition - GetOffset(openDirection);
            currentPosition = window.transform.position;
        }
        else
        {
            window.transform.position = initialPosition;
            currentPosition = initialPosition;
        }

        float elapsedTime = 0f;

        Vector2 targetPosition = open
            ? initialPosition
            : initialPosition + GetOffset(closeDirection);

        while (elapsedTime < animationDuration)
        {
            float t = easingCurve.Evaluate(elapsedTime / animationDuration);

            window.transform.position = Vector2.Lerp(currentPosition, targetPosition, t);

            windowCanvasGroup.alpha = open
                ? Mathf.Lerp(0, 1, t)
                : Mathf.Lerp(1, 0, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        window.transform.position = targetPosition;
        windowCanvasGroup.alpha = open ? 1 : 0;
        windowCanvasGroup.interactable = open;
        windowCanvasGroup.blocksRaycasts = open;

        if (!open)
        {
            window.transform.position = initialPosition;
            window.gameObject.SetActive(false);
        }
    }
}
