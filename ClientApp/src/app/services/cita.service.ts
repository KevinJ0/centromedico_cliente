import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { cita, citaResult, UserInfo } from '../interfaces/InterfacesDto';
import { BehaviorSubject, Observable, of, throwError } from 'rxjs';
import { map, catchError } from 'rxjs/operators';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class CitaService {
  baseUrl: string;
  _citaResult: any;
  prueba: string;
  // Url to access our Web API’s

  constructor(private router: Router, private http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.baseUrl = baseUrl;
  }


  GetNewCita(medicoID: number): Observable<any> {

    try {
      return this.http.get<any>(this.baseUrl +
        `/api/citas/getNewCita?medicoID=${medicoID}`)
        .pipe(map(result => {
          return result;
        }));
    } catch (error) {
      console.log('Ha ocurrido un error al tratar de obtener los datos de la cita: ', error);
      return of([]);
    }
  }

  GetTimeList(fecha: Date, medicoID: number): Observable<any> {

    try {
      return this.http.get<any>(this.baseUrl +
        `/api/citas/GetTimeList?fecha_hora=${fecha.toISOString()}&medicoID=${medicoID}`)
        .pipe(map(result => {
          return result;
        }), catchError(err => {
          console.log('Ha ocurrido un error al tratar de obtener la lista de horas: ', err);
          return of([]);
        }));

    } catch (error) {
      console.log('Ha ocurrido un error al tratar de obtener la lista de horas: ', error);
      return of([]);
    }
  }

  CreateCita(_cita: cita): Observable<citaResult> {
    console.info(_cita);
    try {
      return this.http.post<citaResult>(this.baseUrl +
        `/api/citas/createCita`, _cita)
        .pipe(map(result => {
          this._citaResult = result;
          return result;
        }), catchError(err => {
          return throwError(err);
        }));

    } catch (err) {
      console.log('Ha ocurrido un error al  : ', err.error);
      return throwError(err);
    }
  }
}
