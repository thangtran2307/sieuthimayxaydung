namespace Marketplace.Domain.Common;

/// <summary>A single field-level validation failure surfaced in the error envelope `details`.</summary>
public sealed record ValidationError(string Path, string Message);
