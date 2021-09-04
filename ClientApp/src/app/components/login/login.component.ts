import { StepperOrientation } from '@angular/cdk/stepper';
import { ChangeDetectionStrategy, Component, Inject, OnInit, OnDestroy } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { Observable, BehaviorSubject, Subject, of } from 'rxjs';
import { ActivatedRoute, Router } from '@angular/router';
import { BreakpointObserver } from '@angular/cdk/layout';
import { map, startWith, debounceTime, switchMap, catchError, finalize } from 'rxjs/operators';
import * as _moment from 'moment';
import { MatSnackBar, MatSnackBarConfig } from '@angular/material/snack-bar'; 
import { CitaService } from 'src/app/services/cita.service';
import { AccountService } from 'src/app/services/account.service';
import { trigger, style, animate, transition } from '@angular/animations';
import { ProgressSpinnerMode } from '@angular/material/progress-spinner';
import { AutoUnsubscribe } from "ngx-auto-unsubscribe";

@AutoUnsubscribe()
@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
  animations: [
    trigger(
      'inOutAnimation',
      [
        transition(
          ':enter',
          [
            style({ opacity: 0 }),
            animate('30ms ease-out',
              style({ opacity: 1 }))
          ]
        ),
        transition(
          ':leave',
          [
            style({ opacity: 1, position: "absolute" }),
            animate('10ms ease-in',
              style({ opacity: 0, position: "relative" }))
          ]
        )
      ]
    )
  ]
})
export class LoginComponent implements OnInit {

  mode: ProgressSpinnerMode = 'indeterminate';

  loginFormGroup: FormGroup;
  signupFormGroup: FormGroup;
  loading: boolean = false;
  returnUrl: string;
  ErrorMessage: string;
  invalidLogin: boolean = false;

  constructor(private router: Router,
    private rutaActiva: ActivatedRoute,
    private _snackBar: MatSnackBar,
    public citaSvc: CitaService,
    private accountSvc: AccountService,
    private _formBuilder: FormBuilder) {
//go back user is already logged in
    if (this.accountSvc.checkLoginStatus())
      this.router.navigate(['..']);
  }

  openSnackBar(message: string) {
    const config = new MatSnackBarConfig();
    config.panelClass = 'background-red';
    config.duration = 5000;
    this._snackBar.open(message, null, config);
  }

  ngOnInit(): void {

    this.returnUrl = this.rutaActiva.snapshot.queryParams['returnUrl'] || '/';

    this.loginFormGroup = this._formBuilder.group({
      loginEmailControl: ['', [
        Validators.required,
        Validators.email,
      ]],
      loginPasswordControl: ['', Validators.required],
    });

    this.signupFormGroup = this._formBuilder.group({
      signupEmailControl: ['', [
        Validators.required,
        Validators.email,
      ]],
      password: ['', [Validators.required]],
      password2: ['', [Validators.required]]
    }, { validator: passwordMatchValidator });

  }

  get password() { return this.signupFormGroup.get('password'); }
  get password2() { return this.signupFormGroup.get('password2'); }

  onPasswordInput() {
    if (this.signupFormGroup.hasError('passwordMismatch'))
      this.password2.setErrors([{ 'passwordMismatch': true }]);
    else
      this.password2.setErrors(null);
  }

  Login(): void {

    if (this.loginFormGroup.valid) {
      if (!this.loading) {
        this.loading = true;
        const credentials = JSON.stringify(this.loginFormGroup.value);
        let userLogin = this.loginFormGroup.value;

        this.accountSvc
          .Login(userLogin.loginEmailControl, userLogin.loginPasswordControl)
          .subscribe(result => {

            this.loading = false;
            let token = (<any>result).authToken.token;
            console.log("User Logged In Successfully");
            this.invalidLogin = false;
            this.router.navigateByUrl(this.returnUrl);

          },
            (error) => {
              this.invalidLogin = true;
              this.loading = false;

              this.ErrorMessage = "Ha ocurrido un error al intentar iniciar sessión";
              this.openSnackBar(error);
              console.log(error);
            }
          )
      }
    }
  }

  Signup(): void {

    if (this.signupFormGroup.valid) {
      if (!this.loading) {
        this.loading = true;
        let userSignup = this.signupFormGroup.value;

        this.accountSvc
          .Signup(userSignup.signupEmailControl, userSignup.password)
          .subscribe(result => {

            this.loading = false;
            let token = (<any>result).authToken.token;
            console.log("User Register and Login In Successfully");
            this.router.navigateByUrl(this.returnUrl);

          },
            (error) => {
              this.invalidLogin = true;
              this.loading = false;

              this.ErrorMessage = "Ha ocurrido un error al intentar iniciar sessión";
              this.openSnackBar(error);
              console.log(error);
            }
          )
      }
    }

  }
  ngOnDestroy() { 

  }
}

export const passwordMatchValidator: ValidatorFn = (formGroup: FormGroup): ValidationErrors | null => {
  if (formGroup.get('password').value === formGroup.get('password2').value)
    return null;
  else
    return { passwordMismatch: true };
};
