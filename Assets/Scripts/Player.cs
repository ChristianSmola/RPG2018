using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour, IDragHandler {
    public Animator HandAnim;

    private bool ShowDialog = false;
    private bool ShowInventory = false;
    private bool ShowLootScreen;

    public Button DialogNextBtn;
    public Button InventoryButton;

    public Canvas DialogCanvas;
    public Canvas InventoryCanvas;

    public float Charge = 0f;

    private GameObject HostileSpell;
    
    private GameObject Master;
    private GameObject Spell;
    public GameObject InventoryDisplayItemLocation;
    public GameObject InventoryGeneralInfo;

    public List<GameObject> HostileList;
    public List<Button> InventoryButtonList = new List<Button>();
    private List<Combat> KnownSpells = new List<Combat>();
    public List<Items> PlayerInventory = new List<Items>();

    private Spell SpellScript;

    public Text ItemText;

    private NPC NpcWithinView;

    private UnityStandardAssets.Characters.FirstPerson.FirstPersonController FPC;

    // Use this for initialization
    void Start () {
        FPC = gameObject.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>();
        Master = GameObject.FindGameObjectWithTag("Master");

        //Player Spells Setup
        Database.Moves();
        KnownSpells.Add(Database.Magic[0]);
        KnownSpells.Add(Database.Magic[1]);
        KnownSpells.Add(Database.Magic[2]);
        KnownSpells.Add(Database.Magic[3]);
        KnownSpells.Add(Database.Magic[4]);
        Spell = KnownSpells[4].Spell;
        SpellScript = Spell.GetComponent<Spell>();

        //Player Inventory Setup 
        Database.ItemList();
        PlayerInventory.Add(Database.Item[0]);
        PlayerInventory.Add(Database.Item[1]);
        PlayerInventory.Add(Database.Item[0]);

        //Player Inventory Display Setup
        InventoryButtonList.Add(InventoryButton);

        DialogNextBtn.onClick.AddListener(DialogNextClick);

        //Close Canvas Menus
        DialogCanvas.enabled = false;
        InventoryCanvas.enabled = false;
    }
    
    // Update is called once per frame
    void Update() { 
        //Menus
        if (Input.GetButtonDown("Talk"))
        {
            CheckSight();
        }
        if (Input.GetButtonDown("Inventory"))
        {
            ShowInventory = !ShowInventory;
            Menus();
        }
        if (ShowInventory == false)
        {
            //Charge Attack
            if (Input.GetMouseButtonDown(1))
            {
                HandAnim.Play("Magic Shot Expressive");
                LaunchSpell(Charge);
            }

            if (Input.GetKey(KeyCode.RightShift) || Input.GetKey(KeyCode.LeftShift))
            {
                Debug.Log("I am Running");
            }

            if (Input.GetAxis("Mouse ScrollWheel") > 0.0f)
            {
                for (int x = 0; x < KnownSpells.Count; x++)
                {
                    if (Spell == KnownSpells[x].Spell)
                    {
                        if (x < KnownSpells.Count - 1)
                        {
                            Spell = KnownSpells[x + 1].Spell;
                            SpellScript = Spell.GetComponent<Spell>();
                        }
                        else
                        {
                            Spell = KnownSpells[0].Spell;
                            SpellScript = Spell.GetComponent<Spell>();
                        }
                        break;
                    }
                }
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0.0f)
            {
                for (int x = 0; x < KnownSpells.Count; x++)
                {
                    if (Spell == KnownSpells[x].Spell)
                    {
                        if (x > 0)
                        {
                            Spell = KnownSpells[x - 1].Spell;
                            SpellScript = Spell.GetComponent<Spell>();
                        }
                        else
                        {
                            Spell = KnownSpells[KnownSpells.Count - 1].Spell;
                            SpellScript = Spell.GetComponent<Spell>();
                        }
                        break;


                    }
                }
            }
        }
    }

    public void Menus()
    {
        if (ShowDialog == true)
        {
            DialogCanvas.enabled = true;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            FPC.enabled = false;
        }
        else
        {
            DialogCanvas.enabled = false;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            FPC.enabled = true;
        }

        if (ShowInventory == true)
        {
            UpdateInventory();
            InventoryCanvas.enabled = true;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            FPC.enabled = false;
        }
        else
        {
            InventoryCanvas.enabled = false;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            FPC.enabled = true;

        }
    }

    private void CheckSight()
    {
        try
        {
            Transform cam = Camera.main.transform;
            bool foundhit;
            RaycastHit hit = new RaycastHit();
            Physics.Raycast(cam.position, cam.forward, out hit, 15f);
            //at some point i should probably make this into a switch statement
            if (hit.transform.tag == "NPC")
            {
                NpcWithinView = hit.transform.gameObject.GetComponent<NPC>();
                ShowDialog = !ShowDialog;
                Menus();
            }
            else
            {
                NpcWithinView = null;
                ShowDialog = false;
            }
            if (hit.transform.tag == "Lootable")
            {
                ShowLootScreen = true;
            }
            else
                ShowLootScreen = false;
        }
        catch
        {

        }
    }

    public void UpdateInventory()
    {
        try
        {

            List<Button> NewInventoryButtonList = new List<Button>();

            for (int x = 0; x < PlayerInventory.Count; x++)
            {
                Button NewButton = Instantiate(InventoryButtonList[0], InventoryButtonList[0].transform.parent);
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
        catch
        {

        }
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        if (ShowInventory == true)
        {
            Vector2 startPosition = eventData.pressPosition;
            Vector2 currentPosition = eventData.position;
            Debug.Log("start position: " + startPosition + "; current position: " + currentPosition);
        }
        
    }

    public void UpdateInventorySelection()
    {
        try
        {
            Destroy(InventoryDisplayItemLocation.gameObject);
            GameObject taco = Instantiate(PlayerInventory[1].ItemMesh, InventoryDisplayItemLocation.transform.position, InventoryDisplayItemLocation.transform.rotation);
            taco.transform.rotation = new Quaternion(0, 90, 0, 0);
            Text ItemDiscription = InventoryGeneralInfo.GetComponentInChildren<Text>();
            ItemDiscription.text = PlayerInventory[1].ItemDiscription;
        }
        catch{

        }
    }


    private void LaunchSpell(float charge) {
        if (SpellScript.ThisSpell.Spelleffect == Combat.SpellEffect.GroundSpikes)
        {
            RaycastHit hitInfo;
            if (Physics.Raycast(this.transform.position + transform.forward, Vector3.down, out hitInfo))
            {
                GameObject xSpell = Instantiate(Spell, hitInfo.point, Quaternion.identity);
                Spell spell = xSpell.GetComponent<Spell>();
                spell.OwnerGO = this.gameObject;
                spell.enabled = true;
            }
        }
        else if (SpellScript.ThisSpell.Spelleffect == Combat.SpellEffect.Swarm)
        {
            Quaternion q = new Quaternion(transform.rotation.x, transform.rotation.y, 0.0f, transform.rotation.w);
            GameObject xSpell = Instantiate(Spell, Master.transform.position, q);
            Spell spell = xSpell.GetComponent<Spell>();
            spell.OwnerGO = this.gameObject;
            spell.enabled = true;

        }
        else
        {
            GameObject xSpell = Instantiate(Spell, Master.transform.position, transform.rotation);
            Spell spell = xSpell.GetComponent<Spell>();
            spell.OwnerGO = this.gameObject;
            spell.enabled = true;
        }
    }

    private void DialogNextClick()
    {
        Debug.Log("Taco Cat");
    }

    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Spell" || col.gameObject.tag == "Weapon")
        {
            //HostileSpell = col.gameObject.GetComponent<Spell>();
        }
    }
}
