import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { AuthService } from '../Services/auth.service';
import { APIService } from '../Services/api.service';

@Component({
  selector: 'app-homepage',
  templateUrl: './homepage.component.html',
  styleUrls: ['./homepage.component.css']
})
export class HomepageComponent implements OnInit {

  userName: string;
  userNameSubscription: Subscription;
  userId: number;
  userGroups: any;
  role: boolean;

  constructor(
    private router: Router,
    private authService: AuthService,
    private apiService: APIService,
  ) { }

  ngOnInit(): void {
    this.userNameSubscription = this.authService
      .GetUserSession()
      .subscribe((user) => {
        this.userName = this.authService.GetUserName(user);
        this.role = this.authService.GetUserRole(user);
        console.log(this.userName);
      });
      this.userId = this.authService.GetId();
      this.fetchUserGroups();
  }

  ngOnDestroy(): void {
    this.userNameSubscription.unsubscribe();
  }

  fetchUserGroups() {
    this.apiService.GetAllGroups().subscribe({
      next: (response) => {
        // Extract the groups from the "$values" property
        this.userGroups = response.$values || [];
      },
      error: (error) => {
        console.error('Error fetching user groups:', error);
      }
    });
  }

  editGroup(groupId: number): void {
    this.router.navigate(['/edit-group', groupId]); // Adjust the route as per your application
  }

  deleteGroup(groupId: number): void {
    if (confirm('Are you sure you want to delete this group?')) {
      this.apiService.DeleteMyGroup(groupId).subscribe({
        next: () => {
          // Update the local userGroups array after deletion
          this.userGroups = this.userGroups.filter(group => group.GroupId !== groupId);
          alert('Group deleted successfully');
        },
        error: (error) => {
          console.error('Error deleting group:', error);
          alert('Failed to delete the group');
        },
        complete: () => {
          // After deletion, fetch the updated list of groups
          this.fetchUserGroups();
        }
      });
    }
  }
}
