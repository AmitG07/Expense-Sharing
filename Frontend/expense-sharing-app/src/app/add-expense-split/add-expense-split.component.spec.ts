import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddExpenseSplitComponent } from './add-expense-split.component';

describe('AddExpenseSplitComponent', () => {
  let component: AddExpenseSplitComponent;
  let fixture: ComponentFixture<AddExpenseSplitComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [AddExpenseSplitComponent]
    });
    fixture = TestBed.createComponent(AddExpenseSplitComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
