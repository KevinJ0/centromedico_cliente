import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './components/nav-menu/nav-menu.component';
import { HomeComponent } from './components/home/home.component';
import { FooterComponent } from './components/footer/footer.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { NewHomeComponent } from './components/home/new-home/new-home.component';
import { BarnerComponent } from './components/barner/barner.component';
import { CreateAppointmentComponent } from './components/create-appointment/create-appointment.component';
import { LoginComponent } from './components/login/login.component';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatStepperModule } from '@angular/material/stepper';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MatSelectModule } from '@angular/material/select';
import { MatListModule } from '@angular/material/list';
import { MatRadioModule } from '@angular/material/radio';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule, MatRippleModule } from '@angular/material/core';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { NgxMaskModule, IConfig } from 'ngx-mask';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatDividerModule } from '@angular/material/divider';
import { TicketAppointmentComponent } from './components/create-appointment/ticket-appointment/ticket-appointment.component';
import { MedicalDirectoryComponent } from './components/medical-directory/medical-directory.component';
import { AccountService } from './services/account.service';
import { JwtInterceptor } from './_helpers/jwt.Interceptor';
import { JwtModule } from '@auth0/angular-jwt';
import { CitaService } from './services/cita.service';
import { CoberturaService } from './services/cobertura.service';
import { DoctorService } from './services/doctor.service';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import * as _moment from 'moment';
import { MAT_MOMENT_DATE_ADAPTER_OPTIONS, MAT_MOMENT_DATE_FORMATS, MomentDateAdapter } from '@angular/material-moment-adapter';
import { DateAdapter, MAT_DATE_FORMATS, MAT_DATE_LOCALE } from '@angular/material/core';
import { DoctorCardComponent } from './components/medical-directory/doctor-card/doctor-card.component';
import { MatPaginatorModule } from '@angular/material/paginator';
import { NoProfilePhotoPipe } from './Pipes/no-imagen.pipe';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { DoctorComponent } from './components/doctor/doctor.component';
import { MatTabsModule } from '@angular/material/tabs';
import { MaterialModule } from "./shared/material.module";
import { AuthGuardService } from './guards/auth-guard.service';
import { ListAppointmentComponent } from './components/list-appointment/list-appointment.component';
import { LocationComponent } from './components/location/location.component';
import { ContactComponent } from './components/contact/contact.component';
import {MatExpansionModule} from '@angular/material/expansion';
import {MatTableModule} from '@angular/material/table';


export function tokenGetter() {
  //return "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJKb3NlQGdtYWlsLmNvbSIsImp0aSI6IjdjOGY5ZGIyLTAyNzYtNDJkMS1iNTc3LTUyNTg1NjhjMTdlZSIsIm5hbWVpZCI6IjAxZTNhMjJiLTI2MjctNDgyMS05ZTBlLTE0NzE1MTNhOWY5NCIsInJvbGUiOiJQYXRpZW50IiwiTG9nZ2VkT24iOiI1LzI0LzIwMjEgMTA6Mjk6NTggUE0iLCJuYmYiOjE2MjE5MDk3OTgsImV4cCI6MTcxNDYyMzcxOCwiaWF0IjoxNjIxOTA5Nzk4LCJpc3MiOiJodHRwczovL2xvY2FsaG9zdDo0NDMzNyIsImF1ZCI6Imh0dHBzOi8vbG9jYWxob3N0OjQ0MzM3In0.Auc5Om1B4G5M5BJ31EEEtElCsBTug4WMO1ugChYdcEE";
  return localStorage.getItem("jwt");
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
@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    FooterComponent,
    NewHomeComponent,
    BarnerComponent,
    CreateAppointmentComponent,
    LoginComponent,
    MedicalDirectoryComponent,
    NoProfilePhotoPipe,
    TicketAppointmentComponent,
    DoctorCardComponent,
    DoctorComponent,
    ContactComponent,
    LocationComponent,
    ListAppointmentComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    MatStepperModule,
    MatTabsModule,
    MatCheckboxModule,
    BrowserAnimationsModule,
    MatButtonModule,
    MaterialModule,
    MatFormFieldModule,
    MatExpansionModule,
    MatButtonToggleModule,
    MatTableModule,
    MatListModule,
    MatDividerModule,
    MatRadioModule,
    MatNativeDateModule,
    MatPaginatorModule,
    MatSnackBarModule,
    NgxMaskModule.forRoot(),
    MatRippleModule,
    MatSelectModule,
    MatProgressSpinnerModule,
    MatIconModule,
    MatDatepickerModule,
    MatCardModule,
    FormsModule,
    MatInputModule,
    ReactiveFormsModule,
    JwtModule.forRoot({
      config: {
        tokenGetter: tokenGetter,
        allowedDomains: ['localhost:4211', 'hospitalsalvador-001-site1.htempurl.com'],
        disallowedRoutes: [],
        authScheme: "Bearer ",
      }
    }),
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full' },
      { path: 'paciente-login', component: LoginComponent },
      { path: 'crear-cita', component: CreateAppointmentComponent, canActivate: [AuthGuardService] },
      { path: 'ticket', component: TicketAppointmentComponent, canActivate: [AuthGuardService] },
      { path: 'medicos', component: MedicalDirectoryComponent },
      { path: 'medicos/:id', component: DoctorComponent },
      { path: 'mis-citas', component: ListAppointmentComponent },
      { path: 'ubicacion', component: LocationComponent },
      { path: 'contactos', component: ContactComponent },
      { path: '**', component: HomeComponent },
    ],
      { useHash: false,scrollPositionRestoration: 'enabled'}),
  ],
  providers: [

    { provide: MAT_DATE_LOCALE, useValue: 'es' },
    {
      provide: DateAdapter,
      useClass: MomentDateAdapter,
      deps: [MAT_DATE_LOCALE, MAT_MOMENT_DATE_ADAPTER_OPTIONS]
    },
    { provide: MAT_DATE_FORMATS, useValue: MY_FORMATS },
    { provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true },
    AccountService, CitaService, DoctorService, CoberturaService, AuthGuardService],
  bootstrap: [AppComponent]
})
export class AppModule { }
