import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MatchPage } from './match-page';

describe('MatchPage', () => {
  let component: MatchPage;
  let fixture: ComponentFixture<MatchPage>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MatchPage]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MatchPage);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
