import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Params } from '@angular/router';
import { medico } from 'src/app/interfaces/InterfacesDto';
import { DoctorService } from 'src/app/services/doctor.service';

@Component({
  selector: 'app-doctor',
  templateUrl: './doctor.component.html',
  styleUrls: ['./doctor.component.css']
})
export class DoctorComponent implements OnInit {


  constructor(private rutaActiva: ActivatedRoute, private doctorSvc: DoctorService,) { }

  _medico: medico;
  ngOnInit(): void {

    this._medico = this.doctorSvc.GetMedicoById(this.rutaActiva.snapshot.params.id);


  }

}
