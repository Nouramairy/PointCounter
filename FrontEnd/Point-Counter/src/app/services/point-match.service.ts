import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import {
  CreatePointMatch,
  PointMatch
} from '../models/point-match.model';

@Injectable({
  providedIn: 'root'
})
export class PointMatchService {
  private apiUrl = '/api/PointMatches';

  constructor(private http: HttpClient) {}

  createMatch(dto: CreatePointMatch): Observable<PointMatch> {
    return this.http.post<PointMatch>(this.apiUrl, dto);
  }

  addPlayer(publicId: string, name: string): Observable<PointMatch> {
  return this.http.post<PointMatch>(
    `${this.apiUrl}/${publicId}/players`,
    { name }
  );
}

  updatePlayerName(publicId: string, playerId: number, name: string): Observable<PointMatch> {
  return this.http.put<PointMatch>(
    `${this.apiUrl}/${publicId}/players/${playerId}/name`,
    { name }
  );
}
  getMatch(publicId: string): Observable<PointMatch> {
    return this.http.get<PointMatch>(`${this.apiUrl}/${publicId}`);
  }

  updateScore(publicId: string, playerId: number, score: number): Observable<PointMatch> {
    return this.http.put<PointMatch>(
      `${this.apiUrl}/${publicId}/players/${playerId}/score`,
      { score }
    );
  }

  restart(publicId: string): Observable<PointMatch> {
    return this.http.post<PointMatch>(
      `${this.apiUrl}/${publicId}/restart`,
      {}
    );
  }

  deletePlayer(publicId: string, playerId: number): Observable<PointMatch> {
  return this.http.delete<PointMatch>(
    `${this.apiUrl}/${publicId}/players/${playerId}`
  );
  }

  clone(publicId: string): Observable<PointMatch> {
    return this.http.post<PointMatch>(
      `${this.apiUrl}/${publicId}/clone`,
      {}
    );
  }
}
