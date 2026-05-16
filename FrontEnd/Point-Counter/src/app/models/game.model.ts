export interface Game {
  id: number;
  name: string;
  duration: number;
  teams: string[];
}

export interface CreateGame {
  name: string;
  duration: number;
  teamIds: number[];
}

export interface UpdateGame {
  name: string;
  duration: number;
}