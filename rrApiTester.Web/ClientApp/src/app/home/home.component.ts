import { Component, Inject } from '@angular/core';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})
export class HomeComponent {

  url: string = "";
  execute: boolean = false;

  constructor(@Inject('BASE_URL') private baseUrl: string) {
    this.url = this.baseUrl + 'api/SampleData/WeatherForecasts';
  }

  onExec() {
    this.execute = true;
  }
}
