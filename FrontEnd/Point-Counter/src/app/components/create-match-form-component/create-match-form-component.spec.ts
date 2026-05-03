import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateMatchFormComponent } from './create-match-form-component';

describe('CreateMatchFormComponent', () => {
  let component: CreateMatchFormComponent;
  let fixture: ComponentFixture<CreateMatchFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CreateMatchFormComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CreateMatchFormComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
