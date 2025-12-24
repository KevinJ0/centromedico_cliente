import { Component, ViewChild } from '@angular/core';
import { MatAccordion } from '@angular/material/expansion';
import { AnimationItem, AnimationOptions } from 'ngx-lottie/lib/symbols';
import './home.component.css';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent {
  @ViewChild(MatAccordion) accordion: MatAccordion;

  options: AnimationOptions = {
    path: 'assets/icons/animated/question.json',
  };

  animationCreated(animationItem: AnimationItem): void {
    console.log(animationItem);
  }
}
