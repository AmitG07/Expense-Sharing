import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { APIService } from '../Services/api.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-add-expense-split',
  templateUrl: './add-expense-split.component.html',
  styleUrls: ['./add-expense-split.component.css']
})
export class AddExpenseSplitComponent implements OnInit {
  addExpenseSplitForm: FormGroup;
  currentExpenseId: number;

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private apiService: APIService,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      this.currentExpenseId = +params.get('expenseId');
      console.log('Current Expense ID:', this.currentExpenseId);
      this.initializeForm();
    });
  }

  initializeForm() {
    this.addExpenseSplitForm = this.fb.group({
      ExpenseId: [{ value: this.currentExpenseId, disabled: true }],
      SplitWithUserID: ['', [Validators.required, Validators.pattern('^[0-9]+$')]], // Ensure only numbers
      SplitAmount: [0, [Validators.required, Validators.min(0.01)]],
      ExpenseSplitCreatedAt: [new Date().toISOString().substring(0, 10)],
      IsSettled: [false]
    });
  }

  addExpenseSplit() {
    if (this.addExpenseSplitForm.valid) {
      const formData = {
        ...this.addExpenseSplitForm.getRawValue(), // Get values including disabled fields
        ExpenseId: this.currentExpenseId // Explicitly add the ExpenseId
      };
      console.log('Form Data:', formData);

      this.apiService.AddExpenseSplit(formData).subscribe({
        next: () => {
          alert('Expense-Split added successfully');
          this.router.navigate(['/view-expense/' + this.currentExpenseId]);
        },
        error: (error) => {
          console.error('Error adding expense:', error);
          alert(`Failed to add the expense. Error: ${error.message}`);
        }
      });
    } else {
      alert('Form is invalid. Please check the entered data.');
    }
  }
}
