import { Component, OnInit } from '@angular/core';
import { CorreoPregunta } from '../../interfaces/InterfacesDto';
import { PreguntaService } from '../../services/pregunta.service';
import { MatDialog } from '@angular/material/dialog';
import { catchError, of, throwError } from 'rxjs';
import { FormBuilder, FormControl, FormGroup, FormGroupDirective, NgForm, Validators } from '@angular/forms';
import { ErrorStateMatcher } from '@angular/material/core';

/** Error when invalid control is dirty, touched, or submitted. */
export class MyErrorStateMatcher implements ErrorStateMatcher {
  isErrorState(control: FormControl | null, form: FormGroupDirective | NgForm | null): boolean {
    const isSubmitted = form && form.submitted;
    return !!(control && control.invalid && (control.dirty || control.touched || isSubmitted));
  }
}

@Component({
  selector: 'app-contact',
  templateUrl: './contact.component.html',
  styleUrls: ['./contact.component.css']
})
export class ContactComponent implements OnInit {
  contactFormGroup: FormGroup;
  serverMsj: string = "";
  contactForm: CorreoPregunta;
  constructor(public dialog: MatDialog, private _formBuilder: FormBuilder, private preguntaSvc: PreguntaService) { }

  openDialog() {
    this.dialog.open(DialogSuccessedEmail);
  }
  matcher = new MyErrorStateMatcher();
  ngOnInit(): void {
    this.contactFormGroup = this._formBuilder.group({
      nameControl: ['', Validators.required],
      reasonControl: [0, Validators.required],
      emailControl: ['', [
        Validators.required,
        Validators.email,
      ]],
      phoneControl: ['', Validators.required],
      messageControl: ['', Validators.required]
    });
  }

  onClickSubmit() {
    let formdata = this.contactFormGroup.value
    if (this.contactFormGroup.valid) {

      this.contactForm = {
        "nombre": formdata.nameControl,
        "correo": formdata.emailControl,
        "motivo": formdata.reasonControl,
        "mensaje": formdata.messageControl,
        "contacto": formdata.phoneControl
      };

      try {
        this.preguntaSvc.SendQuestion(this.contactForm).subscribe((r) => {

          this.serverMsj = "";
          this.contactFormGroup.reset();
          this.openDialog();

        }, err => {
          this.serverMsj = err;
        });
      } catch (e) {
        this.serverMsj = e;
      }
    }
  }
}

@Component({
  selector: 'dialog-successed-email',
  template: `<h1 mat-dialog-title>Mensaje exitoso</h1>
    <div mat-dialog-content>Su mensaje ha sido enviado correctamente.</div>
    <div mat-dialog-actions align="end">
  <button  mat-button mat-dialog-close cdkFocusInitial>Cerrar</button>
</div>`,
})
export class DialogSuccessedEmail { }
