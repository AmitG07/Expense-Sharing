import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { APIService } from '../Services/api.service';
import { Router } from '@angular/router';
import { AlertService } from '../Services/alert.service';

@Component({
  selector: 'app-update-group-member',
  templateUrl: './update-group-member.component.html',
  styleUrls: ['./update-group-member.component.css']
})
export class UpdateGroupMemberComponent implements OnInit {
  updateGroupMemberForm: FormGroup;
  groupMemberId: number;
  groupId: number;

  constructor(
    private formBuilder: FormBuilder,
    private apiService: APIService,
    private alertService: AlertService,
    private router: Router,
    private route: ActivatedRoute
  ) { }

  ngOnInit(): void {
    this.groupMemberId = Number(this.route.snapshot.paramMap.get('groupMemberId')); // Use Number() to parse the parameter
    console.log("GroupMemberID:", this.groupMemberId);

    if (isNaN(this.groupMemberId)) {
      console.error('Invalid groupMemberId:', this.groupMemberId);
      this.alertService.openSnackBar('Invalid group member ID.');
      this.router.navigate(['/error']); // Navigate to an error page or handle appropriately
      return;
    }

    this.initializeForm();
    this.fetchGroupMemberDetails();
  }

  initializeForm(): void {
    this.updateGroupMemberForm = this.formBuilder.group({
      GroupId: [{ value: '', disabled: true }],
      UserId: ['', [Validators.required]],
      UserExpense: [null, [Validators.required, Validators.min(0)]],
      GivenAmount: [null, [Validators.required, Validators.min(0)]],
      TakenAmount: [null, [Validators.required, Validators.min(0)]]
    });
  }

  fetchGroupMemberDetails(): void {
    console.log(`Fetching group member details for groupMemberId: ${this.groupMemberId}`);
    this.apiService.GetGroupMemberByIdAsync(this.groupMemberId).subscribe({
      next: (groupMemberData: any) => {
        this.groupId = groupMemberData.groupId;
        console.log('Fetched groupMemberData:', groupMemberData);

        if (!groupMemberData) {
          console.error('No valid group member data found.');
          this.alertService.openSnackBar('No group member details found or data is incomplete.');
          return;
        }

        this.updateGroupMemberForm.patchValue({
          GroupId: groupMemberData.groupId,
          UserId: groupMemberData.userId,
          UserExpense: groupMemberData.userExpense,
          GivenAmount: groupMemberData.givenAmount,
          TakenAmount: groupMemberData.takenAmount
        });

        console.log('Form values after patching:', this.updateGroupMemberForm.value);
      },
      error: (error: any) => {
        console.error('Error fetching group member details:', error);
        this.alertService.openSnackBar('Failed to fetch group member details.');
      }
    });
  }

  updateGroupMember(): void {
    if (this.updateGroupMemberForm.valid) {
      const formData = { ...this.updateGroupMemberForm.getRawValue(), GroupMemberId: this.groupMemberId, GroupId: this.groupId };
      console.log('Form data to be submitted:', formData);

      this.apiService.UpdateGroupMember(this.groupMemberId, formData).subscribe({
        next: () => {
          this.alertService.openSnackBar('Group member updated successfully');
          this.router.navigate(['/view-group/' + this.groupId]);
        },
        error: (error: any) => {
          console.error('Error updating group member:', error);
          this.alertService.openSnackBar('Failed to update group member. Please try again.');
        }
      });
    } else {
      this.alertService.openSnackBar('Please fill in all required fields.');
    }
  }
}
