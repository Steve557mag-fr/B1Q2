using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UINavigation : MonoBehaviour
{
    public bool canUseNavigation = true;

    [SerializeField] GameObject cursor;
    [SerializeField] KeyCode selectUp = KeyCode.W,
                             selectDown = KeyCode.S,
                             navigationPrev = KeyCode.D,
                             navigationNext = KeyCode.A,
                             navigationEnter = KeyCode.Return;

    [SerializeField] NavigationMenu[] navigationMenus;

    int currentMenu = 0, currentSelect = 0;
    int oldSelect = 0;

    public void Update()
    {

        if (!canUseNavigation) return;

        int amountSel = System.Convert.ToInt16(Input.GetKeyDown(selectUp)) - System.Convert.ToInt16(Input.GetKeyDown(selectDown));
        int amountNav = System.Convert.ToInt16(Input.GetKeyDown(navigationNext)) - System.Convert.ToInt16(Input.GetKeyDown(navigationPrev));

        currentMenu = Mathf.Clamp(currentMenu + amountNav, 0, navigationMenus.Length-1);
        currentSelect = Mathf.Clamp(currentSelect + amountSel, 0, navigationMenus[currentMenu].buttons.Length-1);

        for(int i = 0; i < navigationMenus.Length; i++)
        {
            navigationMenus[i].gContainer.SetActive(i == currentMenu);
        }

        cursor.SetActive(navigationMenus[currentMenu].buttons.Length > 0);
        if (navigationMenus[currentMenu].buttons.Length == 0) return;
        
        Button selected = navigationMenus[currentMenu].buttons[currentSelect];
        if(oldSelect != currentMenu)
        {
            oldSelect = currentMenu;
            cursor.LeanMoveLocalY(selected.transform.position.y, 1);
        }
        if (Input.GetKeyDown(navigationEnter))
        {
            selected.onClick.Invoke();
        }

    }
    
}

[System.Serializable]
public struct NavigationMenu
{
    public GameObject gContainer;
    public Button[] buttons;
}
