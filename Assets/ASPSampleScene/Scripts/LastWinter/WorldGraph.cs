using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGraph 
{
    public int[] room;
    public int[] width;
    public int[] height;
    public Door[] door;
}

public class Door
{
    int[] rooms;
}