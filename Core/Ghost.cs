using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum State
{
    Chase=1,
    Scatter=0
}
public class Ghost : MonoBehaviour
{
  
    public string ghostName;
    private State ghostState;
    //Movement Variables
    [Header("Movement")]
    [SerializeField] public float ghostSpeed;
    private Graph graph;
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
    


        
    }
    public void Move(Node goalNode)
    {
        
        transform.position=Vector3.MoveTowards(transform.position,goalNode.position,ghostSpeed*Time.deltaTime);
        currentNode=goalNode;
    }
    public void SwitchState()
    {
        if(ghostState==State.Chase)
        {
            ghostState=State.Scatter;
        }
        else
        {
            ghostState=State.Chase;
        }

    }
}
