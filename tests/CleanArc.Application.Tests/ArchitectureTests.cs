namespace CleanArc.Application.Tests;

using System.Linq;
using CleanArc.Domain.Entities;
using CleanArc.Application.Orders.Commands.CreateOrder;
using CleanArc.Infrastructure.Persistence;
using NetArchTest.Rules;
using Xunit;

/// <summary>
/// Architecture tests that enforce Clean Architecture constraints using NetArchTest.Rules.
/// These tests verify dependency inversion principle is maintained as the codebase grows.
/// </summary>
public class ArchitectureTests
{
    private readonly System.Reflection.Assembly _domainAssembly = typeof(Order).Assembly;
    private readonly System.Reflection.Assembly _applicationAssembly = typeof(CreateOrderHandler).Assembly;
    private readonly System.Reflection.Assembly _infrastructureAssembly = typeof(AppDbContext).Assembly;

    [Fact]
    public void Domain_Should_Not_Depend_On_Application()
    {
        var result = Types.InAssembly(_domainAssembly)
            .That()
            .ResideInNamespace("CleanArc.Domain")
            .ShouldNot()
            .HaveDependencyOn("CleanArc.Application")
            .GetResult();

        Assert.True(result.IsSuccessful, "VIOLATION: Domain layer depends on Application layer");
    }

    [Fact]
    public void Domain_Should_Not_Depend_On_Infrastructure()
    {
        var result = Types.InAssembly(_domainAssembly)
            .That()
            .ResideInNamespace("CleanArc.Domain")
            .ShouldNot()
            .HaveDependencyOn("CleanArc.Infrastructure")
            .GetResult();

        Assert.True(result.IsSuccessful, "VIOLATION: Domain layer depends on Infrastructure layer");
    }

    [Fact]
    public void Application_Should_Not_Depend_On_Infrastructure()
    {
        var result = Types.InAssembly(_applicationAssembly)
            .That()
            .ResideInNamespace("CleanArc.Application")
            .ShouldNot()
            .HaveDependencyOn("CleanArc.Infrastructure")
            .GetResult();

        Assert.True(result.IsSuccessful, "VIOLATION: Application depends on Infrastructure (use IAppDbContext)");
    }

    [Fact]
    public void Infrastructure_Should_Not_Depend_On_Api()
    {
        var result = Types.InAssembly(_infrastructureAssembly)
            .That()
            .ResideInNamespace("CleanArc.Infrastructure")
            .ShouldNot()
            .HaveDependencyOn("CleanArc.Api")
            .GetResult();

        Assert.True(result.IsSuccessful, "VIOLATION: Infrastructure depends on API layer");
    }

    [Fact]
    public void Domain_Layer_Should_Have_Entities()
    {
        var result = Types.InAssembly(_domainAssembly)
            .That()
            .ResideInNamespace("CleanArc.Domain.Entities")
            .Should()
            .BeClasses()
            .GetResult();

        Assert.True(result.IsSuccessful, "Domain layer should contain entity classes");
    }

    [Fact]
    public void Infrastructure_Layer_Should_Have_DbContext()
    {
        var result = Types.InAssembly(_infrastructureAssembly)
            .That()
            .ResideInNamespace("CleanArc.Infrastructure.Persistence")
            .Should()
            .BeClasses()
            .GetResult();

        Assert.True(result.IsSuccessful, "Infrastructure layer should contain DbContext classes");
    }
}
