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
    public Node currentNode;
    private Vector2 currentDirection;

    // Update is called once per frame
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
    public void MoveGhost(Node node,Vector2 direction)
    {
        currentNode=node;
        currentDirection=direction;
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
