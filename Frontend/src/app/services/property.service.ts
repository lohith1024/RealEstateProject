import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class PropertyService {
  private apiUrl = 'http://your-api-url/api/property'; // Update with your API URL

  constructor(private http: HttpClient) {}

  getProperties(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/list`);
  }
} 