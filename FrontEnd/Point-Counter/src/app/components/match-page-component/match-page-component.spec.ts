import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MatchPageComponent } from './match-page-component';

describe('MatchPageComponent', () => {
  let component: MatchPageComponent;
  let fixture: ComponentFixture<MatchPageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MatchPageComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MatchPageComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
