import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddGroupMembersComponent } from './add-group-members.component';

describe('AddGroupMembersComponent', () => {
  let component: AddGroupMembersComponent;
  let fixture: ComponentFixture<AddGroupMembersComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [AddGroupMembersComponent]
    });
    fixture = TestBed.createComponent(AddGroupMembersComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
