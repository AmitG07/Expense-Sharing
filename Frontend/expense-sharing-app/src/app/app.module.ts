import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatRadioModule } from '@angular/material/radio';
import { MatSelectModule } from '@angular/material/select';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatSnackBarModule } from '@angular/material/snack-bar';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HomepageComponent } from './homepage/homepage.component';
import { LoginComponent } from './login/login.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HeaderComponent } from './header/header.component';
import { HttpClientModule } from '@angular/common/http';
import { UserDetailsComponent } from './Admin/user-details/user-details.component';
import { AddExpenseComponent } from './add-expense/add-expense.component';
import { ViewGroupComponent } from './view-group/view-group.component';
import { MyGroupsComponent } from './my-groups/my-groups.component';
import { UpdateGroupComponent } from './update-group/update-group.component';
import { UpdateExpenseComponent } from './update-expense/update-expense.component';
import { CreateGroupComponent } from './create-group/create-group.component';
import { ViewExpenseComponent } from './view-expense/view-expense.component';
import { AddGroupMembersComponent } from './add-group-members/add-group-members.component';
import { GroupsInvitedToComponent } from './groups-invited-to/groups-invited-to.component';
import { MyDetailsComponent } from './my-details/my-details.component';
import { AddExpenseSplitComponent } from './add-expense-split/add-expense-split.component';
import { UpdateGroupMemberComponent } from './update-group-member/update-group-member.component';

@NgModule({
  declarations: [
    AppComponent,
    HomepageComponent,
    LoginComponent,
    HeaderComponent,
    UserDetailsComponent,
    AddExpenseComponent,
    ViewGroupComponent,
    MyGroupsComponent,
    UpdateGroupComponent,
    UpdateExpenseComponent,
    CreateGroupComponent,
    ViewExpenseComponent,
    AddGroupMembersComponent,
    GroupsInvitedToComponent,
    MyDetailsComponent,
    AddExpenseSplitComponent,
    UpdateGroupMemberComponent,
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    FormsModule,
    MatButtonModule,
    MatCardModule,
    MatFormFieldModule,
    ReactiveFormsModule,
    MatRadioModule,
    MatSelectModule,
    MatInputModule,
    MatIconModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatSnackBarModule,
    BrowserAnimationsModule,
    HttpClientModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
