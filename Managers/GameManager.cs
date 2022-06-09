using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Maze Initalization")]
    [SerializeField] public MazeData mazeData;
    [SerializeField] public Graph graph;
    [SerializeField] public GraphView graphView;
    [Header("Portals")]
    [SerializeField] GameObject leftPortal;
    [SerializeField] GameObject rightPortal;
    [Header("Power ups")]
    [SerializeField] public GameObject pelletPrefab;
    [SerializeField] public GameObject pillPrefab;
    [SerializeField] Vector3[] pillStartNodes;
    [Header("Pacman Initialization")]
    [SerializeField] GameObject pacmanPrefab;
    [SerializeField] Vector3 pacmanStartNode;
    [SerializeField] Vector3 movementDirection;
    [SerializeField] int pacmanLives;
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
    [Header("Waiting times")]
    [SerializeField] float startTime;
    [SerializeField] float restartTime;
    [Header("Sound")]
    [SerializeField] SoundManager soundManager;
    [Header("UI Components")]
    [SerializeField] TextMeshProUGUI pacmanLivesText;
    [SerializeField] TextMeshProUGUI pacmanScoreText;

    private GameObject pacManObject;
    private Pacman pacMan;
    private List<Ghost> ghosts;
    private List<GameObject> pellets;
    private Node rightPortalNode;
    private Node leftPortalNode;
    private string[] ghostNames={"Blinky","Clyde","Inky","Pinky"};
    private int score=0;
    //Observer Events
    void OnEnable()
    {
        Pellet.OnPelletEaten+=EatPellet;
        Pill.OnPillEaten+=EatPill;
        Ghost.OnGhostCaptured+=GhostRespawn;
        Pacman.OnPacmanDeath+=PacmanDeath;
        Pacman.OnPacmanTeleport+=PacmanTeleport;
    }
    void OnDisable()
    {
        Pellet.OnPelletEaten-=EatPellet;
        Pill.OnPillEaten-=EatPill;
        Ghost.OnGhostCaptured-=GhostRespawn;
        Pacman.OnPacmanDeath-=PacmanDeath;
        Pacman.OnPacmanTeleport-=PacmanTeleport;
    }
  
    // Start is called before the first frame update
    void Start()
    {
       
        CreateMaze();
        StartCoroutine(StartSequence());

      
    }

    // Update is called once per frame
    void Update()
    {
        if(pacMan && pacmanLives>0)
        {
            PlayerInput();
            PacManMove(movementDirection, pacMan.currentNode);
            
        }
        if(ghosts!=null)
        {
            foreach(Ghost ghost in ghosts)
            {
                if(ghost!=null)
                {
                    ghost.nodePath= GhostPath(ghost);
                    MoveGhost(ghost.nodePath,ghost);
                }   
            }
        }
   
     
    }
    //Start Sequence
    IEnumerator StartSequence()
    {
        soundManager.PlayClip(soundManager.introClip);
        yield return new WaitForSeconds(startTime);
        DrawPacMan();
        DrawGhosts();
        DrawPellets();
        DrawPills();
        InitializePortalNodes();
   
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
    void InitializePortalNodes()
    {
        rightPortalNode=graph.nodes[(int)rightPortal.transform.position.x,(int)rightPortal.transform.position.y];
        leftPortalNode=graph.nodes[(int)leftPortal.transform.position.x,(int)leftPortal.transform.position.y];
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
                var random= new System.Random();
                int index=random.Next(graph.floor.Count);
                return graph.floor[index];
            }
            if(name=="Inky")
            {
    
                foreach(Ghost ghost in ghosts)
                {
                    if(ghost!=null)
                    {
                        if(ghost.ghostName=="Blinky")
                        {
                            Node blinkyNode=ghost.currentNode;
                            int distance= (int)Vector3.Distance(blinkyNode.position,pacMan.currentNode.position);
                            Vector2[] trialDirections= {
                                new Vector2(distance,0),
                                new Vector2(0,distance)
                            };
                            for(int i=0;i<trialDirections.Length;i++)
                            {
                                if(graph.IsValidNode(trialDirections[i],blinkyNode))
                                {
                                    return graph.GetNewNode(trialDirections[i],blinkyNode);
                                }
                            }
                            return pacMan.currentNode;
                        }
                    }
                }
                

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
        Vector3 scale= new Vector3(1,1,1);
        string rotation="";
        switch(direction)
        {
            case "right":
            trialDirection=new Vector3(1,0,0);
            scale=new Vector3(1,1,1);
            rotation="right";
            break;
            case "left":
            trialDirection=new Vector3(-1,0,0);
            scale= new Vector3(-1,1,1);
            rotation="left";
            break;
            case "down":
            trialDirection=new Vector3(0,-1,0);
            rotation="down";
            break;
            case "up":
            trialDirection= new Vector3(0,1,0);
            rotation="up";
            break;
        }
        int pacManX=(int) pacMan.transform.position.x;
        int pacManY=(int) pacMan.transform.position.y;
        Node switchNode =graph.nodes[pacManX,pacManY];
        if(graph.IsValidNode(trialDirection,switchNode))
        {
            movementDirection=trialDirection;
            pacMan.currentPath.Clear();
            pacMan.transform.localScale=scale;
            RotatePacman(rotation);
            PacManMove(movementDirection,switchNode);
        }
    }
    void RotatePacman(string rotation)
    {
        float localAngle=pacMan.transform.localEulerAngles.z;
        float rotationAngle=0f;
        if(localAngle==0 && rotation=="up")
        {
            rotationAngle=90f;
        }
        else if(localAngle==270 && rotation =="up")
        {
            rotationAngle=180f;
        }
        if(localAngle==0 && rotation=="down")
        {
            rotationAngle=-90f;
        }
        else if(localAngle==90 &&rotation=="down")
        {
            rotationAngle=-180f;
        }
        else if (rotation=="left" || rotation=="right")
        {
            pacMan.transform.rotation=Quaternion.identity;
        }
        pacMan.transform.Rotate(0,0,rotationAngle,Space.Self);
    }
    void PacmanDeath()
    {
        pacmanLives--;
        pacmanLivesText.text=pacmanLives.ToString();
        soundManager.PlayClip(soundManager.pacmanDeathClip);
        if(pacmanLives>0)
        {
           
            StartCoroutine(RestartSequence());

        }
    }
    IEnumerator RestartSequence()
    {
        foreach(Ghost ghost in ghosts)
        {
            if(ghost!=null)
            {
                Destroy(ghost.gameObject);
            }
          
        }
        yield return new WaitForSeconds(restartTime);
        DrawPacMan();
        DrawGhosts();
    }

    void PacmanTeleport()
    {
        
        Vector3 pacmanPosition= pacMan.currentNode.position;
        if(pacmanPosition==leftPortal.transform.position)
        {
        
            pacMan.currentNode=rightPortalNode;
            pacMan.transform.position=rightPortal.transform.position;
        }
        else if(pacmanPosition==rightPortal.transform.position)
        {
            
            pacMan.currentNode=leftPortalNode;
            pacMan.transform.position=leftPortal.transform.position;
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
        soundManager.PlayClip(soundManager.pacmanMunchClip);
        score+=pelletScore;
        pacmanScoreText.text=score.ToString();
    }
    void EatPill()
    {
        soundManager.PlayClip(soundManager.pacmanMunchClip);
        score+=pillScore;
        pacmanScoreText.text=score.ToString();
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
        soundManager.PlayClip(soundManager.intermissionClip);
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
        soundManager.PlayClip(soundManager.pacmanEatGhostClip);
        score+=ghostScore;
        pacmanScoreText.text=score.ToString();
        string name=pacMan.capturedGhost;
        int index= Array.IndexOf(ghostNames,name);
        StartCoroutine(GhostRespawnCoroutine(ghostRespawnTime,ghostPrefabs[index],ghostStartNodes[index],ghostNames[index])); 
    }
    IEnumerator GhostRespawnCoroutine(float ghostSpawnTime,GameObject prefab,Vector3 node,string name)
    {
         
       float timeElapsed=0f;
       ghostSpawnTime=Mathf.Clamp(ghostSpawnTime,0.1f,5f);
       while(timeElapsed<ghostSpawnTime)
       {
           timeElapsed+=Time.deltaTime;
           yield return null;
       }
       Node startNode=graph.nodes[(int)node.x,(int)node.y];
       GameObject instance=InstantiateGameObject(node,prefab);
       Ghost ghost = instance.GetComponent<Ghost>();
       ghost.Init(name,startNode,graph);
       if(ghost.ghostState==State.Scatter)
       {
           ghost.SwitchState(scatterSprite);
       }
       ghosts.Add(ghost);
     
    }

       
       
        
    



}
