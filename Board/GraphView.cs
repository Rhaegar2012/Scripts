using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphView : MonoBehaviour
{
    [SerializeField] public GameObject floorTilePrefab;
    [SerializeField] public GameObject wallTilePrefab;
    public NodeView[,] nodeViews;
    public void Init(Graph graph)
    {
        if(graph==null)
        {
            Debug.LogWarning("GRAPHVIEW no graph to initialize");
        }
        nodeViews= new NodeView[graph.width,graph.height];
        foreach(Node n in graph.nodes)
        {
            if(n.nodeType==NodeType.Blocked)
            {
                Instantiate(wallTilePrefab,n.position,Quaternion.identity);
            }
            else
            {
                Instantiate(floorTilePrefab,n.position,Quaternion.identity);
            }
        }
    }
}
