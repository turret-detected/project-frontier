using System.Collections;
using System.Collections.Generic;
using UnityEngine;





public class Inventory : MonoBehaviour
{
    public List<IItem> ItemList;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public List<string> getStoreableList() {
        List<string> list = new List<string>();
        
        foreach (IItem item in ItemList) {
            list.Add(item.GetItemPrefabString());
        }

        return list;
    }
}
