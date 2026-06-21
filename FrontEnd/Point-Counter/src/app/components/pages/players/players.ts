import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Subscription } from 'rxjs';
import { finalize } from 'rxjs/operators';

import { PlayerService } from '../../../services/player.service';
import { NotificationService } from '../../../services/notification.service';
import { getApiErrorMessage } from '../../../utils/http-error.util';
import { CreatePlayer, Player, UpdatePlayer } from '../../../models/player.model';

type PlayerForm = Omit<CreatePlayer, 'age'> & { age: number | null };
const LOAD_TIMEOUT_MS = 4000;

@Component({
  selector: 'app-players',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './players.html',
  styleUrl: './players.css',
})
export class Players {
  players: Player[] = [];
  editingPlayerId: number | null = null;
  showPlayers = false;
  loadingPlayers = false;
  playersLoaded = false;
  playersLoadError: string | null = null;

  newPlayer: PlayerForm = {
    name: '',
    age: null,
    address: '',
    phone: ''
  };
  editPlayer: PlayerForm = {
    name: '',
    age: null,
    address: '',
    phone: ''
  };

  constructor(
    private playerService: PlayerService,
    private notifications: NotificationService
  ) {}

  showAllPlayers(): void {
    if (this.loadingPlayers) {
      return;
    }

    this.showPlayers = true;
    this.loadPlayers();
  }

  loadPlayers(): void {
    this.loadingPlayers = true;
    this.playersLoadError = null;
    let request: Subscription | null = null;
    const timeoutId = window.setTimeout(() => {
      this.playersLoaded = false;
      this.playersLoadError = 'Could not load players. Check that the API is running.';
      this.notifications.show(this.playersLoadError, 'error');
      request?.unsubscribe();
    }, LOAD_TIMEOUT_MS);

    request = this.playerService.getAll().pipe(
      finalize(() => {
        window.clearTimeout(timeoutId);
        this.loadingPlayers = false;
      })
    ).subscribe({
      next: players => {
        this.players = players;
        this.playersLoaded = true;
      },
      error: err => {
        console.error(err);
        this.playersLoaded = false;
        this.playersLoadError = getApiErrorMessage(err);
        this.notifications.show(this.playersLoadError, 'error');
      }
    });
  }

  createPlayer(): void {
    if (this.newPlayer.age === null) {
      this.notifications.show('Enter an age between 1 and 120.', 'error');
      return;
    }

    const player: CreatePlayer = {
      ...this.newPlayer,
      name: this.newPlayer.name.trim(),
      address: this.newPlayer.address.trim(),
      phone: this.newPlayer.phone.trim(),
      age: this.newPlayer.age
    };

    this.playerService.create(player).subscribe({
      next: () => {
        this.notifications.show('The player has been added.', 'success');
        if (this.showPlayers) {
          this.loadPlayers();
        }

        this.newPlayer = {
          name: '',
          age: null,
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
      age: null,
      address: '',
      phone: ''
    };
  }

  savePlayer(): void {
    if (this.editingPlayerId === null) {
      return;
    }

    if (this.editPlayer.age === null) {
      this.notifications.show('Enter an age between 1 and 120.', 'error');
      return;
    }

    const player: UpdatePlayer = {
      ...this.editPlayer,
      name: this.editPlayer.name.trim(),
      address: this.editPlayer.address.trim(),
      phone: this.editPlayer.phone.trim(),
      age: this.editPlayer.age
    };

    this.playerService.update(this.editingPlayerId, player).subscribe({
      next: () => {
        this.notifications.show('The player has been updated.', 'success');
        this.cancelEdit();
        if (this.showPlayers) {
          this.loadPlayers();
        }
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
        this.notifications.show('The player has been deleted.', 'success');
        this.loadPlayers();
      },
      error: err => {
        console.error(err);
        this.notifications.show(getApiErrorMessage(err), 'error');
      }
    });
  }
}
