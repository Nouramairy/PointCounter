using System.ComponentModel.DataAnnotations;

namespace pointCounterBackend.DTOs.Teams;

public class UpdateTeamDto : IValidatableObject
{
    [Required(ErrorMessage = "Lagnamn är obligatoriskt.")]
    [StringLength(120, MinimumLength = 1, ErrorMessage = "Lagnamnet måste vara 1–120 tecken.")]
    public string Name { get; set; } = null!;

    [Range(1, 100, ErrorMessage = "Max antal spelare måste vara mellan 1 och 100.")]
    public int MaximumPlayersAllowed { get; set; }

    public List<int> PlayerIds { get; set; } = new();

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(Name))
            yield return new ValidationResult("Lagnamnet får inte vara tomt eller bara mellanslag.", [nameof(Name)]);

        if (PlayerIds.Count > MaximumPlayersAllowed)
            yield return new ValidationResult(
                $"Du kan inte välja fler spelare ({PlayerIds.Count}) än max antal ({MaximumPlayersAllowed}).",
                [nameof(PlayerIds)]);
    }
}