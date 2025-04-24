import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, map, Observable, of } from 'rxjs';
import { group, turno, turno_paciente } from '../interfaces/InterfacesDto';

@Injectable({
  providedIn: 'root'

})
export class TurnosMedicoService {


  baseUrl: string;
  _citaResult: any;

  constructor(private router: Router, private http: HttpClient, @Inject("DOCTOR_URL") DoctorApiUrl: string) {
    this.baseUrl = DoctorApiUrl;
  }

  GetGrupoTurnos(medicoId: number): Observable<group> {
    try {

      let res = this.http.get<group>(this.baseUrl +
        `api/grupos/getGrupoTurno?medicoID=${medicoId}`)
        .pipe(
          map((result) => { return result }),
          catchError(err => {
            console.log('Ha ocurrido un error al tratar de obtener el nombre del grupo para la notificación: ', err);
            return of(null);
          })
        );

      return res;

    } catch (error) {
      console.log('Ha ocurrido un error al tratar de obtener los médicos: ', error);
      return of(null);
    }
  }

  GetPacienteTurno(medicoId: number, fecha_hora_cita: String): Observable<turno_paciente> {

    try {

      let res = this.http.get<turno_paciente>(this.baseUrl +
        `api/turnos/getTurnoPaciente?medicosID=${medicoId}&fecha_hora_cita=${fecha_hora_cita}`)
        .pipe(
          map((result) => { return result }),
          catchError(err => {
            console.log('Ha ocurrido un error al tratar de obtener el turno: ', err);
            return of(null);
          })
        );

      return res;

    } catch (error) {
      console.log('Ha ocurrido un error al tratar de obtener el turno: ', error);
      return of(null);
    }


  }


}
