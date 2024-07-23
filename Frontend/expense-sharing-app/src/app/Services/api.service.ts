import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})

export class APIService {
  placeOrder(customerId: any, cartData: any[]) {
    return this.http.post<any>(this.baseApiUrl + '/product/OrderProduct/' + customerId, cartData);
  }

  constructor(
    private http: HttpClient
  ) { }

  baseApiUrl = 'https://localhost:7163/api';
  ApiUrl = 'https://localhost:7163/api/user';

  // USER CONTROLLER
  LoginCustomer(data: any): Observable<any> {
    return this.http.post<any>(this.ApiUrl + '/login', data);
  }

  GetUserById(id: any): Observable<any> {
    return this.http.get<any>(this.ApiUrl + '/GetUserById/' + id);
  }

  GetAllUsers(): Observable<any> {
    return this.http.get<any>(this.baseApiUrl + '/user/GetAllUsers');
  }

  UpdateUserBalance(id: any): Observable<any> {
    return this.http.get<any>(this.ApiUrl + '/UpdateUserBalance/' + id);
  }

  // GROUP CONTROLLER
  CreateNewGroup(data: any): Observable<any> {
    return this.http.post<any>(this.baseApiUrl + '/group/CreateGroup', data);
  }

  GetAllGroups(): Observable<any> {
    return this.http.get<any>(this.baseApiUrl + '/group/GetAllGroups');
  }

  GetGroupByGroupId(id: any): Observable<any> {
    return this.http.get<any>(this.baseApiUrl + '/group/GetGroupByGroupId/' + id);
  }

  GetGroupMembersByUserId(id: any): Observable<any> {
    return this.http.get<any>(this.baseApiUrl + '/group/GetGroupMembersByUserId/' + id);
  }

  GetGroupsByUserId(id: any): Observable<any> {
    return this.http.get<any>(this.baseApiUrl + '/group/GetGroupsByUserId/' + id);
  }

  UpdateGroup(id: any, data: any) {
    return this.http.put<any>(this.baseApiUrl + '/group/UpdateGroup/' + id, data);
  }

  DeleteMyGroup(id: any) {
    return this.http.delete(this.baseApiUrl + '/group/DeleteGroup/' + id);
  }

  GetGroupDetails(id: any) {
    return this.http.get(this.baseApiUrl + '/group/GetGroupDetails/' + id);
  }

  // EXPENSE CONTROLLER
  AddExpense(data: any): Observable<any> {
    return this.http.post<any>(this.baseApiUrl + '/expense/CreateExpense', data);
  }

  GetExpensesByExpenseId(id: any): Observable<any> {
    return this.http.get<any>(this.baseApiUrl + '/expense/GetExpenseByExpenseId/' + id);
  }

  GetExpenseByExpenseId(id: any): Observable<any> {
    return this.http.get<any>(this.baseApiUrl + '/expense/GetExpenseByExpenseId/' + id);
  }

  UpdateExpense(id: any, data: any): Observable<any> {
    return this.http.put<any>(this.baseApiUrl + '/expense/UpdateExpense/' + id, data);
  }

  UpdateExpenseSettledStatus(updatePayload: { expenseId: number, isSettled: boolean }): Observable<any> {
    return this.http.put<any>(this.baseApiUrl + '/expense/SettleExpense', updatePayload);
  }

  DeleteExpense(id: any) {
    return this.http.delete(this.baseApiUrl + '/expense/DeleteExpense/' + id);
  }

  SettleExpense(id: any, data: any): Observable<any> {
    return this.http.put<any>(`${this.baseApiUrl}/expense/SettleExpense/${id}`, data)
      .pipe(
        catchError((error: HttpErrorResponse) => {
          console.error('SettleExpense request failed:', error);
          return throwError(error);
        })
      );
  }  

  // GROUP MEMBER CONTROLLER
  AddMemberToGroup(data: any): Observable<any> {
    return this.http.post<any>(this.baseApiUrl + '/groupmember/AddMemberToGroup', data);
  }

  UpdateGroupMember(id: any, data: any): Observable<any> {
    return this.http.put<any>(this.baseApiUrl + '/groupmember/UpdateGroupMember/' + id, data);
  }

  GetGroupMemberByIdAsync(id: any): Observable<any> {
    return this.http.get<any>(this.baseApiUrl + '/groupmember/GetGroupMemberById/' + id);
  }

  GetGroupMembersByGroupId(id: any): Observable<any> {
    return this.http.get<any>(this.baseApiUrl + '/GroupMember/GetGroupMembersByGroupId/' + id);
  }

  AddExpenseSplit(data: any): Observable<any> {
    return this.http.post<any>(this.baseApiUrl + '/expensesplit/AddExpenseSplit', data);
  }
}

