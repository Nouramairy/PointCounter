import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Subscription } from 'rxjs';
import { finalize } from 'rxjs/operators';

import { TeamService } from '../../../services/Team.service';
import { PlayerService } from '../../../services/player.service';
import { NotificationService } from '../../../services/notification.service';
import { getApiErrorMessage } from '../../../utils/http-error.util';

import { CreateTeam, Team, UpdateTeam } from '../../../models/team.model';
import { Player } from '../../../models/player.model';

const LOAD_TIMEOUT_MS = 4000;

@Component({
  selector: 'app-teams',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './teams.html',
  styleUrl: './teams.css',
})
export class Teams implements OnInit {
  teams: Team[] = [];
  players: Player[] = [];
  editingTeamId: number | null = null;
  showTeams = false;
  loadingTeams = false;
  teamsLoadError: string | null = null;

  newTeam: CreateTeam = {
    name: '',
    maximumPlayersAllowed: 0,
    playerIds: []
  };
  editTeam: UpdateTeam = {
    name: '',
    maximumPlayersAllowed: 0,
    playerIds: []
  };

  constructor(
    private teamService: TeamService,
    private playerService: PlayerService,
    private notifications: NotificationService
  ) {}

  ngOnInit(): void {
    this.loadPlayers();
  }

  showAllTeams(): void {
    if (this.loadingTeams) {
      return;
    }

    this.showTeams = true;
    this.loadTeams();
  }

  loadTeams(): void {
    this.loadingTeams = true;
    this.teamsLoadError = null;
    let request: Subscription | null = null;
    const timeoutId = window.setTimeout(() => {
      this.teamsLoadError = 'Could not load teams. Check that the API is running.';
      this.notifications.show(this.teamsLoadError, 'error');
      request?.unsubscribe();
    }, LOAD_TIMEOUT_MS);

    request = this.teamService.getAll().pipe(
      finalize(() => {
        window.clearTimeout(timeoutId);
        this.loadingTeams = false;
      })
    ).subscribe({
      next: teams => {
        this.teams = teams;
      },
      error: err => {
        console.error(err);
        this.teamsLoadError = getApiErrorMessage(err);
        this.notifications.show(this.teamsLoadError, 'error');
      }
    });
  }

  loadPlayers(): void {
    this.playerService.getAll().subscribe({
      next: players => (this.players = players),
      error: err => {
        console.error(err);
        this.notifications.show(getApiErrorMessage(err), 'error');
      }
    });
  }

  togglePlayer(playerId: number): void {
    if (this.newTeam.playerIds.includes(playerId)) {
      this.newTeam.playerIds = this.newTeam.playerIds.filter(id => id !== playerId);
    } else {
      this.newTeam.playerIds.push(playerId);
    }
  }

  createTeam(): void {
    this.teamService.create(this.newTeam).subscribe({
      next: () => {
        this.notifications.show('The team has been created.', 'success');
        if (this.showTeams) {
          this.loadTeams();
        }

        this.newTeam = {
          name: '',
          maximumPlayersAllowed: 0,
          playerIds: []
        };
      },
      error: err => {
        console.error(err);
        this.notifications.show(getApiErrorMessage(err), 'error');
      }
    });
  }

  startEdit(team: Team): void {
    this.teamService.getById(team.id).subscribe({
      next: teamDetails => {
        const selectedIds = this.players
          .filter(player => teamDetails.players.includes(player.name))
          .map(player => player.id);

        this.editingTeamId = team.id;
        this.editTeam = {
          name: teamDetails.name,
          maximumPlayersAllowed: teamDetails.maximumPlayersAllowed,
          playerIds: selectedIds
        };
      },
      error: err => {
        console.error(err);
        this.notifications.show(getApiErrorMessage(err), 'error');
      }
    });
  }

  toggleEditPlayer(playerId: number): void {
    if (this.editTeam.playerIds.includes(playerId)) {
      this.editTeam.playerIds = this.editTeam.playerIds.filter(id => id !== playerId);
    } else {
      this.editTeam.playerIds.push(playerId);
    }
  }

  cancelEdit(): void {
    this.editingTeamId = null;
    this.editTeam = {
      name: '',
      maximumPlayersAllowed: 0,
      playerIds: []
    };
  }

  saveTeam(): void {
    if (this.editingTeamId === null) {
      return;
    }

    this.teamService.update(this.editingTeamId, this.editTeam).subscribe({
      next: () => {
        this.notifications.show('The team has been updated.', 'success');
        this.cancelEdit();
        if (this.showTeams) {
          this.loadTeams();
        }
      },
      error: err => {
        console.error(err);
        this.notifications.show(getApiErrorMessage(err), 'error');
      }
    });
  }

  deleteTeam(id: number): void {
    this.teamService.delete(id).subscribe({
      next: () => {
        this.notifications.show('The team has been deleted.', 'success');
        this.loadTeams();
      },
      error: err => {
        console.error(err);
        this.notifications.show(getApiErrorMessage(err), 'error');
      }
    });
  }
}
