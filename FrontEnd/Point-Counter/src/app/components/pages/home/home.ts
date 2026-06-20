import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';

import { PointMatchService } from '../../../services/point-match.service';
import { CreatePointMatch } from '../../../models/point-match.model';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './home.html',
  styleUrl: './home.css'
})
export class Home {

  playerName = '';

  newMatch: CreatePointMatch = {
    name: '',
    higherScoreWins: true,
    startingScore: 0,
    playersLocked: false,
    playerNames: []
  };

  constructor(
    private pointMatchService: PointMatchService,
    private router: Router
  ) {}

  addPlayer(): void {

    const trimmedName = this.playerName.trim();

    if (!trimmedName) return;

    this.newMatch.playerNames.push(trimmedName);

    this.playerName = '';
  }

  removePlayer(index: number): void {
    this.newMatch.playerNames.splice(index, 1);
  }

  createMatch(): void {

    console.log(this.newMatch);

    this.pointMatchService.createMatch(this.newMatch).subscribe({

      next: match => {

        console.log(match);

        this.router.navigate(['/match', match.publicId]);
      },

      error: err => {
        console.error(err);
      }
    });
  }
}
