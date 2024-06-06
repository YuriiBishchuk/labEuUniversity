import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { UserLogin } from '../models/user-login.model';
import { UserRegistration } from '../models/user-registration.model';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  private baseUrl = 'https://localhost:7058/api/User';

  constructor(private http: HttpClient) { }

  register(user: UserRegistration): Observable<any> {
    return this.http.post(`${this.baseUrl}/register`, user);
  }

  login(user: UserLogin): Observable<any> {
    return this.http.post(`${this.baseUrl}/authenticate`, user);
  }
}
