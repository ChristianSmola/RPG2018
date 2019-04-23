using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Script : MonoBehaviour {
    private bool ShowDialog;
    private bool ShowInventory;
    public Canvas DialogCanvas;
    public Canvas InventoryCanvas;
    private Button DialogNext;
    // Use this for initialization
    void Start () {
       
	}
	
	// Update is called once per frame
	void Update () {
        //Menu Inputs
        if (Input.GetButtonDown("Talk"))
        {
            ShowDialog = !ShowDialog;
        }
        if (Input.GetButtonDown("Inventory"))
        {
            ShowInventory = !ShowInventory;
        }

        //Display Menus
        if (ShowDialog == true)
        {
            DialogCanvas.enabled = true;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            DialogCanvas.enabled = false;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        if (ShowInventory == true)
        {
            InventoryCanvas.enabled = true;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            InventoryCanvas.enabled = false;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

        }
    }
}
