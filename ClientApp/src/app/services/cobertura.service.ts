import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { cobertura, seguro } from '../interfaces/InterfacesDto';

@Injectable({
  providedIn: 'root'
})
export class CoberturaService {

  baseUrl: string;

  // Url to access our Web API’s
  private baseEndPoint: string = "/api/citas/getCoberturas";

  constructor(private http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.baseUrl = baseUrl;

  }

  GetCobertura(medicoID: number, seguroID: number, servicioID: number): Observable<cobertura> {

    return this.http.get<cobertura>(this.baseUrl + this.baseEndPoint +
      `?servicioID=${servicioID}&medicoID=${medicoID}&seguroID=${seguroID}`)
      .pipe(map((result: cobertura) => {
        return result;
      }));
  }

  GetSegurosByServicio(medicoID: number, servicioID: number): Observable<seguro[]> {

    return this.http.get<seguro[]>(this.baseUrl +  
      `/api/seguros/GetSegurosByServicio?servicioID=${servicioID}&medicoID=${medicoID}`)
      .pipe(map((result: seguro[]) => {
        return result;
      }));
  }
}
