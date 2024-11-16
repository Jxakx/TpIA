using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    private HashSet<Node> _closedNodes = new HashSet<Node>();
    private PriorityQueue _openNodes = new PriorityQueue();
}
