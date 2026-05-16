export interface Scoreboard {
  id: number;
  gameId: number;
  gameName: string;
  teamId: number;
  teamName: string;
  score: number;
}

export interface UpdateScoreboard {
  gameId: number;
  teamId: number;
  score: number;
}