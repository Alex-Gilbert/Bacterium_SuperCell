/// <summary>
/// Bacterium Interactive - Script for upcoming immune system game 2015
/// Programmers: Alex Gilbert, Darias Skiedra, Zhan Simeonov, Dave Elliott
/// Artists: Linda Marie Martinez, Steph Rivo
///
/// Bacterium Homepage: https://sites.google.com/site/bacteriuminteractive
/// </summary>

using UnityEngine;
using System.Collections;

public enum Axis
{
    X,
    Y,
    Z
}

/// <summary>
/// #DESCRIPTION OF CLASS#
/// </summary>
public class SC_RibLook : MonoBehaviour
{
    #region Variables (private)

    [SerializeField]
    private float maxRotation = 40;
    [SerializeField]
    private float minRotation = -22;
    [SerializeField]
    private Axis axis = Axis.X;

    #endregion


    #region Properties (public)
    public float curRotation = 0;

    public float curZRotation = 0;
    #endregion


    #region Unity event functions

    /// <summary>
    /// Use this for initialization.
    /// </summary>
    void Start()
    {

    }

    /// <summary>
    /// Update is called once per frame.
    /// </summary>	
    void LateUpdate()
    {
        this.transform.Rotate(Vector3.forward, curRotation);
        this.transform.Rotate(Vector3.left, curZRotation);
    }

    /// <summary>
    /// Debugging information should be placed here.
    /// </summary>
    void OnDrawGizmos()
    {

    }

    #endregion


    #region Methods
    public void AddRotation(float amountToAdd)
    {
        curRotation = Mathf.Clamp(curRotation + amountToAdd, minRotation, maxRotation);
    }

    public void SetRotation(float rotation)
    {
        curRotation = rotation;
    }
    #endregion
}
