export interface Team {
  id: number;
  name: string;
  maximumPlayersAllowed: number;
  players: string[];
}

export interface CreateTeam {
  name: string;
  maximumPlayersAllowed: number;
  playerIds: number[];
}

export interface UpdateTeam {
    name: string;
    maximumPlayersAllowed: number;
    playerIds: number[];
}