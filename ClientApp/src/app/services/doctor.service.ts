import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { doctorCard, medico } from '../interfaces/InterfacesDto';
import { BehaviorSubject, Observable, of, throwError } from 'rxjs';
import { map, catchError } from 'rxjs/operators';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class DoctorService {

  baseUrl: string;

  constructor(private http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.baseUrl = baseUrl;

  }


  SearchMedicos(nombre: string = "", especialidadId: string = "", seguroId: string = ""): Observable<doctorCard[]> {

    try {

      return this.http.get<doctorCard[]>(this.baseUrl +
        `/api/medicos?nombre=${nombre}&especialidadID=${especialidadId}&seguroID=${seguroId}`)
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



  GetMedicoById(medicoId: number): medico {

    try {

      return this.http.get<medico>(this.baseUrl +
        `/api/medicos/${medicoId}`)
        .subscribe(r => {
          console.log(r)
          return r;
        }, err => {
          console.log('Ha ocurrido un error al tratar de obtener al médico: ', err);
          return of();
        });

    } catch (error) {
      console.log('Ha ocurrido un error al tratar de obtener los médicos: ', error);
      return of();
    }
  }
}
