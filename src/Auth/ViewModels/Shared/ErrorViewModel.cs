using System.ComponentModel.DataAnnotations;

namespace Auth.ViewModels.Shared;

public class ErrorViewModel
{
    [Display(Name = "Error")]
    public string? Error { get; set; }

    [Display(Name = "Description")]
    public string? ErrorDescription { get; set; } = string.Empty;
    public string? RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}
