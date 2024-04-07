using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class Menu : MonoBehaviour
{
    [Header("Navigation Settings")]
    public InteractionSet[] interactionSets;
    public int currentInteract = 0, currentSet = 0;
    public bool canNavigate = true;

    [Header("Menu UI Panel")]
    public GameObject menuPanel;

    public float menuPanelOffScreen;
    public float menuPanelOnScreen;

    public float menuPanelTransitionTime;
    public LeanTweenType menuPanelTransitionType;

    [Header("Cursor Settings")]
    public GameObject cursor;
    public float cursorTransitionTime;
    public LeanTweenType cursorTransitionType;

    void Update()
    {

        if (!canNavigate) return;

        int intrAmount = Player.GetAxisDown(KeyCode.S, KeyCode.W);
        currentInteract = Mathf.Clamp(currentInteract + intrAmount, 0, interactionSets[currentSet].interactions.Length-1);

        if (Input.GetKey(KeyCode.Return)) interactionSets[currentSet].interactions[currentInteract].interaction.Invoke();
        cursor.LeanMoveLocalY(interactionSets[currentSet].interactions[currentInteract].yPosition, cursorTransitionTime).setEase(cursorTransitionType);

    }

    public void SetSetInteractionIndex(int index)
    {
        currentSet = Mathf.Clamp(currentSet, 0, interactionSets.Length - 1);
    }

    public void ToggleMenuPanel(bool visible)
    {
        menuPanel.LeanMoveY(visible ? menuPanelOnScreen : menuPanelOffScreen, menuPanelTransitionTime).setEase(menuPanelTransitionType);
    }

}

[System.Serializable]
public struct InteractionSet
{
    public Interation[] interactions;
}

[System.Serializable]
public struct Interation
{
    public UnityEvent interaction;
    public float yPosition;
}
