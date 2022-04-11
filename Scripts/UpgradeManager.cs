using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{

    public List<GameObject> LootPanels;
    public List<GameObject> LootableItems;

    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject obj in LootPanels) {
            GameObject item = GenerateLootItem();
            GameObject preview = obj.transform.GetChild(0).GetChild(0).gameObject;
            Destroy(preview);
            preview = Instantiate(item);
            preview.transform.position = obj.transform.GetChild(0).position;
            preview.transform.SetParent(obj.transform.GetChild(0));
            preview.transform.localScale = new Vector3(1, 1, 1);
            preview.layer = 5;
            Text name = obj.transform.GetChild(2).GetComponent<Text>();
            Text desc = obj.transform.GetChild(3).GetComponent<Text>();
            name.text = item.GetComponent<IItem>().GetItemName();
            desc.text = item.GetComponent<IItem>().GetItemDesc();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject GenerateLootItem() {
        int roll = Random.Range(0, LootableItems.Count);
        GameObject temp = LootableItems[roll];
        LootableItems.Remove(temp);
        return temp;
    }



}
