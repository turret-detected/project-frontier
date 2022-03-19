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
    public Text unitName;
    public Text unitDesc;
    private Combatant selectedUnit;
    public GameObject escMenuPanel;

    // Start is called before the first frame update
    void Start()
    {
        selectedUnit = null;
        //gm = gameMaster.GetComponent<Gamemaster>();
    }

    // Update is called once per frame
    void Update()
    {
        if (selectedUnit != null) {
            unitSelectionPanel.gameObject.SetActive(true);
            unitName.text = selectedUnit.UnitName;
            unitDesc.text = 
            "Health: " + selectedUnit.CurrentHealth + 
            "\nArmor: " + selectedUnit.Armor +
            "\nWeave: " + selectedUnit.Weave +
            "\nAttack: " + selectedUnit.AttackDamage + " " + selectedUnit.AttackType +
            "\nRange: " + selectedUnit.AttackRange;
        } else {
            unitSelectionPanel.gameObject.SetActive(false);
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
}
