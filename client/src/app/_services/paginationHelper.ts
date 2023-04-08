 //allow access commonly used functions into services

import { HttpClient, HttpParams } from "@angular/common/http";
import { PaginatedResult } from "../_models/pagination";
import { map } from "rxjs";

 // Make method generic using <T> to be reused on other class/objects
  export function getPaginatedResult<T>(url: string, params: HttpParams, http: HttpClient) {
    const paginatedResult: PaginatedResult<T> = new PaginatedResult<T>;
    return http.get<T>(url, { observe: 'response', params }).pipe(
      map(response => {
        if (response.body) {
          paginatedResult.result = response.body;
        }
        const pagination = response.headers.get('Pagination');
        if (pagination) {
          paginatedResult.pagination = JSON.parse(pagination); // get the serialized JSON into an object.
        }
        console.log(paginatedResult);
        return paginatedResult;
      })
    );
  }


  export function getPaginationHeaders(pageNumber: number, pageSize: number) {
    let params = new HttpParams();

    params = params.append('pageNumber', pageNumber);
    params = params.append('pageSize', pageSize);
    
    return params;
  }

