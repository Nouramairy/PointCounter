import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of } from 'rxjs';

import { ScoreboardView } from './scoreboard-view';
import { GameService } from '../../../services/Game.service';
import { ScoreboardService } from '../../../services/ScoreBoard.service';
import { NotificationService } from '../../../services/notification.service';

describe('ScoreboardView', () => {
  let fixture: ComponentFixture<ScoreboardView>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ScoreboardView],
      providers: [
        {
          provide: GameService,
          useValue: {
            getById: () => of({ id: 1, name: 'Testmatch', duration: 30, teams: ['A'] })
          }
        },
        {
          provide: ScoreboardService,
          useValue: {
            getByGameId: () =>
              of([
                {
                  id: 0,
                  gameId: 1,
                  gameName: 'Testmatch',
                  teamId: 10,
                  teamName: 'Lag A',
                  score: 0
                }
              ]),
            updateScore: () =>
              of({
                id: 1,
                gameId: 1,
                gameName: 'Testmatch',
                teamId: 10,
                teamName: 'Lag A',
                score: 1
              })
          }
        },
        {
          provide: NotificationService,
          useValue: { show: () => {} }
        }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(ScoreboardView);
    fixture.componentInstance.gameId = 1;
    fixture.componentInstance.embedded = false;
    fixture.detectChanges();
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(fixture.componentInstance).toBeTruthy();
  });
});
