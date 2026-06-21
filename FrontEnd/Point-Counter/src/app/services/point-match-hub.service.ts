import { Injectable, OnDestroy, PLATFORM_ID, inject } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import * as signalR from '@microsoft/signalr';
import { Observable, Subject } from 'rxjs';

import { PointMatch } from '../models/point-match.model';

@Injectable({
  providedIn: 'root',
})
export class PointMatchHubService implements OnDestroy {
  private readonly isBrowser = isPlatformBrowser(inject(PLATFORM_ID));

  private hubConnection: signalR.HubConnection | null = null;
  private startPromise: Promise<void> | null = null;
  private currentPublicId: string | null = null;

  private readonly matchUpdated$ = new Subject<PointMatch>();
  private readonly reconnected$ = new Subject<void>();

  readonly matchUpdates: Observable<PointMatch> = this.matchUpdated$.asObservable();
  readonly reconnected: Observable<void> = this.reconnected$.asObservable();

  private ensureStarted(): Promise<void> {
    if (!this.isBrowser) {
      return Promise.resolve();
    }

    if (!this.hubConnection) {
      const connection = new signalR.HubConnectionBuilder()
        .withUrl('/hubs/point-matches')
        .withAutomaticReconnect()
        .build();

      connection.on('MatchUpdated', (match: PointMatch) => {
        this.matchUpdated$.next(match);
      });

      connection.onreconnected(() => {
        if (this.currentPublicId) {
          connection.invoke('JoinMatch', this.currentPublicId).catch(err =>
            console.error('Failed to rejoin match group after reconnect:', err)
          );
        }
        this.reconnected$.next();
      });

      this.hubConnection = connection;
    }

    if (!this.startPromise) {
      this.startPromise = this.hubConnection.start().catch(err => {
        console.error('SignalR connection failed to start:', err);
        this.startPromise = null;
        throw err;
      });
    }

    return this.startPromise;
  }

  async joinMatch(publicId: string): Promise<void> {
    if (!this.isBrowser) return;

    await this.ensureStarted();

    if (this.currentPublicId === publicId) return;

    if (this.currentPublicId) {
      await this.leaveMatch(this.currentPublicId);
    }

    this.currentPublicId = publicId;
    await this.hubConnection?.invoke('JoinMatch', publicId);
  }

  async leaveMatch(publicId: string): Promise<void> {
    if (!this.isBrowser || !this.hubConnection) return;

    try {
      await this.hubConnection.invoke('LeaveMatch', publicId);
    } catch (err) {
      console.error('Failed to leave match group:', err);
    }

    if (this.currentPublicId === publicId) {
      this.currentPublicId = null;
    }
  }

  ngOnDestroy(): void {
    this.hubConnection?.stop();
  }
}
