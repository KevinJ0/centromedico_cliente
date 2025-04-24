import { animate, style, transition, trigger } from '@angular/animations';

export const inOutAnimation =
  trigger('inOutAnimation', [
    transition(':enter', [
      style({ opacity: 0 }),
      animate('900ms ease-out', style({ opacity: 1 }))
    ]),
    transition(':leave', [
      style({ opacity: 1, position: 'absolute' }),
      animate('900ms ease-in', style({ opacity: 0, position: 'relative' }))
    ])
  ]);

export const turnoChangedAnimation = trigger('turnoChanged', [
  transition(':enter', [
    style({ opacity: 0, transform: 'scale(0.8)' }),
    animate('300ms ease-out', style({ opacity: 1, transform: 'scale(1)' }))
  ]),
  transition(':increment', [
    animate('200ms ease-in', style({ color: 'green' }))
  ]),
  transition(':decrement', [
    animate('200ms ease-in', style({ color: 'red' }))
  ])
])
