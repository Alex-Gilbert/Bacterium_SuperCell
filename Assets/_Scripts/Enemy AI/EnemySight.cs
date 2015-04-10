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
public class EnemySight : MonoBehaviour
{
    #region Variables (private)

    private RaycastHit hit;
    private GameObject Player;
    [SerializeField]
    private int FoV = 110;

    #endregion


    #region Properties (public)
    public float distance = 12;

    #endregion


    #region Unity event functions

    /// <summary>
    /// Use this for initialization.
    /// </summary>
    void Awake()
    {
        Player = GameObject.FindWithTag("Player");
    }

    #endregion


    #region Methods

    public bool Sighted()
    {
        Vector3 direction = Player.transform.position - transform.position;
        float angle = Vector3.Angle(direction, transform.forward);
        if (angle < FoV * 0.5f)
        {
            if (Physics.Raycast(transform.position + transform.up, direction, out hit, distance))
                if (hit.collider.gameObject.tag == Player.gameObject.tag)
                    return true;
                else
                    print("Collided with " + hit.collider.gameObject);
        }
        return false;
    }

    #endregion
}
