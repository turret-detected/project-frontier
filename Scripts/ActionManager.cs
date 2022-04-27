using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum ActionState {
    NONE,
    MOVE,
    ATTACK,
    SPECIAL_ABILITY
}



public class ActionManager : MonoBehaviour
{
    public Camera cam;
    public Texture2D attackCursorTexture;
    public Texture2D moveCursorTexture;
    private Gamemaster gm;
    public GameObject UIContainer;
    private UIManager ui;
    private GameObject selectedSelectable;
    private bool showingEscPanel = false;
    private int ignoreActionLayer = 1 << 20;
    private ActionState actionState;
    

    // Start is called before the first frame update
    void Start()
    {
        //isMoving = false;
        gm = GameObject.Find("GameMaster").GetComponent<Gamemaster>();
        ui = UIContainer.GetComponent<UIManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cheat Key")) {
            gm.endGame();
        }

        if (selectedSelectable == null) {
            if (Input.GetButtonDown("Cancel")) {
                ui.toggleEscPanel(!showingEscPanel);
                showingEscPanel = !showingEscPanel;
            }
        } else {
            if (actionState == ActionState.ATTACK) {
                selectedSelectable.GetComponent<Combatant>().ToggleRangeHighlight(true);
            } else {
                selectedSelectable.GetComponent<Combatant>().ToggleRangeHighlight(false);
            }
        }

        if (Input.GetButtonDown("Cancel") && selectedSelectable != null) {
            if (actionState == ActionState.NONE) {
                ui.setSelectedUnit(null);
                selectedSelectable.GetComponent<Combatant>().SetSelected(false);
                selectedSelectable = null;
            } else {
                Revert();
            }
            
        }

        RaycastHit hit;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * 100);
        if (Physics.Raycast(ray, out hit, 1000f, ~ignoreActionLayer)) {
            if (Input.GetButtonDown("Select") && !EventSystem.current.IsPointerOverGameObject()) { // UI blocks raycast
                
                
                // PLACEMENT
                if (gm.GetCurrentState() == State.SETUP && hit.transform.gameObject.tag == "PlayerSpawnPoint") {
                    Vector3 pos = hit.transform.position + (Vector3.down/2);
                    string temp = ui.getSelectedUnitToPlace();
                    UnitData unit = gm.GetUnitDataFromName(temp);
                    gm.SpawnUnitFromDataAtPos(unit, pos);
                    ui.placedUnit(temp);

                    GameObject spawnPointParent = hit.transform.parent.gameObject;
                    Destroy(hit.transform.gameObject); // gets rid of collider
                    spawnPointParent.GetComponentInChildren<ParticleSystem>().Stop(); // turn off smoke
                }

                // SELECT A UNIT
                if (hit.transform.gameObject.tag == "Selectable" && hit.transform.gameObject != selectedSelectable && actionState == ActionState.NONE) {
                    if (selectedSelectable != null) {
                        selectedSelectable.GetComponent<Combatant>().SetSelected(false);
                    }
                    selectedSelectable = hit.transform.gameObject;
                    selectedSelectable.GetComponent<Combatant>().SetSelected(true);
                    ui.setSelectedUnit(selectedSelectable.GetComponent<Combatant>());
                }

                Combatant selected;
                if (selectedSelectable != null) {
                    selected = selectedSelectable.GetComponent<Combatant>();
                } else {
                    selected = null;
                }

                // MOVE A UNIT
                if (selectedSelectable != null && actionState == ActionState.MOVE && hit.transform.gameObject.tag == "Terrain") {
                    int x = (int)Mathf.Round(hit.point.x);
                    int z = (int)Mathf.Round(hit.point.z);
                    selected.Move(x, z);
                    // TODO block other orders while unit is moving
                }

                // TARGETED ACTIONS
                Combatant target;
                hit.transform.gameObject.TryGetComponent<Combatant>(out target);
                if (target != null) {
                    // ATTACK A UNIT
                    if (selected != null && actionState == ActionState.ATTACK) {
                        
                        if (target.UnitFaction == Faction.COMPUTER) {
                            selected.Attack(target);
                        }
                        // TODO fix this so it does TryGetComponent, bc it causes script error
                    }

                    // SPECIAL ABILITY
                    if (selected != null && actionState == ActionState.SPECIAL_ABILITY) {
                        if (selectedSelectable.GetComponent<IAbility>().isValidTarget(selected, target)) {
                            selectedSelectable.GetComponent<IAbility>().performAbility(selected, target);
                        } else {
                            Debug.Log("Invalid target or on cooldown!");
                        }
                    }
                }



            }
        }

        if (gm.GetCurrentState() == State.AI_MOVE && actionState != ActionState.NONE) {
            Revert();
        }            
    }

    public void SetActionState(ActionState state) {
        actionState = state;
    }

    public void Attack() {
        SetActionState(ActionState.ATTACK);
        Cursor.SetCursor(attackCursorTexture, Vector2.zero, CursorMode.Auto);
        // change cursor to sword
    }

    public void Move() {
        SetActionState(ActionState.MOVE);
        Cursor.SetCursor(moveCursorTexture, Vector2.zero, CursorMode.Auto);
        // change cursor to finger pointer
    }

    public void Special() {
        SetActionState(ActionState.SPECIAL_ABILITY);
        Cursor.SetCursor(moveCursorTexture, Vector2.zero, CursorMode.Auto);
        // change cursor to finger pointer
    }

    public void Revert() {
        SetActionState(ActionState.NONE);
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        // revert cursor
    }
}
