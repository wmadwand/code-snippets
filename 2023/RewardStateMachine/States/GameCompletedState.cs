using Cysharp.Threading.Tasks;
using Project.Utils.StateMachineMono;
using System;
using UnityEngine;

public class GameCompletedState : GameState
{
    public async override UniTask Enter()
    {
        Debug.Log($"Enter to state: {this.GetType()}");
    }

    public async override UniTask Exit()
    {

    }

    public async override UniTask Update()
    {

    }

    public async override UniTask InputEvent(GameEvent eventt, params object[] data)
    {
        //throw new NotImplementedException();
    }


}
