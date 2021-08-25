import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { UserInfo } from '../interfaces/InterfacesDto';
import { BehaviorSubject, throwError, of, Observable } from 'rxjs';
import { map, catchError } from 'rxjs/operators';
import { Router } from '@angular/router';
@Injectable({
  providedIn: 'root'
})
export class AccountService {
  baseUrl: string;

  // Url to access our Web API’s
  private baseUrlLogin: string = "api/account/login";

  private baseUrlRegister: string = "api/account/register";

  // Token Controller
  private baseUrlToken: string = "api/token/auth";


  // User related properties
  public loginStatus = new BehaviorSubject<boolean>(this.checkLoginStatus());
  public UserName = new BehaviorSubject<string>(localStorage.getItem('username'));
  public UserRole = new BehaviorSubject<string>(localStorage.getItem('userRoles'));

  constructor(private router: Router, private http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.baseUrl = baseUrl;
  }

  getNewRefreshToken(): Observable<any> {

    let usercredential = localStorage.getItem('username');
    let refreshToken = localStorage.getItem('refreshToken');
    const grantType = "refresh_token";


    return this.http.post<any>(this.baseUrl + this.baseUrlToken, { usercredential, refreshToken, grantType }).pipe(
      map((result: any) => {
        if (result && result.authToken.token) {
          this.loginStatus.next(true);
          localStorage.setItem('loginStatus', '1');
          localStorage.setItem('jwt', result.authToken.token);
          localStorage.setItem('username', result.authToken.username);
          localStorage.setItem('expiration', result.authToken.expiration);
          localStorage.setItem('userRole', result.authToken.roles);
          localStorage.setItem('refreshToken', result.authToken.refresh_token);
        }

        return <any>result;

      })
    );

  }


  //Login Method
  login(usercredential: string, password: string) {
    const grantType = "password";
    
    return this.http.post<any>(this.baseUrl + this.baseUrlToken, { usercredential, password, grantType }).pipe(
      map((result: any) => {

        // login successful if there's a jwt token in the response
        if (result && result.authToken.token) {
          // store user details and jwt token in local storage to keep user logged in between page refreshes

          this.loginStatus.next(true);
          localStorage.setItem('loginStatus', '1');
          localStorage.setItem('jwt', result.authToken.token);
          localStorage.setItem('username', result.authToken.username);
          localStorage.setItem('expiration', result.authToken.expiration);
          localStorage.setItem('userRole', result.authToken.roles);
          localStorage.setItem('refreshToken', result.authToken.refresh_token);
          this.UserName.next(result.authToken.username);
          this.UserRole.next(result.authToken.roles);
 
        }
        console.log(result);
        return result;

      })

    );
  }

  checkLoginStatus(): boolean {

    var loginCookie = localStorage.getItem("loginStatus");

    if (loginCookie == "1") {
      if (localStorage.getItem('jwt') != null || localStorage.getItem('jwt') != undefined) {
        return true;
      }
    }
    return false;
  }


  isUserDocIdentConfirm(): Observable<boolean> {
    return this.http.get<any>(this.baseUrl + "api/account/isUserDocIdentConfirm")
      .pipe(map((result: any) => {

        return result;

      }));
  }

  getUserInfo(): Observable<UserInfo> {
    return this.http.get<UserInfo>(this.baseUrl + "api/account/getUserInfo")
      .pipe(map((data: UserInfo) => data),
        catchError(err => {

          return throwError(err);
        })
      );

  }


  setUserInfo(userInfo: UserInfo): Observable<boolean> {
    return this.http.post<boolean>(this.baseUrl + "api/account/setUserInfo", userInfo)
      .pipe(
        map(() => true),
        catchError(err => {
          return throwError(err);
        })
      );
  }


  logout() {
    // Set Loginstatus to false and delete saved jwt cookie
    this.loginStatus.next(false);
    localStorage.removeItem('jwt');
    localStorage.removeItem('userRole');
    localStorage.removeItem('username');
    localStorage.removeItem('expiration');
    localStorage.setItem('loginStatus', '0');
    //this.router.navigate(['/login']);
    console.log("Logged Out Successfully");

  }

  
  get isLoggesIn() {
    return this.loginStatus.asObservable();
  }

  get currentUserName() {
    return this.UserName.asObservable();
  }

  get currentUserRole() {
    return this.UserRole.asObservable();
  }

}
