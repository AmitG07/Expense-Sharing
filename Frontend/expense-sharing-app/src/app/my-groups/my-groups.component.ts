import { Component, OnInit } from '@angular/core';
import { APIService } from '../Services/api.service';
import { AuthService } from '../Services/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-my-groups',
  templateUrl: './my-groups.component.html',
  styleUrls: ['./my-groups.component.css']
})
export class MyGroupsComponent implements OnInit {
  userId: number;
  userGroups: any;

  constructor(
    private apiService: APIService,
    private authService: AuthService,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.userId = this.authService.GetId();
    this.fetchUserGroups();
  }

  fetchUserGroups() {
    this.apiService.GetGroupsByUserId(this.userId).subscribe({
      next: (groups) => {
        console.log(groups);
        this.userGroups = groups.$values || [];
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
          this.router.navigate(['/view-group']);
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
