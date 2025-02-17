import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { BookingService } from '../services/booking.service'; // Ensure you have a BookingService

@Component({
  selector: 'app-booking',
  templateUrl: './booking.component.html',
  styleUrls: ['./booking.component.css']
})
export class BookingComponent implements OnInit {
  propertyId: string;
  bookingDetails: any = {}; // Define your booking details structure

  constructor(private route: ActivatedRoute, private bookingService: BookingService) {}

  ngOnInit() {
    this.propertyId = this.route.snapshot.paramMap.get('id')!;
  }

  onSubmit() {
    this.bookingService.bookProperty(this.propertyId, this.bookingDetails).subscribe(
      response => {
        // Handle successful booking
      },
      error => {
        // Handle booking error
      }
    );
  }
} 