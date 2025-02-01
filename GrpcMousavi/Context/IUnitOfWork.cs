using GrpcMousavi.Model;
using GrpcMousavi.Repositories;
using Microsoft.AspNetCore.Identity;

namespace GrpcMousavi.Context
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<Person> Persons { get; }        
        Task SaveAsync();
    }

}
