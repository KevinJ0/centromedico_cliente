import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { doctorCard, seguro } from '../interfaces/InterfacesDto';
import { BehaviorSubject, Observable, of, throwError } from 'rxjs';
import { map, catchError } from 'rxjs/operators';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class SeguroService {
  baseUrl: string;

  constructor(private http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.baseUrl = baseUrl;

  }

  GetSegurosByServicio(medicoID: number, servicioID: number): Observable<seguro[]> {

    return this.http.get<seguro[]>(this.baseUrl +  
      `/api/seguros/GetSegurosByServicio?servicioID=${servicioID}&medicoID=${medicoID}`)
      .pipe(map((result: seguro[]) => {
        return result;
      }));
  }
  
  GetSeguros(): Observable<seguro[]> {

    try {

      return this.http.get<seguro[]>(this.baseUrl +
        `/api/seguros/getAllSeguros`)
        .pipe(map(result => {
          console.log(result)
          return result;
        }), catchError(err => {
          console.log('Ha ocurrido un error al tratar de obtener los médicos: ', err);
          return of([]);
        }));

    } catch (error) {
      console.log('Ha ocurrido un error al tratar de obtener los médicos: ', error);
      return of([]);
    }
  }

}
