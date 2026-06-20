import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { finalize, timeout } from 'rxjs/operators';

import { GameService } from '../../../services/Game.service';
import { TeamService } from '../../../services/Team.service';
import { NotificationService } from '../../../services/notification.service';
import { getApiErrorMessage } from '../../../utils/http-error.util';

import { CreateGame, Game, UpdateGame } from '../../../models/game.model';
import { Team } from '../../../models/team.model';

@Component({
  selector: 'app-games',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './games.html',  
  styleUrl: './games.css',
})
export class Games implements OnInit {
  games: Game[] = [];
  teams: Team[] = [];
  editingGameId: number | null = null;
  showGames = false;
  loadingGames = false;
  gamesLoadError: string | null = null;

  newGame: CreateGame = {
    name: '',
    duration: 0,
    teamIds: []
  };
  editGame: UpdateGame = {
    name: '',
    duration: 0
  };

  constructor(
    private gameService: GameService,
    private teamService: TeamService,
    private notifications: NotificationService
  ) {}

  ngOnInit(): void {
    this.loadTeams();
  }

  showAllMatches(): void {
    if (this.loadingGames) {
      return;
    }

    this.showGames = true;
    this.loadGames();
  }

  loadGames(): void {
    this.loadingGames = true;
    this.gamesLoadError = null;

    this.gameService.getAll().pipe(
      timeout(4000),
      finalize(() => (this.loadingGames = false))
    ).subscribe({
      next: games => {
        this.games = games;
      },
      error: err => {
        console.error(err);
        this.gamesLoadError = err instanceof Error && err.name === 'TimeoutError'
          ? 'Could not load matches. Check that the API is running.'
          : getApiErrorMessage(err);
        this.notifications.show(this.gamesLoadError, 'error');
      }
    });
  }

  loadTeams(): void {
    this.teamService.getAll().subscribe({
      next: teams => (this.teams = teams),
      error: err => {
        console.error(err);
        this.notifications.show(getApiErrorMessage(err), 'error');
      }
    });
  }

  toggleTeam(teamId: number): void {
    if (this.newGame.teamIds.includes(teamId)) {
      this.newGame.teamIds = this.newGame.teamIds.filter(id => id !== teamId);
    } else {
      this.newGame.teamIds.push(teamId);
    }
  }

  createGame(): void {
    this.gameService.create(this.newGame).subscribe({
      next: () => {
        this.notifications.show('The match has been created.', 'success');
        if (this.showGames) {
          this.loadGames();
        }

        this.newGame = {
          name: '',
          duration: 0,
          teamIds: []
        };
      },
      error: err => {
        console.error(err);
        this.notifications.show(getApiErrorMessage(err), 'error');
      }
    });
  }

  startEdit(game: Game): void {
    this.editingGameId = game.id;
    this.editGame = {
      name: game.name,
      duration: game.duration
    };
  }

  cancelEdit(): void {
    this.editingGameId = null;
    this.editGame = {
      name: '',
      duration: 0
    };
  }

  saveGame(): void {
    if (this.editingGameId === null) {
      return;
    }

    this.gameService.update(this.editingGameId, this.editGame).subscribe({
      next: () => {
        this.notifications.show('The match has been updated.', 'success');
        this.cancelEdit();
        if (this.showGames) {
          this.loadGames();
        }
      },
      error: err => {
        console.error(err);
        this.notifications.show(getApiErrorMessage(err), 'error');
      }
    });
  }

  deleteGame(id: number): void {
    this.gameService.delete(id).subscribe({
      next: () => {
        this.notifications.show('The match has been deleted.', 'success');
        this.loadGames();
      },
      error: err => {
        console.error(err);
        this.notifications.show(getApiErrorMessage(err), 'error');
      }
    });
  }
}
