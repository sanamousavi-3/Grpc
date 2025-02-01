using GrpcMousavi.Model;
using GrpcMousavi.Repositories;

namespace GrpcMousavi.Context
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly GrpcContext _context;
        private IGenericRepository<Person> _persons;

        public UnitOfWork(GrpcContext context)
        {
            _context = context;
        }

        public IGenericRepository<Person> Persons => _persons ??= new GenericRepository<Person>(_context);
        

        public async Task SaveAsync() => await _context.SaveChangesAsync();

        public void Dispose() => _context.Dispose();
    }

}
