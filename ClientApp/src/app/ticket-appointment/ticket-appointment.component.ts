import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-ticket-appointment',
  templateUrl: './ticket-appointment.component.html',
  styleUrls: ['./ticket-appointment.component.css']
})
export class TicketAppointmentComponent implements OnInit {

  constructor() { }
  isDependent:boolean = false;
  ngOnInit(): void {

  }

}
