using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    private Node startNode;
    private Node goalNode;
    private Graph graph;
    PriorityQueue<Node> frontierNodes;
    List<Node> exploredNodes;
    List<Node> pathNodes;
    public bool isComplete=false;
    int iterations=0;
    public void Init(Graph graph, Node start, Node goal )
    {
        this.graph=graph;
        this.startNode=start;
        this.goalNode=goal;
        this.frontierNodes=new PriorityQueue<Node>();
        this.exploredNodes= new List<Node>();
        //Clears data from previous pathfinding
        

    }

}
