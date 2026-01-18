using System.Reflection;
using Infrastructure.Database;
using SharedKernel;
using Web.Api;

namespace ArchitectureTests;

public abstract class BaseTest
{
  
    //protected static readonly Assembly ApplicationAssembly = typeof(ICommand).Assembly;
    protected static readonly Assembly InfrastructureAssembly = typeof(ApplicationDbContext).Assembly;
    protected static readonly Assembly PresentationAssembly = typeof(Program).Assembly;
}
