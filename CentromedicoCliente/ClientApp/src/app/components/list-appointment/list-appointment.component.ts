import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import * as _moment from 'moment';
import { CitaService } from 'src/app/services/cita.service';
import { citaCard } from 'src/app/interfaces/InterfacesDto';

import { NgZone } from '@angular/core';
import { ProgressSpinnerMode } from '@angular/material/progress-spinner';
import { AutoUnsubscribe } from "ngx-auto-unsubscribe";
import { BehaviorSubject, catchError } from 'rxjs';
import { inOutAnimation, turnoChangedAnimation } from 'src/app/animations/animations';
import { group, turno_paciente } from 'src/app/interfaces/InterfacesDto';
import { SignalrCustomService } from 'src/app/services/signalr-custom.service';
import { TurnosMedicoService } from 'src/app/services/turnos-medico.service';
import { HubConnectionState } from '@microsoft/signalr';

@AutoUnsubscribe()
@Component({
  selector: 'app-list-appointment',
  templateUrl: './list-appointment.component.html',
  styleUrls: ['./list-appointment.component.css'],
  animations: [inOutAnimation, turnoChangedAnimation]

})
export class ListAppointmentComponent implements OnInit {

  showNoContent: boolean = false;
  loading: boolean = true;
  mode: ProgressSpinnerMode = 'indeterminate';
  citaCardList$ = new BehaviorSubject<citaCard[]>([]);
  aproxAppointTime: number = 0;
  gruposUnidos: { grupo: string, callback: (data: any) => void }[] = [];
  private joinedGroups: { grupo: string, callback: (data: any) => void, medicoId: number }[] = [];


  turno: number;

  constructor(

    private router: Router,
    private rutaActiva: ActivatedRoute,
    private turnosSvc: TurnosMedicoService,
    private signalRSvc: SignalrCustomService,
    private zone: NgZone,
    public citaSvc: CitaService) {

  }

  navigateToTurnos(medicoId: number, fecha: Date) {
    localStorage.setItem('fecha_hora_cita', fecha.toString());

    this.router.navigate(['/ver-turnos/' + medicoId]); // Cambia '/ruta-deseada' por la ruta a la que quieras redirigir
  }

  ngOnInit(): void {
    this.citaSvc.GetCitaList().subscribe((r) => {
      this.loading = false;
      if (!r || r.length === 0) {
        this.showNoContent = true;
        return;
      }
      updateWaitingTime(r);


      this.citaCardList$.next(r);

      this.signalRSvc.Connect().then(() => {
        r.forEach((citaCard) => {
          this.connectTurnosSignalR(citaCard);
        });
      }).catch(err => console.error(err));

    }, (err) => {
      this.loading = false;
      this.showNoContent = true;
      console.error(err);
    });

  }

  connectTurnosSignalR(citaCard: citaCard) {

    //*Hub
    //*Obtengo el nombre del grupo de turnos
    this.turnosSvc.GetGrupoTurnos(citaCard.medicosID).subscribe(
      (grupoTurno: group) => {
        if (grupoTurno) {
          let grupoNombre: string = grupoTurno.group_name;

          let grupos: string[] = JSON.parse(localStorage.getItem("grupo_turno")) || [];
          grupos.push(grupoNombre);
          localStorage.setItem("grupo_turno", JSON.stringify(grupos));

          //*Me conecto al grupo de turnos

          if (this.signalRSvc.hubConnection.state == HubConnectionState.Connected) {
            this.signalRSvc.joinGroup(citaCard.medicosID, grupoNombre, (msj: string) => {
              this.updateTurns(citaCard);
            });
          }

        }
      },
      catchError((err) => {
        this.loading = false;
        this.showNoContent = true;
        console.error(err);
        return null;
      }));

  }

  updateTurns(citaCard: citaCard) {

    this.zone.run(() => {
      if (this.router.url.includes("mis-citas")) {
        this.turnosSvc.GetPacienteTurno(citaCard.medicosID, citaCard.fecha_hora).subscribe((turno: turno_paciente) => {

          const currentList = this.citaCardList$.getValue();
          const index = currentList.findIndex(c => c.medicosID === citaCard.medicosID && c.fecha_hora === citaCard.fecha_hora);
          if (index !== -1) {

            currentList[index] = { ...currentList[index], turno_paciente: turno }; // <- cambia referencia
            updateWaitingTime(currentList);
            this.citaCardList$.next([...currentList]); // <- emite nueva copia
          }

        });
      }
    });
  }

  trackByCita(index: number, item: citaCard): any {
    return item.id; // o cualquier identificador único que tengas
  }
  getTurnoKey(cita: any): string {
    return 'turno-' + cita.turno;
  }
  convertDateFull(date: string): string {
    return _moment(date).format('dddd DD MMM Y hh:mm A');
  }
  convertDateHours(date: string): string {
    return _moment(date).format('hh:mm A');
  }
  convertDate(date: string): string {
    return _moment(date).format('dddd DD MMM Y');
  }
  ngOnDestroy() {
    this.signalRSvc.Disconnect();

  }
}
function updateWaitingTime(r: citaCard[]) {
  r.forEach((citaCard) => {
    const timePerPatient = (new Date(citaCard.turno_paciente.ultima_entrada).getTime() - new Date(citaCard.turno_paciente.primera_entrada).getTime()) / citaCard.turno_paciente.turno_atendido;
    const diffHoras = timePerPatient / (1000 * 60 * 60);
    const diffMinutos = timePerPatient / (1000 * 60);

    const horas = Math.floor(diffMinutos / 60);
    const minutos = Math.floor(diffMinutos % 60);

    if (!Number.isFinite(timePerPatient) || timePerPatient == 0) {
      citaCard.turno_paciente.tiempo_aprox = `Aún no disponible.`;
    } else {
      citaCard.turno_paciente.tiempo_aprox = `${horas}h ${minutos}min`;
    }


  });
}

