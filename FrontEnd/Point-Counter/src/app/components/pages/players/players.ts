import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { PlayerService } from '../../../services/player.service';
import { NotificationService } from '../../../services/notification.service';
import { getApiErrorMessage } from '../../../utils/http-error.util';
import { CreatePlayer, Player, UpdatePlayer } from '../../../models/player.model';

@Component({
  selector: 'app-players',
  imports: [CommonModule, FormsModule],
  templateUrl: './players.html',
  styleUrl: './players.css',
})
export class Players implements OnInit {
  players: Player[] = [];
  editingPlayerId: number | null = null;

  newPlayer: CreatePlayer = {
    name: '',
    age: 0,
    address: '',
    phone: ''
  };
  editPlayer: UpdatePlayer = {
    name: '',
    age: 0,
    address: '',
    phone: ''
  };

  constructor(
    private playerService: PlayerService,
    private notifications: NotificationService
  ) {}

  ngOnInit(): void {
    this.loadPlayers();
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

  createPlayer(): void {
    this.playerService.create(this.newPlayer).subscribe({
      next: () => {
        this.notifications.show('Spelaren har lagts till.', 'success');
        this.loadPlayers();

        this.newPlayer = {
          name: '',
          age: 0,
          address: '',
          phone: ''
        };
      },
      error: err => {
        console.error(err);
        this.notifications.show(getApiErrorMessage(err), 'error');
      }
    });
  }

  startEdit(player: Player): void {
    this.editingPlayerId = player.id;
    this.editPlayer = {
      name: player.name,
      age: player.age,
      address: player.address,
      phone: player.phone
    };
  }

  cancelEdit(): void {
    this.editingPlayerId = null;
    this.editPlayer = {
      name: '',
      age: 0,
      address: '',
      phone: ''
    };
  }

  savePlayer(): void {
    if (this.editingPlayerId === null) {
      return;
    }

    this.playerService.update(this.editingPlayerId, this.editPlayer).subscribe({
      next: () => {
        this.notifications.show('Spelaren har uppdaterats.', 'success');
        this.cancelEdit();
        this.loadPlayers();
      },
      error: err => {
        console.error(err);
        this.notifications.show(getApiErrorMessage(err), 'error');
      }
    });
  }

  deletePlayer(id: number): void {
    this.playerService.delete(id).subscribe({
      next: () => {
        this.notifications.show('Spelaren har tagits bort.', 'success');
        this.loadPlayers();
      },
      error: err => {
        console.error(err);
        this.notifications.show(getApiErrorMessage(err), 'error');
      }
    });
  }
}