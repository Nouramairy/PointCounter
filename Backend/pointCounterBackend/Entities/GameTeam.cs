namespace pointCounterBackend.Entities
{
    public class GameTeam
    {
        public int GameId { get; set; }
        public int TeamId { get; set; }

        // Navigation
        public Game Game { get; set; } = null!;
        public Team Team { get; set; } = null!;
    }
}
