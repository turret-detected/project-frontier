using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{

    public GameObject EquipLootPanel;
    public List<GameObject> LootPanels;
    public List<GameObject> LootableItems;
    private GameObject SelectedItem;
    private List<Combatant> PlayerUnits = new List<Combatant>();
    private Gamemaster gm;
    private Dropdown dropdown;

    // Start is called before the first frame update
    void Start()
    {
        dropdown = EquipLootPanel.GetComponentInChildren<Dropdown>();
        gm = GameObject.Find("GameMaster").GetComponent<Gamemaster>();

        foreach (GameObject obj in LootPanels) {
            GameObject item = GenerateLootItem(); // get item from loot list
            GameObject preview = obj.transform.GetChild(0).GetChild(0).gameObject;
            Destroy(preview); // delete existing
            preview = Instantiate(item); // add new

            // reposition
            preview.transform.position = obj.transform.GetChild(0).position;
            preview.transform.SetParent(obj.transform.GetChild(0));
            ItemWeapon wep;
            if (preview.TryGetComponent<ItemWeapon>(out wep)) {
                preview.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
                preview.transform.Translate(Vector3.down * 1);
                preview.transform.Rotate(Vector3.down * 90);
                Debug.Log("WEAPON");
            } else {
                preview.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            }
            
            // UI layer
            preview.layer = 5;

            // set text
            Text name = obj.transform.GetChild(2).GetComponent<Text>();
            Text desc = obj.transform.GetChild(3).GetComponent<Text>();
            name.text = item.GetComponent<IItem>().GetItemName();
            desc.text = item.GetComponent<IItem>().GetItemDesc();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject GenerateLootItem() { // Roll and remove then return item from loot list
        int roll = Random.Range(0, LootableItems.Count);
        GameObject temp = LootableItems[roll];
        LootableItems.Remove(temp);
        return temp;
    }

    public void SelectLootItem(GameObject lootPanel) {
        SelectedItem = lootPanel.GetComponentInChildren<ItemWeapon>().gameObject;
        this.gameObject.transform.GetChild(0).gameObject.SetActive(false); // disable frame
        EquipLootPanel.SetActive(true);
        PopulateUnitList();        
    }

    public void AttachLootItem(Combatant c) {
        c.EquipItem(SelectedItem);
    }

    public void PopulateUnitList() { // add player unit string to dropdown
        PlayerUnits = gm.GetPlayerCombatants();
        List<string> displayList = new List<string>();
        
        foreach (Combatant c in PlayerUnits) {
            displayList.Add(c.UnitName + " (" + StringEnums.GetUnitClassString(c.UnitClass) + ")");
        }

        dropdown.AddOptions(displayList);
    }

    public Combatant getSelectedCombatant() {
        return PlayerUnits[dropdown.value];
    }

    public void EquipOnSelectedButton() {
        AttachLootItem(getSelectedCombatant());
        gm.doneLooting();
    }



}
