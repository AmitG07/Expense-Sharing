import { Component, OnInit } from '@angular/core';
import { APIService } from '../../Services/api.service';
import { AuthService } from '../../Services/auth.service';

@Component({
  selector: 'app-user-details',
  templateUrl: './user-details.component.html',
  styleUrls: ['./user-details.component.css']
})
export class UserDetailsComponent implements OnInit {

  userId: number;
  userGroups: any[] = []; // Ensure it's an array

  constructor(
    private apiService: APIService,
    private authService: AuthService
  ) { }

  ngOnInit(): void {
    this.userId = this.authService.GetId();
    this.fetchUserGroups();
  }

  fetchUserGroups() {
    this.apiService.GetAllUsers().subscribe({
      next: (users) => {
        this.userGroups = users.$values || []; // Handle data format appropriately
        console.log(this.userGroups);
      },
      error: (error) => {
        console.error('Error fetching user groups:', error);
      }
    });
  }
}
