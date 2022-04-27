using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Text turnIndicator;
    public Text selectionIndicator;
    public Button startButton;
    public GameObject unitSelectionPanel;
    public GameObject unitActionPanel;
    public Text unitName;
    public Text unitDesc;
    private Combatant selectedUnit;
    public GameObject escMenuPanel;
    public GameObject placementMenuPanel;
    public GameObject lootMenuPanel;
    public GameObject damageLogPanel;
    public Text betaVersionText;
    
    private string lastDamage = "";

    // Start is called before the first frame update
    void Start()
    {
        setVersionIndicator();
        selectedUnit = null;
        //gm = gameMaster.GetComponent<Gamemaster>();
    }

    // Update is called once per frame
    void Update()
    {
        if (selectedUnit != null) { 
            // Cover
            string cover;
            if (!selectedUnit.InCover) {
                cover = "Not in cover";
            } else {
                cover = "In cover";
            }

            // Shorten dmg type
            string dmg_type;
            if (selectedUnit.AttackType == DamageType.PHYSICAL) {
                dmg_type = "Phys";
            } else if (selectedUnit.AttackType == DamageType.MAGICAL) {
                dmg_type = "Magic";
            } else {
                dmg_type = "BAD DMG TYPE";
            }

            unitSelectionPanel.gameObject.SetActive(true);
            unitName.text = selectedUnit.UnitName;
            unitDesc.text = 
            selectedUnit.CurrentHealth + "/" + selectedUnit.MaxHealth + " Health\n" +
            selectedUnit.ArmorName + ": " + selectedUnit.Armor + " Armor | " + selectedUnit.Weave + " Weave\n" + 
            selectedUnit.WeaponName + ": " + selectedUnit.AttackDamage + " " + dmg_type + " Damage\n" +
            selectedUnit.RemainingMoves + "/" + selectedUnit.MaxMoves + " Moves | " + selectedUnit.RemainingAttacks + "/" + selectedUnit.MaxAttacks + " Attacks\n" +
            "Range: " + selectedUnit.AttackRange + " | " + cover + "\n";


            /*
            "Health: " + selectedUnit.CurrentHealth + 
            "\nArmor: " + selectedUnit.Armor +
            "\nWeave: " + selectedUnit.Weave +
            "\nAttack: " + selectedUnit.AttackDamage + " " + selectedUnit.AttackType +
            "\nRange: " + selectedUnit.AttackRange +
            "\nCover: " + selectedUnit.InCover;
            */
        } else {     
            unitSelectionPanel.gameObject.SetActive(false);
        }

        // swap string compare with a gm reference and bool check
        if (selectedUnit != null && selectedUnit.UnitFaction == Faction.PLAYER && turnIndicator.text == "Turn: Player") {
            unitActionPanel.gameObject.SetActive(true);
        } else {
            unitActionPanel.gameObject.SetActive(false);
        }
    }

    public void destroyStartButton() {
        Destroy(startButton.gameObject);
    }

    public void setTurnIndicator(Faction faction) {
        if (faction == Faction.PLAYER) {
            turnIndicator.text = "Turn: Player";
        }
        if (faction == Faction.COMPUTER) {
            turnIndicator.text = "Turn: Computer";
        }
    }

    public void setVictory(Faction faction) {
        if (faction == Faction.PLAYER) {
            turnIndicator.text = "VICTORY!";
        }
        if (faction == Faction.COMPUTER) {
            turnIndicator.text = "DEFEAT!";
        }
    }

    public void setSelectedUnit(Combatant combatant) {
        selectedUnit = combatant;
    }

    public void toggleEscPanel(bool value) {
        escMenuPanel.gameObject.SetActive(value);
    }

    public void togglePlacementPanel(bool value) {
        placementMenuPanel.gameObject.SetActive(value);
    }

    public void toggleLootPanel(bool value) {
        lootMenuPanel.gameObject.SetActive(value);
    }

    public void populatePlacementList(List<string> namelist) {
        Dropdown dropdown = placementMenuPanel.GetComponentInChildren<Dropdown>();
        dropdown.ClearOptions();
        dropdown.AddOptions(namelist);
        dropdown.RefreshShownValue();
    }

    public string getSelectedUnitToPlace() {
        Dropdown dropdown = placementMenuPanel.GetComponentInChildren<Dropdown>();
        string name = dropdown.options[dropdown.value].text;
        return name;
    }

    public void placedUnit(string name) {
        Dropdown dropdown = placementMenuPanel.GetComponentInChildren<Dropdown>();
        // Code from:
        // https://forum.unity.com/threads/how-to-use-dropdown-options-remove.501916/#post-3267568
        dropdown.options.Remove(dropdown.options.Find((x) => x.text == name)); 
        dropdown.RefreshShownValue();
    }

    public bool isPlacementListEmpty() {
        Dropdown dropdown = placementMenuPanel.GetComponentInChildren<Dropdown>();
        return dropdown.options.Count == 0;
    }

    public void logDamage(string log) {
        
        damageLogPanel.GetComponentInChildren<Text>().text = lastDamage + "\n" + log;
                
        lastDamage = log;

    }

    public void setVersionIndicator() {
        betaVersionText.text = "TPF Beta v." + Application.version;
    }
    

    /// https://www.youtube.com/watch?v=8yzpjkoE0YA
    /// Good video
}
