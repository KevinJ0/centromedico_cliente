import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-location',
  templateUrl: './location.component.html',
  styleUrls: ['./location.component.css']
})
export class LocationComponent implements OnInit {

  constructor() { }

  ngOnInit(): void {
    this.loadScript('https://maps.googleapis.com/maps/api/js?key=AIzaSyCXB5KBEe0E5ElF7_k1neDKuhNzH_loPvw&callback=initMap');
    this.loadScript('../../../assets/js/map.js');
    

  }
  public loadScript(url: string) {
    const body = <HTMLDivElement>document.body;
    const script = document.createElement('script');
    script.innerHTML = '';
    script.src = url;
    script.async = true;
    script.defer = true;
    body.appendChild(script);
  }
}
