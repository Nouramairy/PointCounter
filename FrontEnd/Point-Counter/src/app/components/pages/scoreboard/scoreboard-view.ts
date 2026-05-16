import { CommonModule } from '@angular/common';
import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';
import { RouterLink } from '@angular/router';
import { forkJoin } from 'rxjs';
import { switchMap } from 'rxjs/operators';

import { ScoreboardService } from '../../../services/ScoreBoard.service';
import { GameService } from '../../../services/Game.service';
import { NotificationService } from '../../../services/notification.service';
import { getApiErrorMessage } from '../../../utils/http-error.util';
import { Scoreboard as ScoreboardModel } from '../../../models/scoreboard.model';
import { Game } from '../../../models/game.model';

@Component({
  selector: 'app-scoreboard-view',
  imports: [CommonModule, RouterLink],
  templateUrl: './scoreboard-view.html',
  styleUrl: './scoreboard.css',
})
export class ScoreboardView implements OnChanges {
  @Input({ required: true }) gameId!: number;
  @Input() embedded = false;

  game: Game | null = null;
  scores: ScoreboardModel[] = [];
  private baselineScores: ScoreboardModel[] = [];
  dirty = false;
  saving = false;
  loading = false;
  loadError: string | null = null;

  constructor(
    private gameService: GameService,
    private scoreboardService: ScoreboardService,
    private notifications: NotificationService
  ) {}

  ngOnChanges(changes: SimpleChanges): void {
    const idCh = changes['gameId'];
    if (!idCh || this.gameId == null || !Number.isFinite(this.gameId) || this.gameId < 1) {
      return;
    }
    if (idCh.firstChange || idCh.previousValue !== idCh.currentValue) {
      this.loadGameAndScores();
    }
  }

  loadGameAndScores(): void {
    this.loading = true;
    this.loadError = null;
    forkJoin({
      game: this.gameService.getById(this.gameId),
      scores: this.scoreboardService.getByGameId(this.gameId),
    }).subscribe({
      next: ({ game, scores }) => {
        this.game = game;
        this.applyScoresFromServer(scores);
        this.loading = false;
      },
      error: err => {
        console.error(err);
        this.loading = false;
        this.loadError = getApiErrorMessage(err);
      },
    });
  }

  private applyScoresFromServer(rows: ScoreboardModel[]): void {
    this.scores = rows.map(s => ({ ...s }));
    this.baselineScores = rows.map(s => ({ ...s }));
    this.dirty = false;
  }

  private recomputeDirty(): void {
    this.dirty =
      this.scores.length !== this.baselineScores.length ||
      this.scores.some(s => {
        const b = this.baselineScores.find(
          x => x.teamId === s.teamId && x.gameId === s.gameId
        );
        return !b || b.score !== s.score;
      });
  }

  saveChanges(): void {
    if (!this.dirty || this.saving || this.scores.length === 0) {
      return;
    }

    const changed = this.scores.filter(s => {
      const b = this.baselineScores.find(
        x => x.teamId === s.teamId && x.gameId === s.gameId
      );
      return !b || b.score !== s.score;
    });

    if (changed.length === 0) {
      this.dirty = false;
      return;
    }

    this.saving = true;
    forkJoin(
      changed.map(s =>
        this.scoreboardService.updateScore({
          gameId: s.gameId,
          teamId: s.teamId,
          score: s.score,
        })
      )
    )
      .pipe(switchMap(() => this.scoreboardService.getByGameId(this.gameId)))
      .subscribe({
        next: rows => {
          this.applyScoresFromServer(rows);
          this.saving = false;
          this.notifications.show('Dina ändringar har sparats.', 'success');
        },
        error: err => {
          console.error(err);
          this.saving = false;
          this.notifications.show(getApiErrorMessage(err), 'error');
        },
      });
  }

  increase(score: ScoreboardModel): void {
    if (this.saving) return;
    score.score += 1;
    this.recomputeDirty();
  }

  decrease(score: ScoreboardModel): void {
    if (this.saving) return;
    if (score.score === 0) return;
    score.score -= 1;
    this.recomputeDirty();
  }
}
