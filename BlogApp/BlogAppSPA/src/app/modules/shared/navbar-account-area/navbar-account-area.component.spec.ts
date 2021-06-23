import { ComponentFixture, TestBed } from '@angular/core/testing';

import { NavbarAccountAreaComponent } from './navbar-account-area.component';

describe('NavbarAccountAreaComponent', () => {
  let component: NavbarAccountAreaComponent;
  let fixture: ComponentFixture<NavbarAccountAreaComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [NavbarAccountAreaComponent],
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(NavbarAccountAreaComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
