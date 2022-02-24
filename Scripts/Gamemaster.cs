using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public enum DamageType {
    PHYSICAL,
    MAGICAL
}

public enum Faction {
    PLAYER,
    COMPUTER
}

public enum State {
    PLAYER_MOVE,
    AI_MOVE,
    WAITING,
}

public class Gamemaster : MonoBehaviour
{
    List<Combatant> combatants = new List<Combatant>();
    public const float damageReductionMod = 0.975f; 
    private State gameState;
    private int PlayerUnitCount;
    private int AIUnitCount;
    public GameObject UIContainer;
    private UIManager ui;

    // Start is called before the first frame update
    void Start()
    {
        gameState = State.WAITING;
        PlayerUnitCount = 0;
        AIUnitCount = 0;
        ui = UIContainer.GetComponent<UIManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameState != State.WAITING) {
            if (PlayerUnitCount > 0 && AIUnitCount < 1) {
                // Player victory
            }
            if (AIUnitCount > 0 && PlayerUnitCount < 1) {
                // AI victory
            }            
        }

        if (gameState == State.AI_MOVE) {
            // iterate through AI and make them move
        }

        if (gameState == State.PLAYER_MOVE) {
            // exist
        }
    }

    public void endPlayerTurn() {
        if (gameState == State.PLAYER_MOVE) {
            gameState = State.AI_MOVE;
            ui.setTurnIndicator(Faction.COMPUTER);
        } else {
            Debug.Log("Can't end turn, not your turn!");        
        }
    }

    public void startGame() {
        if (gameState == State.WAITING) {
            ui.destroyStartButton();
            ui.setTurnIndicator(Faction.PLAYER);
            gameState = State.PLAYER_MOVE;
        } else {
            Debug.Log("Tried to start game but it already started!");
        }
    }

    public void AddCombatant(Combatant c) {
        combatants.Add(c);
        if (c.UnitFaction == Faction.PLAYER) {
            PlayerUnitCount++;
        }
        if (c.UnitFaction == Faction.COMPUTER) {
            AIUnitCount++;
        }
    }

    public void RemoveCombatant(Combatant c) {
       if (c.UnitFaction == Faction.PLAYER) {
            PlayerUnitCount--;
        }
        if (c.UnitFaction == Faction.COMPUTER) {
            AIUnitCount--;
        } 
    }

    public float GetDamageReductionMod() {
        return damageReductionMod;
    }

    public State GetCurrentState() {
        return gameState;
    }
}
