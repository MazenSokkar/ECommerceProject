using System;

namespace ecommerce.Core.IRepositories;

public interface IUnitOfWork : IDisposable
{
    Task<int> Complete();
}
