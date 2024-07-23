import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { APIService } from '../Services/api.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-add-group-members',
  templateUrl: './add-group-members.component.html',
  styleUrls: ['./add-group-members.component.css']
})
export class AddGroupMembersComponent implements OnInit {
  addGroupMembersForm: FormGroup;
  currentGroupId: number;

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private apiService: APIService,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      this.currentGroupId = +params.get('groupId'); // Fetch group ID from route params
      console.log('Current Group ID:', this.currentGroupId);
      this.initializeForm();
    });
  }

  initializeForm() {
    this.addGroupMembersForm = this.fb.group({
      GroupId: [{ value: this.currentGroupId, disabled: true }],
      UserId: ['', Validators.required]
    });
  }

  addGroupMembers() {
    if (this.addGroupMembersForm.valid) {
      const formData = { ...this.addGroupMembersForm.value, GroupId: this.currentGroupId };
      this.apiService.AddMemberToGroup(formData).subscribe({
        next: () => {
          alert('Member added successfully');
          this.router.navigate(['/view-group/' + this.currentGroupId]); // Navigate to view group page after successful creation
        },
        error: (error) => {
          console.error('Error adding member:', error);
          const errorMessage = error.error?.message || 'Failed to add the member. Please try again.';
          alert(errorMessage);
        }
      });
    }
  }  
}
