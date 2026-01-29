import { Component } from '@angular/core';
import { RouterOutlet, RouterLink } from '@angular/router';
import { AuthService } from './services/auth.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, RouterLink, CommonModule],
  template: `
    <!-- Header -->
    <nav class="navbar">
      <div class="logo">
        <h2 style="margin: 0; background: linear-gradient(to right, #6366f1, #ec4899); -webkit-background-clip: text; -webkit-text-fill-color: transparent; font-weight: 800; letter-spacing: -0.025em; cursor: pointer;" routerLink="/">UrlShorty</h2>
      </div>
      <div class="nav-links">
        <a routerLink="/">Home</a>
        <a href="/About">About</a>
        <ng-container *ngIf="authService.user$ | async as user; else loginBtn">
          <span style="margin-left: 2rem; color: var(--text-muted); font-size: 0.9rem;">
            <i class="user-icon"></i> {{user.userName}}
          </span>
          <a href="javascript:void(0)" (click)="authService.logout()" class="logout-link">Logout</a>
        </ng-container>
        <ng-template #loginBtn>
          <a routerLink="/login" class="login-btn">Sign In</a>
        </ng-template>
      </div>
    </nav>

    <!-- Main Content -->
    <main class="content-wrapper">
      <router-outlet></router-outlet>
    </main>

    <!-- Footer -->
    <footer class="footer">
      <div class="footer-content">
        <div class="footer-section">
          <span class="footer-logo">UrlShorty</span>
          <p class="footer-tagline">Making links shorter, one click at a time.</p>
        </div>
        <div class="footer-section">
          <span class="footer-copyright">&copy; 2026 UrlShorty. Built with ASP.NET Core & Angular.</span>
        </div>
      </div>
    </footer>

    <style>
      .content-wrapper {
        padding: 3rem 10%;
        min-height: calc(100vh - 160px);
      }
      
      .footer {
        background: rgba(15, 23, 42, 0.9);
        border-top: 1px solid var(--glass-border);
        padding: 2rem 10%;
        margin-top: 4rem;
        backdrop-filter: blur(8px);
      }

      .footer-content {
        display: flex;
        justify-content: space-between;
        align-items: center;
        flex-wrap: wrap;
        gap: 1.5rem;
      }

      .footer-logo {
        font-weight: 700;
        font-size: 1.1rem;
        color: var(--text-main);
      }

      .footer-tagline {
        color: var(--text-muted);
        font-size: 0.85rem;
        margin: 0.25rem 0 0 0;
      }

      .footer-copyright {
        color: var(--text-muted);
        font-size: 0.8rem;
      }

      .login-btn {
        background: var(--primary);
        padding: 0.5rem 1.25rem;
        border-radius: 9999px;
        color: white !important;
        font-weight: 600;
      }

      .logout-link {
        color: #ef4444 !important;
      }

      .logout-link:hover {
        color: #f87171 !important;
      }

      @media (max-width: 768px) {
        .footer-content {
          flex-direction: column;
          text-align: center;
        }
        .content-wrapper {
          padding: 2rem 5%;
        }
      }
    </style>
  `
})
export class App {
  constructor(public authService: AuthService) { }
}
