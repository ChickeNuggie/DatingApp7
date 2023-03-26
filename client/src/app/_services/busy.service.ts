import { Injectable } from '@angular/core';
import { NgxSpinnerService } from 'ngx-spinner';

@Injectable({
  providedIn: 'root'
})

//Create a service that effectively enables spinne if Http request going on and stops when Http request finished.
export class BusyService {
  busyRequestCount = 0; // dsplay spinner if count >0 

  constructor(private spinnerService: NgxSpinnerService) { }

  busy() {
    this.busyRequestCount++;
    this.spinnerService.show(undefined, {
      type: 'pacman',
      bdColor: 'rgba(255,255,255,0)',
      color: '#333333'
    })
  }

  idle() {
    this.busyRequestCount--;
    if (this.busyRequestCount <= 0 ){
      this.busyRequestCount = 0;
      this.spinnerService.hide();
    }
  }
}
