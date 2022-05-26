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
    private Ghost blinky;
    private Ghost clyde;
    private Ghost inky;
    private Ghost pinky;
    private List<Node> path;
    private Node chaseNode;
    private bool goalReached=true;
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
        if(goalReached)
        {
            Debug.Log("Accessed");
            chaseNode=pacMan.currentNode;
            path=CalculatePath(blinky.currentNode,chaseNode);
            goalReached=false;
        }
        MoveGhosts(path);
        
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
        
        for(int i=0;i<ghostPrefabs.Length;i++)
        {
            GameObject instance =InstantiateGameObject(ghostStartNodes[i],ghostPrefabs[i]);
            Ghost ghost = instance.GetComponent<Ghost>();
            Node startNode=graph.nodes[(int)ghostStartNodes[i].x,(int)ghostStartNodes[i].y];
            ghost.Init(ghostNames[i],startNode,graph);
            ghosts.Add(instance);

        }

        blinky=ghosts[0].GetComponent<Ghost>();
        clyde=ghosts[1].GetComponent<Ghost>();
        inky=ghosts[2].GetComponent<Ghost>();
        pinky=ghosts[3].GetComponent<Ghost>();
        //chaseNode=graph.nodes[(int)pacmanStartNode.x,(int)pacmanStartNode.y];
    
    }
    GameObject InstantiateGameObject(Vector3 nodePosition,GameObject gameObjectPrefab)
    {
         GameObject instance=Instantiate(gameObjectPrefab,nodePosition,Quaternion.identity);
         return instance;
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
            Debug.Log("PACMAN SWTICHES DIRECTION");
            //PacManMove(movementDirection,switchNode);
        }
    }

    void MoveGhosts(List<Node> path)
    {
        if(path!=null)
        {
             //Debug.Log($"Ghost current node (movement){blinky.currentNode.position}");
             int pathIndex=path.IndexOf(blinky.currentNode);
             if(pathIndex==path.Count-1)
             {
                 Debug.Log("END OF PATH REACHED");
                 goalReached=true;
             }
             else
             {
          
                Node goalNode=path[pathIndex+1];
                blinky.Move(goalNode);
                
             }
         
        }


      
    }
    List<Node> CalculatePath(Node startNode,Node targetNode )
    {
       Debug.Log($"Start Node {startNode.position}");
       Debug.Log($"Target Node {targetNode.position}");
       pathfinder.Init(graph,startNode,targetNode);
       List<Node> nodePath=pathfinder.SearchRoutine();
       Debug.Log($"Path Length {nodePath.Count}");
       return nodePath;
    }
       
       
        
    



}
