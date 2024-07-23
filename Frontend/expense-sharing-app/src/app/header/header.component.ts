import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { AuthService } from '../Services/auth.service';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css']
})
export class HeaderComponent implements OnInit {

  userName: string;
  role: boolean;
  userNameSubscription: Subscription;

  constructor(
    private router: Router,
    private authService: AuthService
  ) { }

  ngOnInit(): void {
    this.userNameSubscription = this.authService
      .GetUserSession()
      .subscribe((user) => {
        this.userName = this.authService.GetUserName(user);
        this.role = this.authService.GetUserRole(user);
      });
  }

  ngOnDestroy(): void {
    this.userNameSubscription.unsubscribe();
  }

  LogoutButton() {
    this.authService.RemoveUserSession();
    this.router.navigate(['/']);
  }
}
