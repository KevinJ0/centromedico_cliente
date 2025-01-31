import { Component, Inject, OnInit, OnDestroy } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, ValidationErrors, Validators } from '@angular/forms';
import { Observable, BehaviorSubject, Subject, of } from 'rxjs';
import { ActivatedRoute, Router } from '@angular/router';
import { BreakpointObserver } from '@angular/cdk/layout';
import { map, startWith, debounceTime, switchMap, catchError, finalize } from 'rxjs/operators';
import * as _moment from 'moment';
import { MatSnackBar, MatSnackBarConfig } from '@angular/material/snack-bar';
import { CoberturaService } from 'src/app/services/cobertura.service';
import { CitaService } from 'src/app/services/cita.service';
import { AccountService } from 'src/app/services/account.service';
import { citaCard } from 'src/app/interfaces/InterfacesDto';
import { SeguroService } from 'src/app/services/seguro.service';
import { ProgressSpinnerMode } from '@angular/material/progress-spinner';
import { trigger, state, style, animate, transition } from '@angular/animations';

const moment = _moment;
import { AutoUnsubscribe } from "ngx-auto-unsubscribe";

@AutoUnsubscribe()
@Component({
  selector: 'app-list-appointment',
  templateUrl: './list-appointment.component.html',
  styleUrls: ['./list-appointment.component.css'],
  animations: [
    trigger(
      'inOutAnimation',
      [
        transition(
          ':enter',
          [
            style({ opacity: 0 }),
            animate('900ms ease-out',
              style({ opacity: 1 }))
          ]
        ),
        transition(
          ':leave',
          [
            style({ opacity: 1, position: "absolute" }),
            animate('900ms ease-in',
              style({ opacity: 0, position: "relative" }))
          ]
        )
      ]
    )
  ]
})
export class ListAppointmentComponent implements OnInit {

  showNoContent: boolean = false;
  loading: boolean = true;
  citaCardList$: Observable<citaCard[]>;
  mode: ProgressSpinnerMode = 'indeterminate';

  constructor(

    private router: Router,
    private rutaActiva: ActivatedRoute,
    private _snackBar: MatSnackBar,
    private coberturaSvc: CoberturaService,
    private seguroSvc: SeguroService,
    public citaSvc: CitaService, private accountSvc: AccountService,
    private _formBuilder: FormBuilder, breakpointObserver: BreakpointObserver) {

  }

  navigateToMedicos() {
    this.router.navigate(['/medicos']); // Cambia '/ruta-deseada' por la ruta a la que quieras redirigir
  }

  ngOnInit(): void {
    this.citaCardList$ = this.citaSvc.GetCitaList();
    this.citaCardList$
      .subscribe((r) => {
        this.loading = false;
        if (!r)
          this.citaCardList$ = null;
        console.log(this.citaCardList$)
      }, (err) => {
        this.loading = false;
        this.showNoContent = true;
        console.error(err);
      });
  }

  convertDateFull(date: string): string {
    return _moment(date).format('dddd DD MMM Y hh:mm A');
  }
  convertDateHours(date: string): string {
    return _moment(date).format('hh:mm A');
  }
  convertDate(date: string): string {
    return _moment(date).format('dddd DD MMM Y');
  }
  ngOnDestroy() {

  }
}
