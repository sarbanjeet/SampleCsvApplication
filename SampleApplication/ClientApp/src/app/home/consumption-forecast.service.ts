import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class ConsumptionForecastService {
  private baseUrl = 'https://localhost:7016'; // Adjust with your .NET Core API address and port

  constructor(private http: HttpClient) { }

  getStatistics(): Observable<any> {
    const url = `${this.baseUrl}/ConsumptionForecast`;
    return this.http.get<any>(url);
  }
}
