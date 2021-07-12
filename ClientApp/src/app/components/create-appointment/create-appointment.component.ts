import { StepperOrientation } from '@angular/cdk/stepper';
import { ChangeDetectionStrategy, Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { Observable, BehaviorSubject, Subject, of } from 'rxjs';
import { BreakpointObserver } from '@angular/cdk/layout';
import { map, startWith, debounceTime, switchMap } from 'rxjs/operators';
import { MAT_MOMENT_DATE_ADAPTER_OPTIONS, MAT_MOMENT_DATE_FORMATS, MomentDateAdapter } from '@angular/material-moment-adapter';
import { DateAdapter, MAT_DATE_FORMATS, MAT_DATE_LOCALE } from '@angular/material/core';
import * as _moment from 'moment';
import { AccountService } from 'src/app/services/account.service';
import { hora, UserInfo, especialidad, seguro, cita, cobertura } from 'src/app/interfaces/InterfacesDto';
import { CitaService } from 'src/app/services/cita.service';
import { DoctorHorarioService } from 'src/app/services/doctor-horario.service';
import { CoberturaService } from 'src/app/services/cobertura.service';
// tslint:disable-next-line:no-duplicate-imports

const moment = _moment;


export const MY_FORMATS = {
  // parse: {
  //   dateInput: 'LL',
  // },
  display: {
    dateInput: 'dddd DD MMM Y',
    monthYearLabel: 'MMM YYYY',
    dateA11yLabel: 'LL',
    monthYearA11yLabel: 'MMMM YYYY',
  },
};
@Component({
  // changeDetection: ChangeDetectionStrategy.OnPush,
  selector: 'app-create-appointment',
  templateUrl: './create-appointment.component.html',
  styleUrls: ['./create-appointment.component.css'],
  providers: [
    // `MomentDateAdapter` and `MAT_MOMENT_DATE_FORMATS` can be automatically provided by importing
    // `MatMomentDateModule` in your applications root module. We provide it at the component level
    // here, due to limitations of our example generation script.
    //  { provide: DateAdapter, useClass: MomentDateAdapter, deps: [MAT_DATE_LOCALE] },
    // { provide: MAT_DATE_FORMATS, useValue: MAT_MOMENT_DATE_FORMATS },
    { provide: MAT_DATE_LOCALE, useValue: 'es' },
    {
      provide: DateAdapter,
      useClass: MomentDateAdapter,
      deps: [MAT_DATE_LOCALE, MAT_MOMENT_DATE_ADAPTER_OPTIONS]
    },

    { provide: MAT_DATE_FORMATS, useValue: MY_FORMATS }
  ],
})
export class CreateAppointmentComponent implements OnInit {

  baseUrl: string;
  private userData$ = new Subject<UserInfo>();
  private availableDates$ = new Subject<Date[]>();
  isUserConfirmed: boolean;

  firstFormGroup: FormGroup;
  secondFormGroup: FormGroup;
  thirdFormGroup: FormGroup;
  
  _cobertura: cobertura;
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
  myHolidayDates = [
    new Date("6/24/2021"),
    new Date("6/25/2021"),
    new Date("6/27/2021"),
    new Date("6/28/2021"),
    new Date("6/29/2021"),
    new Date("6/30/2021"),
    new Date("7/1/2021"),
    new Date("7/2/2021")
  ];

  seguros: seguro[] = [
    { value: 1, viewValue: 'Privado' },
    { value: 2, viewValue: 'ARS' },
    { value: 3, viewValue: 'SENASA' }
  ];
  Horas: hora[] = [
    { value: new Date(2021, 6, 23, 11, 0, 0), viewValue: moment(new Date(2021, 6, 23, 11, 0, 0)).format('LT') },
    { value: new Date(2021, 6, 23, 11, 20, 0), viewValue: moment(new Date(2021, 6, 23, 11, 20, 0)).format('LT') },
    { value: new Date(2021, 6, 23, 11, 40, 0), viewValue: moment(new Date(2021, 6, 23, 11, 40, 0)).format('LT') },
  ];
  especialidades: especialidad[] = [
    { value: 1, viewValue: "Alergelogía" },
    { value: 2, viewValue: "Urología" },
    { value: 3, viewValue: "Cardiología" }
  ];

  dateFilter = (d: Date): boolean => {
    const time = new Date(d).getTime();
    return this.myHolidayDates.find(x => x.getTime() == time) ? true : false;
  }


  ngOnInit() {

    

    this.firstFormGroup = this._formBuilder.group({
      insuranceOptionControl: [true],
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

    this.firstFormGroup.get("insuranceOptionControl").valueChanges.subscribe(option => {
      if (option == true) {
        this.firstFormGroup.get("insuranceControl")
          .clearValidators();
      } else {
        this.firstFormGroup.get("insuranceControl")
          .setValidators([Validators.required]);
      }

      this.firstFormGroup.get("insuranceControl").updateValueAndValidity();
    });

    this.thirdFormGroup.get("appointmentTypeControl").valueChanges.subscribe(option => {
      if (option == 1) this.underAgeShow = "block";
      else this.underAgeShow = "none";
      this.isDependent = Boolean(Number.parseInt(option));
    });


  }
  onClickSubmit() {
    this.isSent = true;

    console.log(this.thirdFormGroup.valid);

    let formdata = Object.assign(this.secondFormGroup.value, this.firstFormGroup.value, this.thirdFormGroup.value);
    let _cita: cita;
    let fecha_hora: Date = formdata["timeControl"];
    let nombre;
    let apellido;
    let doc_identidad;
    let sexo;

    if (this.isDependent) {
      nombre = formdata["dependentNameControl"];
      apellido = formdata["dependentLastNameControl"];
      sexo = formdata["dependentSexControl"];

    } else {

      doc_identidad = formdata["identityDocControl"];
      nombre = formdata["userNameControl"];
      apellido = formdata["userLastNameControl"];
      sexo = formdata["userSexControl"];
    }

    _cita = {
      "nombre": nombre,
      "apellido": apellido,
      "sexo": sexo,
      "doc_identidad": doc_identidad,
      "fecha_hora": fecha_hora,
      "medicosID": Number.parseInt(localStorage.getItem("medicoId")),
      "serviciosID": formdata["serviceTypeControl"],
      "fecha_nacimiento": moment(formdata["birthDateControl"]).toDate(),
      "contacto": formdata["contactControl"],
      "contacto_whatsapp": formdata["wsReachControl"],
      "appoiment_type": formdata["appointmentTypeControl"],
      "segurosID": formdata["insuranceControl"],
      "nota": formdata["noteControl"]
    };

    console.log(_cita)

  }

  constructor(private doctorhSvc: DoctorHorarioService, private coberturaSvc: CoberturaService,
    private citaSvc: CitaService, private accountSvc: AccountService,
    private _formBuilder: FormBuilder, breakpointObserver: BreakpointObserver) {
    this.getUserInfo();
    localStorage.setItem("medicoId", "1");
    localStorage.setItem("especialidadId", "1");

    this.stepperOrientation = breakpointObserver.observe('(min-width: 800px)')
      .pipe(map(({ matches }) => matches ? 'horizontal' : 'vertical'));

    this.minDBDDate = new Date(Date.now() + -6574 * 24 * 3600 * 1000);
    this.maxDBDDate = new Date();
    this.minBDDate = new Date(Date.now() + -43825 * 24 * 3600 * 1000);
    this.maxBDDate = new Date(Date.now() + -6575 * 24 * 3600 * 1000);

  }


  getUserInfo() {

    this.accountSvc.getUserInfo().subscribe((re) => {

      console.log(re);

      this.isUserConfirmed = re.confirm_doc_identidad;
      this.thirdFormGroup.get("userNameControl").setValue(re.nombre);
      this.thirdFormGroup.get("userLastNameControl").setValue(re.apellido);
      this.thirdFormGroup.get("birthDateControl").setValue(re.fecha_nacimiento);
      this.thirdFormGroup.get("contactControl").setValue(re.contacto);
      this.thirdFormGroup.get("userSexControl").setValue(re.sexo);
      this.thirdFormGroup.get("identityDocControl").setValue(re.doc_identidad);

    });
  }

  getDSexErrorMessage() {
    return this.thirdFormGroup.get("dependentSexControl").hasError('required') ? 'Debe seleccionar una opción' : "";

  }
  getSexErrorMessage() {
    return this.thirdFormGroup.get("userSexControl").hasError('required') ? 'Debe seleccionar una opción' : "";

  }

  users: { name: string, title: string }[] = [
    { name: 'Carla Espinosa', title: 'Nurse' },
    { name: 'Bob Kelso', title: 'Doctor of Medicine' },
    { name: 'Janitor', title: 'Janitor' },
    { name: 'Perry Cox', title: 'Doctor of Medicine' },
    { name: 'Ben Sullivan', title: 'Carpenter and photographer' },
  ];

}
