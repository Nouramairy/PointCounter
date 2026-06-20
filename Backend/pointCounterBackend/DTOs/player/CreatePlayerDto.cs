using System.ComponentModel.DataAnnotations;

namespace pointCounterBackend.DTOs.Players;

public class CreatePlayerDto : IValidatableObject
{
    [Required(ErrorMessage = "Name is required.")]
    [StringLength(120, MinimumLength = 1, ErrorMessage = "Name must be 1-120 characters.")]
    public string Name { get; set; } = null!;

    [Range(1, 120, ErrorMessage = "Age must be between 1 and 120.")]
    public int Age { get; set; }

    [Required(ErrorMessage = "Address is required.")]
    [StringLength(200, MinimumLength = 1, ErrorMessage = "Address must be 1-200 characters.")]
    public string Address { get; set; } = null!;

    [Required(ErrorMessage = "Phone is required.")]
    [StringLength(40, MinimumLength = 1, ErrorMessage = "Phone must be 1-40 characters.")]
    public string Phone { get; set; } = null!;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(Name))
            yield return new ValidationResult("Name cannot be empty or only whitespace.", [nameof(Name)]);
        if (string.IsNullOrWhiteSpace(Address))
            yield return new ValidationResult("Address cannot be empty or only whitespace.", [nameof(Address)]);
        if (string.IsNullOrWhiteSpace(Phone))
            yield return new ValidationResult("Phone cannot be empty or only whitespace.", [nameof(Phone)]);
    }
}
