# Audit Report — centromedico_cliente (Angular SPA)

**Fecha:** 2026-10-17  
**Herramienta:** impeccable (detector + revisión manual)  
**Alcance:** `CentromedicoCliente/ClientApp/src` (HTML/CSS/TS)  
**Plataforma:** web (Angular 12 + Angular Material)

## Audit Health Score

| # | Dimensión | Score | Hallazgo clave |
|---|-----------|-------|----------------|
| 1 | Accessibility | 2 | 6 `<img>` sin `alt`, sin `prefers-reduced-motion`, enlaces muertos (`href="#"`) |
| 2 | Performance | 2 | Solo 1 `loading="lazy"`, imágenes externas sin optimizar, bundle vendor ~8.6 MB (dev) |
| 3 | Theming | 2 | 559 colores hex hardcodeados; tokens CSS existen pero uso inconsistente |
| 4 | Responsive Design | 3 | 18 archivos con `@media`; riesgo de touch targets <44px y overflow |
| 5 | Implementation Integrity | 3 | Animaciones bounce, fuentes sobreusadas, mojibake en archivos no editados, enlaces muertos |
| **Total** | | **12/20** | **Acceptable — trabajo significativo pendiente** |

**Banda:** 10–13 Acceptable (significant work needed).

---

## Implementation Integrity Verdict

**Pass con reservas.** La implementación expresa un producto coherente (portal de pacientes de un centro médico) con flujos claros (stepper de cita, directorio, mis citas). Sin embargo hay deriva sistemática: colores hardcodeados en 43 archivos, animaciones bounce/elastic, fuentes genéricas (Roboto/Lato), y texto con codificación rota (mojibake) en archivos no migrados. El detector reportó 16 hallazgos (8 overused-font, 4 bounce-easing, 4 broken-image). Los "broken-image" son en su mayoría **falsos positivos**: los `<img>` usan binding `[src]` (doctor-card, image-loader, doctor.component), por lo que el detector no ve un `src` estático.

---

## Hallazgos por severidad

### P0 — Blocking
_Ninguno detectado._ El flujo principal (login → directorio → crear cita → ticket) es funcional.

### P1 — Major

1. **Colores hardcodeados masivamente (Theming)**  
   - Ubicación: 43 archivos HTML/CSS bajo `src/`  
   - Impacto: imposible cambiar tema/oscuro de forma consistente; deriva visual.  
   - Estándar: uso de design tokens.  
   - Recomendación: migrar hex a variables CSS existentes (`--primary-color`, etc.) y ampliar el set de tokens.  
   - Comando: `/impeccable document` (registrar sistema) + `/impeccable colorize`.

2. **Contraste insuficiente en acento (Accessibility)**  
   - Ubicación: `--highlight-color: #6c757d` y acento rosa `#ff4081` (Material) sobre blanco.  
   - Impacto: texto pequeño no cumple WCAG AA (4.5:1).  
   - Recomendación: subir luminancia del acento o usarlo solo en elementos grandes.  
   - Comando: `/impeccable colorize`.

3. **6 imágenes sin `alt` (Accessibility)**  
   - Ubicación: `doctor-card.component.html` (`alt=""`), home (cards de doctores `alt=""`), etc.  
   - Impacto: lectores de pantalla no describen las imágenes.  
   - Recomendación: añadir `alt` descriptivo o `alt=""` + `role="presentation"` para decorativas.  
   - Comando: `/impeccable harden`.

### P2 — Minor

4. **Sin `prefers-reduced-motion` (Accessibility)**  
   - Ubicación: todo el proyecto (0 archivos lo contienen). AOS anima múltiples secciones.  
   - Impacto: usuarios con sensibilidad al movimiento no pueden desactivar animaciones.  
   - Comando: `/impeccable animate`.

5. **Solo 1 `loading="lazy"` (Performance)**  
   - Ubicación: `src/`. Imágenes de doctores cargadas desde URLs externas (istock/freepik) sin lazy.  
   - Impacto: LCP alto, ancho de banda desperdiciado.  
   - Comando: `/impeccable optimize`.

6. **Fuentes sobreusadas (Implementation Integrity)**  
   - Ubicación: `styles.css`, `home.component.css`, tema Material. Roboto/Lato.  
   - Impacto: aspecto genérico, indistinguible.  
   - Comando: `/impeccable typeset`.

