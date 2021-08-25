import { StepperOrientation } from '@angular/cdk/stepper';
import { ChangeDetectionStrategy, Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { Observable, BehaviorSubject, Subject, of } from 'rxjs';
import { ActivatedRoute, Router } from '@angular/router';
import { BreakpointObserver } from '@angular/cdk/layout';
import { map, startWith, debounceTime, switchMap, catchError, finalize } from 'rxjs/operators';
import * as _moment from 'moment';
import { MatSnackBar, MatSnackBarConfig } from '@angular/material/snack-bar';
import { STEPPER_GLOBAL_OPTIONS } from '@angular/cdk/stepper';
import { CoberturaService } from 'src/app/services/cobertura.service';
import { CitaService } from 'src/app/services/cita.service';
import { AccountService } from 'src/app/services/account.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {

  loginFormGroup: FormGroup;
  signupFormGroup: FormGroup;
  loading: boolean;
  returnUrl: string;
  ErrorMessage: string;
  invalidLogin: boolean;

  constructor(private router: Router,
    private rutaActiva: ActivatedRoute,
    private _snackBar: MatSnackBar,
    public citaSvc: CitaService,
    private accountSvc: AccountService,
    private _formBuilder: FormBuilder) { }

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
      signupPasswordControl: ['', Validators.required],
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

    this.loading = true;
    const credentials = JSON.stringify(this.loginFormGroup.value);
    let userlogin = this.loginFormGroup.value;

    this.accountSvc
    .login(userlogin.loginEmailControl, userlogin.loginPasswordControl)
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

        console.log(error);
      })


  }

  Signup(): void {


  }

}

export const passwordMatchValidator: ValidatorFn = (formGroup: FormGroup): ValidationErrors | null => {
  if (formGroup.get('password').value === formGroup.get('password2').value)
    return null;
  else
    return { passwordMismatch: true };
};
