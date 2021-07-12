import { TestBed } from '@angular/core/testing';

import { DoctorHorarioService } from './doctor-horario.service';

describe('DoctorHorarioService', () => {
  let service: DoctorHorarioService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(DoctorHorarioService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
