using System;
using System.Linq; 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeData : MonoBehaviour
{
    [SerializeField] public int width=0;
    [SerializeField] public int height=0;
    [SerializeField] public TextAsset textAsset;
    
    public  void SetDimensions(List<string> textLines)
    {
        height=textLines.Count;
        foreach(string line in textLines)
        {
            if(line.Length>width)
            {
                width=line.Length;
            }
        }

    }

    public int[,] MakeMap()
    {
        List<string> lines=GetTextFromFile();
        SetDimensions(lines);
        int[,] mazeData= new int[width,height];
        for(int y=0;y<height;y++)
        {
            for(int x=0;x<width;x++)
            {
                if(lines[y].Length>x)
                {
                    mazeData[x,y]=(int)Char.GetNumericValue(lines[y][x]);
                }
             
            }
        }
        return mazeData;


    }
    public List<string> GetTextFromFile(TextAsset textAsset)
    {
        List<string> lines=new List<string>();
        if(textAsset!=null)
        {
            string textData=textAsset.text;
            string[] delimeters={"\r\n","\n"};
            lines.AddRange(textData.Split(delimeters,System.StringSplitOptions.None));
            lines.Reverse();
        }
        else
        {
            Debug.LogWarning("MAZEDATA GetTextFromFile Error: Invalid TextAsset");
        }
        return lines;

    }
    //Overload
    public List<string> GetTextFromFile()
    {
        return GetTextFromFile(textAsset);
    }

}
