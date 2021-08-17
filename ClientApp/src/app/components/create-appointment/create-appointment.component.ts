import { StepperOrientation } from '@angular/cdk/stepper';
import { ChangeDetectionStrategy, Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, ValidationErrors, Validators } from '@angular/forms';
import { Observable, BehaviorSubject, Subject, of } from 'rxjs';
import { Router } from '@angular/router';
import { BreakpointObserver } from '@angular/cdk/layout';
import { map, startWith, debounceTime, switchMap, catchError, finalize } from 'rxjs/operators';
import * as _moment from 'moment';
import { MatSnackBar, MatSnackBarConfig } from '@angular/material/snack-bar';
import { STEPPER_GLOBAL_OPTIONS } from '@angular/cdk/stepper';
import { CoberturaService } from 'src/app/services/cobertura.service';
import { CitaService } from 'src/app/services/cita.service';
import { AccountService } from 'src/app/services/account.service';
import { hora, UserInfo, seguro, cita, cobertura, citaResult } from 'src/app/interfaces/InterfacesDto';
import { SeguroService } from 'src/app/services/seguro.service';

const moment = _moment;

@Component({
  // changeDetection: ChangeDetectionStrategy.OnPush,
  selector: 'app-create-appointment',
  templateUrl: './create-appointment.component.html',
  styleUrls: ['./create-appointment.component.css'],
  providers: [

    {
      provide: STEPPER_GLOBAL_OPTIONS,
      useValue: { showError: true, displayDefaultIndicatorType: false }
    },
  ],
})
export class CreateAppointmentComponent implements OnInit {

  baseUrl: string;

  firstFormGroup: FormGroup;
  secondFormGroup: FormGroup;
  thirdFormGroup: FormGroup;

  isUserConfirmed: boolean;

  seguros$: Observable<any>;
  servicios$: Observable<any>;
  medicoID: number = 1;
  diferencia: number = 0;
  pago: number = 0;
  cobertura: number = 0;
  loadingPayment: boolean;

  insuranceOption: boolean = true;

  isDependent = false;
  isEditable = false;
  isSent = false;
  stepperOrientation: Observable<StepperOrientation>;
  minBDDate: Date;
  minDBDDate: Date;
  maxBDDate: Date;
  maxDBDDate: Date;
  underAgeShow: string = "none";
  diasLaborables: Date[];

  Horas: hora[];
  dateFilter;

  ngOnInit() {

    this.firstFormGroup = this._formBuilder.group({
      //insuranceOptionControl: [true],
      insuranceControl: [''],
      serviceTypeControl: ['', Validators.required],
    });

    this.secondFormGroup = this._formBuilder.group({
      dateControl: [''],
      timeControl: [''],
    });

    this.thirdFormGroup = this._formBuilder.group({
      wsReachControl: [''],
      appointmentTypeControl: [0, Validators.required],
      dependentBirthDateControl: [''],
      dependentNameControl: [''],
      dependentLastNameControl: [''],
      identityDocControl: ['', [Validators.required, Validators.minLength(11), Validators.maxLength(15)]],
      userNameControl: ['', Validators.required],
      userLastNameControl: ['', Validators.required],
      birthDateControl: ['', Validators.required],
      contactControl: [''],
      noteControl: [''],
      dependentSexControl: [''],
      userSexControl: ['', Validators.required],
    });

    // this.firstFormGroup.get("insuranceOptionControl").valueChanges.subscribe(option => {
    //   if (option == true) {
    //     this.firstFormGroup.get("insuranceControl")
    //       .clearValidators();
    //   } else {
    //     this.firstFormGroup.get("insuranceControl")
    //       .setValidators([Validators.required]);
    //   }
    // this.firstFormGroup.get("insuranceControl").updateValueAndValidity();
    // });

    this.thirdFormGroup.get("appointmentTypeControl")
      .valueChanges
      .subscribe(option => {
        if (option == 1) this.underAgeShow = "block";
        else this.underAgeShow = "none";
        this.isDependent = Boolean(Number.parseInt(option));
      });

    //actualiza los costos por el seguro que se escoja
    this.firstFormGroup.get("insuranceControl")
      .valueChanges
      .subscribe(value => this.setCostos());

    //actualiza los seguros disponibles al cambiar de servicio
    this.firstFormGroup.get("serviceTypeControl")
      .valueChanges
      .subscribe(value => {
        this.seguroSvc.GetSegurosByServicio(this.medicoID, value)
          .pipe(catchError(err => {
            console.log('Ha ocurrido un error al tratar de obtener la lista de seguros: ', err);
            return of([]);
          }))
          .subscribe((r: any) => {
            this.seguros$ = r;
            this.firstFormGroup.get("insuranceControl").reset(null, { onlySelf: true, emitEvent: false });
            //console.table(r);
            this.setCostos();
          })
      });

    //actualiza las horas disponibles
    this.secondFormGroup.get("dateControl")
      .valueChanges
      .subscribe(value => {

        if (value.length != 0) {

          this.secondFormGroup.get("timeControl").reset(null, { onlySelf: true, emitEvent: false });

          this.citaSvc.GetTimeList(value, this.medicoID)
            .pipe(catchError(err => {
              console.log('Ha ocurrido un error al tratar de obtener la lista de las horas disponibles: ', err);
              return of([]);
            }))
            .subscribe((r: any) => {
              this.Horas = r.map((r: Date) => {
                return { id: r, descrip: _moment(r).utc().format(' hh:mm A') };
              });
            })
        }
      });
  }


