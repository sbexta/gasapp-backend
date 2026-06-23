namespace GasApp.Domain.Enums;

public enum InspectionStatus
{
    Pending,
    PreCheck,
    InProgress,
    TechnicalReview,
    GeneratingDocs,
    Completed,
    RequiresFollowup,
    Rejected
}
