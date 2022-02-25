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
    VICTORY,
    DEFEAT
}

public class Gamemaster : MonoBehaviour
{
    List<Combatant> combatants = new List<Combatant>();
    public const float damageReductionMod = 0.975f; 
    private State gameState;
    private int PlayerUnitCount;
    private int AIUnitCount;
    private int AIMovesRemaining;
    private bool AIGaveMoves;
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
        if (gameState == State.VICTORY || gameState == State.DEFEAT) return;

        if (gameState != State.WAITING) {
            if (PlayerUnitCount > 0 && AIUnitCount <= 0) {
                // Player victory
                gameState = State.VICTORY;
                ui.setVictory(Faction.PLAYER);
                Debug.Log("VICTORY!");
            }
            if (AIUnitCount > 0 && PlayerUnitCount <= 0) {
                // AI victory
                gameState = State.DEFEAT;
                ui.setVictory(Faction.COMPUTER);
                Debug.Log("DEFEAT!");
            }            
        }

        if (gameState == State.AI_MOVE) {
            // iterate through AI and make them move
            if (!AIGaveMoves) {
                AIMovesRemaining = 0;
                AIGaveMoves = true;
                foreach (Combatant c in combatants) {
                    if (c.UnitFaction == Faction.COMPUTER) {
                        c.GetComponentInParent<EnemyAI>().gmMove();
                        AIMovesRemaining++;
                    }
                }
            }

            // wait for 0 count on AIMovesRemaining
            if (AIMovesRemaining == 0) {
                AIMovesRemaining = -1;
                endAITurn();
            } 

                      
            
        }

        if (gameState == State.PLAYER_MOVE) {
            // exist
        }
    }

    public void endPlayerTurn() {
        if (gameState == State.PLAYER_MOVE) {
            gameState = State.AI_MOVE;
            ui.setTurnIndicator(Faction.COMPUTER);
            ResetActions(Faction.PLAYER);
        } else {
            Debug.Log("Can't end turn, not your turn!");        
        }
    }

    public void endAITurn() {
        ui.setTurnIndicator(Faction.PLAYER);
        gameState = State.PLAYER_MOVE;
        AIGaveMoves = false;
        ResetActions(Faction.COMPUTER);
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
        Debug.Log("P:" + PlayerUnitCount);
        Debug.Log("AI:" + AIUnitCount);
    }

    public void RemoveCombatant(Combatant c) {
       combatants.Remove(c);
       if (c.UnitFaction == Faction.PLAYER) {
            PlayerUnitCount--;
        }
        if (c.UnitFaction == Faction.COMPUTER) {
            AIUnitCount--;
        }
        Debug.Log("P:" + PlayerUnitCount);
        Debug.Log("AI:" + AIUnitCount);
    }

    public float GetDamageReductionMod() {
        return damageReductionMod;
    }

    public State GetCurrentState() {
        return gameState;
    }

    public GameObject getNearestEnemy(GameObject obj) { // ONLY TARGETS PLAYER UNITS
        GameObject target = null;
        float distance = 100000;
        foreach (Combatant c in combatants) {
            if (c.UnitFaction == Faction.PLAYER) {
                float temp = Vector3.Distance(obj.transform.position, c.gameObject.transform.position);
                if (temp < distance) {
                    distance = temp;
                    target = c.gameObject;
                }
            }
        }
        return target;
    } 

    public void AIFinishedAction() {
        AIMovesRemaining--;
        Debug.Log("AI Finished a turn. " + AIMovesRemaining + " moves left.");
    }

    public void ResetActions(Faction faction) {
        foreach (Combatant c in combatants) {
            if (c.UnitFaction == faction) {
                c.ResetActions();
            }

        }
    } 
}
