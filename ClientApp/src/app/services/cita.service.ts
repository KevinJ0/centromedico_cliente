import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { UserInfo } from '../interfaces/InterfacesDto';
import { BehaviorSubject, Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class CitaService {
  baseUrl: string;

  // Url to access our Web API’s

  constructor(private router: Router, private http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.baseUrl = baseUrl;
  }
  

  GetNewCita(medicoID: number): Observable<any> {

    return this.http.get<any>(this.baseUrl +
        `/api/citas/getNewCita?medicoID=${medicoID}`)
        .pipe(map(result => {
        return result;
      }));
  }
}
