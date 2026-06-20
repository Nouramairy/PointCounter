import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

import { ScoreboardView } from './scoreboard-view';

@Component({
  selector: 'app-scoreboard',
  imports: [ScoreboardView],
  template: `
    @if (gameId > 0) {
      <app-scoreboard-view [gameId]="gameId" [embedded]="false" />
    } @else {
      <p class="error">Invalid match ID.</p>
    }
  `,
  styles: [
    `
      .error {
        color: #b91c1c;
      }
    `,
  ],
})
export class Scoreboard implements OnInit {
  gameId = 0;

  constructor(private route: ActivatedRoute) {}

  ngOnInit(): void {
    const raw = this.route.snapshot.paramMap.get('gameId');
    const id = Number(raw);
    if (raw && Number.isFinite(id) && id >= 1) {
      this.gameId = id;
    }
  }
}
