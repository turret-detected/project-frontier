using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIMainMenu : MonoBehaviour
{
    public Text betaVersionText;

    // Start is called before the first frame update
    void Start()
    {
        setVersionIndicator();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnStartGame() {
        SceneManager.LoadScene("CharacterCreator", LoadSceneMode.Single);
    }

    public void OnLoadGame() {
        string sceneName = IOManager.ReadLastLevel();
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        IOManager.loadSaveOnNewLevel = true;
    }

    public void LoadMe() {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }

    public void OpenBugs() {
        string url = "https://github.com/turret-detected/project-frontier/issues";
        Application.OpenURL(url);
    }

    public void setVersionIndicator() {
        betaVersionText.text = "TPF Beta v." + Application.version;
    }

    public void QuitGame() {
        Application.Quit();
    }

}
