// login.component.ts

import { Component } from '@angular/core';
import { UserService } from '../services/user.service';
import { UserLogin } from '../models/user-login.model';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent {

  user: UserLogin = {
    email: '',
    password: ''
  };

  constructor(private userService: UserService) { }

  onSubmit(): void {
    this.userService.login(this.user).subscribe(
      response => {
        // Обробка успішного входу
        console.log(response);
      },
      error => {
        // Обробка помилки входу
        console.error(error);
      }
    );
  }
}
