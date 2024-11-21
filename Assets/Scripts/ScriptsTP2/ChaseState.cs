using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : State
{
    public override void EnterState(PlayerEnemies enemy)
    {
        //Debug.Log("Iniciando Persecución " + enemy.gameObject.name);
    }

    public override void UpdateState(PlayerEnemies enemy)
    {
        GameManager.Instance.alert = true;
        GameManager.Instance.alertGameObject = enemy.gameObject.name;

        if (!enemy.IsPlayerInSight())
        {
            GameManager.Instance.alert = false;
            GameManager.Instance.alertGameObject = "";

            // Si el jugador ya no está visible, utiliza A* para dirigirse al último nodo conocido
            if (enemy.lastVisitedNode != null)
            {
                //enemy.PathFindingState(); // Calcula el camino con A* hacia la última posición conocida del jugador
                enemy.StateMachine.ChangeState(new PatrolAStar(), enemy);
            }
            else
            {
                enemy.StateMachine.ChangeState(new PatrolState(), enemy); // Patrullaje normal si no hay nodo guardado
            }
            return;
        }

        // Perseguir al jugador directamente si está en la línea de visión
        enemy.MoveTowards(enemy.Player.position);
    }


    public override void ExitState(PlayerEnemies enemy)
    {
        //Debug.Log("Saliendo de Persecución");
    }
}
