import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { APIService } from '../Services/api.service';
import { catchError, switchMap } from 'rxjs/operators';
import { throwError } from 'rxjs';
import { AlertService } from '../Services/alert.service';
import { AuthService } from '../Services/auth.service';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-view-expense',
  templateUrl: './view-expense.component.html',
  styleUrls: ['./view-expense.component.css']
})
export class ViewExpenseComponent implements OnInit {
  expense: any;
  expenseId: number;
  loggedInUserId: number;
  groupAdminId: number;
  newSettledValue: boolean = true; // Initialize newSettledValue as true

  constructor(
    private route: ActivatedRoute,
    private apiService: APIService,
    private alertService: AlertService,
    private authService: AuthService,
    private http: HttpClient // Inject HttpClient
  ) { }

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      this.expenseId = +params.get('expenseId'); // Fetch expense ID from route params
      console.log("Expense ID:", this.expenseId);
      this.loggedInUserId = this.authService.GetId();
      this.fetchExpenseAndGroupDetails();
    });
  }  

  fetchExpenseAndGroupDetails(): void {
    // Fetch expense details and then fetch associated group details
    this.apiService.GetExpensesByExpenseId(this.expenseId).pipe(
      switchMap((expenseData: any) => {
        this.expense = expenseData; // Store expense details
        return this.apiService.GetGroupByGroupId(this.expense.groupId); // Fetch group details
      }),
      catchError(error => {
        console.error('Error fetching expense and group details:', error);
        return throwError(error);
      })
    ).subscribe({
      next: (groupData: any) => {
        this.groupAdminId = groupData.groupAdminId; // Extract groupAdminId from groupData
        console.log('Fetched expense:', this.expense);
        console.log('Fetched group admin ID:', this.groupAdminId);
      },
      error: (error) => {
        // Handle specific error cases here if needed
        console.error('Error fetching expense and group details:', error);
        // Optionally, set a flag or display an error message to the user
      }
    });
  }

  deleteExpense(expenseId: number): void {
    if (confirm('Are you sure you want to delete this expense?')) {
      this.apiService.DeleteExpense(expenseId).subscribe({
        next: () => {
          this.alertService.openSnackBar('Expense deleted successfully');
          // After deletion, fetch expenses again to update the list
          this.fetchExpenseAndGroupDetails();
        },
        error: (error: any) => {
          console.error('Error deleting expense:', error);
          this.alertService.openSnackBar('Failed to delete expense. Please try again.');
        }
      });
    }
  }

  UpdateSettledStatus(): void {
    const settleData = {
      isSettled: this.newSettledValue // Set newSettledValue to true
    };
    console.log("HEllo"+this.expense.expenseId);
  
    this.apiService.SettleExpense(this.expense.expenseId, settleData).subscribe({
      next: (response: any) => {
        this.alertService.openSnackBar('Settled status updated successfully');
        this.fetchExpenseAndGroupDetails();
      },
      error: (error: any) => {
        console.error('Error updating settled status:', error);
        this.alertService.openSnackBar('Failed to update settled status. Please try again.');
      }
    });
  }    
}
