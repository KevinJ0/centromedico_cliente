import { Component, OnInit, OnDestroy } from '@angular/core';

import { ProgressSpinnerMode } from '@angular/material/progress-spinner';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { medico } from 'src/app/interfaces/InterfacesDto';
import { DoctorService } from 'src/app/services/doctor.service';
import { trigger, style, animate, transition } from '@angular/animations';
import { NoProfilePhotoPipe } from '../../Pipes/no-imagen.pipe';
import * as _moment from 'moment';
import { catchError, finalize } from 'rxjs/operators';
import { AutoUnsubscribe } from "ngx-auto-unsubscribe";
const moment = _moment;

@AutoUnsubscribe()
@Component({
  selector: 'app-doctor',
  templateUrl: './doctor.component.html',
  styleUrls: ['./doctor.component.css'],
  animations: [
    trigger(
      'inOutAnimation',
      [
        transition(
          ':enter',
          [
            style({ opacity: 0 }),
            animate('300ms ease-out',
              style({ opacity: 1 }))
          ]
        ),
        transition(
          ':leave',
          [
            style({ opacity: 1, position: "absolute" }),
            animate('300ms ease-in',
              style({ opacity: 0, position: "relative" }))
          ]
        )
      ]
    )
  ]
})
export class DoctorComponent implements OnInit {


  constructor(
    private rutaActiva: ActivatedRoute,
    private doctorSvc: DoctorService,
    private router: Router) { }


  loading: boolean = true;
  _medico: medico;
  mode: ProgressSpinnerMode = 'indeterminate';
  medicoId: number;
  _especialidades: string = "";

  ngOnInit(): void {

    this.rutaActiva.params.subscribe(
      (params: Params) => {
        this.medicoId = params.id;

        this.doctorSvc.GetMedicoById(this.medicoId)
          .pipe(catchError(err => {
            this.loading = false;
            console.error('Error: ' + err);
            return null;
          }), finalize(() => {
            this.loading = false;
          }))
          .subscribe((r: medico) => {
            this._medico = r;

            if (r) {

              let arrLength: number = r.especialidades.length - 1;

              r.especialidades.map((value, index) => {
                if (index != arrLength)
                  this._especialidades += value + ", ";
                else
                  this._especialidades += value;
              })
            }
          }
          );
      }
    );
  }

  Cita(): void {
    try {
      this.router.navigate(['/crear-cita', { medicoId: this.medicoId }]);
    } catch (error) {
      console.error(error);
    }
  }
  ngOnDestroy() {

  }
}
