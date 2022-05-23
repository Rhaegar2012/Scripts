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
    private List<Node> path;
    private string[] ghostNames={"Blinky","Clyde","Inky","Pinky"};


    
    

    // Start is called before the first frame update
    void Start()
    {
        CreateMaze();
        DrawPacMan();
        DrawGhosts();
        path= new List<Node>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if(pacMan)
        {
            PlayerInput();
            PacManMove(movementDirection, pacMan.currentNode);
            
        }
        if(ghosts!=null)
        {
            MoveGhosts();
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
        ghosts= new List<GameObject>();
        
        for(int i=0;i<ghostPrefabs.Length;i++)
        {
            GameObject instance =InstantiateGameObject(ghostStartNodes[i],ghostPrefabs[i]);
            Ghost ghost = instance.GetComponent<Ghost>();
            Node startNode=graph.nodes[(int)ghostStartNodes[i].x,(int)ghostStartNodes[i].y];
            ghost.Init(ghostNames[i],startNode,graph);
            ghosts.Add(instance);

        }
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
            pacMan.currentPath.Clear();
            PacManMove(movementDirection,switchNode);
        }
    }

    void MoveGhosts()
    {
        GameObject instance=ghosts[0];
        Ghost ghost=instance.GetComponent<Ghost>();
        
        
        //Updates path if pacman moved
        
        path=CalculatePath(ghost.currentNode,pacMan.currentNode);
        pacmanStart=pacMan.currentNode;
        int  pathIndex=path.IndexOf(ghost.currentNode);
        Debug.Log($"Index {pathIndex} ");
        if(pathIndex<path.Count-1)
        {
            Node goalNode=path[pathIndex+1];
            ghost.Move(goalNode);
        }
    
        
        
      
    }
    List<Node> CalculatePath(Node startNode,Node targetNode )
    {
      
       pathfinder.Init(graph,startNode,targetNode);
       List<Node> nodePath=pathfinder.SearchRoutine();
       return nodePath;
    }
       
       
        
    



}
