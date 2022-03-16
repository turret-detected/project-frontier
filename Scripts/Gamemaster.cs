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
    VICTORY,
    DEFEAT
}

public class Gamemaster : MonoBehaviour
{
    List<Combatant> combatants = new List<Combatant>();
    public const float damageReductionMod = 0.975f; 
    private State gameState;
    private int PlayerUnitCount = 0;
    private int AIUnitCount = 0;
    private int AIMovesRemaining;
    private bool AIGaveMoves;
    public GameObject UIContainer;
    private UIManager ui;

    // Start is called before the first frame update
    void Start()
    {
        gameState = State.WAITING;
        //PlayerUnitCount = 0;
        //AIUnitCount = 0;
        ui = UIContainer.GetComponent<UIManager>();
    }

    // Update is called once per frame
    void Update()
    {
        // Game over
        if (gameState == State.VICTORY || gameState == State.DEFEAT) return;

        // Check for victory or defeat
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
        Debug.Log("P:" + PlayerUnitCount);
        Debug.Log("AI:" + AIUnitCount);
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
        Debug.Log("P:" + PlayerUnitCount);
        Debug.Log("AI:" + AIUnitCount);
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

    public void saveGame() {
        // only if player's turn
        if (gameState == State.PLAYER_MOVE) {
            List<UnitData> unitlist = new List<UnitData>();

            foreach (Combatant c in combatants) {
                unitlist.Add(c.CreateDataClass());
            }

            
            //Code from: https://gamedevelopment.tutsplus.com/tutorials/how-to-save-and-load-your-players-progress-in-unity--cms-20934
            /*
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(Application.persistentDataPath + "/savedGame.gd");
            bf.Serialize(file, unitlist);
            file.Close();
            */

            // TODO WHY DOES THIS ADD A BUNCH OF NULL AT THE END???
            //Code from: https://stackoverflow.com/questions/36852213/how-to-serialize-and-save-a-gameobject-in-unity
            FileStream file = File.Create(Application.persistentDataPath + "/savedGame.dat");

            //Serialize to xml
            DataContractSerializer bf = new DataContractSerializer(unitlist.GetType());
            MemoryStream streamer = new MemoryStream();

            //Serialize the file
            bf.WriteObject(streamer, unitlist);
            streamer.Seek(0, SeekOrigin.Begin);

            //Save to disk
            file.Write(streamer.GetBuffer(), 0, streamer.GetBuffer().Length);

            // Close the file to prevent any corruptions
            file.Close();

            // DEBUG
            string result = XElement.Parse(Encoding.ASCII.GetString(streamer.GetBuffer()).Replace("\0", "")).ToString();
            Debug.Log(Application.persistentDataPath);
            Debug.Log("Serialized Result: " + result);


        } else {
            Debug.Log("Can't save during AI turn!");
        }
    }

    public void loadGame() {
        gameState = State.WAITING;
        List<UnitData> unitlist = new List<UnitData>();
        
        //Code from: https://gamedevelopment.tutsplus.com/tutorials/how-to-save-and-load-your-players-progress-in-unity--cms-20934
        if(File.Exists(Application.persistentDataPath + "/savedGame.dat")) {

            FileStream file = File.Open(Application.persistentDataPath + "/savedGame.dat", FileMode.Open);

            // XML reader
            XmlDictionaryReader reader = XmlDictionaryReader.CreateTextReader(file, new XmlDictionaryReaderQuotas());
            DataContractSerializer bf = new DataContractSerializer(typeof(List<UnitData>));
                     
            // Read XML and convert
            unitlist = (List<UnitData>)bf.ReadObject(reader, true);
            Debug.Log("Test : "+unitlist[0].CurrentHealth);
            reader.Close();
            file.Close();

            //Delete existing entities
            foreach (Combatant c in combatants) {
                Destroy(c.transform.parent.gameObject);
            }
            combatants.Clear();
            AIUnitCount = 0;
            PlayerUnitCount = 0;

            //Create entities
            foreach (UnitData u in unitlist) {
                GameObject prefab = Resources.Load<GameObject>(u.UnitPrefabName);   
                GameObject spawn = Instantiate(prefab);
                Debug.Log("CHILD COUNT: "+spawn.transform.childCount);
                
                // Set pos
                GameObject moveable = spawn.transform.GetChild(0).gameObject;
                moveable.GetComponent<CharacterController>().enabled = false;
                moveable.transform.position = u.Position; // okay thanks character controller. 
                moveable.GetComponent<CharacterController>().enabled = true;
                Debug.Log("Added Object at: X " + u.Position.x + " Y " + u.Position.y + " Z " + u.Position.z);   

                // TODO
                // INVENTORY/WEAPON/STATS/ROTATION
            }
        }
        gameState = State.PLAYER_MOVE;
    }

    public void quitGame() {
        //if saved
        //exit
    }
}
