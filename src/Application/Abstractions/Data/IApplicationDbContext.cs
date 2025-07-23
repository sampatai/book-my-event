using Domain.Todos;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Abstractions.Data;

public interface IApplicationDbContext : IUnitOfWork
{
    DbSet<User> Users { get; }
    DbSet<TodoItem> TodoItems { get; }

}
