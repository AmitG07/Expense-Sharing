import { Component, OnInit } from '@angular/core';
import { APIService } from '../Services/api.service';
import { AuthService } from '../Services/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-groups-invited-to',
  templateUrl: './groups-invited-to.component.html',
  styleUrls: ['./groups-invited-to.component.css']
})
export class GroupsInvitedToComponent implements OnInit {
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
    this.apiService.GetGroupMembersByUserId(this.userId).subscribe({
      next: (groups) => {
        console.log(groups);
        this.userGroups = groups.$values || [];
      },
      error: (error) => {
        console.error('Error fetching user groups:', error);
      }
    });
  }
}
