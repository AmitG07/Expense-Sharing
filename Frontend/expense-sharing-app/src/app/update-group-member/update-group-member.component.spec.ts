import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UpdateGroupMemberComponent } from './update-group-member.component';

describe('UpdateGroupMemberComponent', () => {
  let component: UpdateGroupMemberComponent;
  let fixture: ComponentFixture<UpdateGroupMemberComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [UpdateGroupMemberComponent]
    });
    fixture = TestBed.createComponent(UpdateGroupMemberComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
