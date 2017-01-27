using System.Threading.Tasks;
using MediatR;

namespace Example.Bootstrapping.TopShelf.FindJobs
{
    public class FindJobsConsoleCommandProcessor : IConsoleCommandProcessor
    {
        private readonly IMediator _mediator;
        public char InputCharacter => 'f';
        public string LongName => "Find jobs";

        public FindJobsConsoleCommandProcessor(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Execute()
        {
            this.Log().Debug($"Executing {LongName.ToLower()}");
            await _mediator.Send(new FindJobs());
            this.Log().Debug($"Finished executing {LongName.ToLower()}");
        }
    }
}