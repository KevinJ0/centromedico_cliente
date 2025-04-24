import { Inject, Injectable } from '@angular/core';
import { HubConnectionBuilder, HubConnection, HubConnectionState } from '@microsoft/signalr';
import { Observable, throwError } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class SignalrCustomService {
  public hubConnection: HubConnection;
  baseUrl: string;
  accessToken: string = "";
  private joinedGroups: { grupo: string, callback: (data: any) => void, medicoId: number }[] = [];

  constructor(@Inject("DOCTOR_URL") DoctorApiUrl: string) {

    this.baseUrl = DoctorApiUrl;

  }

  async Connect(): Promise<void> {

    this.accessToken = localStorage.getItem("jwt");

    if (!this.accessToken)
      throwError(() => new Error("No hay token de acceso"));

    let builder = new HubConnectionBuilder();
    this.hubConnection = builder
      .withUrl(this.baseUrl + "notificacion", {
        accessTokenFactory: () => this.accessToken
      })
      .withAutomaticReconnect({
        nextRetryDelayInMilliseconds: retryContext => {
          if (retryContext.elapsedMilliseconds < 5000) {
            // wait between 5 seconds before the next reconnect attempt.
            return 5000;
          }
        }
      }).build();

    if (this.hubConnection?.state == "Disconnected") {
      await this.hubConnection
        .start()
        .then(() => {
          console.info('Connection started')
        })
        .catch(err => {
          console.error('Error while starting connection: ' + err);
        });
    }

    this.hubConnection.onreconnected(async (connectionId) => {
      console.log("Reconectado al hub:", connectionId);

      for (const g of this.joinedGroups) {
        try {
          if (g.medicoId) {
            await this.hubConnection.invoke("JoinTurnGroup", g.medicoId);
            this.hubConnection.on(g.grupo, g.callback);
          }
        } catch (err) {
          console.error("Error al re-entrar al grupo:", g.grupo, err);
        }
      }
    });

  }


  async joinGroup(medicoId: number, grupoNombre: string, callback: (data: any) => void): Promise<void> {
    await this.hubConnection.invoke("JoinTurnGroup", medicoId);
    this.hubConnection.on(grupoNombre, callback);

    this.joinedGroups.push({ grupo: grupoNombre, callback, medicoId });
  }



  //! Disconnect from the hub and leave the group
  async Disconnect(): Promise<void> {

    if (this.hubConnection) {
      let group: Array<any> = new Array<any>();
      let p = localStorage.getItem("grupo_turno");
      group.push(p);

      await this.hubConnection.invoke("LeaveGroups", group).then(async (msj) => {


        if (group[0])
          this.hubConnection.off(group[0]);

        //stop the connection
        await this.hubConnection.stop();
        console.log("se ha desconectado del hub")

      }).catch(err => {
        throwError(() => new Error(err));
      });
    }
  }
}

