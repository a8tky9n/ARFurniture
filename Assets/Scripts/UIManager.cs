using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum UIState 
{
    none,
    memu,
    furniture
}
public class UIManager : MonoBehaviour {
    [SerializeField]
    GameObject u_MenuButton;
    [SerializeField]
    GameObject u_FurnitureCategory;
    [SerializeField]
    GameObject u_Furniture;

    private UIState uState;
	void Update () {
		
	}
    public void onMenuButtonPressed()
    {
        uState = UIState.memu;
    } 
    public void onGategoryPressed()
    {
        uState = UIState.furniture;
    }
    public void onFurniturePressed()
    {
        uState = UIState.none;
    }
}
