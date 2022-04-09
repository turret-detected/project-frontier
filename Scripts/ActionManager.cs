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
    private bool isMoving;
    private bool showingEscPanel = false;
    private int ignoreActionLayer = 1 << 20;
    private ActionState actionState;
    

    // Start is called before the first frame update
    void Start()
    {
        isMoving = false;
        gm = GameObject.Find("GameMaster").GetComponent<Gamemaster>();
        ui = UIContainer.GetComponent<UIManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (selectedSelectable != null) {
            isMoving = selectedSelectable.GetComponent<MovementAI>().IsMoving();
        } else {
            if (Input.GetButtonDown("Cancel")) {
                ui.toggleEscPanel(!showingEscPanel);
                showingEscPanel = !showingEscPanel;
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
            if (Input.GetButtonDown("Select") && !EventSystem.current.IsPointerOverGameObject()) {
                
                
                // PLACEMENT
                if (gm.GetCurrentState() == State.SETUP && hit.transform.gameObject.tag == "PlayerSpawnPoint") {
                    Vector3 pos = hit.transform.position + (Vector3.down/2);
                    string temp = ui.getSelectedUnitToPlace();
                    UnitData unit = gm.GetUnitDataFromName(temp);
                    gm.SpawnUnitFromDataAtPos(unit, pos);
                    ui.placedUnit(temp);
                    
                    // destroy parent to remove smoke
                    Destroy(hit.transform.parent.gameObject);
                    // but not this way, because the smoke stops instantly, it should fade out
                    // instead: disable hitbox, and turn off emitter
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

                // MOVE A UNIT
                if (selectedSelectable != null && actionState == ActionState.MOVE && hit.transform.gameObject.tag == "Terrain") {
                    int x = (int)Mathf.Round(hit.point.x);
                    int z = (int)Mathf.Round(hit.point.z);
                    selectedSelectable.GetComponent<Combatant>().Move(x, z);
                    // TODO set moving and edit movement script to say when its done!
                }

                // ATTACK A UNIT
                if (selectedSelectable != null && actionState == ActionState.ATTACK) {
                    if (hit.transform.gameObject.GetComponent<Combatant>().UnitFaction == Faction.COMPUTER) {
                        selectedSelectable.GetComponent<Combatant>().Attack(hit.transform.gameObject.GetComponent<Combatant>());
                    }
                    // TODO fix this so it tries to get component, bc it causes script error
                }

                // SPECIAL ABILITY
                if (selectedSelectable != null && actionState == ActionState.SPECIAL_ABILITY) {
                    // TODO
                    // is ability friendly or hostile?
                    // is the hit target the correct type?
                    // do the ability
                }


            }
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
