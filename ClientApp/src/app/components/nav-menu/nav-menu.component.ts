import { Component, OnInit } from '@angular/core';
import { AccountService } from 'src/app/services/account.service';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css']
})
export class NavMenuComponent {
  userName: string ;
  userRole: string ;

  constructor(private accountSvc: AccountService) {


  }
  ngOnInit(): void {
    this.currentUserName$.subscribe(r => {
      this.userName = r;
    });
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
