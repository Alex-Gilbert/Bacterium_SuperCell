using UnityEngine;
using System.Collections;

public class PauseMenu : MonoBehaviour 
{

    public bool MenuEnabled = false;
    private static PauseMenu Instance;
    private bool Paused = false;
    [SerializeField]
    public GameObject PauseObject;
	// Use this for initialization
	void Awake () 
    {
        if(Instance)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            Instance = this;
            MenuEnabled = false;
            PauseObject.SetActive(false);
            DontDestroyOnLoad(gameObject);
        }
	}
	
	// Update is called once per frame
	void LateUpdate() 
    {
	    if(MenuEnabled && Input.GetKeyDown(KeyCode.Escape))
        {
            if (!Paused)
            {
                Cursor.visible = true;
                Paused = true;
                Time.timeScale = 0f;
                PauseObject.SetActive(true);
            }
            else resume();
        }     
	}

    public void exit()
    {
        Application.Quit();
    }

    public void resume()
    {
        PauseObject.SetActive(false);
        Paused = false;
        Time.timeScale = 1f;
        Cursor.visible = true;
    }

    public void MainMenu()
    {
        MenuEnabled = false;
        resume();
        Application.LoadLevel(0);
    }
}
