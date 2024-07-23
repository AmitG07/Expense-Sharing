import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  constructor() { }

  private userSubject = new BehaviorSubject<string>(sessionStorage.getItem('user'));

  CreatingUserSession(user: any) {
    sessionStorage.setItem('user', JSON.stringify(user));
    this.userSubject.next(sessionStorage.getItem('user'));
  }

  RemoveUserSession() {
    sessionStorage.removeItem('user');
    this.userSubject.next(sessionStorage.getItem('user'));
  }

  GetUserSession(): Observable<string> {
    return this.userSubject.asObservable();
  }

  GetUserName(user: any) {
    try {
      const parsedUser = JSON.parse(user);
      const name = parsedUser?.name?.toUpperCase(); // Use 'name' instead of 'Name'
      // console.log('Parsed Customer:', parsedCustomer);
      // console.log('Customer Name:', name);
      return name;
    } catch (error) {
      // console.error('Error parsing customer data:', error);
      return 'Error Parsing Data';
    }
  }

  GetId(): number {
    const userData = sessionStorage.getItem('user');
    if (userData) {
      const user = JSON.parse(userData);
      return user.userId;
    }
    return 0; // Default return if user data is not found
  }

  isloggedin() {
    return sessionStorage.getItem('user') != null;
  }

  GetUserRole(user: any) {
    const parsedUser = JSON.parse(user);
    const name = parsedUser?.name?.toUpperCase();
    if (name === "ADMIN1" || name === "ADMIN2")
      return true;
    else
      return false;
  }

}
