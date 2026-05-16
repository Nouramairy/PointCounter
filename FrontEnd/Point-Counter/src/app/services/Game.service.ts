import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CreateGame, Game, UpdateGame } from '../models/game.model';

@Injectable({
  providedIn: 'root'
})
export class GameService {
  private apiUrl = '/api/games';

  constructor(private http: HttpClient) {}

  getAll(): Observable<Game[]> {
    return this.http.get<Game[]>(this.apiUrl);
  }

  getById(id: number): Observable<Game> {
    return this.http.get<Game>(`${this.apiUrl}/${id}`);
  }

  create(game: CreateGame): Observable<Game> {
    return this.http.post<Game>(this.apiUrl, game);
  }

  update(id: number, game: UpdateGame): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, game);
  }

  addTeamToGame(gameId: number, teamId: number): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/${gameId}/teams/${teamId}`, {});
  }

  removeTeamFromGame(gameId: number, teamId: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${gameId}/teams/${teamId}`);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
