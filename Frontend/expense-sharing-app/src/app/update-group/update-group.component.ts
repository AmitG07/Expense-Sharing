import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { APIService } from '../Services/api.service';
import { AlertService } from '../Services/alert.service';

@Component({
  selector: 'app-update-group',
  templateUrl: './update-group.component.html',
  styleUrls: ['./update-group.component.css']
})
export class UpdateGroupComponent implements OnInit {
  updateGroupForm: FormGroup;
  groupId: number;
  originalAdminId: number; // Store original AdminId

  constructor(
    private formBuilder: FormBuilder,
    private apiService: APIService,
    private alertService: AlertService,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.groupId = +this.route.snapshot.paramMap.get('groupId'); // Fetch groupId from route params
    this.initializeForm();
    this.fetchGroupDetails();
  }

  initializeForm(): void {
    this.updateGroupForm = this.formBuilder.group({
      GroupName: ['', [Validators.required, Validators.maxLength(50)]],
      Description: ['', [Validators.required, Validators.maxLength(200)]],
      CreatedDate: ['', Validators.required],
      GroupAdminId: [''],
      TotalMembers: [0],
      TotalExpense: [0.0],
      GroupMember: [[]],
      Expense: [[]]
    });
  }

  fetchGroupDetails(): void {
    this.apiService.GetGroupByGroupId(this.groupId).subscribe({
      next: (groupData: any) => {
        // Store original AdminId
        this.originalAdminId = groupData.groupAdminId;

        // Convert date to YYYY-MM-DD format
        const formattedDate = groupData.createdDate ? new Date(groupData.createdDate).toISOString().substring(0, 10) : new Date().toISOString().substring(0, 10);

        this.updateGroupForm.patchValue({
          GroupName: groupData.groupName,
          Description: groupData.description,
          CreatedDate: formattedDate,
          GroupAdminId: groupData.groupAdminId // Assign GroupAdminId to form field
        });

        console.log('Fetched group data:', groupData);
        console.log('Form values after patching:', this.updateGroupForm.value);
      },
      error: (error: any) => {
        console.error('Error fetching group details:', error);
        this.alertService.openSnackBar('Failed to fetch group details.');
      }
    });
  }

  updateGroup(): void {
    if (this.updateGroupForm.valid) {
      const formData = { ...this.updateGroupForm.value, GroupId: this.groupId, GroupAdminId: this.originalAdminId };
      this.apiService.UpdateGroup(this.groupId, formData).subscribe({
        next: (response: any) => {
          this.alertService.openSnackBar('Group updated successfully');
          this.router.navigate(['my-group']);
        },
        error: (error: any) => {
          console.error('Error updating group:', error);
          this.alertService.openSnackBar('Failed to update group. Please try again.');
        }
      });
    } else {
      this.alertService.openSnackBar('Please fill in all required fields.');
    }
  }
}