  openSnackBar(message: string) {
    const config = new MatSnackBarConfig();
    config.panelClass = 'background-red';
    config.duration = 5000;
    this._snackBar.open(message, null, config);
  }

  onClickSubmit() {

    if (!this.firstFormGroup.valid || !this.secondFormGroup.valid || !this.thirdFormGroup.valid) {
      this.openSnackBar("Las información ingresada no es valida");
    } else {
      if (!this.isSent) {
        this.isSent = true;

        let formdata = Object.assign(this.firstFormGroup.value, this.secondFormGroup.value, this.thirdFormGroup.value);
        let _cita: cita;
        let fecha_hora: Date = formdata["timeControl"];
        let contacto = formdata["contactControl"];
        let nombre = formdata["userNameControl"];
        let apellido = formdata["userLastNameControl"];
        let doc_identidad = formdata["identityDocControl"];
        let sexo = formdata["userSexControl"];
        let fecha_nacimiento = moment(formdata["birthDateControl"]).toDate();

        if (this.isDependent) {
          nombre = formdata["dependentNameControl"];
          apellido = formdata["dependentLastNameControl"];
          sexo = formdata["dependentSexControl"];
          fecha_nacimiento = moment(formdata["dependentBirthDateControl"]).toDate();
        }

        let userInfo: UserInfo = {
          doc_identidad: formdata["identityDocControl"],
          nombre: formdata["userNameControl"],
          apellido: formdata["userLastNameControl"],
          fecha_nacimiento: fecha_nacimiento,
          sexo: formdata["userSexControl"],
          contacto: contacto
        }
        
        _cita = {
          "nombre": nombre,
          "apellido": apellido,
          "sexo": sexo,
          "doc_identidad": doc_identidad,
          "fecha_hora": fecha_hora,
          "medicosID": Number.parseInt(localStorage.getItem("medicoId")),
          "serviciosID": formdata["serviceTypeControl"],
          "fecha_nacimiento": fecha_nacimiento,
          "contacto": formdata["contactControl"],
          "contacto_whatsapp": formdata["wsReachControl"],
          "appointment_type": formdata["appointmentTypeControl"],
          "segurosID": formdata["insuranceControl"],
          "nota": formdata["noteControl"]
        };

        this.accountSvc.setUserInfo(userInfo).subscribe(arg => {

        }, err => of([])
          , () => {

            this.citaSvc.CreateCita(_cita).subscribe((r: citaResult) => {
              this.citaSvc._citaResult = r;
              this.router.navigate(['ticket']);
            }, (err: string) => {
              this.isSent = false;
              this.openSnackBar(err);
              console.error(err);
            }, () => {
              this.isSent = false;
            });
          });
      }
    }
  }
  setCostos() {

    var servicioID = Number.parseInt(this.firstFormGroup.get("serviceTypeControl").value);
    var seguroID = Number.parseInt(this.firstFormGroup.get("insuranceControl").value);

    //console.log(servicioID, seguroID)

    if (Number.isInteger(seguroID) && Number.isInteger(servicioID)) {
      this.loadingPayment = true; //muestro la pagina de carga

      setTimeout(() => {

        this.coberturaSvc.GetCobertura(this.medicoID, seguroID, servicioID)
          .pipe(catchError(err => {
            this.loadingPayment = false;
            console.error('Error al intentar acceder a la cobertura');
            return null;
          }), finalize(() => {
            this.loadingPayment = false;
          }))
          .subscribe((r: cobertura) => {
            if (r != null) {
              this.cobertura = r.cobertura;
              this.pago = r.pago;
              this.diferencia = r.diferencia;
            } else {
              this.cobertura = 0;
              this.pago = 0;
              this.diferencia = 0;
            }
          });
      }, 400)
    }
    else {
      this.cobertura = 0;
      this.pago = 0;
      this.diferencia = 0;
    }
  }

