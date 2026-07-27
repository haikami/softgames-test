using System;
using Core.Enums;
using Cysharp.Threading.Tasks;

namespace Core.Interfaces
{
    public interface ITaskMenuService
    {
        FeatureId? CurrentTask { get; }
        event Action<FeatureId> OnTaskSelected;
        event Action<FeatureId> OnTaskExited;

        UniTask SelectTask(FeatureId id);
        UniTask ExitToMenu();
    }
}