import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { APIService } from '../Services/api.service';
import { AuthService } from '../Services/auth.service';
import { MatSnackBar, MatSnackBarConfig } from '@angular/material/snack-bar';

@Component({
  selector: 'app-view-group',
  templateUrl: './view-group.component.html',
  styleUrls: ['./view-group.component.css']
})
export class ViewGroupComponent implements OnInit {
  groupDetails: any; 
  currentGroupId: number;
  loggedInUserId: number;
  groupMembersLoaded: boolean = false;
  groupMembers: any[] = [];
  expenses: any[] = [];
  hasExpenses: boolean = false;
  groupAdminName: string = ''; 
  isUserMember: boolean = false;
  expenseButtonClicked: boolean = false; // Flag to track if the "Add Expense" button was clicked

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private apiService: APIService,
    private authService: AuthService,
    private snackBar: MatSnackBar
  ) { }

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      this.currentGroupId = +params.get('groupId');
      this.loggedInUserId = this.authService.GetId();

      // Check local storage for the flag
      this.expenseButtonClicked = localStorage.getItem(`expenseButtonClicked_${this.currentGroupId}`) === 'true';

      this.fetchGroupDetails();
    });
  }

  fetchGroupDetails(): void {
    this.apiService.GetGroupDetails(this.currentGroupId).subscribe({
      next: (groupData: any) => {
        this.groupDetails = groupData;
        this.fetchGroupAdminDetails(this.groupDetails.groupAdminId);
        this.fetchGroupMembers();
        this.fetchExpenses();
      },
      error: (error: any) => {
        console.error('Error fetching group details:', error);
      }
    });
  }

  fetchGroupAdminDetails(adminId: number): void {
    this.apiService.GetUserById(adminId).subscribe({
      next: (userData: any) => {
        this.groupAdminName = userData.name;
      },
      error: (error: any) => {
        console.error(`Error fetching group admin details for ID ${adminId}:`, error);
      }
    });
  }

  fetchGroupMembers(): void {
    this.apiService.GetGroupMembersByGroupId(this.currentGroupId).subscribe({
      next: (groupMemberData: any) => {
        this.groupMembers = groupMemberData.$values.map((member: any) => ({
          ...member,
          userName: member.user?.name || 'Unknown',
          emailId: member.user?.emailId || 'Unknown',
          groupMemberId: member.groupMemberId,
          availableBalance: member.user?.availableBalance || 0  // Assuming availableBalance is a property of user
        }));
        this.isUserMember = this.groupMembers.some(member => member.userId === this.loggedInUserId);
        this.groupMembersLoaded = true;
      },
      error: (error: any) => {
        console.error('Error fetching group members:', error);
      }
    });
  }

  fetchExpenses(): void {
    if (this.groupDetails && this.groupDetails.expense && this.groupDetails.expense.$values) {
      this.expenses = this.groupDetails.expense.$values;
      this.hasExpenses = this.expenses.length > 0;
    } else {
      this.expenses = [];
      this.hasExpenses = false;
      console.error('Invalid expenses data format:', this.groupDetails?.expense);
    }
  }  

  calculateMemberBalances(member: any): void {
    let totalPaid = 0;
    let totalOwed = 0;

    this.expenses.forEach(expense => {
      if (expense.paidByUserId === member.userId) {
        totalPaid += expense.expenseAmount;
      }
      if (expense.splitDetails && expense.splitDetails.length > 0) {
        expense.splitDetails.forEach(split => {
          if (split.userId === member.userId) {
            totalOwed += split.amount;
          }
        });
      }
    });

    member.totalSpend = totalPaid;
    member.availableBalance = totalPaid - totalOwed;
  }

  deleteExpense(expenseId: number): void {
    if (confirm('Are you sure you want to delete this expense?')) {
      this.apiService.DeleteExpense(expenseId).subscribe({
        next: () => {
          const config = new MatSnackBarConfig();
          config.duration = 3000;
          this.snackBar.open('Expense deleted successfully', 'Close', config);
          this.fetchExpenses(); // Refresh expenses after deletion
        },
        error: (error: any) => {
          console.error('Error deleting expense:', error);
          const config = new MatSnackBarConfig();
          config.duration = 3000;
          this.snackBar.open('Failed to delete expense. Please try again.', 'Close', config);
        }
      });
    }
  }  

  canAddExpense(): boolean {
    return this.groupDetails.groupAdminId === this.loggedInUserId || this.isUserMember;
  }

  isGroupAdmin(): boolean {
    return this.groupDetails.groupAdminId === this.loggedInUserId;
  }

  canEditOrDelete(): boolean {
    return this.groupDetails.groupAdminId === this.loggedInUserId;
  }

  onAddExpenseClick(): void {
    // Set local storage flag when "Add Expense" button is clicked
    localStorage.setItem(`expenseButtonClicked_${this.currentGroupId}`, 'true');
    this.router.navigate(['/add-expense', this.currentGroupId]);
  }
}
