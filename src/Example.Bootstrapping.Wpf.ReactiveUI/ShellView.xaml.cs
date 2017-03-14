using System.Reactive.Disposables;
using System.Windows;
using ReactiveUI;

namespace Example.Bootstrapping.Wpf.ReactiveUI
{
    public partial class ShellView : Window, IViewFor<ShellViewModel>
    {
        public ShellView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                //this.OneWayBind(ViewModel, vm => vm.Header, v => v.Header.Text).DisposeWith(d);
            });
        }

        public ShellViewModel ViewModel
        {
            get { return (ShellViewModel) GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(ShellViewModel), typeof(ShellView), new PropertyMetadata(null));

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (ShellViewModel) value; }
        }
    }
}
