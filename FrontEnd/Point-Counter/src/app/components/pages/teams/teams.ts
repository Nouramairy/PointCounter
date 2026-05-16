import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { TeamService } from '../../../services/Team.service';
import { PlayerService } from '../../../services/player.service';
import { NotificationService } from '../../../services/notification.service';
import { getApiErrorMessage } from '../../../utils/http-error.util';

import { CreateTeam, Team, UpdateTeam } from '../../../models/team.model';
import { Player } from '../../../models/player.model';

@Component({
  selector: 'app-teams',
  imports: [CommonModule, FormsModule],
  templateUrl: './teams.html',
  styleUrl: './teams.css',
})
export class Teams implements OnInit {
  teams: Team[] = [];
  players: Player[] = [];
  editingTeamId: number | null = null;

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
    this.loadTeams();
    this.loadPlayers();
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
        this.notifications.show('Laget har skapats.', 'success');
        this.loadTeams();

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
        this.notifications.show('Laget har uppdaterats.', 'success');
        this.cancelEdit();
        this.loadTeams();
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
        this.notifications.show('Laget har tagits bort.', 'success');
        this.loadTeams();
      },
      error: err => {
        console.error(err);
        this.notifications.show(getApiErrorMessage(err), 'error');
      }
    });
  }
}