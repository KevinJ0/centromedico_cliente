import { Component, OnInit, AfterViewInit } from '@angular/core';
import { AccountService } from 'src/app/services/account.service';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css', './mobile-menu.css']
})
export class NavMenuComponent {
  userName: string;
  userRole: string;

  constructor(private accountSvc: AccountService) {


  }
  ngOnInit(): void {
    this.currentUserName$.subscribe(r => {
      this.userName = r;
    });

    this.loadScript('../../../assets/js/main.js');
  }



  public loadScript(url: string) {
    const body = <HTMLDivElement>document.body;
    const script = document.createElement('script');
    script.innerHTML = '';
    script.src = url;
    script.async = false;
    script.defer = true;
    body.appendChild(script);
  }

  isExpanded = false;
  currentUserName$ = this.accountSvc.UserName;
  currentUserRole$ = this.accountSvc.UserRole;

  collapse() {
    this.isExpanded = false;
  }

  toggle() {
    this.isExpanded = !this.isExpanded;
  }
}