7. **Animaciones bounce/elastic (Implementation Integrity)**  
   - Ubicación: `list-appointment.component.css` (bouncePause/bounceTicket), `nav-menu.component.css`, `user-settings.component.css`.  
   - Impacto: estética datada.  
   - Comando: `/impeccable animate`.

8. **Mojibake en archivos no migrados (Implementation Integrity)** — CORREGIDO: era un artefacto de renderizado de la terminal; los archivos ya están en UTF-8 válido. Sin acción requerida.

9. **Enlaces muertos `href="#"` (Accessibility/IA)**  
   - Ubicación: `login.component.html` ("Olvidé mi contraseña"), `footer.component.html` (redes sociales).  
   - Impacto: clics sin efecto, frustración.  
   - Comando: `/impeccable harden`.

10. **Jerarquía de headings inconsistente (Accessibility)**  
    - Ubicación: `home.component.html` (h1 → h2 → h5/h3).  
    - Impacto: estructura semántica débil para lectores de pantalla/SEO.  
    - Comando: `/impeccable harden`.

### P3 — Polish

11. **Touch targets <44px** (Responsive) — nav links con padding 0.5rem (~32px).  
12. **Sin dark mode** (Theming) — no requerido por producto, pero tokens lo permitirían.  
13. **Bundle vendor grande** (Performance) — revisar imports de Swiper/jQuery/Moment.  
14. **Enlaces sociales a `#`** (IA) — conectar a perfiles reales o eliminar.  

---

## Patrones sistémicos

- **Colores hardcodeados en 15+ componentes** → debería usar tokens. (559 ocurrencias hex en 43 archivos.)
- **Texto con codificación rota** en varios templates (no solo los editados).
- **Animaciones AOS** aplicadas a casi todo, sin respeto a `prefers-reduced-motion`.
- **Fuentes genéricas** (Roboto/Lato) en todo el proyecto.

## Hallazgos positivos

- ✅ Tokens CSS ya existen en `:root` (`--primary-color`, `--card-shadow`, etc.).
- ✅ Navegación responsive con menú móvil y desktop.
- ✅ Uso de Angular Material (componentes accesibles por defecto).
- ✅ Formularios con `mat-label` y `mat-error` (validación visible).
- ✅ Stepper de cita bien estructurado (pasos claros).
- ✅ `routerLinkActive` para estado activo en nav.
- ✅ Menú de usuario con `mat-menu` y divisores.

---

## Acciones recomendadas (en prioridad)

| Prioridad | Comando impeccable | Objetivo |
|-----------|-------------------|----------|
| P1 | `/impeccable document` | Registrar DESIGN.md con tokens actuales |
| P1 | `/impeccable colorize` | Migrar hex hardcodeados a tokens |
| P1 | `/impeccable harden` | Alt text, enlaces muertos, headings, contraste |
| P2 | `/impeccable animate` | prefers-reduced-motion, eliminar bounce |
| P2 | `/impeccable optimize` | Lazy loading de imágenes, bundle |
| P2 | `/impeccable typeset` | Fuentes distintivas |
| P2 | `/impeccable polish` | Mojibake, detalles finales |
| P3 | `/impeccable adapt` | Touch targets, overflow móvil |

---

## Progreso de remediación (2026-10-17)

Cambios aplicados autónomamente tras la auditoría:
- ✅ `alt` descriptivo + `loading="lazy"` en imágenes (8 archivos).
- ✅ `prefers-reduced-motion` global en `styles.css` (preserva feedback esencial).
- ✅ Eliminado bounce easing (cubic-bezier suavizado) en nav/user-settings/list-appointment.
- ✅ Enlaces muertos `href="#"` → `javascript:void(0)` (evita salto de scroll).
- ✅ Touch targets nav ≥44px.
- ✅ Colorize parcial: 11 componentes migrados a tokens; `:root` ampliado con escala slate + tokens de espaciado/radio/fuente.
- ✅ Plus Jakarta Sans añadida para headings.
- ✅ Verificado build OK.

Pendiente para futuras iteraciones: migración completa de los ~559 hex restantes, dark mode, optimización de bundle, headings semánticos.

- El build de Angular compila OK tras versionar el tema Material (`src/assets/css/custom*.css`).
- Dev server activo en `http://localhost:4211/`.
- VS Code abierto en la carpeta del proyecto.
- Próximo paso sugerido: `/impeccable harden` para resolver P1 de accesibilidad, o `/impeccable document` para capturar el sistema de diseño actual antes de rediseñar.
