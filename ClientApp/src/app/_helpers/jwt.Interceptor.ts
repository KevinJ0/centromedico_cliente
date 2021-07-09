import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent, HttpResponse, HttpErrorResponse } from '@angular/common/http';
import { AccountService } from '../services/account.service';
import { Observable, BehaviorSubject, pipe, throwError } from 'rxjs';
import { tap, catchError, switchMap, finalize, filter, take } from 'rxjs/operators';




@Injectable({
  providedIn: 'root'
})


export class JwtInterceptor implements HttpInterceptor {

  private isTokenRefreshing: boolean = false;

  tokenSubject: BehaviorSubject<string> = new BehaviorSubject<string>(null);

  constructor(private acct: AccountService) { }

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    // Check if the user is logging in for the first time
    var authReq = request.clone({
      setHeaders: {
        Accept: "application/json",
        "Content-Type": "application/json",
        Authorization: "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJKb3NlQGdtYWlsLmNvbSIsImp0aSI6IjdjOGY5ZGIyLTAyNzYtNDJkMS1iNTc3LTUyNTg1NjhjMTdlZSIsIm5hbWVpZCI6IjAxZTNhMjJiLTI2MjctNDgyMS05ZTBlLTE0NzE1MTNhOWY5NCIsInJvbGUiOiJQYXRpZW50IiwiTG9nZ2VkT24iOiI1LzI0LzIwMjEgMTA6Mjk6NTggUE0iLCJuYmYiOjE2MjE5MDk3OTgsImV4cCI6MTcxNDYyMzcxOCwiaWF0IjoxNjIxOTA5Nzk4LCJpc3MiOiJodHRwczovL2xvY2FsaG9zdDo0NDMzNyIsImF1ZCI6Imh0dHBzOi8vbG9jYWxob3N0OjQ0MzM3In0.Auc5Om1B4G5M5BJ31EEEtElCsBTug4WMO1ugChYdcEE"
      }
    });

    return next.handle(authReq).pipe(
      tap((event: HttpEvent<any>) => {
        if (event instanceof HttpResponse) {
          console.log("Success");
        }
      }),
      catchError((err): Observable<any> => {
        if (err instanceof HttpErrorResponse) {
          console.log((<HttpErrorResponse>err).status);
          console.log((<HttpErrorResponse>err).message);

          var grantType = "";
          if (request.url.toLowerCase().includes("auth"))
            grantType = request.body.grantType;

          if ((<HttpErrorResponse>err).status == 401 && grantType == "refresh_token") {

            console.log("TokenRefresh has expired");
            return <any>this.acct.logout();

          } else {
            switch ((<HttpErrorResponse>err).status) {
              case 401:
                console.log("Token expired. Attempting refresh ...");
                return this.handleHttpResponseError(request, next);
              case 400:
                return <any>this.acct.logout();
            }
          }
        } else {
          return throwError(this.handleError);
        }
      })

    );

  }


  // Global error handler method 
  private handleError(errorResponse: HttpErrorResponse) {
    let errorMsg: string;

    if (errorResponse.error instanceof Error) {
      // A client-side or network error occurred. Handle it accordingly.
      errorMsg = "An error occured : " + errorResponse.error.message;
    } else {
      // The backend returned an unsuccessful response code.
      // The response body may contain clues as to what went wrong,
      errorMsg = `Backend returned code ${errorResponse.status}, body was: ${errorResponse}`;
    }

    return throwError(errorMsg);
  }


  // Method to handle http error response
  private handleHttpResponseError(request: HttpRequest<any>, next: HttpHandler) {

    // First thing to check if the token is in process of refreshing
    if (!this.isTokenRefreshing)  // If the Token Refresheing is not true
    {
      this.isTokenRefreshing = true;

      // Any existing value is set to null
      // Reset here so that the following requests wait until the token comes back from the refresh token API call
      this.tokenSubject.next(null);

      /// call the API to refresh the token
      return this.acct.getNewRefreshToken().pipe(
        switchMap((tokenresponse: any) => {
          if (tokenresponse) {

            this.tokenSubject.next(tokenresponse.authToken.token);
            localStorage.setItem('loginStatus', '1');
            localStorage.setItem('jwt', tokenresponse.authToken.token);
            localStorage.setItem('username', tokenresponse.authToken.username);
            localStorage.setItem('expiration', tokenresponse.authToken.expiration);
            localStorage.setItem('userRole', tokenresponse.authToken.roles);
            localStorage.setItem('refreshToken', tokenresponse.authToken.refresh_token);
            console.log("Token refreshed...");
            return next.handle(this.attachTokenToRequest(request));
          }
          return <any>this.acct.logout();
        }),
        catchError(err => {
          //this.acct.logout();
          return this.handleError(err);
        }),
        finalize(() => {
          this.isTokenRefreshing = false;
        })
      );
    }
    else {
      this.isTokenRefreshing = false;
      return this.tokenSubject.pipe(filter(token => token != null),
        take(1),
        switchMap(token => {
          return next.handle(request);
        }));
    }
  }


  private attachTokenToRequest(request: HttpRequest<any>) {
    var token = localStorage.getItem('jwt');
       return request.clone({ setHeaders: { Authorization: `Bearer ${token}` } });
  }
}
