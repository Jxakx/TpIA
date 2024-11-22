using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public bool alert;
    public string alertGameObject = "";

    public List<GameObject> allSkulls = new List<GameObject>();

    public List<GameObject> skullsInTravel = new List<GameObject>();
    private void Start()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
