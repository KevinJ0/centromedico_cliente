import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TicketAppointmentComponent } from './ticket-appointment.component';

describe('TicketAppointmentComponent', () => {
  let component: TicketAppointmentComponent;
  let fixture: ComponentFixture<TicketAppointmentComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ TicketAppointmentComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(TicketAppointmentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
