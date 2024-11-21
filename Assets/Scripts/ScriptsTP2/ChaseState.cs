using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : State
{
    public override void EnterState(PlayerEnemies enemy)
    {
        //Debug.Log("Iniciando Persecuci�n " + enemy.gameObject.name);
    }

    public override void UpdateState(PlayerEnemies enemy)
    {
        GameManager.Instance.alert = true;
        GameManager.Instance.alertGameObject = enemy.gameObject.name;

        if (!enemy.IsPlayerInSight())
        {
            GameManager.Instance.alert = false;
            GameManager.Instance.alertGameObject = "";

            // Si el jugador ya no est� visible, utiliza A* para dirigirse al �ltimo nodo conocido
            if (enemy.lastVisitedNode != null)
            {
                //enemy.PathFindingState(); // Calcula el camino con A* hacia la �ltima posici�n conocida del jugador
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
        //Debug.Log("Saliendo de Persecuci�n");
    }
}
