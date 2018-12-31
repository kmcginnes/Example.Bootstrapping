using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Example.Bootstrapping.TopShelf.FindJobs
{
    public class FindJobsHandler : IRequestHandler<FindJobs>, IDisposable
    {
        private readonly DatabaseContext _context;

        public FindJobsHandler(DatabaseContext context)
        {
            this.Log().Debug($"{nameof(FindJobsHandler)}.ctor()");
            _context = context;
        }

        public async Task<Unit> Handle(FindJobs message, CancellationToken cancellationToken)
        {
            this.Log().Debug($"{nameof(FindJobsHandler)}.Handle()");

            this.Log().Debug($"Doing some work to find jobs");
            await _context.QueryAsync();

            return Unit.Value;
        }

        public void Dispose()
        {
            this.Log().Debug($"{nameof(FindJobsHandler)}.Dispose()");
        }
    }
}
