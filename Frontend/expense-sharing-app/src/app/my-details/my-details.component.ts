import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { APIService } from '../Services/api.service';
import { AuthService } from '../Services/auth.service';

@Component({
  selector: 'app-my-details',
  templateUrl: './my-details.component.html',
  styleUrls: ['./my-details.component.css']
})
export class MyDetailsComponent implements OnInit {

  userId: number;
  userDetails: any; // Assuming userDetails is an object to store fetched user details

  constructor(
    private route: ActivatedRoute,
    private apiService: APIService,
    private authService: AuthService
  ) { }

  ngOnInit(): void {
    this.userId = this.authService.GetId(); // Assuming GetId() returns the logged-in user's ID
    this.fetchUserDetails();
  }

  fetchUserDetails(): void {
    this.apiService.GetUserById(this.userId).subscribe({
      next: (user) => {
        console.log(user); // Check the structure of the returned user object in the console
        this.userDetails = user; // Assign fetched user details to userDetails
      },
      error: (error) => {
        console.error('Error fetching user details:', error);
        // Handle error as needed
      }
    });
  }

}
