using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Linq;
using System.Text;
using System;

[System.Serializable]
public enum DamageType {
    PHYSICAL,
    MAGICAL
}

[System.Serializable]
public enum Faction {
    PLAYER,
    COMPUTER
}

[System.Serializable]
public enum State {
    PLAYER_MOVE,
    AI_MOVE,
    WAITING,
    SETUP,
    VICTORY,
    DEFEAT
}

[System.Serializable]
public enum UnitClass {
    NO_CLASS,
    WARRIOR,
    SCOUT,
    MAGE,
    CLERIC
}

public class Gamemaster : MonoBehaviour
{
    List<Combatant> combatants = new List<Combatant>();
    public const float damageReductionMod = 0.975f; 
    private State gameState;
    private int PlayerUnitCount = 0;
    private int AIUnitCount = 0;
    private bool AIGaveMoves;
    private EnemyAI activeAI;
    public GameObject UIContainer;
    private UIManager ui;
    private List<UnitData> unitlist;

    // Start is called before the first frame update
    void Start()
    {
        unitlist = new List<UnitData>();
        gameState = State.WAITING;
        ui = UIContainer.GetComponent<UIManager>();
    }

    // Update is called once per frame
    void Update()
    {
        // Game over
        if (gameState == State.DEFEAT) return;

        // Victory!
        if (gameState == State.VICTORY) {
            ui.toggleLootPanel(true);
        }

        // Placement is done, start the game!
        if (gameState == State.SETUP && ui.isPlacementListEmpty()) {
            endSetup();
        }

        // Check for victory or defeat
        if (gameState == State.PLAYER_MOVE || gameState == State.AI_MOVE) {
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
                AIGaveMoves = true;
                StartCoroutine(AITurn());
            }   
        }

        if (gameState == State.PLAYER_MOVE) {
            // exist
        }
    }

    IEnumerator AITurn() {
        List<EnemyAI> aiList = new List<EnemyAI>();

        foreach (Combatant c in combatants) {
            if (c.UnitFaction == Faction.COMPUTER) {
                aiList.Add(c.GetComponentInParent<EnemyAI>());
            }
        }

        while (aiList.Count > 0) {
            aiList[0].gmMove();
            while (activeAI != aiList[0]) {
                yield return new WaitForSeconds(1); 
            }
            aiList.Remove(activeAI);
        }
        yield return new WaitForSeconds(1); 
        endAITurn();
    }

    public void AITurnComplete(EnemyAI ai) {
        activeAI = ai;
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
        activeAI = null;
        ResetActions(Faction.COMPUTER);
    }

    public void startGame() {
        if (gameState == State.WAITING) {
            // ui updates
            ui.destroyStartButton();
            ui.togglePlacementPanel(true);
            gameState = State.SETUP;
            
            // get list of players
            List<string> namelist = new List<string>();
            unitlist = IOManager.ReadPlayerDataFromFile();
            foreach (UnitData unit in unitlist) {
                if (unit.UnitFaction == Faction.PLAYER) {
                    namelist.Add(unit.UnitDisplayName);
                }
            }

            // update ui
            ui.populatePlacementList(namelist);
        } else {
            Debug.Log("Tried to move to setup but was in state " + gameState);
        }
    }

    public void endSetup() {
        if (gameState == State.SETUP) {
            ui.togglePlacementPanel(false);
            ui.setTurnIndicator(Faction.PLAYER);
            gameState = State.PLAYER_MOVE;
        } else {
            Debug.Log("Tried to start game but but was in state " + gameState);
        }
    }

    public void AddCombatant(Combatant c) {
        //Debug.Log("P:" + PlayerUnitCount);
        //Debug.Log("AI:" + AIUnitCount);
        combatants.Add(c);
        if (c.UnitFaction == Faction.PLAYER) {
            PlayerUnitCount++;
        }
        if (c.UnitFaction == Faction.COMPUTER) {
            AIUnitCount++;
        }
        //Debug.Log("P:" + PlayerUnitCount);
        //Debug.Log("AI:" + AIUnitCount);
    }

    public void RemoveCombatant(Combatant c) {
        //Debug.Log("P:" + PlayerUnitCount);
        //Debug.Log("AI:" + AIUnitCount);
        combatants.Remove(c);
        if (c.UnitFaction == Faction.PLAYER) {
            PlayerUnitCount--;
        }
        if (c.UnitFaction == Faction.COMPUTER) {
            AIUnitCount--;
        }
        //Debug.Log("P:" + PlayerUnitCount);
        //Debug.Log("AI:" + AIUnitCount);
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

    

    public void ResetActions(Faction faction) {
        foreach (Combatant c in combatants) {
            if (c.UnitFaction == faction) {
                c.ResetActions();
            }

        }
    }

    public UnitData GetUnitDataFromName(string s) {
        foreach (UnitData u in unitlist) {
            if (u.UnitDisplayName.Equals(s)) {
                return u;
            }
        }


        // TODO turn the string into a unitdata from the stored list
        return null;
    }

    public void SpawnUnitFromDataAtPos(UnitData data, Vector3 pos) {
        data.Position = pos;
        SpawnUnitFromData(data);
    }

    public void SpawnUnitFromData(UnitData data) {
        // Spawn the gameobject from prefab
        GameObject spawn;
        try {
            GameObject prefab = Resources.Load<GameObject>(data.UnitPrefabName);   
            spawn = Instantiate(prefab);
        } catch (ArgumentException e) {
            spawn = null;
            Debug.Log("Couldn't find prefab. THIS IS A BUG! " + data.UnitPrefabName + " doesn't exist!");
            Debug.Log(e.StackTrace);
        }
                        
        // Set pos, rotation, and stats
        GameObject moveable = spawn.transform.GetChild(0).gameObject;
        moveable.GetComponent<CharacterController>().enabled = false;
        moveable.transform.position = data.Position; // char controller needs to off to do this for some reason? 
        moveable.transform.rotation = data.Rotation;
        moveable.GetComponent<CharacterController>().enabled = true;
        Combatant c = moveable.GetComponent<Combatant>();
        c.SetUnitData(data);
        Debug.Log("Added Object at: X " + data.Position.x + " Y " + data.Position.y + " Z " + data.Position.z);
    
    }

    public void saveGame() {
        // only if player's turn
        if (gameState == State.PLAYER_MOVE) {
            List<UnitData> unitlist = new List<UnitData>();

            foreach (Combatant c in combatants) {
                unitlist.Add(c.CreateDataClass());
            }

            IOManager.WritePlayerDataToFile(unitlist); // TODO don't ignore boolean
        } else {
            Debug.Log("Can't save during AI turn!");
        }
    }

    public void loadGame() {
        gameState = State.WAITING;
        List<UnitData> unitlist = IOManager.ReadPlayerDataFromFile();
        if (unitlist == null) {
            Debug.Log("Couldn't load, error!");
        } else {
            //Delete existing entities
            foreach (Combatant c in combatants) {
                Destroy(c.transform.parent.gameObject);
            }
            combatants.Clear();
            AIUnitCount = 0;
            PlayerUnitCount = 0;

            //Create entities
            foreach (UnitData u in unitlist) {
                SpawnUnitFromData(u);
            }
        }
        gameState = State.PLAYER_MOVE; // Since we can only save during player turn
    }

    public void quitGame() {
        //if saved
        //exit
        Debug.Log("Received Exit Command");
        Application.Quit();
    }
}
