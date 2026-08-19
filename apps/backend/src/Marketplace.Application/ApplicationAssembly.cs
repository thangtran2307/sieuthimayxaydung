using System.Reflection;

namespace Marketplace.Application;

/// <summary>Marker for assembly scanning (FluentValidation validators, Mediator source generator).</summary>
public static class ApplicationAssembly
{
    public static readonly Assembly Reference = typeof(ApplicationAssembly).Assembly;
}
