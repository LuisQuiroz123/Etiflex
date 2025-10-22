using CommunityToolkit.Mvvm.Input;
using EtiMovilT.Models;

namespace EtiMovilT.PageModels
{
    public interface IProjectTaskPageModel
    {
        IAsyncRelayCommand<ProjectTask> NavigateToTaskCommand { get; }
        bool IsBusy { get; }
    }
}