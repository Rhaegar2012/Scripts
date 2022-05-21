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
    private Vector3 direction;
    public Node currentNode;
    public List<Node> nodePath;


    void Update()
    {
       transform.position=Vector3.MoveTowards(transform.position,currentNode.position,ghostSpeed*Time.deltaTime);
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
        float distance=Vector3.Distance(goalNode.position,transform.position);
        float distanceTolerance=0.05f;
        while(distance>distanceTolerance)
        {
            Vector3 direction=(goalNode.position-currentNode.position).normalized;
            transform.position+=direction*ghostSpeed*Time.deltaTime;
            distance=Vector3.Distance(goalNode.position,transform.position);
        }
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
