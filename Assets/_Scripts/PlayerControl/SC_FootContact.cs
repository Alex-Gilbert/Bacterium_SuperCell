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
public class SC_FootContact : MonoBehaviour
{
	#region Variables (private)
    private int numOfContacts = 0;
	#endregion


	#region Properties (public)
    public int NumOfContacts { get { return numOfContacts; } }
	#endregion

    

	#region Unity event functions

    public void Update()
    {
        //print(numOfContacts);
    }

    public void OnTriggerEnter(Collider collision)
    {
        ++numOfContacts;
    }

    public void OnTriggerExit(Collider collision)
    {
        --numOfContacts;
    }

	#endregion


	#region Methods

	#endregion
}
	