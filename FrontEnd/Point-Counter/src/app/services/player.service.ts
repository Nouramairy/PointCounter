import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CreatePlayer, Player, UpdatePlayer } from '../models/player.model';

@Injectable({
  providedIn: 'root'
})
export class PlayerService {
  private apiUrl = '/api/players';

  constructor(private http: HttpClient) {}

  getAll(): Observable<Player[]> {
    return this.http.get<Player[]>(this.apiUrl);
  }

  create(player: CreatePlayer): Observable<Player> {
    return this.http.post<Player>(this.apiUrl, player);
  }

  getById(id: number): Observable<Player> {
    return this.http.get<Player>(`${this.apiUrl}/${id}`);
  }

  update(id: number, player: UpdatePlayer): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, player);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
