import { Component, OnInit, inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { HttpClient } from '@angular/common/http'

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit{
  users: any;
  ngOnInit(): void {
    this.http.get('https://localhost:5001/api/users').subscribe({
      next: respone => this.users = respone,
      error: error => console.log(error),
      complete: () => console.log("Request has complete")
    });
  }
  title = 'Dating App';
  http = inject(HttpClient);
}
