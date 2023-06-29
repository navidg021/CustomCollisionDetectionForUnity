using UnityEngine;
using System.Collections;
using System;
using CustomCollisionDetection;

public class AreaCollisionExp1 : AreaManager
{
    [Header("New Other")]
    public int numberInt = 1;

    protected override void OnOverlap(Action<object> action)
    {
        Action<object> act = action;
        if (act != null) {
            useAutoCallBack = false;
            act(numberInt);
        }
    }
}