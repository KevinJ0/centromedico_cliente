import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './components/nav-menu/nav-menu.component';
import { HomeComponent } from './components/home/home.component';
import { NbLayoutModule, NbThemeModule } from '@nebular/theme';
import { FooterComponent } from './components/footer/footer.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { NewHomeComponent } from './components/home/new-home/new-home.component';
import { BarnerComponent } from './barner/barner.component';


@NgModule({
  declarations: [ 
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    FooterComponent,
    NewHomeComponent,
    BarnerComponent,
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    NbThemeModule.forRoot({ name: 'default' }),
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full' },
    
    ], { useHash: false }),
    NbLayoutModule,

  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
