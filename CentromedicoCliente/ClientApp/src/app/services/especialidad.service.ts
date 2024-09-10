import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { doctorCard, especialidad } from '../interfaces/InterfacesDto';
import { map, Observable, of } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class EspecialidadService {
  baseUrl: string;

  constructor(private http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.baseUrl = baseUrl;

  }


  GetEspecialidades(): Observable<especialidad[]> {

    try {
      return this.http.get<especialidad[]>(this.baseUrl +
        `api/especialidades/getAllEspecialidades`)
        .pipe(map(result => {
          console.log(result)
          return result;
        }));

    } catch (error) {
      console.log('Ha ocurrido un error al tratar de obtener las especialidades disponibles: ', error);
      return of([]);
    }
  }

}
