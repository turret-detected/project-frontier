using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UICharacterCreator : MonoBehaviour
{

    public Text betaVersionText;
    public string nextMissionName;
    UICharCreatorPanel[] PanelList;

    // Start is called before the first frame update
    void Start()
    {
        setVersionIndicator();
        PanelList = GetComponentsInChildren<UICharCreatorPanel>();
        Debug.Log("Found " + PanelList.Length + " ui panels!");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame() {

        // Get character from each UI panel and put it in a list for saving
        List<UnitData> unitlist = new List<UnitData>();
        foreach (UICharCreatorPanel p in PanelList) {
            unitlist.Add(p.GetUnitData());
        }

        // TODO Run user safety checks here (nameless, etc.)

        // Save and start game
        IOManager.WritePlayerDataToFile(unitlist);
        Debug.Log("Loading " + nextMissionName);
        SceneManager.LoadScene(nextMissionName, LoadSceneMode.Single);


    }

    public void setVersionIndicator() {
        betaVersionText.text = "TPF Beta v." + Application.version;
    }

}
