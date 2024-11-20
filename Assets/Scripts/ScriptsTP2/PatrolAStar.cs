using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolAStar : State
{
    public override void EnterState(PlayerEnemies enemy)
    {
        Debug.Log("Entre a clase Star");
    }

    public override void ExitState(PlayerEnemies enemy)
    {
        throw new System.NotImplementedException();
    }

    public override void UpdateState(PlayerEnemies enemy)
    {
        throw new System.NotImplementedException();
    }
}
