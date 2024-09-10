

import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { CorreoPregunta } from '../interfaces/InterfacesDto';
 import { map, catchError } from 'rxjs/operators';
import { Observable, throwError } from 'rxjs';
 
@Injectable({
  providedIn: 'root'
})
export class PreguntaService {

  baseUrl: string;

  constructor(private http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.baseUrl = baseUrl;

  }

  SendQuestion(formdata: CorreoPregunta): Observable<any> {
 
    return this.http.post<any>(this.baseUrl +
        "api/preguntas/sendQuestion", formdata)
        .pipe(map((result: any) => {
          return result;
        }), catchError(err => {
          return throwError(err);
        }));
    
  }
}
