using System.ComponentModel.DataAnnotations;

namespace pointCounterBackend.DTOs.Players;

public class CreatePlayerDto : IValidatableObject
{
    [Required(ErrorMessage = "Namn är obligatoriskt.")]
    [StringLength(120, MinimumLength = 1, ErrorMessage = "Namnet måste vara 1–120 tecken.")]
    public string Name { get; set; } = null!;

    [Range(1, 120, ErrorMessage = "Ålder måste vara mellan 1 och 120.")]
    public int Age { get; set; }

    [Required(ErrorMessage = "Adress är obligatorisk.")]
    [StringLength(200, MinimumLength = 1, ErrorMessage = "Adressen måste vara 1–200 tecken.")]
    public string Address { get; set; } = null!;

    [Required(ErrorMessage = "Telefon är obligatorisk.")]
    [StringLength(40, MinimumLength = 1, ErrorMessage = "Telefon måste vara 1–40 tecken.")]
    public string Phone { get; set; } = null!;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(Name))
            yield return new ValidationResult("Namnet får inte vara tomt eller bara mellanslag.", [nameof(Name)]);
        if (string.IsNullOrWhiteSpace(Address))
            yield return new ValidationResult("Adressen får inte vara tom eller bara mellanslag.", [nameof(Address)]);
        if (string.IsNullOrWhiteSpace(Phone))
            yield return new ValidationResult("Telefon får inte vara tom eller bara mellanslag.", [nameof(Phone)]);
    }
}