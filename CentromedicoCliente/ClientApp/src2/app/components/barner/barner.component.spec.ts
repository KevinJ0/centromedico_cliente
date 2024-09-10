import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BarnerComponent } from './barner.component';

describe('BarnerComponent', () => {
  let component: BarnerComponent;
  let fixture: ComponentFixture<BarnerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ BarnerComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(BarnerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
