namespace Features.MagicWords.Enums
{
    /// <summary>
    /// NotSetup and Failed states have the same visual on purpose, since final user
    /// shouldn't know the reason of the error.
    /// </summary>
    public enum AvatarVisualState { NotSetup, Pending, Ready, Failed }
}