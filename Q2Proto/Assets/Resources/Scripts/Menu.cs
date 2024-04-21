using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

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

        if (currentInteract >= interactionSets[currentSet].interactions.Length) return;
        if (Input.GetKeyDown(KeyCode.Return)) interactionSets[currentSet].interactions[currentInteract].interaction.Invoke();
        try
        {
            cursor.LeanMoveLocalY(interactionSets[currentSet].interactions[currentInteract].yPosition, cursorTransitionTime).setEase(cursorTransitionType);
        }
        catch (Exception ex) { }

    }

    public void SetSetInteractionIndex(int index)
    {
        currentSet = Mathf.Clamp(index, 0, interactionSets.Length - 1);
    }

    public void ToggleMenuPanel(bool visible, System.Action callback)
    {
        menuPanel.LeanMoveY(visible ? menuPanelOnScreen : menuPanelOffScreen, menuPanelTransitionTime).setEase(menuPanelTransitionType).setOnComplete(callback);
    }

    public void PlayClick()
    {
        ToggleMenuPanel(false, () => { GameManager.instance.StartSession(); });
    }
    
    public void QuitApp()
    {
        Application.Quit();
    }

    public static Menu instance
    {
        get { return FindAnyObjectByType<Menu>(); }
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
