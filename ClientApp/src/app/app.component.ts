import { Component } from '@angular/core';
import { MaterialModule } from "./shared/material.module";
import { MatIconRegistry } from "@angular/material/icon";
import { DomSanitizer, SafeResourceUrl } from "@angular/platform-browser"; 

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html'
})

export class AppComponent {
  title = 'app';
  constructor(
    private domSanitizer: DomSanitizer,
    private matIconRegistry: MatIconRegistry) {
      this.matIconRegistry.addSvgIcon(
        "twitter",
        this.domSanitizer.bypassSecurityTrustResourceUrl("./assets/icons/twitter.svg")
      ).addSvgIcon(
        "instagram",
        this.domSanitizer.bypassSecurityTrustResourceUrl("./assets/icons/instagram.svg")
      );
   } 
  }

