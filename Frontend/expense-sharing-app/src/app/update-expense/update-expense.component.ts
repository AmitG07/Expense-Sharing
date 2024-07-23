import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { APIService } from '../Services/api.service';
import { Router } from '@angular/router';
import { AlertService } from '../Services/alert.service';

@Component({
  selector: 'app-update-expense',
  templateUrl: './update-expense.component.html',
  styleUrls: ['./update-expense.component.css']
})
export class UpdateExpenseComponent implements OnInit {
  updateExpenseForm: FormGroup;
  expenseId: number;
  groupId: number;
  currentPaidByUserId: string; // To store the current PaidByUserId

  constructor(
    private formBuilder: FormBuilder,
    private apiService: APIService,
    private alertService: AlertService,
    private router: Router,
    private route: ActivatedRoute
  ) { }

  ngOnInit(): void {
    this.expenseId = +this.route.snapshot.paramMap.get('expenseId');
    console.log("ExpenseID:" + this.expenseId);
    this.initializeForm();
    this.fetchExpenseDetails(); // Fetch expense details when component initializes
  }

  initializeForm(): void {
    this.updateExpenseForm = this.formBuilder.group({
      GroupId: [{ value: '', disabled: true }],
      Description: ['', [Validators.required, Validators.maxLength(200)]],
      ExpenseAmount: [null, [Validators.required, Validators.min(0.01)]],
      PaidByUserId: [{ value: '', disabled: true }],
      ExpenseCreatedAt: [new Date().toISOString().substring(0, 10)],
      IsSettled: [false]
    });
  }

  fetchExpenseDetails(): void {
    console.log(`Fetching expense details for expenseId: ${this.expenseId}`);
    this.apiService.GetExpensesByExpenseId(this.expenseId).subscribe({
      next: (expenseData: any) => {
        this.groupId = expenseData.groupId;
        console.log('Fetched expenseData:', expenseData);

        // Assume expenseData is an object containing expense details
        if (!expenseData) {
          console.error('No valid expense data found.');
          this.alertService.openSnackBar('No expense details found or data is incomplete.');
          return;
        }

        const formattedDate = expenseData.expenseCreatedAt ? new Date(expenseData.expenseCreatedAt).toISOString().substring(0, 10) : new Date().toISOString().substring(0, 10);

        // Store the current PaidByUserId
        this.currentPaidByUserId = expenseData.paidByUserId || '';

        // Patch the form with fetched data
        this.updateExpenseForm.patchValue({
          GroupId: this.expenseId,
          Description: expenseData.description || '',
          ExpenseAmount: expenseData.expenseAmount != null ? expenseData.expenseAmount : null,
          PaidByUserId: this.currentPaidByUserId,
          ExpenseCreatedAt: formattedDate,
          IsSettled: expenseData.isSettled != null ? expenseData.isSettled : false
        });

        console.log('Form values after patching:', this.updateExpenseForm.value);
      },
      error: (error: any) => {
        console.error('Error fetching expense details:', error);
        this.alertService.openSnackBar('Failed to fetch expense details.');
      }
    });
  }

  updateExpense(): void {
    if (this.updateExpenseForm.valid) {
      const formData = { ...this.updateExpenseForm.value, ExpenseId: this.expenseId, GroupId: this.groupId, PaidByUserId: this.currentPaidByUserId };
      console.log('Form data to be submitted:', formData); 

      // Set the PaidByUserId to the current value fetched
      formData.PaidByUserId = this.currentPaidByUserId;

      console.log('Form data to be submitted:', formData);
      this.apiService.UpdateExpense(this.expenseId, formData).subscribe({
        next: () => {
          this.alertService.openSnackBar('Expense updated successfully');
          this.router.navigate(['/view-group/'+ this.groupId]);
        },
        error: (error: any) => {
          console.error('Error updating expense:', error);
          this.alertService.openSnackBar('Failed to update expense. Please try again.');
        }
      });
    } else {
      this.alertService.openSnackBar('Please fill in all required fields.');
    }
  }
}
