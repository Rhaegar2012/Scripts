using System;
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
    private Animator animator;
    public List<Node> currentPath;
    public List<string> capturedGhosts;
    public string capturedGhost;
    public static event Action OnPacmanDeath;
    public static event Action OnPacmanTeleport;

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
        animator= GetComponent<Animator>();
        
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

            animator.SetBool("isEating",false);
            Ghost ghost = other.gameObject.GetComponent<Ghost>();
            string name=ghost.ghostName;
            if(ghost.ghostState==State.Chase)
            {
                OnPacmanDeath?.Invoke();
                animator.SetBool("isDead",true);
                StartCoroutine(DeathSequence());
            }
            else
            {
                capturedGhosts.Add(name);
                capturedGhost=name;
                ghost.Capture();
                Destroy(other.gameObject);
            }
        }
        if(other.tag=="pellet"||other.tag=="pill")
        {
            animator.SetBool("isEating",true);
        }
        if(other.tag=="portal")
        {
            OnPacmanTeleport?.Invoke();
        }
    }
    IEnumerator DeathSequence()
    {
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
        
    }
    IEnumerator EatAnimation()
    {
        yield return null;
        animator.SetBool("isEating",true);
        
    }



 
}
