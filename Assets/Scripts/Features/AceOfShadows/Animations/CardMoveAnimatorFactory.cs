using System.Collections.Generic;
using Features.AceOfShadows.Interfaces;

namespace Features.AceOfShadows.Animations
{
    /// <summary>
    /// List of available animations, expand in the future.
    /// Note: for this task, one animation is chosen at random each time a card needs to move from one stack to the other
    /// </summary>
    public class CardMoveAnimatorFactory
    {
        private readonly Dictionary<CardAnimationStyle, ICardMoveAnimator> _animators = new()
        {
            { CardAnimationStyle.Linear, new LinearMoveAnimator() },
            { CardAnimationStyle.Arc, new ArcMoveAnimator() },
        };

        public ICardMoveAnimator Get(CardAnimationStyle style) => _animators[style];
    }
}