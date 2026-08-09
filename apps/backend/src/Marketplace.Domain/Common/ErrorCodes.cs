namespace Marketplace.Domain.Common;

/// <summary>
/// Stable, machine-readable error codes shared with the API/clients (mirrors the frontend
/// contract's ErrorCode enum). Used in the error envelope emitted by the API.
/// </summary>
public static class ErrorCodes
{
    public const string ValidationFailed = "VALIDATION_FAILED";
    public const string Unauthenticated = "UNAUTHENTICATED";
    public const string Forbidden = "FORBIDDEN";
    public const string NotFound = "NOT_FOUND";
    public const string ListingNotFound = "LISTING_NOT_FOUND";
    public const string ListingNotPublic = "LISTING_NOT_PUBLIC";
    public const string NotListingOwner = "NOT_LISTING_OWNER";
    public const string EmailInUse = "EMAIL_IN_USE";
    public const string InvalidCredentials = "INVALID_CREDENTIALS";
    public const string BoostConflict = "BOOST_CONFLICT";
    public const string ListingNotActiveForBoost = "LISTING_NOT_ACTIVE_FOR_BOOST";
    public const string UnsupportedMedia = "UNSUPPORTED_MEDIA";
    public const string RateLimited = "RATE_LIMITED";
    public const string InternalError = "INTERNAL_ERROR";
}
