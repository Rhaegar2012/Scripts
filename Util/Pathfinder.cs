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
    List<Node> aStarNodes;
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
        //Debug.Log($"search start node {startNode.position}");
        frontierNodes.Enqueue(start);
        this.exploredNodes= new List<Node>();
        this.aStarNodes= new List<Node>();
        //Clears data from previous pathfinding
        for(int x=0;x<graph.width;x++)
        {
            for(int y=0;y<graph.height;y++)
            {
                graph.nodes[x,y].Reset();
            }
        }
        aStarNodes.Clear();
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
            //Debug.Log($"search iterations {iterations}");
            if(frontierNodes.Count>0)
            {
                Node currentNode=frontierNodes.Dequeue();
                //Debug.Log($"search current node {currentNode.position}");
                
                if(!exploredNodes.Contains(currentNode))
                {
                    exploredNodes.Add(currentNode);
                }
                ExpandFrontierAStar(currentNode);
                aStarNodes.Add(currentNode);
                if(frontierNodes.Contains(goalNode))
                {
                    //Debug.Log("Found target node");
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
    //Expand Frontier (A* Algorithm)
    void ExpandFrontierAStar(Node node)
    {
        if(node!=null)
        {
            for(int i=0;i<node.neighbors.Count;i++)
            {
                if(!exploredNodes.Contains(node.neighbors[i]))
                {
                    float distanceToNeighbor=graph.GetNodeDistance(node,node.neighbors[i]);
                    float newDistanceTraveled=distanceToNeighbor+node.distanceTraveled+(int)node.nodeType;
                    if(float.IsPositiveInfinity(node.neighbors[i].distanceTraveled)||
                    newDistanceTraveled<node.neighbors[i].distanceTraveled)
                    {
                        //Debug.Log($"Previous node added to {node.neighbors[i].position}");
                        node.neighbors[i].previous=node;
                        //Debug.Log($"Previous node added {node.position}");
                        node.neighbors[i].distanceTraveled=newDistanceTraveled;
                    }
                    if(!frontierNodes.Contains(node.neighbors[i])&&graph!=null)
                    {
                        int distanceToGoal=(int) graph.GetNodeDistance(node.neighbors[i],goalNode);
                        node.neighbors[i].priority=(int)node.neighbors[i].distanceTraveled+distanceToGoal;
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
        //Debug.Log($"End node position {endNode.position}");
        path.Add(endNode);
        Node currentNode=endNode.previous;
        if(currentNode==null)
        {
            return aStarNodes;
        }
        while(currentNode!=null)
        {
            //Debug.Log("Path loop accessed");
            path.Insert(0,currentNode);
            currentNode=currentNode.previous;
            //Debug.Log($"backtracking node position {currentNode.position}");
        }
        return path;
    }

}
