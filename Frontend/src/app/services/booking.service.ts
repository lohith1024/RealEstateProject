import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class BookingService {
  private apiUrl = 'http://your-api-url/api/booking'; // Update with your API URL

  constructor(private http: HttpClient) {}

  bookProperty(propertyId: string, bookingDetails: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/book/${propertyId}`, bookingDetails);
  }
} 