using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class GameState
{
    abstract public void Update();
    abstract public void Enter();
    abstract public void Exit();
}
