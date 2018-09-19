import { Injectable } from '@angular/core';
import { HttpClient, HttpResponse, HttpRequest, HttpEventType, HttpEvent } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { map, catchError, finalize } from 'rxjs/operators';


export interface IFetchResult<T> {

  request: HttpRequest<{ requestProgress: true }>;
  response: HttpResponse<T>;

}

@Injectable({
  providedIn: 'root'
})
export class FetchService {

  constructor(private http: HttpClient) { }

  fetch<T>(url: string, cb: (resuult: IFetchResult<T>) => void, err: (resuult: IFetchResult<T>) => void) {


    const req = new HttpRequest('GET', url, {
      requestProgress: true
    });

    this.http.request<T>(req).subscribe(event => {
      if (event.type === HttpEventType.ResponseHeader) {
      }
      else if (event.type === HttpEventType.Response) {
        var response = event.body as T;
        if (cb) {
          cb(
            {
              request: req,
              response: event
            } as IFetchResult<T>
          );
        }
      }
    }, error => {
      if (err) {
        err(error);
      }
      else {
        console.debug("FetchService", error)
      }
    });
  }
}
