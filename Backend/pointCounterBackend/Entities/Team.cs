namespace pointCounterBackend.Entities
{
    public class Team
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;
        public int MaximumPlayersAllowed { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public ICollection<TeamPlayer> TeamPlayers { get; set; } = new List<TeamPlayer>();
        public ICollection<GameTeam> GameTeams { get; set; } = new List<GameTeam>();
    }
}
