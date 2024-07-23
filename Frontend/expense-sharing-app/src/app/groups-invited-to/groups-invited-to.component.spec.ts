import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GroupsInvitedToComponent } from './groups-invited-to.component';

describe('GroupsInvitedToComponent', () => {
  let component: GroupsInvitedToComponent;
  let fixture: ComponentFixture<GroupsInvitedToComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [GroupsInvitedToComponent]
    });
    fixture = TestBed.createComponent(GroupsInvitedToComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
