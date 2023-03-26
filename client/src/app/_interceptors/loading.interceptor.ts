import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { delay, finalize, Observable } from 'rxjs';
import { BusyService } from '../_services/busy.service';

// Intercept to determine when http request is sent and coming back.
@Injectable()
export class LoadingInterceptor implements HttpInterceptor {

  constructor(private busyService: BusyService) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    this.busyService.busy(); // increase busy count of http request when is ongoing

    return next.handle(request).pipe(
      delay(1000),
      finalize(() => {
        this.busyService.idle() // turn off spinner once request has been completed
      }) 
    );

  }
}
