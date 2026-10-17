# Product

<!-- impeccable:product-schema 1 -->

## Platform

web

## Users

Pacientes del centro médico que visitan el portal para agendar citas, consultar el directorio médico, ver su ubicación y contactar al centro. Usan la página principalmente desde el navegador (desktop y móvil) para reservar citas con doctores.

## Product Purpose

Portal de pacientes del centro médico. Su propósito principal es facilitar el agendamiento de citas médicas: permitir al paciente iniciar sesión, buscar doctores, crear una cita, ver su ticket, consultar sus citas futuras y gestionar su cuenta. También ofrece información del centro (ubicación, contacto, FAQ).

## Positioning

Portal centrado en el paciente con flujo de reserva de citas guiado por pasos (stepper) y notificaciones en tiempo real (SignalR). Diferenciador: experiencia de agendamiento unificada con ticket digital y gestión de citas.

## Operating Context

Aplicación web SPA en Angular 12 sobre ASP.NET Core (con SSR/prerender). Backend expone API REST y SignalR. El paciente interactúa desde navegador; requiere autenticación JWT para acciones protegidas (crear cita, mis citas, ajustes).

## Capabilities and Constraints

- Rutas: home, login, crear-cita (protegida), ticket (protegida), médicos y detalle, mis-citas (protegida), ubicación, contactos, faq, ajustes-usuario (protegida).
- Stack: Angular 12, Angular Material, ngx-mask, Swiper, AOS, Lottie, Google Maps. Backend .NET.
- Restricción: mantener colores y logo actuales del centro (identidad de marca existente).
- Idioma: español.

## Brand Commitments

Conservar la paleta de colores y el logotipo actuales del centro médico. No cambiar la identidad de marca; refinar UX/UI y arquitectura de información respetándola.

## Evidence on Hand

Repositorio `kevinj0/centromedico_cliente` (público). Componentes en `CentromedicoCliente/ClientApp/src/app/components`. Assets en `src/assets`.

## Product Principles

- Priorizar el flujo de agendamiento de citas con el menor número de pasos y fricción.
- Mantener la identidad de marca (colores/logo) mientras se mejora claridad y jerarquía visual.
- Diseñar mobile-first y accesible (navegación clara, estados vacíos, feedback).
- Dar al paciente siempre un siguiente paso evidente (CTA principal visible).

## Accessibility & Inclusion

Requerimientos específicos no definidos aún; aplicar buenas prácticas (contraste, foco visible, navegación por teclado, textos alternativos).
