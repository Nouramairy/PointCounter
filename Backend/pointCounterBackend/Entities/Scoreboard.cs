namespace pointCounterBackend.Entities
{
    public class Scoreboard
    {
        public int Id { get; set; }

        public int GameId { get; set; }
        public int TeamId { get; set; }

        public int Score { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Game Game { get; set; } = null!;
        public Team Team { get; set; } = null!;
    }
}
