using ReactiveUI;

namespace Example.Bootstrapping.Wpf.ReactiveUI
{
    public class ShellViewModel : ReactiveObject
    {
        public string Header { get; } = "Some value";
    }
}