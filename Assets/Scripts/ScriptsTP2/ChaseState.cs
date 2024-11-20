using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : State
{
    public override void EnterState(PlayerEnemies enemy)
    {
        Debug.Log("Iniciando Persecuci�n " + enemy.gameObject.name);
    }

    public override void UpdateState(PlayerEnemies enemy)
    {
        GameManager.Instance.alert = true;
        GameManager.Instance.alertGameObject = enemy.gameObject.name;

        if (!enemy.IsPlayerInSight())
        {
            GameManager.Instance.alert = false;
            GameManager.Instance.alertGameObject = "";

            // Calcula el camino hacia la �ltima posici�n conocida del jugador
            if (enemy.lastVisitedNode != null)
            {
                enemy.PathFindingState();  // Aqu� invocamos el c�lculo del camino usando A*
                enemy.StateMachine.ChangeState(new PatrolAStar(), enemy);
            }
            else
            {
                enemy.StateMachine.ChangeState(new PatrolState(), enemy); // Patrullaje normal si no hay nodo guardado
            }
            return;
        }

        // Perseguir al jugador directamente si est� en la l�nea de visi�n
        enemy.MoveTowards(enemy.Player.position);
    }

    public override void ExitState(PlayerEnemies enemy)
    {
        Debug.Log("Saliendo de Persecuci�n");
    }
}
