import { Component } from '@angular/core';
import { MaterialModule } from "./shared/material.module";
import { MatIconRegistry } from "@angular/material/icon";
import { DomSanitizer, SafeResourceUrl } from "@angular/platform-browser";
import { trigger, style, animate, transition } from '@angular/animations';
import * as _moment from 'moment';
import { NavigationEnd, NavigationStart, Router } from '@angular/router';
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
  showLoadingIndicator = true;
  constructor(
    private domSanitizer: DomSanitizer,
    private matIconRegistry: MatIconRegistry,
    private router: Router) {
    this.router.events.subscribe((routerEvent: Event) => {
      if (routerEvent instanceof NavigationStart)
        this.showLoadingIndicator = true;

      if (routerEvent instanceof NavigationEnd)
        this.showLoadingIndicator = false;

    });

    moment.locale('es');

    this.matIconRegistry.addSvgIcon(
      "twitter",
      this.domSanitizer.bypassSecurityTrustResourceUrl("./assets/icons/twitter.svg")
    ).addSvgIcon(
      "instagram",
      this.domSanitizer.bypassSecurityTrustResourceUrl("./assets/icons/instagram.svg")
    );
  }
}

