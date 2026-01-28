import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AccountService } from '../../services/account.service';
import { UserInfo } from '../../interfaces/InterfacesDto';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
    selector: 'app-user-settings',
    templateUrl: './user-settings.component.html',
    styleUrls: ['./user-settings.component.css']
})
export class UserSettingsComponent implements OnInit {
    userForm: FormGroup;
    passwordForm: FormGroup;
    loading: boolean = true;
    saving: boolean = false;
    userInfo: UserInfo;

    accountCreationDate: Date = new Date(2023, 5, 15);
    lastLogin: Date = new Date();
    accountStatus: string = 'Activa';
    accountType: string = 'Paciente Premium';

    constructor(
        private fb: FormBuilder,
        private accountSvc: AccountService,
        private snackBar: MatSnackBar
    ) {
        this.initForms();
    }

    ngOnInit(): void {
        this.loadUserInfo();
    }

    initForms() {
        this.userForm = this.fb.group({
            nombre: ['', Validators.required],
            apellido: ['', Validators.required],
            doc_identidad: ['', Validators.required],
            contacto: ['', Validators.required],
            sexo: ['', Validators.required],
            fecha_nacimiento: ['', Validators.required],
            correo: [{ value: '', disabled: true }] // Non-editable
        });

        this.passwordForm = this.fb.group({
            currentPassword: ['', Validators.required],
            newPassword: ['', [Validators.required, Validators.minLength(6)]],
            confirmPassword: ['', Validators.required]
        }, { validator: this.passwordMatchValidator });
    }

    passwordMatchValidator(g: FormGroup) {
        return g.get('newPassword').value === g.get('confirmPassword').value
            ? null : { 'mismatch': true };
    }

    loadUserInfo() {
        this.loading = true;
        this.accountSvc.getUserInfo().subscribe(
            (info: UserInfo) => {
                this.userInfo = info;
                this.userForm.patchValue({
                    nombre: info.nombre,
                    apellido: info.apellido,
                    doc_identidad: info.doc_identidad,
                    contacto: info.contacto,
                    sexo: info.sexo,
                    fecha_nacimiento: info.fecha_nacimiento,
                    correo: localStorage.getItem('userName')
                });
                this.loading = false;
            },
            err => {
                this.snackBar.open('Error al cargar la información del usuario', 'Cerrar', { duration: 3000 });
                this.loading = false;
            }
        );
    }

    saveUserInfo() {
        if (this.userForm.invalid) return;

        this.saving = true;
        const updatedInfo: UserInfo = {
            ...this.userInfo,
            ...this.userForm.getRawValue()
        };

        this.accountSvc.setUserInfo(updatedInfo).subscribe(
            () => {
                this.snackBar.open('Información actualizada correctamente', 'Cerrar', { duration: 3000 });
                this.saving = false;
            },
            err => {
                this.snackBar.open('Error al actualizar la información', 'Cerrar', { duration: 3000 });
                this.saving = false;
            }
        );
    }

    changePassword() {
        if (this.passwordForm.invalid) return;

        this.snackBar.open('Funcionalidad de cambio de contraseña en desarrollo', 'Cerrar', { duration: 3000 });
        this.passwordForm.reset();
    }
}
