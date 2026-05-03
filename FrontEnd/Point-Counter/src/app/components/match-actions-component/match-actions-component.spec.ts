import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MatchActionsComponent } from './match-actions-component';

describe('MatchActionsComponent', () => {
  let component: MatchActionsComponent;
  let fixture: ComponentFixture<MatchActionsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MatchActionsComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MatchActionsComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
