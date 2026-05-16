import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';

import { GameService } from '../../../services/Game.service';
import { getApiErrorMessage } from '../../../utils/http-error.util';
import { Game } from '../../../models/game.model';
import { ScoreboardView } from '../scoreboard/scoreboard-view';

@Component({
  selector: 'app-poangstallning-hub',
  imports: [CommonModule, FormsModule, RouterLink, ScoreboardView],
  templateUrl: './poangstallning-hub.html',
  styleUrl: './poangstallning-hub.css',
})
export class PoangstallningHub implements OnInit {
  games: Game[] = [];
  selectedGameId: number | null = null;
  gamesLoadError: string | null = null;

  constructor(
    private gameService: GameService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadGames();
    this.route.queryParamMap.subscribe(params => {
      const raw = params.get('match');
      if (!raw) {
        this.selectedGameId = null;
        return;
      }
      const id = Number(raw);
      if (Number.isFinite(id) && id >= 1) {
        this.selectedGameId = id;
      }
    });
  }

  loadGames(): void {
    this.gameService.getAll().subscribe({
      next: games => {
        this.games = games;
        this.gamesLoadError = null;
      },
      error: err => {
        console.error(err);
        this.gamesLoadError = getApiErrorMessage(err);
      },
    });
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
