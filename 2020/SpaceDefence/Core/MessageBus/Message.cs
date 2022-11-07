using System;

namespace MiniGames.Games.SpaceDefence.Core.MessageBus
{
    public sealed class Message
    {
        public event Action Receive;

        public void Send()
        {
            Receive?.Invoke();
        }
    }

    public sealed class Message<T>
    {
        public event Action<T> Receive;

        public void Send(T arg)
        {
            Receive?.Invoke(arg);
        }
    }

    public sealed class Message<T1, T2>
    {
        public event Action<T1, T2> Receive;

        public void Send(T1 arg1, T2 arg2)
        {
            Receive?.Invoke(arg1, arg2);
        }
    }

    public sealed class Message<T1, T2, T3>
    {
        public event Action<T1, T2, T3> Receive;

        public void Send(T1 arg1, T2 arg2, T3 arg3)
        {
            Receive?.Invoke(arg1, arg2, arg3);
        }
    }

    public sealed class Message<T1, T2, T3, T4>
    {
        public event Action<T1, T2, T3, T4> Receive;

        public void Send(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            Receive?.Invoke(arg1, arg2, arg3, arg4);
        }
    }
}