import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'noProfilePhoto'
})
export class NoProfilePhotoPipe implements PipeTransform {

  transform(poster: string ): string {
    if(poster) {
      return poster;
    }else {
      return '../assets/imgs/no-image.png';
    } 
  }
}



