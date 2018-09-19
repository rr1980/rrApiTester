import { Component, Inject, Input, Output, EventEmitter } from '@angular/core';
import { FetchService, IFetchResult } from '../services/fetch.service';
import { HttpHeaders } from '@angular/common/http';

@Component({
  selector: 'app-fetch-data',
  templateUrl: './fetch-data.component.html',
  styleUrls: ['./fetch-data.component.css'],
  providers: [FetchService]
})
export class FetchDataComponent {
  public result: IFetchResult<{}>;

  @Input() url: string = "";

  _execute: boolean = false;
  _isError: boolean = false;

  @Input()
  set execute(val) {
    this._execute = val;

    if (val) {
      this.result = null;

      this.fetchService.fetch(this.url, result => {
        this._isError = false;

        this.result = result;



        this._execute = false;
        this.executeChange.next(false);

      }, err => {
        this.result = err;

        this.executeChange.next(false);
        this._execute = false;

        this._isError = true;

      })
    }

  }

  //private printHeadersToConsole(scope: string, headers: HttpHeaders) {
  //  var keys = headers.keys();
  //  var headerItems = keys.map(key => `${key}: ${headers.get(key)}`);
  //  console.debug(scope, headerItems);
  //}

  @Output() executeChange: EventEmitter<boolean> = new EventEmitter<boolean>();

  constructor(private fetchService: FetchService) {
  }

}

interface WeatherForecast {
  dateFormatted: string;
  temperatureC: number;
  temperatureF: number;
  summary: string;
}