  setUserInfo() {
    this.accountSvc.getUserInfo().subscribe((re: UserInfo) => {

      console.log(re);
      this.isUserConfirmed = re.confirm_doc_identidad;
      this.thirdFormGroup.get("userNameControl").setValue(re.nombre);
      this.thirdFormGroup.get("userLastNameControl").setValue(re.apellido);
      this.thirdFormGroup.get("birthDateControl").setValue(re.fecha_nacimiento);
      this.thirdFormGroup.get("contactControl").setValue(re.contacto);
      this.thirdFormGroup.get("userSexControl").setValue(re.sexo);
      this.thirdFormGroup.get("identityDocControl").setValue(re.doc_identidad);

    }, err => {
      console.error(err)
    });
  }

  getDSexErrorMessage() {
    return this.thirdFormGroup.get("dependentSexControl").hasError('required') ? 'Debe seleccionar una opción' : "";
  }
  getSexErrorMessage() {
    return this.thirdFormGroup.get("userSexControl").hasError('required') ? 'Debe seleccionar una opción' : "";
  }

  constructor(

    private router: Router,
    private _snackBar: MatSnackBar,
     private coberturaSvc: CoberturaService,
     private seguroSvc: SeguroService,
    public citaSvc: CitaService, private accountSvc: AccountService,
    private _formBuilder: FormBuilder, breakpointObserver: BreakpointObserver) {

    localStorage.setItem("medicoId", "1");
    localStorage.setItem("especialidadId", "1");

    this.stepperOrientation = breakpointObserver.observe('(min-width: 800px)')
      .pipe(map(({ matches }) => matches ? 'horizontal' : 'vertical'));

    //inicializa las fechas permitidas
    this.citaSvc.GetNewCita(this.medicoID)
      .pipe(
        catchError(err => {
          console.error('Error al tratar de acceder a los pre-datos de la cita');
          return of([]);
        })).subscribe(r => {
          this.servicios$ = r.servicios;

          this.diasLaborables = r.diasLaborables.map(r => new Date(r));

          this.dateFilter = (d: Date): boolean => {
            const time = new Date(d).getTime();
            return this.diasLaborables.find(x => x.getTime() == time) ? true : false;
          }
          //console.table(this.diasLaborables);
        })

    //Establezco las fechas minimas permitidas en las fechas de nacimientos
    this.minDBDDate = new Date(Date.now() + -6574 * 24 * 3600 * 1000);
    this.maxDBDDate = new Date();
    this.minBDDate = new Date(Date.now() + -43825 * 24 * 3600 * 1000);
    this.maxBDDate = new Date(Date.now() + -6575 * 24 * 3600 * 1000);

    //Relleno los datos del usuario si existe
    this.setUserInfo();
  }
}
