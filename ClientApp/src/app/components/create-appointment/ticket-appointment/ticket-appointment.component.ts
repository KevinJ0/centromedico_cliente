import { Router } from '@angular/router';
import { Component, OnInit } from '@angular/core';
import { citaResult } from '../../../interfaces/InterfacesDto';
import { CitaService } from '../../../services/cita.service';
import * as _moment from 'moment';
const moment = _moment;

@Component({
  selector: 'app-ticket-appointment',
  templateUrl: './ticket-appointment.component.html',
  styleUrls: ['./ticket-appointment.component.css']
})

export class TicketAppointmentComponent implements OnInit {

  citaDataResult: citaResult;

  constructor(private router: Router, public citaSvc: CitaService) {
    moment.locale('es');

    this.citaDataResult = this.citaSvc._citaResult;

    if (this.citaDataResult != null) {
      if (this.citaDataResult.doc_identidad_tutor)
      {
        this.isDependent = true;
      }} else {
      this.router.navigate(['create-cita']);
    }

    let fechaHora: string = this.citaDataResult.fecha_hora;
    this.citaDataResult.fecha_hora = _moment(fechaHora).utc().format('dddd DD MMM Y hh:mm A');
  }

  isDependent: boolean = true;

  ngOnInit(): void {

  }
}
