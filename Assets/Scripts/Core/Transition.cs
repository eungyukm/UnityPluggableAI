using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Transition
{
    public Decision decision;
    public StateSO trueState;
    public StateSO falseState;
}
