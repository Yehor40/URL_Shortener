import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../services/auth.service';
import { Router } from '@angular/router';

@Component({
    selector: 'app-login',
    standalone: true,
    imports: [CommonModule, FormsModule],
    template: `
    <div style="display: flex; justify-content: center; align-items: center; min-height: 70vh;">
      <div class="glass-card" style="width: 100%; max-width: 400px;">
        <h2 style="margin-top: 0;">Login</h2>
        <div *ngIf="error" style="color: #ef4444; margin-bottom: 1rem;">{{error}}</div>
        <form (submit)="onLogin()">
          <div style="margin-bottom: 1.5rem;">
            <label style="display: block; margin-bottom: 0.5rem; color: var(--text-muted);">Email</label>
            <input name="email" [(ngModel)]="email" class="input-glass" type="email" placeholder="admin@example.com" required>
          </div>
          <div style="margin-bottom: 2rem;">
            <label style="display: block; margin-bottom: 0.5rem; color: var(--text-muted);">Password</label>
            <input name="password" [(ngModel)]="password" class="input-glass" type="password" placeholder="••••••••" required>
          </div>
          <button type="submit" class="btn-primary" style="width: 100%;" [disabled]="loading">
            {{ loading ? 'Signing in...' : 'Sign In' }}
          </button>
        </form>
      </div>
    </div>
  `
})
export class LoginComponent {
    email = '';
    password = '';
    loading = false;
    error = '';

    constructor(private authService: AuthService, private router: Router) { }

    onLogin() {
        this.loading = true;
        this.error = '';
        this.authService.login(this.email, this.password).subscribe({
            next: () => {
                this.router.navigate(['/']);
            },
            error: (err) => {
                this.error = 'Invalid email or password.';
                this.loading = false;
            }
        });
    }
}
