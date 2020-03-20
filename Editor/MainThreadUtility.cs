using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MainThreadUtility
{
    private MainThreadUtility(){}

    private static MainThreadUtility _instance;

    public static MainThreadUtility Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new MainThreadUtility();
                EditorApplication.update += _instance.MainThreadUpdate;
            }

            return _instance;
        }
    }
        
    private Queue<Action> _actionQueue = new Queue<Action>(10);

    public void EnqueueAction(Action func)
    {
        _actionQueue.Enqueue(func);
    }

    private void MainThreadUpdate()
    {
        while (_actionQueue.Count > 0)
        {
            var nextAction = _actionQueue.Dequeue();
            nextAction.Invoke();
        }
    }
}
