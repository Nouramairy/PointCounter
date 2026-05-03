namespace pointCounterBackend.Entities
{
    public class Game
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;
        public int Duration { get; set; } 

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public ICollection<GameTeam> GameTeams { get; set; } = new List<GameTeam>();
        public ICollection<Scoreboard> Scoreboards { get; set; } = new List<Scoreboard>();

    }
}
