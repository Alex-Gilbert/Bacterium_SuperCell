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
public class Trigger : MonoBehaviour
{
    #region Variables (private)
    [SerializeField]
    private Chaser Manager;
    [SerializeField]
    private bool left = false, right =  false, back = false;
    private int collisions = 0;

    #endregion


    #region Properties (public)

    #endregion

    #region Unity event functions

    void Update()
    {
        /*if(left)
        print("Left = Collisions" + collisions);
        else if(right)
            print("Right = Collisions" + collisions);
        else print("Back = Collisions" + collisions);*/
    }

    void OnTriggerEnter(Collider col)
    {
        collisions++;
        //print("Trigger");
        if (!(col.gameObject.tag == "Chaser"))
        {
            if (left)
                Manager.left_collide = true;
            else if (right)
                Manager.right_collide = true;
            else Manager.back_collide = true;
        }
    }

    void OnTriggerExit(Collider col)
    {
        collisions--;
        if(!(col.gameObject.tag == "Chaser"))
        {
            if(collisions == 0)
            {
                //print("Trigger Exit!");
                if (left)
                    Manager.left_collide = false;
                else if (right)
                    Manager.right_collide = false;
                else Manager.back_collide = false;
            }
        }
    }


    /// <summary>
    /// Debugging information should be placed here.
    /// </summary>
    void OnDrawGizmos()
    {

    }

    #endregion

    #region Methods

    #endregion
}