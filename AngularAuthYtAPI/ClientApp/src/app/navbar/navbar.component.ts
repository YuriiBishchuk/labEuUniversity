import { Component } from '@angular/core';
import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.scss'
})
export class NavbarComponent {
  title = 'Tic Tac Toe';

  constructor(private _authService: AuthService){
    
  }
  public logout(): void {
    // todo
  }

  public get isAuthenticated() {
    return this._authService.isLoggedIn();
  } 
}
