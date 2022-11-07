using Coconut.Game.Patterns;


public sealed class LikesMessageBus
{
    public readonly Message<MathButton> mathButtonClick = new Message<MathButton>();
    public readonly Message correctAnswer = new Message();
}

