import { StepperOrientation } from '@angular/cdk/stepper';
import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { Observable, of } from 'rxjs';
import { BreakpointObserver } from '@angular/cdk/layout';
import { map, startWith, debounceTime, switchMap } from 'rxjs/operators';
import { MAT_MOMENT_DATE_ADAPTER_OPTIONS, MAT_MOMENT_DATE_FORMATS, MomentDateAdapter } from '@angular/material-moment-adapter';
import { DateAdapter, MAT_DATE_FORMATS, MAT_DATE_LOCALE } from '@angular/material/core';
import * as _moment from 'moment';
// tslint:disable-next-line:no-duplicate-imports

const moment = _moment;

interface especialidad {
  value: number;
  viewValue: string;
}
interface hora {
  value: Date;
  viewValue: string;
}
interface seguro {
  value: number;
  viewValue: string;
}
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


  firstFormGroup: FormGroup;
  secondFormGroup: FormGroup;
  thirdFormGroup: FormGroup;

  dateControl = new FormControl();
  timeControl = new FormControl(false);

  insuranceOption: boolean = true;
  insuranceControl = new FormControl();
  insuranceOptionControl = new FormControl();

  dependentSexControl = new FormControl();
  wsReachControl = new FormControl(false);
  appointmentTypeControl = new FormControl();
  dependentBirthDateControl = new FormControl();
  dependentNameControl = new FormControl();
  dependentLastNameControl = new FormControl();
  identityDocControl = new FormControl();
  userNameControl = new FormControl();
  userLastNameControl = new FormControl();
  birthDateControl = new FormControl();
  contactControl = new FormControl();
  noteControl = new FormControl();
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

  typesOfShoes: string[] = ['Sneakers', 'Clogs', 'Loafers', 'Moccasins', 'Sneakers', 'Clogs', 'Loafers', 'Moccasins', 'Sneakers', 'Clogs', 'Loafers', 'Moccasins', 'Sneakers', 'Clogs', 'Loafers', 'Moccasins', 'Sneakers', 'Clogs', 'Loafers', 'Moccasins', 'Sneakers', 'Clogs', 'Loafers', 'Moccasins', 'Sneakers', 'Clogs', 'Loafers', 'Moccasins', 'Sneakers', 'Clogs', 'Loafers', 'Moccasins', 'Sneakers', 'Clogs', 'Loafers', 'Moccasins', 'Sneakers', 'Clogs', 'Loafers', 'Moccasins', 'Sneakers'];

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
      insuranceControl: ['']
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
      serviceTypeControl: ['', Validators.required],
      noteControl: [''],
      dependentSexControl: ['', Validators.required],
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
      if (option == 1) {
        this.isDependent = option;
        this.thirdFormGroup.get("dependentNameControl")
          .setValidators([Validators.required]);
        this.thirdFormGroup.get("dependentLastNameControl")
          .setValidators([Validators.required]);
        this.thirdFormGroup.get("dependentBirthDateControl")
          .setValidators([Validators.required]);
        this.thirdFormGroup.get("dependentSexControl")
          .setValidators([Validators.required]);
        this.underAgeShow = "block";
      } else {
        this.underAgeShow = "none";
        this.thirdFormGroup.get("dependentNameControl")
          .clearValidators();
        this.thirdFormGroup.get("dependentLastNameControl")
          .clearValidators();
        this.thirdFormGroup.get("dependentBirthDateControl")
          .clearValidators();
        this.thirdFormGroup.get("dependentSexControl")
          .clearValidators();
      }

      this.thirdFormGroup.get("dependentNameControl").updateValueAndValidity();
      this.thirdFormGroup.get("dependentLastNameControl").updateValueAndValidity();
      this.thirdFormGroup.get("dependentBirthDateControl").updateValueAndValidity();

    });


  }
  onClickSubmit() {
    this.isSent = true;
    console.log(this.thirdFormGroup.valid);

    let formdata = Object.assign(this.secondFormGroup.value, this.firstFormGroup.value, this.thirdFormGroup.value);
    let cita: any;
    let fecha_hora: Date = formdata["timeControl"];
    let nombre_tutor;
    let nombre;
    let apellido;

    if (this.isDependent) {
      nombre_tutor = (formdata["userNameControl"] + " " + formdata["userLastNameControl"]).trim();
      nombre = formdata["dependentNameControl"];
      apellido = formdata["dependentLastNameControl"];
    } else {
      nombre = formdata["userNameControl"];
      apellido = formdata["userLastNameControl"];
    }

    cita = {
      "nombre": nombre,
      "apellido": apellido,
      "sexo": formdata["userSexControl"],
      "doc_identidad": formdata["identityDocControl"],
      "fecha_hora": fecha_hora,
      "medicosID": localStorage.getItem("medicoId"),
      "telefono": formdata["userNameControl"],
      "serviciosID": formdata["serviceTypeControl"],
      "fecha_nacimiento": moment(formdata["birthDateControl"]).toDate(),
      "contacto": formdata["contactControl"],
      "contacto_whatsapp": formdata["wsReachControl"],
      "doc_identidad_tutor": formdata["userNameControl"],
      "nombre_tutor": nombre_tutor,
      "appoiment_type": formdata["appointmentTypeControl"],
      "segurosID": formdata["insuranceControl"],
      "nota": formdata["noteControl"]
    };

    console.log(cita)

  }

  constructor(private _formBuilder: FormBuilder, breakpointObserver: BreakpointObserver) {
    localStorage.setItem("medicoId", "1");
    localStorage.setItem("especialidadId", "1");

    this.stepperOrientation = breakpointObserver.observe('(min-width: 800px)')
      .pipe(map(({ matches }) => matches ? 'horizontal' : 'vertical'));

    this.minDBDDate = new Date(Date.now() + -6574 * 24 * 3600 * 1000);
    this.maxDBDDate = new Date();
    this.minBDDate = new Date(Date.now() + -43825 * 24 * 3600 * 1000);
    this.maxBDDate = new Date(Date.now() + -6575 * 24 * 3600 * 1000);



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
