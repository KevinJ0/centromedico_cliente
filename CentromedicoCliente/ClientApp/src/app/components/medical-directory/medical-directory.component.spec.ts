import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MedicalDirectoryComponent } from './medical-directory.component';

describe('MedicalDirectoryComponent', () => {
  let component: MedicalDirectoryComponent;
  let fixture: ComponentFixture<MedicalDirectoryComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ MedicalDirectoryComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(MedicalDirectoryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
