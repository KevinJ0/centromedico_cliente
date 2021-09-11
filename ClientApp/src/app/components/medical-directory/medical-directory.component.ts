import { Component, OnInit, OnDestroy } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { catchError } from 'rxjs/operators';
import { Observable, of } from 'rxjs';
import { doctorCard, especialidad, seguro } from 'src/app/interfaces/InterfacesDto';
import { DoctorService } from 'src/app/services/doctor.service';
import { EspecialidadService } from 'src/app/services/especialidad.service';
import { SeguroService } from 'src/app/services/seguro.service';
import { ProgressSpinnerMode } from '@angular/material/progress-spinner';
import { trigger, state, style, animate, transition } from '@angular/animations';
import { AutoUnsubscribe } from "ngx-auto-unsubscribe";

@AutoUnsubscribe()
@Component({
  selector: 'app-medical-directory',
  templateUrl: './medical-directory.component.html',
  styleUrls: ['./medical-directory.component.css'],
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
export class MedicalDirectoryComponent implements OnInit {
  isSent: Boolean = false;

  constructor(
    private doctorSvc: DoctorService,
    private especialidadSvc: EspecialidadService,
    private seguroSvc: SeguroService,
    private _formBuilder: FormBuilder) { }

  prueba: doctorCard;
  doctorCardList: doctorCard[];
  splicedData: doctorCard[];
  seguros$: Observable<seguro[]>;
  especialidades$: Observable<especialidad[]>;
  searchFormGroup: FormGroup;
  doctorsCount: number;
  pageLength: number;
  pageSize: number = 5;
  pageSizeOptions = [5, 10, 50, 100];
  showNoContent: Boolean;
  mode: ProgressSpinnerMode = 'indeterminate';

  ngOnInit(): void {
    this.searchFormGroup = this._formBuilder.group({
      nameControl: [''],
      specialityControl: [''],
      insuranceControl: [''],
    });

    this.Search();

    this.seguros$ = this.seguroSvc.GetSeguros()
      .pipe(catchError(err => {
        console.log('Ha ocurrido un error al tratar de obtener la lista de seguros: ', err);
        return of([]);
      }));

    this.especialidades$ = this.especialidadSvc.GetEspecialidades()
      .pipe(catchError(err => {
        console.log('Ha ocurrido un error al tratar de obtener la lista de seguros: ', err);
        return of([]);
      }));
  }

  Search(): void {

    if (!this.isSent) {
      this.isSent = true;
      let formData = this.searchFormGroup.value;
      let nombre = formData["nameControl"].trim();
      let especialidadId = formData["specialityControl"];
      let seguroId = formData["insuranceControl"];

      try {

        this.doctorSvc.SearchMedicos(nombre, especialidadId, seguroId)
          .subscribe((r: doctorCard[]) => {
            if (r) {
              this.doctorsCount = r.length;
              this.pageLength = r.length;
              this.splicedData = r.slice(0, this.pageSize);
              //console.table(this.splicedData)
              this.doctorCardList = r;
              this.showNoContent = false;
            } else {
              this.doctorsCount = 0;
              this.pageLength = 0;
              this.splicedData = [];
              this.doctorCardList = [];
              this.showNoContent = true;
            }
          }, err => {
            console.error('Error al intentar acceder a la lista de médicos: ' + err);
          }, () => {
            this.isSent = false;
          });

      } catch (error) {
        console.error('Error al intentar acceder a la lista de médicos: ' + error);
      }
    }
  }


  pageChangeEvent(event) {
    const offset = ((event.pageIndex + 1) - 1) * event.pageSize;
    this.splicedData = this.doctorCardList.slice(offset).slice(0, event.pageSize);
  }
  ngOnDestroy() {

  }
}
