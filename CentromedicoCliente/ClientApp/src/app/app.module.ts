import { BrowserModule } from '@angular/platform-browser';
import { NgModule, NO_ERRORS_SCHEMA } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './components/nav-menu/nav-menu.component';
import { HomeComponent } from './components/home/home.component';
import { FooterComponent } from './components/footer/footer.component';
import { BarnerComponent } from './components/barner/barner.component';
import { CreateAppointmentComponent } from './components/create-appointment/create-appointment.component';
import { LoginComponent } from './components/login/login.component';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatStepperModule } from '@angular/material/stepper';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MatSelectModule } from '@angular/material/select';
import { MatListModule } from '@angular/material/list';
import { MatRadioModule } from '@angular/material/radio';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { NgxMaskModule, IConfig } from 'ngx-mask'
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
import { MAT_MOMENT_DATE_ADAPTER_OPTIONS, MAT_MOMENT_DATE_FORMATS, MomentDateAdapter } from '@angular/material-moment-adapter';
import { DateAdapter, MatNativeDateModule, MatRippleModule, MAT_DATE_FORMATS, MAT_DATE_LOCALE } from '@angular/material/core';
import { DoctorCardComponent } from './components/medical-directory/doctor-card/doctor-card.component';
import { MatPaginatorModule } from '@angular/material/paginator';
import { NoProfilePhotoPipe } from './Pipes/no-imagen.pipe';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { DoctorComponent } from './components/doctor/doctor.component';
import { MatTabsModule } from '@angular/material/tabs';
import { AuthGuardService } from './guards/auth-guard.service';
import { ListAppointmentComponent } from './components/list-appointment/list-appointment.component';
import { LocationComponent } from './components/location/location.component';
import { ContactComponent, DialogSuccessedEmail } from './components/contact/contact.component';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatTableModule } from '@angular/material/table';
import { MatIconModule } from '@angular/material/icon';
import { FaqComponent } from './components/faq/faq.component';
import { FaqExpansionPanelComponent } from './components/faq/faq-expansion-panel/faq-expansion-panel.component';
import { GoogleMapsModule } from '@angular/google-maps';
import { SwiperModule } from 'ngx-swiper-wrapper';
import { PreguntaService } from './services/pregunta.service';
import { MatDialogModule } from '@angular/material/dialog';
import { MatPaginatorIntl } from '@angular/material/paginator';
import { CustomPaginator } from './shared/CustomPaginatorConfiguration';
import { LottieModule } from 'ngx-lottie';
import player from 'lottie-web';
import { SignalrCustomService } from './services/signalr-custom.service';
import { TurnosMedicoService } from './services/turnos-medico.service';

export function playerFactory() {
  return player;
}
export function tokenGetter() {
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
    ListAppointmentComponent,
    FaqComponent,
    FaqExpansionPanelComponent,
    DialogSuccessedEmail,
  ],
  schemas: [NO_ERRORS_SCHEMA],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    GoogleMapsModule,
    SwiperModule,
    NgxMaskModule.forRoot(),
    MatStepperModule,
    MatTabsModule,
    MatCheckboxModule,
    BrowserAnimationsModule,
    MatButtonModule,
    MatFormFieldModule,
    MatExpansionModule,
    MatButtonToggleModule,
    MatTableModule,
    MatListModule,
    MatDialogModule,
    MatDividerModule,
    MatRadioModule,
    MatNativeDateModule,
    MatPaginatorModule,
    MatSnackBarModule,
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
        allowedDomains: ['localhost:4211', 'kevinj9-001-site1.etempurl.com'],
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
      { path: 'mis-citas', component: ListAppointmentComponent, canActivate: [AuthGuardService] },
      { path: 'ubicacion', component: LocationComponent },
      { path: 'contactos', component: ContactComponent },
      { path: 'faq', component: FaqComponent },
      { path: '**', component: HomeComponent },
    ],
      { scrollPositionRestoration: 'enabled', anchorScrolling: 'enabled', useHash: false }),
  ],
  providers: [


    { provide: MatPaginatorIntl, useValue: CustomPaginator() },
    { provide: MAT_DATE_LOCALE, useValue: 'es' },
    {
      provide: DateAdapter,
      useClass: MomentDateAdapter,
      deps: [MAT_DATE_LOCALE, MAT_MOMENT_DATE_ADAPTER_OPTIONS]
    },
    { provide: MAT_DATE_FORMATS, useValue: MY_FORMATS },
    { provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true },
    AccountService, CitaService, DoctorService, CoberturaService, AuthGuardService, PreguntaService, SignalrCustomService, TurnosMedicoService],
  bootstrap: [AppComponent],
  entryComponents: [DialogSuccessedEmail]
})
export class AppModule { }