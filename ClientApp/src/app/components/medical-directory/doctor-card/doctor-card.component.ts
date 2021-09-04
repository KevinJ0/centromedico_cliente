import { Component, OnInit,OnDestroy, Input } from '@angular/core';
import { doctorCard } from 'src/app/interfaces/InterfacesDto';
import { AutoUnsubscribe } from "ngx-auto-unsubscribe";

@AutoUnsubscribe()
@Component({
  selector: 'app-doctor-card',
  templateUrl: './doctor-card.component.html',
  styleUrls: ['./doctor-card.component.css']
})
export class DoctorCardComponent implements OnInit {
  @Input() doctorData: doctorCard;
  _especialidades: string="";
  constructor() { }

  ngOnInit(): void {
    let arrLength: number = this.doctorData.especialidades.length-1;

    this.doctorData.especialidades.map((value, index, arr) => {
      if (index != arrLength)
        this._especialidades += value + ", ";
      else
        this._especialidades += value;
    })

   }
   ngOnDestroy() { 

  }
}
