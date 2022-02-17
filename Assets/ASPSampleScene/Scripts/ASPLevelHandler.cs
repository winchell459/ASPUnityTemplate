using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ASPLevelHandler : MonoBehaviour
{
    [SerializeField] private Map.MapKey mapKey;
    private GolfASP golfASP;
    //private GolfMoveFinder moveFinder;
    private bool waitingForASP;
    //public GameObject Player;
    public bool Ready { get { return !waitingForASP; } }

    private float tileSpacing { get { return FindObjectOfType<Map.MapBoard>().tileSpacing; } }

    static ASPMemory<MoveEvents> memory;
    [SerializeField] private ASPMemory<MoveEvents> _memory;
    public static void ClearMemory() { memory = null; }
    public GameObject LoadingScreen;
   
    // Start is called before the first frame update
    void Start()
    {
        LoadingScreen.SetActive(true);
        if (memory == null)
        {
            //???should move to parent class or to within ASPMemory???
            memory = new ASPMemory<MoveEvents>();
            
            memory.Events = new MoveEvents();
        }
        _memory = memory;

        golfASP = GetComponent<GolfASP>();
        
        addNewMemory();
        startGolfASP();
    }

    // Update is called once per frame
    void Update()
    {
        
        if(waitingForASP && golfASP.SolverDone)
        {
            FindObjectOfType<Map.Map>().DisplayMap(golfASP.answerSet, mapKey);
            FindObjectOfType<Map.Map>().AdjustCamera();
            
            waitingForASP = false;
            LoadingScreen.SetActive(false);
        }
    }
    
    public Vector2Int GetLastMove()
    {
        return getLastMove();
    }

    Vector2Int getLastMove()
    {
        Vector2Int previous, current;
        int turnCount = memory.Events.MovesPath[memory.Events.MovesPath.Count - 1].Moves.Count - 1;
        previous = memory.Events.MovesPath[memory.Events.MovesPath.Count - 1].Moves[turnCount - 1];
        current = memory.Events.MovesPath[memory.Events.MovesPath.Count - 1].Moves[turnCount];
        return new Vector2Int(current.x - previous.x, current.y - previous.y);
    }

    public void UndoMemory()
    {
        undoMemory();
    }
    void undoMemory()
    {
        int turnCount = memory.Events.MovesPath[memory.Events.MovesPath.Count - 1].Moves.Count - 1;
        memory.Events.MovesPath[memory.Events.MovesPath.Count - 1].Moves.RemoveAt(turnCount);
        memory.Events.MovesPath[memory.Events.MovesPath.Count - 1].Moves.RemoveAt(turnCount - 1);
    }

    void addNewMemory()
    {

        memory.Events.MovesPath.Add(new MoveEvent());
    }

    void addToMemory(Vector2Int nextPos)
    {
        memory.Events.MovesPath[memory.Events.MovesPath.Count - 1].Moves.Add(nextPos);
    }

    public void AddToMemory(Vector2Int nextPos)
    {
        addToMemory(nextPos);
    }

    

    public Vector2 GetTilePos(Vector2Int tileIndex)
    {
        return new Vector2(tileIndex.x * tileSpacing, tileIndex.y * tileSpacing);
    }
    private void startGolfASP()
    {
        int round = GameHandler.Round;
        int minJump = 1;
        int maxJump = 3;
        if(round < 5){

        }
        else if(round < 7)
        {
            minJump = 2;
            maxJump = 4;
        }else if(round < 12)
        {
            minJump = 3;
            maxJump = 6;
        }else if(round < 20)
        {
            minJump = 1;
            maxJump = 9;
        }
        else
        {
            maxJump = Random.Range(3, 10);
            minJump = Random.Range(1, maxJump);
        }
        waitingForASP = true;
        golfASP.StartJob(memory, minJump, maxJump);
    }

    public void GolfTileClicked(GolfTile tile)
    {
        FindObjectOfType<GameHandler>().GolfTileClicked(tile);
        

    }

   
}

[System.Serializable]
public class ASPMemory<T>
{
    public T Events;
    public float Weight;
    public string ASPCode;
    
}

[System.Serializable]
public class MoveEvents
{
    public List<MoveEvent> MovesPath;
    public MoveEvents()
    {
        MovesPath = new List<MoveEvent>();
    }

    public string GetMoves()
    {
        string aspMoves = "";
        foreach(MoveEvent moveEvent in MovesPath)
        {
            for(int i = 1; i < moveEvent.Moves.Count; i += 1)
            {
                Vector2Int start = moveEvent.Moves[i-1];
                Vector2Int end = moveEvent.Moves[i];
                aspMoves += $":- move({start.x + 1},{start.y + 1},{end.x + 1},{end.y + 1}). \n";
            }
        }
        return aspMoves;
    }
}
[System.Serializable]
public class MoveEvent
{
    public List<Vector2Int> Moves;
    public MoveEvent()
    {
        Moves = new List<Vector2Int>();
    }
}
