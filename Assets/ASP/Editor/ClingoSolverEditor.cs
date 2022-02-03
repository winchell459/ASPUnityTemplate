﻿using UnityEditor;
using UnityEngine;
using Clingo;


public class ClingoSolverEditor : Editor
{
    string solutionOutput = "";

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        ClingoSolver solver = (ClingoSolver)target;

        EditorGUILayout.LabelField("Duration: ", solver.Duration.ToString());
        EditorGUILayout.LabelField("Seed: ", solver.Seed.ToString());
        EditorGUILayout.LabelField("Solutions Found: ", solver.SolutionsFound.ToString());
        EditorGUILayout.LabelField("More Solutions: ", solver.MoreSolutions.ToString());
        EditorGUILayout.LabelField("Is Solver Running: ", solver.IsSolverRunning.ToString());
        EditorGUILayout.LabelField("Status: ", solver.SolverStatus.ToString());

        if (solver.clingoThread != null)
        {
            EditorGUILayout.LabelField("Thread Alive: ", solver.clingoThread.IsAlive.ToString());
            EditorGUILayout.LabelField("Thread State: ", solver.clingoThread.ThreadState.ToString());
        }

        
        if (GUILayout.Button("Solve in Thread"))
        {
            solver.Solve();
        }

        solutionOutput = solver.SolutionOutput;

        EditorGUILayout.PrefixLabel("Solution");
        EditorGUILayout.TextArea(solutionOutput);

        EditorGUILayout.PrefixLabel("Raw Clingo Output");
        EditorGUILayout.TextArea(solver.ClingoConsoleOutput);

        EditorGUILayout.PrefixLabel("Raw Clingo Error Output");
        EditorGUILayout.TextArea(solver.ClingoConsoleError);
    }

}
[CustomEditor(typeof(ClingoLocal))]
public class ClingoLocalEditor : ClingoSolverEditor
{

}
[CustomEditor(typeof(ClingoOnline))]
public class ClingoOnlineEditor : ClingoSolverEditor
{

}

