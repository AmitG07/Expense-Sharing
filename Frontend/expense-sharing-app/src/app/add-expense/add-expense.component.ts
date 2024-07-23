import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { APIService } from '../Services/api.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-add-expense',
  templateUrl: './add-expense.component.html',
  styleUrls: ['./add-expense.component.css']
})
export class AddExpenseComponent implements OnInit {
  addExpenseForm: FormGroup;
  currentGroupId: number;
  // currentUserId: number = 1; // Assuming this is obtained from a logged-in user's session

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
    this.addExpenseForm = this.fb.group({
      GroupId: [{ value: this.currentGroupId, disabled: true }],
      Description: ['', Validators.required],
      ExpenseAmount: [0, [Validators.required, Validators.min(0.01)]],
      PaidByUserId: ['', Validators.required],
      ExpenseCreatedAt: [new Date().toISOString().substring(0, 10)], // Set current date
      IsSettled: [false]
    });
  }

  addExpense() {
    if (this.addExpenseForm.valid) {
      const formData = { ...this.addExpenseForm.value, GroupId: this.currentGroupId };
      this.apiService.AddExpense(formData).subscribe({
        next: () => {
          alert('Expense added successfully');
          // Update user balance
          // this.updateUserBalance(this.addExpenseForm.get('PaidByUserId').value);
          // Navigate to view-group page after successful creation
          this.router.navigate(['/view-group/' + this.currentGroupId]); 
        },
        error: (error) => {
          console.error('Error adding expense:', error);
          if (error.error && error.error.message === 'User is not a member of the specified group.') {
            // Handle specific error message related to user not being a member
            this.addExpenseForm.get('PaidByUserId').setErrors({ notGroupMember: true });
          } else {
            alert('Failed to add the expense. Check console for details.');
          }
        }
      });
    }
  }
}
