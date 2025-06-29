import { HttpClient } from '@angular/common/http';
import { Component, inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { AppUser } from './models/app-user.model';
import { FormBuilder, FormControl, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormField } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    RouterOutlet,
    FormsModule, ReactiveFormsModule,
    MatFormField, MatInputModule, MatButtonModule
  ],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  http = inject(HttpClient); // post, get, put, delete
  fB = inject(FormBuilder);
  userResponse: AppUser | undefined;
  error: string | undefined;
  users: AppUser[] | undefined;
  private _userIn: string = '';
  arePasswordsMatch: boolean | undefined;

  //#region form group
  signUpFg = this.fB.group({
    emailCtrl: ['', [Validators.required, Validators.email]], // formControl
    userNameCtrl: ['', [Validators.required]],
    ageCtrl: ['', [Validators.required, Validators.min(18), Validators.max(70)]],
    passwordCtrl: ['', [Validators.minLength(4), Validators.maxLength(8)]],
    confirmPasswordCtrl: ['', [Validators.required]]
  });

  get EmailCtrl(): FormControl {
    return this.signUpFg.get('emailCtrl') as FormControl;
  }

  get UserNameCtrl(): FormControl {
    return this.signUpFg.get('userNameCtrl') as FormControl;
  }

  get AgeCtrl(): FormControl {
    return this.signUpFg.get('ageCtrl') as FormControl;
  }

  get PasswordCtrl(): FormControl {
    return this.signUpFg.get('passwordCtrl') as FormControl;
  }

  get ConfirmPasswordCtrl(): FormControl {
    return this.signUpFg.get('confirmPasswordCtrl') as FormControl;
  }
  //#endregion

  register(): void {
    if (this.PasswordCtrl.value === this.ConfirmPasswordCtrl.value) {
      this.arePasswordsMatch = true;

      let userInput: AppUser = {
        email: this.EmailCtrl.value,
        userName: this.UserNameCtrl.value,
        age: this.AgeCtrl.value,
        password: this.PasswordCtrl.value,
        confirmPassword: this.ConfirmPasswordCtrl.value
      }

      this.http.post<AppUser>
        ('http://localhost:5000/api/account/register', userInput).subscribe({
          next: (response) => {
            console.log(response);
            this.userResponse = response
          },
          error: (apiError) => {
            this.error = apiError.error
            console.log(apiError)
          }
        });
    }
    else {
      this.arePasswordsMatch = false;
    }

  }

  getAll(): void {
    this.http.get<AppUser[]>
      ('http://localhost:5000/api/account/get-all').subscribe({
        next: (response) => {
          this.users = response
          console.log(response)
        }
      });
  }

  getByUserName(): void {
    this._userIn = this.UserNameCtrl.value;

    this.http.get<AppUser>
      ('http://localhost:5000/api/account/get-by-username/' + this._userIn).subscribe({
        next: (res) => {
          console.log(res);
          this.userResponse = res
        },
        error: (apiError) => {
          console.log(apiError.error)
          this.error = apiError.error
        }
      });
  }

  updateUserById(): void {
    let userInput: AppUser = {
      email: this.EmailCtrl.value,
      userName: this.UserNameCtrl.value,
      age: this.AgeCtrl.value,
      password: this.PasswordCtrl.value,
      confirmPassword: this.ConfirmPasswordCtrl.value
    }

    this.http.put
      ('http://localhost:5000/api/account/update-by-id/68577c05534b5d19c5fa1846', userInput).subscribe();
  }

  deleteUserById(): void {
    this.http.delete
      ('http://localhost:5000/api/account/delete-by-id/684826a43f7d8f1a58713546').subscribe();
  }
}