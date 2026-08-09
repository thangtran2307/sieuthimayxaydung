namespace Marketplace.Domain.Common;

// Domain enums. Serialized to the API/wire contract as their UPPER_SNAKE names and stored as
// text (with CHECK constraints) in Postgres; Dapper maps the string columns back to these.

public enum UserRole
{
    SELLER,
    ADMIN,
}

public enum Condition
{
    NEW,
    USED,
}

public enum ListingStatus
{
    PENDING,
    ACTIVE,
    REJECTED,
    SOLD,
    EXPIRED,
    REMOVED,
}

public enum BoostTier
{
    BASIC,
    FEATURED,
    MAX,
}

public enum BoostStatus
{
    REQUESTED,
    ACTIVE,
    EXPIRED,
    CANCELLED,
}

public enum ReportReason
{
    FRAUD,
    INCORRECT_INFO,
    SPAM,
    PROHIBITED,
    OTHER,
}

public enum ReportStatus
{
    OPEN,
    RESOLVED_REMOVED,
    RESOLVED_CLEARED,
}

public enum InquiryType
{
    PHONE_REVEAL,
    MESSAGE,
}

public enum ModerationAction
{
    APPROVED,
    REJECTED,
    REMOVED,
}
