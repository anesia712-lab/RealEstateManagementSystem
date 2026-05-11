namespace RealEstateManagementSystem.Models
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        /// <summary>Short message everyone can understand.</summary>
        public string UserMessage { get; set; } =
            "Something went wrong on our side. Your work may not have been saved.";

        /// <summary>Optional technical hint (development only).</summary>
        public string? TechnicalDetail { get; set; }

        public bool ShowTechnicalDetail => !string.IsNullOrEmpty(TechnicalDetail);

        public int? StatusCode { get; set; }
    }
}
