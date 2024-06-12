import { Injectable } from '@angular/core';
import { HttpEvent, HttpInterceptor, HttpHandler, HttpRequest, HttpResponse, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { MatSnackBar } from '@angular/material/snack-bar';

@Injectable()
export class SnackbarShowInterceptor implements HttpInterceptor {

  constructor(private snackBar: MatSnackBar) {}

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(req).pipe(
      map(event => {
        if (event instanceof HttpResponse) {
          if (event.status === 200 && event.body?.message) {
            this.snackBar.open(event.body.message, 'Закрити', {
              duration: 3000,
              panelClass: ['snackbar-success']
            });
          }
        }
        return event;
      }),
      catchError((error: HttpErrorResponse) => {
        let errorMessage = 'Сталася невідома помилка';
        if (error.error?.message) {
          errorMessage = error.error.message;
        }
        this.snackBar.open(errorMessage, 'Закрити', {
          duration: 3000,
          panelClass: ['snackbar-error']
        });
        return throwError(() => error);
      })
    );
  }
}
