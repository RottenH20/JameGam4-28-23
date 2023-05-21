using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Events;

[RequireComponent(typeof(Button))]
public class PointerDownUpHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    HammerController HammerController;
    public bool isLeft = true;

    public UnityEvent onPointerDown;
    public UnityEvent onPointerUp;

    // gets invoked every frame while pointer is down
    public UnityEvent whilePointerPressed;

    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
        HammerController = GameObject.Find("Character").GetComponent<HammerController>();
    }

    private IEnumerator WhilePressed()
    {
        // this looks strange but is okey in a Coroutine
        // as long as you yield somewhere
        while (true)
        {
            if (isLeft)
                HammerController.OnEnableLeft();
            else
                HammerController.OnEnableRight();

            whilePointerPressed?.Invoke();
            yield return null;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // ignore if button not interactable
        if (!_button.interactable) return;

        if (isLeft)
            HammerController.OnEnableLeft();
        else
            HammerController.OnEnableRight();

        // just to be sure kill all current routines
        // (although there should be none)
        StopAllCoroutines();
        StartCoroutine("WhilePressed");

        onPointerDown?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        HammerController.OnEnableNor();
        StopAllCoroutines();
        onPointerUp?.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HammerController.OnEnableNor();
        StopAllCoroutines();
        onPointerUp?.Invoke();
    }

    // Afaik needed so Pointer exit works .. doing nothing further
    public void OnPointerEnter(PointerEventData eventData) { }
}