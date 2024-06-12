import { Component } from '@angular/core';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.scss'
})
export class NavbarComponent {
  title = 'Tic Tac Toe';
  public isAuthenticated = false;
  
  public logout(): void {
    // todo
  }
}
