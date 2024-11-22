using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuncionesPaths : MonoBehaviour
{
    

    public static FuncionesPaths Instance;

    public List<Node> nodes = new List<Node>();

    public LayerMask _obstacleMask;

    private void Awake()
    {
        Instance = this;
    }

    
    public Node getClosestNode(Vector3 position)
    {
        var closestNode = nodes[0];
        var closestDistance = Vector3.Distance(closestNode.transform.position, position);

        for (var i = 1; i < nodes.Count; i++)
        {
            if (closestDistance > Vector3.Distance(position, nodes[i].transform.position) && OnSight(nodes[i].transform.position,position, _obstacleMask))
            {
                closestNode = nodes[i];
                closestDistance = Vector3.Distance(position, closestNode.transform.position);     
            }
        }

        return closestNode;
    }

    //Line of sight
    public static bool OnSight(Vector3 from, Vector3 to, LayerMask obstacleMask)
    {
        var dir = to - from;
        return !Physics.Raycast(from, dir, dir.magnitude, obstacleMask);
    }

    //Field of view
    public static bool FieldOfView(Vector3 from, Vector3 forward, Vector3 target, float viewAngle, LayerMask obstacleMask)
    {
        var dir = target - from;

        if(Vector3.Angle(forward, dir) < viewAngle / 2 && OnSight(from, target, obstacleMask))
        {
            return true;
        }

        return false;
    }
    
}
