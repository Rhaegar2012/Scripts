using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Maze Initalization")]
    [SerializeField] public MazeData mazeData;
    [SerializeField] public Graph graph;
    [SerializeField] public GraphView graphView;
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
    private GameObject pacManObject;
    private Pacman pacMan;
    private List<GameObject> ghosts;
    private List<Ghost> ghostScripts;
    private Ghost blinky;
    private Ghost clyde;
    private Ghost inky;
    private Ghost pinky;
    private List<Node> path;
    private List<Node> clydePath;
    private List<Node> inkyPath;
    private List<Node> pinkyPath;
    private Node chaseNode;
    private Node clydeChaseNode;
    private Node inkyChaseNode;
    private Node pinkyChaseNode;
    private bool goalReached=true;
    private bool clydeGoalReached=true;
    private bool inkyGoalReached=true;
    private bool pinkyGoalReached=true;
    private string[] ghostNames={"Blinky","Clyde","Inky","Pinky"};


    // Start is called before the first frame update
    void Start()
    {
        CreateMaze();
        DrawPacMan();
        DrawGhosts();
    
        
    }

    // Update is called once per frame
    void Update()
    {
        if(pacMan)
        {
            PlayerInput();
            PacManMove(movementDirection, pacMan.currentNode);
            
        }
        GhostPaths();
        MoveGhosts(path,blinky,goalReached);
        MoveGhosts(inkyPath,inky,inkyGoalReached);
        MoveGhosts(clydePath,clyde,clydeGoalReached);
        MoveGhosts(pinkyPath,pinky,pinkyGoalReached);

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
        ghosts= new List<GameObject>();
        ghostScripts=new List<Ghost>();
        
        for(int i=0;i<ghostPrefabs.Length;i++)
        {
            GameObject instance =InstantiateGameObject(ghostStartNodes[i],ghostPrefabs[i]);
            Ghost ghost = instance.GetComponent<Ghost>();
            Node startNode=graph.nodes[(int)ghostStartNodes[i].x,(int)ghostStartNodes[i].y];
            ghost.Init(ghostNames[i],startNode,graph);
            ghosts.Add(instance);
            ghostScripts.Add(ghost);
        }

        blinky=ghosts[0].GetComponent<Ghost>();
        clyde=ghosts[1].GetComponent<Ghost>();
        inky=ghosts[2].GetComponent<Ghost>();
        pinky=ghosts[3].GetComponent<Ghost>();
        
    
    }
    GameObject InstantiateGameObject(Vector3 nodePosition,GameObject gameObjectPrefab)
    {
         GameObject instance=Instantiate(gameObjectPrefab,nodePosition,Quaternion.identity);
         return instance;
    }
    Node GetChaseNode(string name)
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
            PacManMove(movementDirection,switchNode);
        }
    }
    void GhostPaths()
    {
        if(goalReached)
        {
            Debug.Log("Blinky updates path");
            chaseNode=GetChaseNode(blinky.name);
            path=CalculatePath(blinky.currentNode,chaseNode);
            goalReached=false;
        }
        if(pinkyGoalReached)
        {
            Debug.Log("Inky updates path");
            chaseNode=GetChaseNode(pinky.name);
            pinkyPath=CalculatePath(inky.currentNode,chaseNode);
            pinkyGoalReached=false;
        }
        if(inkyGoalReached)
        {
            Debug.Log("Pinky updates path");
            chaseNode=GetChaseNode(pinky.name);
            inkyPath=CalculatePath(pinky.currentNode,chaseNode);
            inkyGoalReached=false;
        }
        if(clydeGoalReached)
        {
            Debug.Log("Clyde updates path");
            chaseNode=GetChaseNode(clyde.name);
            clydePath=CalculatePath(clyde.currentNode,chaseNode);
            clydeGoalReached=false;
        }


    }
    void MoveGhosts(List<Node> path,Ghost ghost,bool goal)
    {
        if(path!=null)
        {
             //Debug.Log($"Ghost current node (movement){blinky.currentNode.position}");
             int pathIndex=path.IndexOf(ghost.currentNode);
             if(pathIndex==path.Count-1)
             {
                 goal=true;
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
       //Debug.Log($"Start Node {startNode.position}");
       //Debug.Log($"Target Node {targetNode.position}");
       pathfinder.Init(graph,startNode,targetNode);
       List<Node> nodePath=pathfinder.SearchRoutine();
       //Debug.Log($"Path Length {nodePath.Count}");
       return nodePath;
    }
       
       
        
    



}
