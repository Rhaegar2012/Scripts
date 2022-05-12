using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeView : MonoBehaviour
{
    [SerializeField] public GameObject tile;
    [Range(0,0.5f)]
    [SerializeField] public float borderSize=0.15f;
    public void Init(Node node)
    {
        if(tile!=null)
        {
            gameObject.name="Node("+node.xIndex+","+node.yIndex+")";
            gameObject.transform.position=node.position;
            tile.transform.localScale=new Vector3(1f-borderSize,1f,1f-borderSize);
        }
    }
    
}
