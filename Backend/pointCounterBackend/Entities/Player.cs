namespace pointCounterBackend.Entities
{
    public class Player
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;
        public int Age { get; set; }
        public string Address { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ICollection<TeamPlayer> TeamPlayers { get; set; } = new List<TeamPlayer>();
        
    }
}
