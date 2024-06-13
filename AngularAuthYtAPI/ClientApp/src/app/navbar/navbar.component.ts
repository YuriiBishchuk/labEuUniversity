import { Component } from '@angular/core';
import { AuthService } from '../services/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.scss'
})
export class NavbarComponent {

  constructor(private _authService: AuthService,  private router: Router){
    
  }
  public logout(): void {
     this._authService.logout();
     this.router.navigate(['../login'])
  }

  public get isAuthenticated() {
    return this._authService.isLoggedIn();
  } 
}
