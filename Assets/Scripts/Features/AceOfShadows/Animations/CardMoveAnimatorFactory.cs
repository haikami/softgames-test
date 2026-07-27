using System.Collections.Generic;
using Features.AceOfShadows.Interfaces;

namespace Features.AceOfShadows.Animations
{
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