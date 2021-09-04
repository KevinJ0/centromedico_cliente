import { Component, OnInit, ViewChild } from '@angular/core';
import {MatAccordion} from '@angular/material/expansion';

@Component({
  selector: 'app-faq',
  templateUrl: './faq.component.html',
  styleUrls: ['./faq.component.css'],
  styles: [`
    ::ng-deep .specific-class > .mat-expansion-indicator::after {
      color: blue;
    }
  `],
})
export class FaqComponent implements OnInit {
  @ViewChild(MatAccordion) accordion: MatAccordion;

  constructor() { }

  ngOnInit(): void {
    
  }

}