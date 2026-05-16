import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';

import { PointMatchService } from '../../../services/point-match.service';
import { PointMatch, PointMatchPlayer } from '../../../models/point-match.model';

@Component({
  selector: 'app-match-page',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './match-page.html',
  styleUrl: './match-page.css'
})
export class MatchPage implements OnInit {
  publicId = '';
  match: PointMatch | null = null;
  newPlayerName = '';

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private pointMatchService: PointMatchService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      this.publicId = params.get('publicId') ?? '';

      if (!this.publicId) {
        this.router.navigate(['/404']);
        return;
      }

      this.loadMatch();
    });
  }

  loadMatch(): void {
    this.pointMatchService.getMatch(this.publicId).subscribe({
      next: match => {
        this.match = match;
        this.cdr.detectChanges();
      },
      error: err => {
        console.error('Kunde inte hämta matchen:', err);
        this.router.navigate(['/404']);
      }
    });
  }

  updateScore(player: PointMatchPlayer, score: number): void {
    this.pointMatchService.updateScore(this.publicId, player.id, Number(score)).subscribe({
      next: match => {
        this.match = match;
        this.cdr.detectChanges();
      },
      error: err => console.error(err)
    });
  }

  increase(player: PointMatchPlayer): void {
    this.updateScore(player, player.score + 1);
  }

  decrease(player: PointMatchPlayer): void {
    this.updateScore(player, player.score - 1);
  }

  addPlayer(): void {
    const name = this.newPlayerName.trim();

    if (!name) return;

    this.pointMatchService.addPlayer(this.publicId, name).subscribe({
      next: match => {
        this.match = match;
        this.newPlayerName = '';
        this.cdr.detectChanges();
      },
      error: err => console.error(err)
    });
  }

  updatePlayerName(player: PointMatchPlayer, name: string): void {
    const trimmedName = name.trim();

    if (!trimmedName) return;

    this.pointMatchService.updatePlayerName(this.publicId, player.id, trimmedName).subscribe({
      next: match => {
        this.match = match;
        this.cdr.detectChanges();
      },
      error: err => console.error(err)
    });
  }

  restart(): void {
    this.pointMatchService.restart(this.publicId).subscribe({
      next: match => {
        this.match = match;
        this.cdr.detectChanges();
      },
      error: err => console.error(err)
    });
  }

  clone(): void {
    this.pointMatchService.clone(this.publicId).subscribe({
      next: match => this.router.navigate(['/spel', match.publicId]),
      error: err => console.error(err)
    });
  }

  get sortedPlayers(): PointMatchPlayer[] {
    if (!this.match) return [];

    return [...this.match.players].sort((a, b) =>
      this.match!.higherScoreWins
        ? b.score - a.score
        : a.score - b.score
    );
  }
}