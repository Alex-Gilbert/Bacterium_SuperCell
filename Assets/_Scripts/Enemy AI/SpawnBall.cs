/// <summary>
/// Bacterium Interactive - Script for upcoming immune system game 2015
/// Programmers: Alex Gilbert, Darias Skiedra, Zhan Simeonov, Dave Elliott
/// Artists: Linda Marie Martinez, Steph Rivo
///
/// Bacterium Homepage: https://sites.google.com/site/bacteriuminteractive
/// </summary>

using UnityEngine;
using System.Collections;

public class SpawnBall : MonoBehaviour
{

    #region Variables (private)
    [SerializeField]
    private GameObject Enemy;
    private double life;
    private Vector3 temp;

    #endregion

    #region Properties (public)

    #endregion


    #region Unity event functions

    /// <summary>
    /// Use this for initialization.
    /// </summary>
    void Awake()
    {
        life = Time.time + 20f;
        StartCoroutine(Spawn());
    }

    /// <summary>
    /// Update is called once per frame.
    /// </summary>	
    void LateUpdate()
    {
        if (Time.time >= life)
            DestroyImmediate(gameObject);
    }

    IEnumerator Spawn()
    {
        yield return new WaitForSeconds(4f);
        GameObject enemy = (GameObject)Instantiate(Enemy, transform.position, transform.rotation);
        float height = enemy.GetComponent<MeshRenderer>().bounds.extents.y;
        temp = enemy.transform.position;
        temp.y += height;
        enemy.transform.position = temp;
        DestroyImmediate(gameObject);
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