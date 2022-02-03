using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ASPGenerator : MonoBehaviour
{
    [SerializeField] protected Clingo.ClingoSolver solver;
    [SerializeField] protected Map.Map map;
    [SerializeField] protected Map.MapKey mapKey;
    protected bool waitingOnClingo;

    private void Start()
    {
        //startJob();
        startGenerator();
    }
    // Update is called once per frame
    void Update()
    {
        if (waitingOnClingo)
        {
            if (solver.SolverStatus == Clingo.ClingoSolver.Status.SATISFIABLE)
            {
                //map.DisplayMap(Solver.answerSet,mapKey);
                SATISFIABLE();
                waitingOnClingo = false;
            }
            else if (solver.SolverStatus == Clingo.ClingoSolver.Status.UNSATISFIABLE)
            {
                
                UNSATISFIABLE();
                waitingOnClingo = false;
            }
            else if (solver.SolverStatus == Clingo.ClingoSolver.Status.ERROR)
            {
                
                ERROR();
                waitingOnClingo = false;
            }
            else if (solver.SolverStatus == Clingo.ClingoSolver.Status.TIMEDOUT)
            {
                
                TIMEDOUT();
                waitingOnClingo = false;
            }
        }
    }

    void startJob()
    {
        string filename = Clingo.ClingoUtil.CreateFile(aspCode);
        solver.Solve(filename);
        waitingOnClingo = true;
    }

    void startJob(string clingoArguments)
    {
        string filename = Clingo.ClingoUtil.CreateFile(aspCode);
        solver.Solve(filename, clingoArguments);
        waitingOnClingo = true;
    }

    void startJob<T>(ASPMemory<T> memory)
    {
        string filename = Clingo.ClingoUtil.CreateFile(aspCode + memory.ASPCode);
        solver.Solve(filename);
        waitingOnClingo = true;
    }

    void startJob<T>(string clingoArguments, ASPMemory<T> memory)
    {
        string filename = Clingo.ClingoUtil.CreateFile(aspCode + memory.ASPCode);
        solver.Solve(filename, clingoArguments);
        waitingOnClingo = true;
    }

    private string aspCode { get { return getASPCode(); } }
    
    virtual protected string getASPCode()
    {
        string aspCode = @"

        
        #const max_width = 8.
        #const max_height = 8.

        width(1..max_width).
        height(1..max_height).

        tile_type(filled;empty).

        1{tile(XX, YY, Type): tile_type(Type)}1 :- width(XX), height(YY).

        :- tile(X1,Y1, filled), tile(X2,Y2,filled), X1 == X2, Y1 != Y2.
        :- tile(X1,Y1, filled), tile(X2,Y2,filled), Y1 == Y2, X1 != X2.

        :- Count = {tile(_,_,filled)}, Count != max_width.
  
        :- tile(X1,Y1, filled), tile(X2,Y2,filled), X1 == X2 + Offset, Y1 == Y2 + Offset, width(Offset).
        :- tile(X1,Y1, filled), tile(X2,Y2,filled), X1 == X2 + Offset, Y1 == Y2 - Offset, width(Offset).


        ";

        return aspCode;
    }

    virtual protected void initializeGenerator()
    {

    }

    virtual protected void startGenerator()
    {
        string filename = Clingo.ClingoUtil.CreateFile(aspCode);
        solver.Solve(filename);
        waitingOnClingo = true;
    }

    virtual protected void finalizeGenerator()
    {

    }

    virtual protected void SATISFIABLE()
    {
        map.DisplayMap(solver.answerSet, mapKey);
        Debug.LogWarning("SATISFIABLE unimplemented");
    }

    virtual protected void UNSATISFIABLE()
    {
        Debug.LogWarning("UNSATISFIABLE unimplemented");
    }

    virtual protected void TIMEDOUT()
    {
        Debug.LogWarning("TIMEDOUT unimplemented");
    }

    virtual protected void ERROR()
    {
        Debug.LogWarning("ERROR unimplemented");
    }
}
