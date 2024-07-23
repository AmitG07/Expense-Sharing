import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { APIService } from '../Services/api.service';
import { AlertService } from '../Services/alert.service';
import { Router } from '@angular/router';
import { AuthService } from '../Services/auth.service';
import { tick } from '@angular/core/testing';

@Component({
  selector: 'app-create-group',
  templateUrl: './create-group.component.html',
  styleUrls: ['./create-group.component.css']
})
export class CreateGroupComponent implements OnInit {
  createGroupForm: FormGroup;
  loggedInUserId: number; // Variable to hold the logged-in user's ID

  constructor(
    private formBuilder: FormBuilder,
    private apiService: APIService,
    private alertService: AlertService,
    private router: Router,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    // Initialize the form and get logged-in user's ID
    this.initializeForm();
    this.loggedInUserId = this.authService.GetId();
    console.log(this.loggedInUserId);
  }

  initializeForm(): void {
    this.loggedInUserId = this.authService.GetId();
    this.createGroupForm = this.formBuilder.group({
        UserId: [this.loggedInUserId, Validators.required],
        GroupName: ['', [Validators.required, Validators.maxLength(50)]],
        Description: ['', [Validators.required, Validators.maxLength(200)]],
        CreatedDate: [new Date().toISOString().substring(0, 10), Validators.required],
        GroupAdminId: [this.loggedInUserId, Validators.required],
        TotalMembers: [0],
        TotalExpense: [0.0],
        GroupMember: [[]],
        Expense: [[]]
    });
  }  

  createGroup(): void {
    if (this.createGroupForm.valid) {
        console.log('Form value:', this.createGroupForm.value);

        this.apiService.CreateNewGroup(this.createGroupForm.value).subscribe({
            next: (response: any) => {
                this.alertService.openSnackBar('Group created successfully');
                this.router.navigate(['homepage']);
            },
            error: (error: any) => {
                console.error('Error creating group:', error);
                this.alertService.openSnackBar('Failed to create group. Please try again.');
            }
        });
    } else {
        console.log('Form invalid. Form value:', this.createGroupForm.value);
        console.log('Form errors:', this.createGroupForm.errors);
        this.alertService.openSnackBar('Please fill in all required fields.');
    }
  }      
}
