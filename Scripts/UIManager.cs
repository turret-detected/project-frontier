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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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

    public void displaySelectedUnitBox(Combatant combatant) {
        unitSelectionPanel.gameObject.SetActive(true);
        unitName.text = combatant.UnitName;
        unitDesc.text = 
        "Health: " + combatant.GetCurrentHealth() + 
        "\nArmor: " + combatant.Armor +
        "\nWeave: " + combatant.Weave +
        "\nAttack: " + combatant.AttackDamage + " " + combatant.AttackType +
        "\nRange: " + combatant.AttackRange;
    }

    public void hideSelectedUnitBox() {
        unitSelectionPanel.gameObject.SetActive(false);
    }

}
