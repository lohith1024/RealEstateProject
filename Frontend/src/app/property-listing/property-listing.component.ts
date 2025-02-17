import { Component, OnInit } from '@angular/core';
import { PropertyService } from '../services/property.service'; // Ensure you have a PropertyService

@Component({
  selector: 'app-property-listing',
  templateUrl: './property-listing.component.html',
  styleUrls: ['./property-listing.component.css']
})
export class PropertyListingComponent implements OnInit {
  properties: any[] = [];

  constructor(private propertyService: PropertyService) {}

  ngOnInit() {
    this.loadProperties();
  }

  loadProperties() {
    this.propertyService.getProperties().subscribe(
      data => {
        this.properties = data;
      },
      error => {
        // Handle error
      }
    );
  }
} 