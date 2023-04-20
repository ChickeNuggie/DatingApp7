import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { delay, finalize, identity, Observable } from 'rxjs';
import { BusyService } from '../_services/busy.service';
import { environment } from 'src/environments/environment';

// Intercept to determine when http request is sent and coming back.
@Injectable()
export class LoadingInterceptor implements HttpInterceptor {

  constructor(private busyService: BusyService) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    this.busyService.busy(); // increase busy count of http request when is ongoing

    return next.handle(request).pipe(
      (environment.production ? identity : delay(1000)), // replace null with identity thaat returns nothing.
      finalize(() => {
        this.busyService.idle() // turn off spinner once request has been completed
      }) 
    );

  }
}
