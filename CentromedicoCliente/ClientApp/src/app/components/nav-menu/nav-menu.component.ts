import { Component, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { AccountService } from 'src/app/services/account.service';
import { AutoUnsubscribe } from "ngx-auto-unsubscribe";

@AutoUnsubscribe()
@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css', './mobile-menu.css']
})
export class NavMenuComponent {
  userName: string;
  userRole: string;

  constructor(private accountSvc: AccountService,private router: Router) {
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
  currentUserName$ = this.accountSvc.currentUserName;
  currentUserRole$ = this.accountSvc.currentUserRole;


  logOut() {
    this.accountSvc.logout();
    console.log("logout")
              
      this.router.navigate(['/']);
  
  }

  ngOnDestroy() {
    // We'll throw an error if it doesn't
  }
}
