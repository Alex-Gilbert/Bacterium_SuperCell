/// <summary>
/// Bacterium Interactive - Script for upcoming immune system game 2015
/// Programmers: Alex Gilbert, Darias Skiedra, Zhan Simeonov, Dave Elliott
/// Artists: Linda Marie Martinez, Steph Rivo
///
/// Bacterium Homepage: https://sites.google.com/site/bacteriuminteractive
/// </summary>

using UnityEngine;
using System.Collections;

/// <summary>
/// #DESCRIPTION OF CLASS#
/// </summary>
public class MainMenu : MonoBehaviour
{
	#region Variables (private)

	#endregion


	#region Properties (public)

	#endregion


	#region Unity event functions
	
	/// <summary>
	/// Debugging information should be placed here.
	/// </summary>
	void OnDrawGizmos()
	{

	}

	#endregion


	#region Methods

    public void exit()
    {
        Application.Quit();
    }

    public void LoadLevel(int level)
    {
        GameObject.Find("PauseMenu").GetComponent<PauseMenu>().MenuEnabled = true;
        Cursor.visible = false;
        Application.LoadLevel(level);
    }

    public void LevelMenu()
    {
        Camera.main.transform.rotation = new Quaternion(0, 180, 0, 0);
    }

    public void Main_Menu()
    {
        Camera.main.transform.rotation = new Quaternion(0, 0, 0, 0);
    }

	#endregion
}
	