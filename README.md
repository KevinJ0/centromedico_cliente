# Centro Médico — Portal del paciente (`centromedico_cliente`)

Portal público y API para pacientes del ecosistema **Centro Médico**.

| Repositorio | Rol |
| --- | --- |
| [`centromedico_database`](https://github.com/KevinJ0/centromedico_database) | Capa de datos central (EF Core, 30 entidades) |
| [`centromedico_doctor`](https://github.com/KevinJ0/centromedico_doctor) | API de administración + panel web |
| [`centromedico_cliente`](https://github.com/KevinJ0/centromedico_cliente) | **Este repositorio**: portal público para pacientes + API |

## Características

- **Reserva de cita en 3 pasos**: eliges servicio y seguro → seleccionas fecha/hora con disponibilidad real de la agenda → registras tus datos (con flujo especial para pacientes menores y su tutor).
- **Turno en vivo**: al llegar al centro, el paciente ve su turno y a cuántas personas le faltan por delante, actualizado en tiempo real con **SignalR**.
- **Ticket digital**: cita generada como ticket imprimible (estilo boarding pass) con código de verificación (vía Twilio/WhatsApp).
- **Autenticación JWT**: login y registro, con **refresh tokens rotatorios** y cola de peticiones en el interceptor del frontend.
- **Búsqueda de médicos**: directorio con búsqueda, filtros y ficha profesional por médico.
- **Integraciones**: Google Maps, AWS S3 (fotos de perfil), Twilio (WhatsApp).

## Stack

- **Backend**: ASP.NET Core 5 (Web API) + Entity Framework Core 5 + AutoMapper
- **Frontend**: Angular 12 + Angular Material + RxJS
- **Datos**: SQL Server (modelo central en `centromedico_database`)

## Estructura

```
CentromedicoCliente/     # API + ClientApp (Angular)
Cliente.DTO/             # Objetos de transferencia de datos
Cliente.Repository/      # Repositorios (data access)
```

## Demostración

[Ver demo en YouTube](https://youtu.be/1l4ev4TDqDE)