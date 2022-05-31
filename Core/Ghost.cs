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
    public State ghostState;
    //Movement Variables
    [Header("Movement")]
    [SerializeField] public float ghostSpeed;
    [SerializeField] private float moveTime=0.5f;
    private Graph graph;
    private Vector3 direction;
    private SpriteRenderer spriteRenderer;
    private Sprite chaseSprite;
    public Node currentNode;
    public List<Node> nodePath;
    public bool reachedPathEnd;

    public void Init(string name,Node currentNode,Graph graph)
    {
        this.ghostName=name;
        this.currentNode=currentNode;
        this.graph=graph;
        ghostState=State.Chase;
        reachedPathEnd=true;
        spriteRenderer=gameObject.GetComponent<SpriteRenderer>();
        chaseSprite=spriteRenderer.sprite;
    }

    public void Move(Node goalNode)
    {
        StartCoroutine(MoveToNodeRoutine(goalNode));
    }
    //Coroutine for smooth movement interpolation between nodes
    IEnumerator MoveToNodeRoutine(Node goalNode)
    {
        float elapsedTime=0;
        int breakIterations=0;
        moveTime=Mathf.Clamp(moveTime,0.1f,5f);
        while(elapsedTime<moveTime && goalNode!=null && !HasReachedNode(goalNode))
        {
            elapsedTime+=Time.deltaTime;
            float lerpValue=Mathf.Clamp(elapsedTime/moveTime,0f,1f);
            Vector3 targetPos=goalNode.position;
            transform.position=Vector3.Lerp(currentNode.position,targetPos,lerpValue);
            //if over halfway change parent to next node
            if(lerpValue>0.51f)
            {
               currentNode=goalNode;
            }
            breakIterations++;
            if(breakIterations>100)
            {
                break;
            }
            yield return null;
        }        
    }

    public bool HasReachedNode(Node node)
    {
        float distanceSqr=(node.position-transform.position).sqrMagnitude;
        return(distanceSqr<0.01f);
    }

    public void SwitchState(Sprite scatterSprite)
    {
        if(ghostState==State.Chase)
        {
            ghostState=State.Scatter;
            spriteRenderer.sprite=scatterSprite;
        }
        else
        {
            ghostState=State.Chase;
            spriteRenderer.sprite=chaseSprite;
        }

    }
    void OnTriggerEnter2D(Collider2D other)
    {
    
        //TODO
    }
}
