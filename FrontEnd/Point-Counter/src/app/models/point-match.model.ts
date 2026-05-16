export interface PointMatchPlayer {
  id: number;
  name: string;
  score: number;
}

export interface PointMatch {
  publicId: string;
  name: string;
  higherScoreWins: boolean;
  startingScore: number;
  playersLocked: boolean;
  players: PointMatchPlayer[];
}

export interface CreatePointMatch {
  name: string;
  higherScoreWins: boolean;
  startingScore: number;
  playersLocked: boolean;
  playerNames: string[];
}