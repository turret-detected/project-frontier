using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionManager : MonoBehaviour
{
    public Camera cam;
    public GameObject gameMasterContainer;
    private Gamemaster gm;
    public GameObject UIContainer;
    private UIManager ui;
    private GameObject selectedSelectable;
    private bool isMoving;
    private bool showingEscPanel = false;

    // Start is called before the first frame update
    void Start()
    {
        isMoving = false;
        gm = gameMasterContainer.GetComponent<Gamemaster>();
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

        RaycastHit hit;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * 100);
        if (Physics.Raycast(ray, out hit, 1000f)) {

            // Selection mode
            if (Input.GetButtonDown("Select") && hit.transform.gameObject.tag == "Selectable" && hit.transform.gameObject != selectedSelectable) {
                Debug.Log("Hit Selectable!");
                if (selectedSelectable != null) {
                    selectedSelectable.GetComponent<Combatant>().SetSelected(false);
                }
                selectedSelectable = hit.transform.gameObject;
                selectedSelectable.GetComponent<Combatant>().SetSelected(true);
                ui.setSelectedUnit(selectedSelectable.GetComponent<Combatant>());

            }


            if (gm.GetCurrentState() == State.PLAYER_MOVE && selectedSelectable != null) {
                // Action mode
                if (Input.GetButtonDown("Move") && hit.transform.gameObject.tag == "Terrain") {
                    Debug.Log("Moving!");
                    int x = (int)Mathf.Round(hit.point.x);
                    int z = (int)Mathf.Round(hit.point.z);
                    selectedSelectable.GetComponent<Combatant>().Move(x, z);
                    // TODO set moving and edit movement script to say when its done!
                } else if (Input.GetButtonDown("Move") && hit.transform.gameObject.GetComponent<Combatant>().UnitFaction == Faction.COMPUTER) {
                    Debug.Log("Attacking!");
                    selectedSelectable.GetComponent<Combatant>().Attack(hit.transform.gameObject.GetComponent<Combatant>());
                    // TODO set moving and not moving
                } else if (Input.GetButtonDown("Cancel")) {
                    Debug.Log("Deselected!");
                    ui.setSelectedUnit(null);
                    selectedSelectable.GetComponent<Combatant>().SetSelected(false);
                    selectedSelectable = null;
                }
            }
        }            
    }
}
