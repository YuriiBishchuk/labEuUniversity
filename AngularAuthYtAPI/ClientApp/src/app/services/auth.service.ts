import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { tap } from 'rxjs/operators';
import { ConfigService } from './config.service';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private apiUrl: string;

  constructor(private http: HttpClient, private configService: ConfigService) {
    this.apiUrl = this.configService.apiUrl;
  }

  login(data: {email: string, password: string}) {
    const loginUrl = `${this.apiUrl}/api/User/authenticate`;
    return this.http.post<any>(loginUrl, data)
      .pipe(
        tap(res => {
          console.log(res);
          if (res && res.accessToken && res.refreshToken) {
            
            localStorage.setItem('access_token', res.accessToken);
            localStorage.setItem('refresh_token', res.refreshToken);
          }
        })
      );
  }

  register(data: { firstName: string, lastName: string, email: string, password: string }) {
    const registerUrl = `${this.apiUrl}/api/User/register`;
    return this.http.post<any>(registerUrl, data)
      .pipe(
        tap(() => {
          // Після успішної реєстрації автоматично викликаємо метод authenticate (login)
          return this.login({ email: data.email, password: data.password });
        })
      );
  }

  isLoggedIn(): boolean {
    return !!localStorage.getItem('access_token');
  }

  getAccessToken(): string | null {
    return localStorage.getItem('access_token');
  }

  logout() {
    localStorage.removeItem('access_token');
    localStorage.removeItem('refresh_token');
  }

  refreshToken(refreshToken: string) {
    const refreshUrl = `${this.apiUrl}/api/User/refresh`;
    return this.http.post<any>(refreshUrl, { refreshToken })
      .pipe(
        tap(res => {
          if (res && res.accessToken) {
            localStorage.setItem('access_token', res.accessToken);
          }
        })
      );
  }
}
