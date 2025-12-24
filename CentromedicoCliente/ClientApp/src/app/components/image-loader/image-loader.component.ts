import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-image-loader',
  templateUrl: './image-loader.component.html',
  styleUrls: ['./image-loader.component.css']
})
export class ImageLoaderComponent implements OnInit {

  @Input() highResSrc: string = '';
  @Input() lowResSrc: string = '';
  @Input() alt: string = '';

  isLoaded: boolean = false;

  constructor() { }

  ngOnInit(): void {
  }

  onHighResLoad(): void {
    this.isLoaded = true;
    console.log(`Imagen HD cargada: ${this.highResSrc}`);
  }
}