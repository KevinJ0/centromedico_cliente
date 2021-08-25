import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { Observable } from 'rxjs';

import { take, map } from 'rxjs/operators';
import { AccountService } from '../services/account.service';


@Injectable({
  providedIn: 'root'
})
export class AuthGuardService implements CanActivate {

  constructor(private acct: AccountService, private router: Router) { }

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> {
    return this.acct.isLoggesIn.pipe(take(1), map((loginStatus: boolean) => {
      const destination: string = state.url;

      // To check if user is not logged in
      if (!loginStatus) {
        this.router.navigate(['/login'], { queryParams: { returnUrl: state.url } });

        return false;
      }

      // if the user is already logged in
      switch (destination) {
        case '/analisis-categoria':
          {
            if (localStorage.getItem("userRole") === "Manager" || localStorage.getItem("userRole") === "Admin" || localStorage.getItem("userRole") === "Moderator") {
              return true;
            }
          }

        case '/resultados':
          {
            if (localStorage.getItem("userRole") === "Manager" ) {
              this.router.navigate(['/access-denied'])

              return false;
            }

            if (localStorage.getItem("userRole") === "Admin") {

              return true;
            }

          }

        default:
          return false;
      }

    }));


  }



}
