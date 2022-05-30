using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graph : MonoBehaviour
{
    
    private int mapWidth;
    private int mapHeight;
    public int width{get  {return mapWidth;}}
    public int height{get {return mapHeight;}}
    public Node[,] nodes;
    public List<Node> floor= new List<Node>();
    List<Node> walls= new List<Node>();
    private int[,] m_mazeData;
    //NodeDirections
    public static readonly Vector2[] allDirections=
    {
        new Vector2(-1f,0f),
        new Vector2(1f,0f),
        new Vector2(0f,1f),
        new Vector2(0f,-1f),
    };
    //GraphMethods
    //Constructs graphs with nodes and node relationships
    public void Init(int[,] mazeData)
    {
        m_mazeData=mazeData;
        mapWidth=m_mazeData.GetLength(0);
        mapHeight=m_mazeData.GetLength(1);
        nodes=new Node[mapWidth,mapHeight];
        for(int y=0;y<mapHeight;y++)
        {
            for(int x=0;x<mapWidth;x++)
            {
                NodeType type = (NodeType)m_mazeData[x,y];
                Node newNode= new Node(x,y,type);
                newNode.position=new Vector3(x,y,0f);
                nodes[x,y]= newNode;
                if(type==NodeType.Blocked)
                {
                    walls.Add(newNode);
                }
                else if(type==NodeType.Open)
                {
                    floor.Add(newNode);
                }
            }
        }
        for(int y=0;y<mapHeight;y++)
        {
            for(int x=0;x<mapWidth;x++)
            {
                nodes[x,y].neighbors=GetNeighbors(x,y);
            }
        }
    }
    public bool IsWithinBounds(int x,int y)
    {
        return (x>=0&&x<mapWidth&&y>=0&&y<mapHeight);
    }
    public bool IsValidNode(Vector2 direction, Node currentNode)
    {
        int newX= currentNode.xIndex+(int)direction.x;
        int newY= currentNode.yIndex+(int)direction.y;
        return(IsWithinBounds(newX,newY)&&nodes[newX,newY]!=null&&
        nodes[newX,newY].nodeType!=NodeType.Blocked);
    }
    public Node GetNewNode(Vector2 direction, Node currentNode)
    {
        int newX= currentNode.xIndex+(int)direction.x;
        int newY= currentNode.yIndex+(int)direction.y;
        return nodes[newX,newY];
    }
    public List<Node> GetNeighbors(int x, int y, Node[,] nodeArray,Vector2[] directions)
    {
        List<Node> neighbors= new List<Node>();
        Node node=nodeArray[x,y];
        foreach(Vector2 dir in directions)
        {
            
            int xIndex=x+(int)dir.x;
            int yIndex=y+(int)dir.y;
            if(IsWithinBounds(xIndex,yIndex)&&nodeArray[xIndex,yIndex]!=null&&
            nodeArray[xIndex,yIndex].nodeType!=NodeType.Blocked)
            {
                neighbors.Add(nodeArray[xIndex,yIndex]);
            }
            
        }
        return neighbors;
    }
    //Overload
    public List<Node> GetNeighbors(int x, int y)
    {
        return GetNeighbors(x,y,nodes,allDirections);
    }
    public float GetNodeDistance(Node source, Node target)
    {
        int dx=Mathf.Abs(source.xIndex-target.xIndex);
        int dy=Mathf.Abs(source.yIndex-target.yIndex);
        int min=Mathf.Min(dx,dy);
        int max=Mathf.Max(dx,dy);
        int diagonalSteps=min;
        int straightSteps=max-min;
        return(1.4f*diagonalSteps+straightSteps);
    }
   


    

}
