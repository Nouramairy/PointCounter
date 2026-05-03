import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PlayerEditorComponent } from './player-editor-component';

describe('PlayerEditorComponent', () => {
  let component: PlayerEditorComponent;
  let fixture: ComponentFixture<PlayerEditorComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PlayerEditorComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PlayerEditorComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
