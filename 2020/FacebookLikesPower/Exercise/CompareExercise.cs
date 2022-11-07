using ExerciseGeneration;
using System.Collections.Generic;

namespace MiniGames.Games.FBLikesPower
{
    public class CompareExercise : ExpressionExerciseBase
    {
        private ComparePair _currentPair;

        public CompareExercise(List<int> completeNumbers) : base(completeNumbers)
        {
        }

        public ComparePair NextComparePair()
        {
            Expression leftExpression = expressionGenerator.expressionQueue.Dequeue();
            Expression rightExpression = expressionGenerator.expressionQueue.Dequeue();

            _currentPair = new ComparePair(leftExpression, rightExpression);
            return _currentPair;
        }

        public bool IsAnswerCorrect(string symbol)
        {
            return symbol == _currentPair.GetCorrectSymbol();
        }

        public PositionType GetWinPosition()
        {
            return _currentPair.GetWinPosition();
        }
    }
}