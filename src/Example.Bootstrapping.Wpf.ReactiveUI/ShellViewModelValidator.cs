using FluentValidation;

namespace Example.Bootstrapping.Wpf.ReactiveUI
{
    public class ShellViewModelValidator : AbstractValidator<ShellViewModel>
    {
        public ShellViewModelValidator()
        {
            this.RuleFor(x => x.Header)
                .NotEmpty();
        }
    }
}