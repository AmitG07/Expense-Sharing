import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomepageComponent } from './homepage/homepage.component';
import { LoginComponent } from './login/login.component';
import { CreateGroupComponent } from './create-group/create-group.component';
import { MyGroupsComponent } from './my-groups/my-groups.component';
import { UserDetailsComponent } from './Admin/user-details/user-details.component';
import { AddExpenseComponent } from './add-expense/add-expense.component';
import { ViewGroupComponent } from './view-group/view-group.component';
import { UpdateGroupComponent } from './update-group/update-group.component';
import { UpdateExpenseComponent } from './update-expense/update-expense.component';
import { ViewExpenseComponent } from './view-expense/view-expense.component';
import { AddGroupMembersComponent } from './add-group-members/add-group-members.component';
import { GroupsInvitedToComponent } from './groups-invited-to/groups-invited-to.component';
import { MyDetailsComponent } from './my-details/my-details.component';
import { AddExpenseSplitComponent } from './add-expense-split/add-expense-split.component';
import { UpdateGroupMemberComponent } from './update-group-member/update-group-member.component';

const routes: Routes = [
  { path: '', redirectTo: 'homepage', pathMatch: 'full' },

  { path: 'create-group', component: CreateGroupComponent },
  { path: 'my-details', component: MyDetailsComponent },
  { path: 'add-expense/:groupId', component: AddExpenseComponent },
  { path: 'add-group-members/:groupId', component: AddGroupMembersComponent },
  { path: 'my-group', component: MyGroupsComponent },
  { path: 'groups-invited-to', component: GroupsInvitedToComponent },
  { path: 'view-group/:groupId', component: ViewGroupComponent },
  { path: 'update-group/:groupId', component: UpdateGroupComponent },
  { path: 'update-expense/:expenseId', component: UpdateExpenseComponent },
  { path: 'view-expense/:expenseId', component: ViewExpenseComponent },
  { path: 'add-expense-split/:expenseId', component: AddExpenseSplitComponent },
  { path: 'update-group-member/:groupMemberId', component: UpdateGroupMemberComponent },
  
  { path: 'homepage', component: HomepageComponent },
  { path: 'homepage/login', component: LoginComponent },

  { path: 'admin/user-details', component: UserDetailsComponent },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
