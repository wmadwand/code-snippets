using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TutorialActions
{
    public class Parallel : ITutorialAction
    {
        [SerializeField] List<ITutorialAction> Actions;

        IEnumerator ITutorialAction.Play(TutorialStep tutorial, TutorialContext context)
        {
            var ienumerators = new List<IEnumerator>();
            foreach (var a in Actions)
            {
                var ienumerator = a.Play(tutorial, context);
                ienumerators.Add(ienumerator);
            }
            var iterator = new IterateEnumerators();
            iterator.Add(ienumerators);
            while (iterator.Iterate())
            {
                yield return WaitNextTick.Instance;
            }
        }
    }

    public class Sequence : ITutorialAction
    {
        public List<ITutorialAction> Actions;

        IEnumerator ITutorialAction.Play(TutorialStep tutorial, TutorialContext context)
        {
            foreach (var e in Actions)
            {
                yield return e.Play(tutorial, context);
            }
        }
    }

    public class Wait : ITutorialAction
    {
        [SerializeField] float Time;

        IEnumerator ITutorialAction.Play(TutorialStep tutorial, TutorialContext context)
        {
            var time = UnityEngine.Time.timeSinceLevelLoad + Time;
            while(time > UnityEngine.Time.timeSinceLevelLoad)
            {
                yield return WaitNextTick.Instance;
            }
        }
    }

    public class RunExecutor : ITutorialAction
    {
        public IExecutor Executor;

        IEnumerator ITutorialAction.Play(TutorialStep tutorial, TutorialContext context)
        {
            if (context.BattleSimulator != null)
            {
                yield return Executor.Activate(ExecutorContext.Create(context.BattleSimulator));
            }
            else
            {
                Debug.LogError("Battle not exists");
            }
        }
    }

    public class Empty : ITutorialAction
    {
        IEnumerator ITutorialAction.Play(TutorialStep tutorial, TutorialContext context)
        {
            yield break;
        }
    }

    public class ForceMarkTutorialCompleted : ITutorialAction
    {
        [SerializeField] MetaTutorialConfig Tutorial;
        IEnumerator ITutorialAction.Play(TutorialStep tutorial, TutorialContext context)
        {
            Game.Services.Tutorial.TutorialFinish(Tutorial);
            yield break;
        }
    }
}


