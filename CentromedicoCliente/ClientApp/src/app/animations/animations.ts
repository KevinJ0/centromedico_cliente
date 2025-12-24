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

export const fadeInAnimation =
  trigger('fadeInAnimation', [
    transition(':enter', [
      style({ opacity: 0 }),
      animate('300ms ease-out', style({ opacity: 1 }))
    ]),
    transition(':leave', [
      style({ opacity: 1, position: 'absolute' }),
      animate('300ms ease-in', style({ opacity: 0, position: 'relative' }))
    ])
  ]);

export const fadeInUpAnimation =
  trigger('fadeInUpAnimation', [
    transition(':enter', [
      style({ opacity: 0, transform: 'translateY(20px)' }),
      animate('500ms ease-out', style({ opacity: 1, transform: 'translateY(0)' }))
    ])
  ]);

export const fadeInLeftAnimation =
  trigger('fadeInLeftAnimation', [
    transition(':enter', [
      style({ opacity: 0, transform: 'translateX(-20px)' }),
      animate('500ms ease-out', style({ opacity: 1, transform: 'translateX(0)' }))
    ])
  ]);

export const fadeInRightAnimation =
  trigger('fadeInRightAnimation', [
    transition(':enter', [
      style({ opacity: 0, transform: 'translateX(20px)' }),
      animate('500ms ease-out', style({ opacity: 1, transform: 'translateX(0)' }))
    ])
  ]);

export const popInAnimation =
  trigger('popInAnimation', [
    transition(':enter', [
      style({ opacity: 0, transform: 'scale(0.8)' }),
      animate('500ms cubic-bezier(0.35, 0, 0.25, 1)', style({ opacity: 1, transform: 'scale(1)' }))
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
