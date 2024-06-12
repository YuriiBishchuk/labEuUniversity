// login.component.ts

import { Component } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { UserService } from '../services/user.service';
import { tap } from 'rxjs';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent {
  loginForm: FormGroup = new FormGroup({
    email: new FormControl(null, [Validators.required, Validators.email]),
    password: new FormControl(null, [Validators.required]),
  });

  constructor(
    private userService: UserService,
    private router: Router
  ) { }

  login() {
    if (!this.loginForm.valid) {
      return;
    }
    this.userService.login(this.loginForm.value).pipe(
      // route to protected/dashboard, if login was successfull
      tap(() => this.router.navigate(['../../protected/dashboard']))
    ).subscribe();
  }

}
