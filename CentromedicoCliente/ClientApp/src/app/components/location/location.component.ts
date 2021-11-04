import { Component, OnInit, AfterViewInit, OnDestroy, ViewChild } from '@angular/core';
import { AutoUnsubscribe } from "ngx-auto-unsubscribe";
import { MapInfoWindow, MapMarker, GoogleMap } from '@angular/google-maps';
import { SwiperOptions } from 'swiper';


@AutoUnsubscribe()
@Component({
  selector: 'app-location',
  templateUrl: './location.component.html',
  styleUrls: ['./location.component.css']
})

export class LocationComponent implements OnInit, AfterViewInit {
  @ViewChild(MapInfoWindow) infoWindow: MapInfoWindow;

  onSwiper(swiper) {
    console.log(swiper);
  }
  onSlideChange() {
    console.log('slide change');
  }
  markers: hospital[] = [];
  center = { lat: 18.436234, lng: -69.46064 };
  zoom = 8;
  options: google.maps.MapOptions = {
    fullscreenControl: false,
    scrollwheel: false,
    zoomControl: false,
    scaleControl: true,
    disableDoubleClickZoom: true,
    mapTypeId: google.maps.MapTypeId.ROADMAP,
    maxZoom: 15,
    minZoom: 7,
  }

  constructor() { }
  ngAfterViewInit(): void {

  }
  ngOnInit(): void {
    this.markers.push({
      position: {
        lat: 18.4634403,
        lng: -69.9499557,
      },
      name: 'La Romana'
    })
     
    this.markers.push({
      position: {
        lat: 18.4296925,
        lng: -68.9656023,
      },
      name: 'Santo Domingo'
    })
    this.loadScript('../../../assets/js/carousel.js');

  }

  public loadScript(url: string) {
    const body = <HTMLDivElement>document.body;
    const script = document.createElement('script');
    script.innerHTML = '';
    script.src = url;
    script.async = false;
    script.defer = true;
    body.appendChild(script);
  }

  ngOnDestroy() {

  }
  openInfoWindow(marker: MapMarker) {
    this.infoWindow.open(marker);
  }
  setCenter(lat: number, lng: number) {
    this.zoom = 8;
    this.center = { lat: lat, lng: lng };

  }
  setFocus(lat: number, lng: number) {
    this.zoom = 14;
    this.center = { lat: lat, lng: lng };
  }
}
export interface hospital {
  name: string;
  position: {
    lat: number;
    lng: number;
  };
}
