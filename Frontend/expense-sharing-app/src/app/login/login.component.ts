import { Component, OnInit } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { APIService } from '../Services/api.service';
import { AlertService } from '../Services/alert.service';
import { Router } from '@angular/router';
import { AuthService } from '../Services/auth.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})

export class LoginComponent implements OnInit {

  result: any;
  
  constructor(
    private builder: FormBuilder,
    private _alert: AlertService,
    private service: APIService,
    private router: Router,
    private auth: AuthService
  ) { }

  ngOnInit(): void {
  }

  loginForm = this.builder.group({
    Email: this.builder.control('', [Validators.required, Validators.email]),
    Password: this.builder.control('', [Validators.required, Validators.minLength(4), Validators.maxLength(15)])
  });

  Login() {
    if (this.loginForm.valid) {
      this.service.LoginCustomer(this.loginForm.value).subscribe({
        next: (item: any) => {
          this.result = item;

          if (this.result != null) {
            if (this.result.password === this.loginForm.value.Password) {
              this.auth.CreatingUserSession(this.result)
              this._alert.openSnackBar('Login Successfully');
              this.router.navigate(['/']);
            }
            else {
              this._alert.openSnackBar('Invalid Password!');
            }
          }
        },
      });
    }
  }
}
