using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGraph 
{
    public Door[] doors;
    public Key[] keys;
    public Gate[] gates;
    public Gated[] gatedRooms;
    public int startRoomID;
    public int width;
    public int height;
    public BossRoom bossRoom;

    public WorldGraph(Clingo.AnswerSet world)
    {
        startRoomID = int.Parse(world.Value["start"][0][0]);
        width = UtilityASP.GetMaxInt(world.Value["width"]);
        height = UtilityASP.GetMaxInt(world.Value["height"]);

        List<Door> doors = new List<Door>();
        foreach (List<string> door in world.Value["door"])
        {
            Door newDoor = new Door();
            newDoor.name = door[0] + "->" + door[1];
            newDoor.source = int.Parse(door[0]);
            newDoor.destination = int.Parse(door[1]);
            doors.Add(newDoor);
        }
        this.doors = UtilityASP.GetArray<Door>(doors);

        List<Key> keys = new List<Key>();
        foreach (List<string> key in world.Value["key"])
        {
            Key newKey = new Key();
            newKey.type = int.Parse(key[0]);
            newKey.roomID = int.Parse(key[1]);
            keys.Add(newKey);
        }
        this.keys = UtilityASP.GetArray(keys);

        List<Gate> gates = new List<Gate>();
        foreach (List<string> gate in world.Value["gate"])
        {
            Gate newGate = new Gate();
            newGate.type = int.Parse(gate[0]);
            newGate.source = int.Parse(gate[1]);
            newGate.destination = int.Parse(gate[2]);
            gates.Add(newGate);
        }
        this.gates = UtilityASP.GetArray(gates);

        List<int> bossRooms = new List<int>();
        foreach (List<string> bossRoom in world.Value["boss_room"])
        {
            bossRooms.Add(int.Parse(bossRoom[0]));
        }
        bossRoom = new BossRoom();
        bossRoom.bossRooms = UtilityASP.GetArray(bossRooms);
        bossRoom.bossEntranceRoom = int.Parse(world.Value["boss_room_entrance"][0][0]);
        bossRoom.bossStartRoom = int.Parse(world.Value["boss_room_start"][0][0]);

        List<Gated> gatedRooms = new List<Gated>();
        //if (world.ContainsKey("gated_room"))
        //{
            foreach (List<string> gated in world.Value["gated_room"])
            {
                Gated gatedRoom = new Gated();
                gatedRoom.roomID = int.Parse(gated[0]);
                gatedRoom.type = int.Parse(gated[1]);
                gatedRooms.Add(gatedRoom);
            }
        //}
        this.gatedRooms = UtilityASP.GetArray(gatedRooms);
    }
}

[System.Serializable]
public class BossRoom
{
    public int[] bossRooms;
    public int bossEntranceRoom;
    public int bossStartRoom;
}

[System.Serializable]
public class Door
{
    public string name; //{ get { return source + "->" + destination; } }
    public int source;
    public int destination;
}

[System.Serializable]
public class Key
{
    public int type;
    public int roomID;
}

[System.Serializable]
public class Gate
{

    public int type;
    public int source;
    public int destination;
}

[System.Serializable]
public class Gated
{
    public int roomID;
    public int type;
}