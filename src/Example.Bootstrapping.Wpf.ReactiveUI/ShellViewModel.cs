using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using FluentValidation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Example.Bootstrapping.Wpf.ReactiveUI
{
    public class ShellViewModel : ReactiveObject, INotifyDataErrorInfo
    {
        private readonly IValidator<ShellViewModel> _validator;

        public ShellViewModel(IValidator<ShellViewModel> validator)
        {
            _validator = validator;
            Header = "boom";

            this.Changed.Subscribe(x => ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(x.PropertyName)));
        }

        [Reactive] public string Header { get; set; }

        public IEnumerable GetErrors(string propertyName)
        {
            return _validator.Validate(this).Errors.Where(x => x.PropertyName == propertyName);
        }

        public bool HasErrors => _validator.Validate(this).IsValid;
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
    }
}