import { Router } from '@angular/router';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { citaResult } from '../../../interfaces/InterfacesDto';
import { CitaService } from '../../../services/cita.service';
import * as _moment from 'moment';
const moment = _moment;
import { AutoUnsubscribe } from "ngx-auto-unsubscribe";

@AutoUnsubscribe()
@Component({
  selector: 'app-ticket-appointment',
  templateUrl: './ticket-appointment.component.html',
  styleUrls: ['./ticket-appointment.component.css']
})

export class TicketAppointmentComponent implements OnInit {

  citaDataResult: citaResult;
  isDependent: boolean;

  constructor(private router: Router, public citaSvc: CitaService) {
    moment.locale('es');

    this.citaDataResult = this.citaSvc._citaResult;
    console.log(this.citaDataResult)
    if (this.citaDataResult != null) {
      if (this.citaDataResult.doc_identidad_tutor) {
        console.log("is dependent: " + this.citaDataResult.doc_identidad_tutor)
        this.isDependent = true;
      }
    } else {
      this.router.navigate(['..']);
    }

    let fechaHora: string = this.citaDataResult.fecha_hora;
    this.citaDataResult.fecha_hora = _moment(fechaHora).utc().format('dddd DD MMM Y hh:mm A');
  }


  ngOnInit(): void {

  }
  ngOnDestroy() {

  }
}
