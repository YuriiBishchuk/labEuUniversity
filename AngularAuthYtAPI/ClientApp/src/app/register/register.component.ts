import { Component } from '@angular/core';
import { UserService } from '../services/user.service';
import { UserRegistration } from '../models/user-registration.model';
import { Router } from '@angular/router';
import { tap } from 'rxjs';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { CustomValidators } from '../helpers/custom-validator';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent {

  registerForm = new FormGroup({
    email: new FormControl(null, [Validators.required, Validators.email]),
    username: new FormControl(null, [Validators.required]),
    firstname: new FormControl(null, [Validators.required]),
    lastname: new FormControl(null, [Validators.required]),
    password: new FormControl(null, [Validators.required]),
    passwordConfirm: new FormControl(null, [Validators.required])
  }, { validators: CustomValidators.passwordsMatching });

  constructor(
    private router: Router,
    private userService: UserService
  ) { }

  register() {
    if (!this.registerForm.valid) {
      return;
    }
    this.userService.register(this.registerForm.value as unknown as UserRegistration).pipe(
      // If registration was successfull, then navigate to login route
      tap(() => this.router.navigate(['../login']))
    ).subscribe();
  }
}
