using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pathfinder:MonoBehaviour
{
    private Node startNode;
    private Node goalNode;
    private Graph graph;
    PriorityQueue<Node> frontierNodes;
    List<Node> exploredNodes;
    List<Node> pathNodes;
    public bool isComplete=false;
    [SerializeField] int iterationLimit;
    int iterations=0;
    public void Init(Graph graph, Node start, Node goal )
    {
        if(start==null||goal==null||graph==null)
        {
            Debug.LogWarning("PATHFINDER Init error: missing components");
            return;
        }
        if(start.nodeType==NodeType.Blocked||goal.nodeType==NodeType.Blocked)
        {
            Debug.LogWarning("PATHFINDER Init error: start and end nodes must be unblocked");
            return;
        }
        this.graph=graph;
        this.startNode=start;
        this.goalNode=goal;
        this.frontierNodes=new PriorityQueue<Node>();
        frontierNodes.Enqueue(start);
        this.exploredNodes= new List<Node>();
        //Clears data from previous pathfinding
        for(int x=0;x<graph.width;x++)
        {
            for(int y=0;y<graph.height;y++)
            {
                //graph.nodes[x,y].Reset();
            }
        }
        isComplete=false;
        iterations=0;
        startNode.distanceTraveled=0;

    }

    //Search Routine 
    public List<Node> SearchRoutine()
    {
        while(!isComplete)
        {
            iterations++;
            if(frontierNodes.Count>0)
            {
                Node currentNode=frontierNodes.Dequeue();
                
                if(!exploredNodes.Contains(currentNode))
                {
                    exploredNodes.Add(currentNode);
                }
                ExpandFrontier(currentNode);
                if(frontierNodes.Contains(goalNode))
                {
                    pathNodes=GetPathNodes(goalNode);
                    isComplete=true;
                }
            }
            if(iterations>iterationLimit)
            {
                break;
            }
        }
        return pathNodes;
    }
    //Expand Frontier method (Dijkstra's Algorithm)
    void ExpandFrontier(Node node)
    {
        if(node!=null)
        {
            for(int i=0; i<node.neighbors.Count;i++)
            {
                if(!exploredNodes.Contains(node.neighbors[i]))
                {
                    float distanceToNeighbor=graph.GetNodeDistance(node,node.neighbors[i]);
                    float newDistanceTraveled=distanceToNeighbor+node.distanceTraveled;
                    if(float.IsPositiveInfinity(node.neighbors[i].distanceTraveled)||
                    newDistanceTraveled<node.neighbors[i].distanceTraveled)
                    {
                        node.neighbors[i].previous=node;
                        node.neighbors[i].distanceTraveled=newDistanceTraveled;
                    }
                    if(!frontierNodes.Contains(node.neighbors[i]))
                    {
                        node.neighbors[i].priority=(int)node.neighbors[i].distanceTraveled;
                        frontierNodes.Enqueue(node.neighbors[i]);
                    }
                }
            }
        }

    }

    List<Node> GetPathNodes(Node endNode)
    {
        List<Node> path=new List<Node>();
        if(endNode==null)
        {
            return path;
        }
        path.Add(endNode);
        Node currentNode=endNode.previous;
        while(currentNode!=null)
        {
            path.Insert(0,currentNode);
            currentNode=currentNode.previous;
        }
        return path;
    }

}
