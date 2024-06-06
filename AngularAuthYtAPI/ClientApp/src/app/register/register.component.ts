// register.component.ts

import { Component } from '@angular/core';
import { UserService } from '../services/user.service';
import { UserRegistration } from '../models/user-registration.model';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent {

  user: UserRegistration = {
    firstName: '',
    lastName: '',
    username: '',
    password: '',
    email: ''
  };

  constructor(private userService: UserService) { }

  onSubmit(): void {
    this.userService.register(this.user).subscribe(
      response => {
        // Обробка успішної реєстрації
        console.log(response);
      },
      error => {
        // Обробка помилки реєстрації
        console.error(error);
      }
    );
  }
}
