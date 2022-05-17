using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NodeType
{
    Open=0,
    Blocked=1
}
public class Node :IComparable<Node>
{
    public int xIndex=-1;
    public int yIndex=-1;
    public NodeType nodeType=NodeType.Open;
    public List<Node> neighbors=new List<Node>();
    public Vector3 position;
    public Node previous=null; 
    public int priority;
    public float distanceTraveled=Mathf.Infinity;
    //Constructor
    public Node(int xIndex,int yIndex,NodeType nodeType)
    {
        this.xIndex=xIndex;
        this.yIndex=yIndex;
        this.nodeType=nodeType;
    }
    public void Reset()
    {
        previous=null;
    }
    public int CompareTo(Node other)
    {
        if(this.priority<other.priority)
        {
            return -1;
        }
        else if(this.priority>other.priority)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }


}
