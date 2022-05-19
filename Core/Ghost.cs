using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum State
{
    Chase=1,
    Flee=0
}
public class Ghost : MonoBehaviour
{
  
    public string ghostName;
    private State ghostState;
    //Movement Variables
    [Header("Movement")]
    [SerializeField] public float ghostSpeed;
    private Graph graph;
    private Pathfinder pathfinder;
    public Node currentNode;
    public List<Node> nodePath;


    void Update()
    {
        
        
    } 
    public void Init(string name,Node currentNode,Graph graph)
    {
        this.ghostName=name;
        this.currentNode=currentNode;
        this.graph=graph;
        ghostState=State.Chase;
        pathfinder=GetComponent<Pathfinder>();


        
    }
    public void Move(Node goalNode)
    {

        pathfinder.Init(graph,currentNode,goalNode);
        nodePath=pathfinder.SearchRoutine();
        foreach(Node node in nodePath)
        {
            transform.position=Vector3.MoveTowards(transform.position,node.position,ghostSpeed*Time.deltaTime);
        }
        
    }
    public void SwitchState()
    {
        if(ghostState==State.Chase)
        {
            ghostState=State.Flee;
        }
        else
        {
            ghostState=State.Chase;
        }

    }
}
