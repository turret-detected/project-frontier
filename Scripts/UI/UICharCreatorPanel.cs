using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UICharCreatorPanel : MonoBehaviour
{
    public GameObject ModelCleric;
    public GameObject ModelWarrior;
    public GameObject ModelMage;
    public GameObject ModelScout;
    public InputField NameInputText;
    public Text SelectedClassText;
    public Dropdown ClassDropdown;
    public int DropdownDefaultValue;
    public GameObject ModelPositionMarker;
    public Camera MainCam;
    private GameObject CurrentModel;
    private List<UnitClass> AvailableClasses = new List<UnitClass>();
    private List<GameObject> ClassModels = new List<GameObject>();
    private UnitClass CurrentClass;


    // Start is called before the first frame update
    void Start()
    {
        // Must match order in dropdown
        AvailableClasses.Add(UnitClass.CLERIC);
        AvailableClasses.Add(UnitClass.WARRIOR);
        AvailableClasses.Add(UnitClass.SCOUT);
        AvailableClasses.Add(UnitClass.MAGE);

        ClassModels.Add(ModelCleric);
        ClassModels.Add(ModelWarrior);
        ClassModels.Add(ModelScout);
        ClassModels.Add(ModelMage);
        
        ClassDropdown.value = DropdownDefaultValue;
        ChangeSelectedClass();

    }

    // Update is called once per frame
    void Update()
    {
        if (CurrentModel != null) {
            CurrentModel.GetComponentInChildren<MovementAI>().turnTowardTarget(MainCam.transform.position);
            CurrentModel.GetComponentInChildren<CharacterController>().velocity.Set(0,0,0);
        }
    }

    public void ChangeSelectedClass() {
        CurrentClass = AvailableClasses[ClassDropdown.value];
        Destroy(CurrentModel);
        GameObject newModel = ClassModels[ClassDropdown.value];
        CurrentModel = Instantiate(newModel, ModelPositionMarker.transform.position, ModelPositionMarker.transform.rotation);
        //CurrentModel.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        //CurrentModel.transform.rotation = Quaternion.LookRotation(MainCam.transform.position);
    }


    public UnitData GetUnitData() { //called after character creation is done
        // make and return a new unit data class with given name and class
        string name = NameInputText.text;
        UnitClass c = CurrentClass;
        UnitData generatedData = new UnitData(name, c);
        return generatedData;
    }

    public void GenerateRandomName() {

        List<string> names = new List<string>{"Alex", "Sam", "Taylor", "Quinn", "Ash"};

        // https://answers.unity.com/questions/881597/text-object-cant-be-edited.html <3 random internet stranger
        NameInputText.text = names[UnityEngine.Random.Range(0, names.Count)];
        // Alex, Quinn, Sam, Taylor, Ash // unisex names since there's no gender indicator (only cleric is female tho..)
         
    }



}
