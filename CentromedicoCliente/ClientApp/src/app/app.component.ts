import { Component } from '@angular/core';
import { MatIconRegistry } from "@angular/material/icon";
import { DomSanitizer, SafeResourceUrl } from "@angular/platform-browser";
import { trigger, style, animate, transition } from '@angular/animations';
import * as _moment from 'moment';
const moment = _moment;

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  animations: [
    trigger(
      'inOutAnimation',
      [
        transition(
          ':enter',
          [
            style({ opacity: 0 }),
            animate('300ms ease-out',
              style({ opacity: 1 }))
          ]
        ),
        transition(
          ':leave',
          [
            style({ opacity: 1, position: "absolute" }),
            animate('300ms ease-in',
              style({ opacity: 0, position: "relative" }))
          ]
        )
      ]
    )
  ]
})

export class AppComponent {
  title = 'app';
  constructor(
    private domSanitizer: DomSanitizer,
    private matIconRegistry: MatIconRegistry) {
    moment.locale('es');

    this.matIconRegistry.addSvgIcon(
      "twitter",
      this.domSanitizer.bypassSecurityTrustResourceUrl("./assets/icons/twitter.svg")
    ).addSvgIcon(
      "instagram",
      this.domSanitizer.bypassSecurityTrustResourceUrl("./assets/icons/instagram.svg")
    ).addSvgIcon(
      "login",
      this.domSanitizer.bypassSecurityTrustResourceUrl("./assets/icons/login.svg")
    ).addSvgIcon(
      "mail",
      this.domSanitizer.bypassSecurityTrustResourceUrl("./assets/icons/mail.svg")
    ).addSvgIcon(
      "portfolio",
      this.domSanitizer.bypassSecurityTrustResourceUrl("./assets/icons/portfolio.svg")
    ).addSvgIcon(
      "medicals",
      this.domSanitizer.bypassSecurityTrustResourceUrl("./assets/icons/medical-team.svg")
    );
  }
}

