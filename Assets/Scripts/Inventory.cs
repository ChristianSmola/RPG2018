using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class Inventory : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler {
    public Button InventoryButton;
    public Canvas InventoryCanvas;
    public GameObject InventoryDisplayItemLocation;
    public GameObject InventoryGeneralInfo;
    public List<Button> InventoryButtonList = new List<Button>();
    public List<Items> PlayerInventory = new List<Items>();
    public GameObject PlayerGO;

    private bool ShowInventory;
    private UnityStandardAssets.Characters.FirstPerson.FirstPersonController FPC;
    public Player player;
    //private Vector2 newPosition;
    //private Vector2 currentPosition;
    //private Vector2 startPosition;
    


    // Use this for initialization
    void Start () {
        FPC = gameObject.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>();
        //Player Inventory Setup 
        Database.ItemList();
        player = PlayerGO.GetComponent<Player>();
        PlayerInventory = player.PlayerInventory; 
       

        //Player Inventory Display Setup
        InventoryButtonList.Add(InventoryButton);

        InventoryCanvas.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Inventory")) {
            ShowInventory = !ShowInventory;
            Menus();
        }
    }

    void Menus() {
        if (ShowInventory == true)
        {
            UpdateInventory();
            InventoryCanvas.enabled = true;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            //FPC.enabled = false;
        }
        else
        {
            InventoryCanvas.enabled = false;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            //FPC.enabled = true;

        }
    }

    void UpdateInventory()
    {
        try
        {
            List<Button> NewInventoryButtonList = new List<Button>();

            for (int x = 0; x < PlayerInventory.Count; x++)
            {
                Button NewButton = Instantiate(InventoryButtonList[0], InventoryButtonList[0].transform.parent);
                NewButton.name = string.Concat("Button ", x.ToString());
                Text NewButtonText = NewButton.GetComponentInChildren<Text>();
                NewButtonText.text = PlayerInventory[x].ItemName;
                         
                NewInventoryButtonList.Add(NewButton);
            }

            for (int z = 0; z < InventoryButtonList.Count; z++)
            {
                Destroy(InventoryButtonList[z].gameObject);
            }

            InventoryButtonList = NewInventoryButtonList;
        }
        catch (System.Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }

    public void UpdateInventorySelection(int ButtonIndex)
    {
        try
        {
            Destroy(InventoryDisplayItemLocation.gameObject);
            //Okay it seems that instead of using the selected item we are using whatever the item in the number 1 slot is 
            GameObject taco = Instantiate(PlayerInventory[0].ItemMesh, InventoryDisplayItemLocation.transform.position, InventoryDisplayItemLocation.transform.rotation);
            taco.transform.rotation = new Quaternion(0, 90, 0, 0);
            Text ItemDiscription = InventoryGeneralInfo.GetComponentInChildren<Text>();
            ItemDiscription.text = PlayerInventory[1].ItemDiscription;
            InventoryDisplayItemLocation = taco;
        }
        catch (System.Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }

    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {

    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        InventoryDisplayItemLocation.transform.Rotate(Vector3.up, -(Input.GetAxis("Mouse X") * Mathf.Deg2Rad * 250), Space.World);
        InventoryDisplayItemLocation.transform.Rotate(Vector3.right, (Input.GetAxis("Mouse Y") * Mathf.Deg2Rad * 250), Space.World);
        //InventoryDisplayItemLocation.transform.Rotate(Vector3.forward, -(Input.GetAxis("Mouse Z") * Mathf.Deg2Rad * 250));
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {

    }
}
