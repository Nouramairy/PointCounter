import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { Subscription } from 'rxjs';
import { finalize } from 'rxjs/operators';

import { GameService } from '../../../services/Game.service';
import { NotificationService } from '../../../services/notification.service';
import { getApiErrorMessage } from '../../../utils/http-error.util';
import { Game } from '../../../models/game.model';
import { ScoreboardView } from '../scoreboard/scoreboard-view';

const LOAD_TIMEOUT_MS = 4000;

@Component({
  selector: 'app-poangstallning-hub',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, ScoreboardView],
  templateUrl: './poangstallning-hub.html',
  styleUrl: './poangstallning-hub.css',
})
export class PoangstallningHub implements OnInit {
  games: Game[] = [];
  selectedGameId: number | null = null;
  gamesLoadError: string | null = null;
  showGames = false;
  loadingGames = false;

  constructor(
    private gameService: GameService,
    private notifications: NotificationService,
    private route: ActivatedRoute,
    private router: Router,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.route.queryParamMap.subscribe(params => {
      const raw = params.get('match');
      if (!raw) {
        this.selectedGameId = null;
        return;
      }
      const id = Number(raw);
      if (Number.isFinite(id) && id >= 1) {
        this.selectedGameId = id;
        if (!this.showGames) {
          this.showGames = true;
          this.loadGames();
        }
      }
    });
  }

  showAvailableMatches(): void {
    if (this.loadingGames) {
      return;
    }

    this.showGames = true;
    this.loadGames();
  }

  loadGames(): void {
    this.loadingGames = true;
    this.gamesLoadError = null;
    let request: Subscription | null = null;
    const timeoutId = window.setTimeout(() => {
      this.failLoadGames('Could not load matches. Check that the API is running.');
      request?.unsubscribe();
    }, LOAD_TIMEOUT_MS);

    request = this.gameService.getAll().pipe(
      finalize(() => {
        window.clearTimeout(timeoutId);
        this.loadingGames = false;
        this.cdr.detectChanges();
      })
    ).subscribe({
      next: games => {
        this.games = games;
        this.gamesLoadError = null;
        this.cdr.detectChanges();
      },
      error: err => {
        console.error(err);
        this.failLoadGames(getApiErrorMessage(err));
      },
    });
  }

  private failLoadGames(message: string): void {
    this.gamesLoadError = message;
    this.loadingGames = false;
    this.notifications.show(message, 'error');
    this.cdr.detectChanges();
  }

  onMatchChange(id: number | null): void {
    this.selectedGameId = id;
    void this.router.navigate([], {
      relativeTo: this.route,
      queryParams: id != null ? { match: id } : { match: null },
      replaceUrl: true,
    });
  }
}
