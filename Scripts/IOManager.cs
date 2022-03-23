using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Xml.Linq;
using System.Runtime.Serialization;
using System.Text;

public class IOManager : MonoBehaviour
{

    public static bool WritePlayerDataToFile(List<UnitData> unitlist) {

        try {           
            //Code from: https://gamedevelopment.tutsplus.com/tutorials/how-to-save-and-load-your-players-progress-in-unity--cms-20934
            //Code from: https://stackoverflow.com/questions/36852213/how-to-serialize-and-save-a-gameobject-in-unity
            FileStream file = File.Create(Application.persistentDataPath + "/savedGame.dat");

            //Serialize to xml
            DataContractSerializer bf = new DataContractSerializer(unitlist.GetType());
            MemoryStream streamer = new MemoryStream();

            //Serialize the file
            bf.WriteObject(streamer, unitlist);
            streamer.Seek(0, SeekOrigin.Begin);

            //Save to disk
            // Fix nul bug: https://stackoverflow.com/questions/16086165/lots-of-unexpected-nul-caracters-at-the-end-of-an-xml-with-memorystream
            file.Write(streamer.ToArray(), 0, streamer.ToArray().Length);

            // Close the file to prevent any corruptions
            file.Close();

            // DEBUG
            string result = XElement.Parse(Encoding.ASCII.GetString(streamer.GetBuffer()).Replace("\0", "")).ToString();
            Debug.Log(Application.persistentDataPath);
            Debug.Log("Serialized Result: " + result);

            return true;
        } catch {
            return false;
        }
    }
}


