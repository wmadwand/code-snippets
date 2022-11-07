using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TutorialConditions
{
    public class True : ITutorialCondition
    {
        public bool IsMet(TutorialContext context) => true;
    }

    public class Any : ITutorialCondition
    {
        [SerializeField] List<ITutorialCondition> Conditions;
        public bool IsMet(TutorialContext context) => Conditions.Any(c => c.IsMet(context));
    }

    public class All : ITutorialCondition
    {
        public List<ITutorialCondition> Conditions;
        public bool IsMet(TutorialContext context) => Conditions.All(c => c.IsMet(context));
    }

    public class Not : ITutorialCondition
    {
        [SerializeField] ITutorialCondition Condition;
        public bool IsMet(TutorialContext context) => !Condition.IsMet(context);
    }

    public class StepCompleted : ITutorialCondition
    {
        [SerializeField] string Id;
        public bool IsMet(TutorialContext context) => context.Controller.IsStepCompleted(Id);
    }

    public class TutorialCompleted : ITutorialCondition
    {
        [SerializeField] MetaTutorialConfig Tutorial;
        public bool IsMet(TutorialContext context) => context.Controller.PlayerProfile.Tutorial.CompletedTutorials.Any(t => t == Tutorial.name);
    }

}
