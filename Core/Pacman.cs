using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pacman : MonoBehaviour
{
    private  Graph graph;
    [SerializeField] float pacmanSpeed;
    [SerializeField] float distanceTolerance;
    public Node currentNode;
    public List<Node> pathNodes=new List<Node>();
    private Rigidbody2D myRigidBody;
    private Vector2 currentDirection;
    private bool advance=true;
    //Unity event
    void Update()
    {
        if(advance)
        {
            transform.position+=(Vector3)currentDirection*pacmanSpeed*Time.deltaTime;
            if(IsCurrentNodeReached(currentNode))
            {
                advance=false;
                transform.position=currentNode.position;
            }
        }
    }
    //Initializes PacMan from GameController
    public void Init(Node startNode,Graph maze)
    {
        currentNode=startNode;
        graph=maze;
        myRigidBody=GetComponent<Rigidbody2D>();
        myRigidBody.velocity*=pacmanSpeed;
        
    }

    public void Move(Node node,Vector2 direction)
    {
        currentDirection=direction;
        pathNodes.Add(node);
        currentNode=node;

        if(!graph.IsValidNode(currentDirection,currentNode))
        {
            if(IsCurrentNodeReached(currentNode))
            {
                advance=false;
            }
            else
            {
                advance=true;
            }
           
          
        }
        else
        {
            advance=true;
        }
        
    
    }

    public bool IsCurrentNodeReached(Node node)
    {
        float distance=Vector3.Distance(transform.position,node.position);
        if(distance<distanceTolerance)
        {
            transform.position=node.position;
            return true;
           
        }
        else
        {
            return false;
        }
    }

    void OnTriggerEntry2D(Collider other)
    {
        //TODO
    }

 
}
