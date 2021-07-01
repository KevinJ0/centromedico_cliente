import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './components/nav-menu/nav-menu.component';
import { HomeComponent } from './components/home/home.component';
import { FooterComponent } from './components/footer/footer.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { NewHomeComponent } from './components/home/new-home/new-home.component';
import { BarnerComponent } from './barner/barner.component';
import { CreateAppointmentComponent } from './components/create-appointment/create-appointment.component';
import { LoginComponent } from './components/login/login.component';
import { ListDoctorsComponent } from './components/list-doctors/list-doctors.component';
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
import { TicketAppointmentComponent } from './ticket-appointment/ticket-appointment.component';

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
    ListDoctorsComponent,
    TicketAppointmentComponent,
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    MatStepperModule,
    MatCheckboxModule,
    BrowserAnimationsModule,
    MatButtonModule,
    MatFormFieldModule,
    MatButtonToggleModule,
    MatListModule,
    MatDividerModule,
    MatRadioModule,
    MatNativeDateModule,
    NgxMaskModule.forRoot(),
    MatRippleModule,
    MatSelectModule,
    MatIconModule,
    MatDatepickerModule,
    MatCardModule,
    FormsModule, MatInputModule,
    ReactiveFormsModule,
    RouterModule.forRoot([
      { path: '', component: CreateAppointmentComponent, pathMatch: 'full' },
      { path: 'paciente-login', component: LoginComponent },
      { path: 'crear-cita', component: HomeComponent },
      { path: 'ticket-cita', component: TicketAppointmentComponent },
      
    ],
      { useHash: false }),


  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
