using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public bool alert;
    public PlayerEnemies alertGameObject;

    private void Start()
    {
        if(Instance = null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
