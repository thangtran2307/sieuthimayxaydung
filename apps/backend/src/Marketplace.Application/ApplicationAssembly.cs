using System.Reflection;

namespace Marketplace.Application;

/// <summary>Marker for assembly scanning (AutoMapper profiles, FluentValidation validators, Mediator).</summary>
public static class ApplicationAssembly
{
    public static readonly Assembly Reference = typeof(ApplicationAssembly).Assembly;
}
