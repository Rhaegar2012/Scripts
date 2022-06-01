using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    [Header("Maze Initalization")]
    [SerializeField] public MazeData mazeData;
    [SerializeField] public Graph graph;
    [SerializeField] public GraphView graphView;
    [Header("Power ups")]
    [SerializeField] public GameObject pelletPrefab;
    [SerializeField] public GameObject pillPrefab;
    [SerializeField] Vector3[] pillStartNodes;
    [Header("Pacman Initialization")]
    [SerializeField] GameObject pacmanPrefab;
    [SerializeField] Vector3 pacmanStartNode;
    [SerializeField] Vector3 movementDirection;
    private  Node  pacmanStart;
    [Header("Ghost Initialization")]
    [SerializeField] GameObject[] ghostPrefabs;
    [SerializeField] Vector3[] ghostStartNodes;
    [Header("Pathfinder")]
    [SerializeField] Pathfinder pathfinder;
    [Header("Scatter")]
    [SerializeField] float scatterTime;
    [SerializeField] Sprite scatterSprite;
    [Header("Respawn")]
    [SerializeField] float ghostRespawnTime;
    [Header("Score values")]
    [SerializeField] int pelletScore;
    [SerializeField] int pillScore;
    [SerializeField] int ghostScore;
    private GameObject pacManObject;
    private Pacman pacMan;
    private List<Ghost> ghosts;
    private List<GameObject> pellets;
    private string[] ghostNames={"Blinky","Clyde","Inky","Pinky"};
    private int score=0;
    //Observer Events
    void OnEnable()
    {
        Pellet.OnPelletEaten+=EatPellet;
        Pill.OnPillEaten+=EatPill;
        //Ghost.OnGhostCaptured+=GhostRespawn;
    }
    void OnDisable()
    {
        Pellet.OnPelletEaten-=EatPellet;
        Pill.OnPillEaten-=EatPill;
        //Ghost.OnGhostCaptured-=GhostRespawn;
    }

    // Start is called before the first frame update
    void Start()
    {
        CreateMaze();
        DrawPacMan();
        DrawGhosts();
        DrawPellets();
        DrawPills();
    
    }

    // Update is called once per frame
    void Update()
    {
        if(pacMan)
        {
            PlayerInput();
            PacManMove(movementDirection, pacMan.currentNode);
            
        }
        foreach(Ghost ghost in ghosts)
        {
            if(ghost!=null)
            {
                ghost.nodePath= GhostPath(ghost);
                MoveGhost(ghost.nodePath,ghost);
            }
          
        }
     
    }
    //Draw GameObjects
    void CreateMaze()
    {
        if(mazeData!=null&&graph!=null)
        {
            int[,] maze=mazeData.MakeMap();
            graph.Init(maze);
        }
        if(graphView!=null)
        {
            graphView.Init(graph);
        }
    }
    void DrawPacMan()
    {
        if(pacmanPrefab)
        {
            pacManObject=InstantiateGameObject(pacmanStartNode,pacmanPrefab);
            pacMan=pacManObject.GetComponent<Pacman>();
            pacMan.Init(graph.nodes[(int)pacmanStartNode.x,(int)pacmanStartNode.y],graph);
        }
        pacmanStart=graph.nodes[(int)pacmanStartNode.x,(int)pacmanStartNode.y];
    }
    void DrawGhosts()
    {
        ghosts= new List<Ghost>();
        
        for(int i=0;i<ghostPrefabs.Length;i++)
        {
            GameObject instance =InstantiateGameObject(ghostStartNodes[i],ghostPrefabs[i]);
            Ghost ghost = instance.GetComponent<Ghost>();
            Node startNode=graph.nodes[(int)ghostStartNodes[i].x,(int)ghostStartNodes[i].y];
            ghost.Init(ghostNames[i],startNode,graph);
            ghosts.Add(ghost);
        }
   
    
    }
    void DrawPellets()
    {
        pellets=new List<GameObject>();
        foreach(Node node in graph.floor)
        {
            GameObject instance=InstantiateGameObject(node.position,pelletPrefab);
            pellets.Add(instance);
        }
    }
    void DrawPills()
    {
        for(int i=0;i<pillStartNodes.Length;i++)
        {
            GameObject instance= InstantiateGameObject(pillStartNodes[i],pillPrefab);
        }

    }
    GameObject InstantiateGameObject(Vector3 nodePosition,GameObject gameObjectPrefab)
    {
         GameObject instance=Instantiate(gameObjectPrefab,nodePosition,Quaternion.identity);
         return instance;
    }
    Node GetChaseNode(string name,State state)
    {
        if(state==State.Chase)
        {
            if (name=="Blinky")
            {
                return pacMan.currentNode;
            }
            if(name=="Clyde")
            {
                return pacMan.currentNode;
            }
            if(name=="Inky")
            {
                return pacMan.currentNode;
            }
            if(name=="Pinky")
            {
                return pacMan.currentNode;
            }

        }
        else
        {
            var random= new System.Random();
            int index=random.Next(graph.floor.Count);
            return graph.floor[index];

        }
        return pacMan.currentNode;
        
    }
    void PlayerInput()
    {
        if(Input.GetButtonDown("MoveLeft"))
        {
            SwitchDirection("left");
            
        }
        if(Input.GetButton("MoveRight"))
        {
            SwitchDirection("right");
            
        }
        if(Input.GetButton("MoveUp"))
        {
            SwitchDirection("up");
        }
        if(Input.GetButton("MoveDown"))
        {
            SwitchDirection("down");
        }
         

    }

    void PacManMove(Vector2 direction,Node node)
    {
        if(!graph.IsValidNode(direction,node))
        {
            return;
        }
        else
        {
            Node newNode= graph.GetNewNode(direction,node);
            pacMan.Move(newNode,direction);
        }

    }

    
    void SwitchDirection(string direction)
    {
        Vector3 trialDirection= new();
        switch(direction)
        {
            case "right":
            trialDirection=new Vector3(1,0,0);
            break;
            case "left":
            trialDirection=new Vector3(-1,0,0);
            break;
            case "down":
            trialDirection=new Vector3(0,-1,0);
            break;
            case "up":
            trialDirection= new Vector3(0,1,0);
            break;
        }
        int pacManX=(int) pacMan.transform.position.x;
        int pacManY=(int) pacMan.transform.position.y;
        Node switchNode =graph.nodes[pacManX,pacManY];
        if(graph.IsValidNode(trialDirection,switchNode))
        {
            movementDirection=trialDirection;
            pacMan.currentPath.Clear();
            PacManMove(movementDirection,switchNode);
        }
    }
    List<Node> GhostPath(Ghost ghost)
    {
        List<Node> path= new List<Node>();
        if(ghost.reachedPathEnd)
        {
            Node chaseNode=GetChaseNode(ghost.name,ghost.ghostState);
            path=CalculatePath(ghost.currentNode,chaseNode);
            ghost.reachedPathEnd=false;
            return path;
        }
        else
        {
            return ghost.nodePath;

        }
        
    }
    void MoveGhost(List<Node> path,Ghost ghost)
    {
        if(path!=null)
        {
             int pathIndex=path.IndexOf(ghost.currentNode);
             if(pathIndex==path.Count-1)
             {
                 ghost.reachedPathEnd=true;
             }
             else
             {
                Node goalNode=path[pathIndex+1];
                ghost.Move(goalNode);    
             }
        }
    }
    List<Node> CalculatePath(Node startNode,Node targetNode )
    {
       pathfinder.Init(graph,startNode,targetNode);
       List<Node> nodePath=pathfinder.SearchRoutine();
       return nodePath;
    }
    void EatPellet()
    {
        score+=pelletScore;
    
    }
    void EatPill()
    {
        score+=pillScore;
        ScatterGhosts();
    }
    void ScatterGhosts()
    {
        
        //Switch all ghosts to scatter mode and update sprites 
        foreach(Ghost ghost in ghosts)
        { 
            if(ghost.ghostState==State.Chase)
            {
                if(ghost!=null)
                {
                    ghost.SwitchState(scatterSprite);
                }
                
            }
            
        }
        //Moves the ghosts during the scatter time 
        StartCoroutine(ScatterCoroutine(scatterTime));
    }
    IEnumerator ScatterCoroutine(float scatterTime)
    {
        float elapsedTime=0f;
        scatterTime=Mathf.Clamp(scatterTime,0.1f,7f);
        while(elapsedTime<scatterTime)
        {
            elapsedTime+=Time.deltaTime;
            yield return null;
        }
        //Switch ghosts back to chase state 
        foreach(Ghost ghost in ghosts)
        {
            if(ghost!=null)
            {
                ghost.SwitchState(scatterSprite);
            }
            
        }
    }

    void GhostRespawn()
    {
        foreach(Ghost ghost in ghosts)
        {
            if(ghost.isCaptured)
            {
                StartCoroutine(GhostRespawnCoroutine(ghostRespawnTime));
                ghost.Respawn();
            }
        }
    }
    IEnumerator GhostRespawnCoroutine(float ghostSpawnTime)
    {
       yield return new WaitForSeconds(ghostSpawnTime);
    }

       
       
        
    



}
