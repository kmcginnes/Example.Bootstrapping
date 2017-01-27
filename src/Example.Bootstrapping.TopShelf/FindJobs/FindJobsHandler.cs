using System;
using MediatR;

namespace Example.Bootstrapping.TopShelf.FindJobs
{
    public class FindJobsHandler : IRequestHandler<FindJobs>, IDisposable
    {
        private readonly DatabaseContext _context;

        public FindJobsHandler(DatabaseContext context)
        {
            this.Log().Debug($"Inside ctor()");
            _context = context;
        }

        public void Handle(FindJobs message)
        {
            this.Log().Debug($"Inside Handle()");
        }

        public void Dispose()
        {
            this.Log().Debug($"Inside Dispose()");
        }
    }
}