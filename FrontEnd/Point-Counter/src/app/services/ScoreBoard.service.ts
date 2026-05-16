import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Scoreboard, UpdateScoreboard } from '../models/scoreboard.model';

@Injectable({
  providedIn: 'root'
})
export class ScoreboardService {
  private apiUrl = '/api/scoreboards';

  constructor(private http: HttpClient) {}

  getByGameId(gameId: number): Observable<Scoreboard[]> {
    return this.http.get<Scoreboard[]>(`${this.apiUrl}/game/${gameId}`);
  }

  updateScore(dto: UpdateScoreboard): Observable<Scoreboard> {
    return this.http.put<Scoreboard>(this.apiUrl, dto);
  }
}
