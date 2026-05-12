import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterOutlet],
  template: `
    <div class="container">
      <header class="navbar">
        <h1>SentinelX</h1>
        <nav>
          <a href="/dashboard">Dashboard</a>
          <a href="/users">Users</a>
          <a href="/security">Security</a>
          <a href="/audit">Audit</a>
          <a href="/profile">Profile</a>
        </nav>
      </header>
      <main>
        <router-outlet></router-outlet>
      </main>
    </div>
  `,
  styles: [`
    .container {
      display: flex;
      flex-direction: column;
      height: 100vh;
    }
    .navbar {
      background-color: #333;
      color: white;
      padding: 1rem;
      display: flex;
      justify-content: space-between;
      align-items: center;
    }
    h1 {
      margin: 0;
    }
    nav a {
      color: white;
      margin-left: 2rem;
      text-decoration: none;
    }
    nav a:hover {
      color: #007bff;
    }
    main {
      flex: 1;
      padding: 2rem;
      overflow-y: auto;
    }
  `]
})
export class AppComponent {
  title = 'SentinelX Admin Dashboard';
}
