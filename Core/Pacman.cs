using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pacman : MonoBehaviour
{
    private  Graph graph;
    [SerializeField] float pacmanSpeed;
    [SerializeField] float distanceTolerance;
    public Node currentNode;
    private Rigidbody2D myRigidBody;
    private Vector2 currentDirection;
    public List<Node> currentPath;
    public List<string> capturedGhosts;

    //Unity event
    void Update()
    {
        transform.position=Vector3.MoveTowards(transform.position,currentNode.position,pacmanSpeed*Time.deltaTime);
    }
    //Initializes PacMan from GameController
    public void Init(Node startNode,Graph maze)
    {
        currentNode=startNode;
        graph=maze;
        myRigidBody=GetComponent<Rigidbody2D>();
        myRigidBody.velocity*=pacmanSpeed;
        currentPath=new List<Node>();
        
    }

    public void Move(Node node,Vector2 direction)
    {
        currentDirection=direction;
        currentNode=node;
        currentPath.Add(currentNode);
        
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag=="ghost")
        {
            Ghost ghost = other.gameObject.GetComponent<Ghost>();
            string name=ghost.ghostName;
            if(ghost.ghostState==State.Chase)
            {
                Destroy(gameObject);
            }
            else
            {
                capturedGhosts.Add(name);
                ghost.Capture();
                Destroy(other.gameObject);
            }
        }
    }


 
}
