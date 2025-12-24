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
import { fadeInAnimation } from '../../animations/animations';

@AutoUnsubscribe()
@Component({
  selector: 'app-medical-directory',
  templateUrl: './medical-directory.component.html',
  styleUrls: ['./medical-directory.component.css']
})
export class MedicalDirectoryComponent implements OnInit {
  isSent: Boolean = false;
  loading: boolean = true;

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
  displayDoctorsCount: number = 0;
  pageLength: number;
  pageSize: number = 10;
  pageSizeOptions = [10, 25, 50, 100];
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
              this.animateCount(this.doctorsCount);
            } else {
              this.doctorsCount = 0;
              this.pageLength = 0;
              this.splicedData = [];
              this.doctorCardList = [];
              this.showNoContent = true;
              this.displayDoctorsCount = 0;
            }
          }, err => {
            console.error('Error al intentar acceder a la lista de médicos: ' + err);
          }, () => {
            this.isSent = false;
            this.loading = false;
          });

      } catch (error) {
        console.error('Error al intentar acceder a la lista de médicos: ' + error);
      }
    }
  }

  animateCount(target: number) {
    let start = 0;
    const duration = 1500; // 1.5 seconds
    const stepTime = Math.abs(Math.floor(duration / target));

    // Fallback for very small targets or 0
    if (target === 0) {
      this.displayDoctorsCount = 0;
      return;
    }

    const timer = setInterval(() => {
      start += 1;
      this.displayDoctorsCount = start;
      if (start >= target) {
        this.displayDoctorsCount = target;
        clearInterval(timer);
      }
    }, Math.max(stepTime, 20)); // Minimum 20ms to avoid freezing
  }


  pageChangeEvent(event) {
    const offset = ((event.pageIndex + 1) - 1) * event.pageSize;
    this.splicedData = this.doctorCardList.slice(offset).slice(0, event.pageSize);
  }
  ngOnDestroy() {

  }
}
