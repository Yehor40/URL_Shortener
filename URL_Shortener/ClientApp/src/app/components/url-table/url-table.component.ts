import { Component, OnInit, OnDestroy, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { UrlService, ShortUrl } from '../../services/url.service';
import { AuthService, User } from '../../services/auth.service';
import { RouterLink } from '@angular/router';
import { Subject, takeUntil } from 'rxjs';

@Component({
  selector: 'app-url-table',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  template: `
    <!-- Top Form (Only if authenticated) -->
    <div class="glass-card" style="margin-bottom: 2rem;" *ngIf="user">
      <h3 style="margin-top: 0;">Add new Url</h3>
      <div *ngIf="error" style="color: #ef4444; margin-bottom: 1rem; padding: 0.75rem; background: rgba(239, 68, 68, 0.1); border-radius: 0.5rem; font-size: 0.9rem;">{{error}}</div>
      <form (submit)="onCreate()" style="display: flex; gap: 1rem;">
        <input name="newUrl" [(ngModel)]="newUrl" class="input-glass" style="flex: 1;" placeholder="https://example.com/very/long/url" required>
        <button type="submit" class="btn-primary" [disabled]="loading">
          {{ loading ? 'Shortening...' : 'Shorten' }}
        </button>
      </form>
    </div>

    <!-- Table Section -->
    <div class="glass-card">
      <div style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 1.5rem;">
        <h3 style="margin: 0;">Shortened URLs</h3>
        <button (click)="loadUrls()" class="btn-refresh" [disabled]="loadingTable">
           {{ loadingTable ? 'Updating...' : 'Refresh' }}
        </button>
      </div>

      <div *ngIf="loadingTable && urls.length === 0" style="text-align: center; padding: 3rem; color: var(--text-muted)">
        Gathering links...
      </div>

      <table *ngIf="urls.length > 0">
        <thead>
          <tr>
            <th>Original URL</th>
            <th>Short URL</th>
            <th>By</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          <tr *ngFor="let url of urls">
            <td style="max-width: 250px; overflow: hidden; text-overflow: ellipsis; white-space: nowrap;">
              <a [href]="url.originalUrl" target="_blank" style="color: var(--text-main); text-decoration: none;">{{url.originalUrl}}</a>
            </td>
            <td>
              <a [href]="'http://localhost:5094/' + url.shortCode" target="_blank" style="color: var(--primary); font-weight: 600;">{{url.shortCode}}</a>
            </td>
            <td style="font-size: 0.85rem; color: var(--text-muted)">{{url.createdBy}}</td>
            <td>
              <div style="display: flex; gap: 0.75rem;">
                <a [routerLink]="['/info', url.id]" style="color: var(--text-muted); text-decoration: none; font-size: 0.85rem;">Details</a>
                <button (click)="onDelete(url)" *ngIf="canDelete(url)" style="background: none; border: none; color: #ef4444; cursor: pointer; padding: 0; font-size: 0.85rem;">Delete</button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>

      <div *ngIf="!loadingTable && urls.length === 0" style="text-align: center; padding: 3rem; color: var(--text-muted)">
        No URLs found yet.
      </div>
    </div>

    <style>
      .btn-refresh {
        background: rgba(255, 255, 255, 0.05);
        color: var(--text-muted);
        border: 1px solid var(--glass-border);
        padding: 0.4rem 0.8rem;
        border-radius: 0.3rem;
        font-size: 0.8rem;
        cursor: pointer;
        transition: all 0.2s;
      }
      .btn-refresh:hover {
        background: rgba(255, 255, 255, 0.1);
        color: var(--text-main);
      }
    </style>
  `
})
export class UrlTableComponent implements OnInit, OnDestroy {
  urls: ShortUrl[] = [];
  user: User | null = null;
  newUrl = '';
  loading = false;
  loadingTable = false;
  error = '';
  private destroy$ = new Subject<void>();

  constructor(
    public urlService: UrlService,
    private authService: AuthService,
    private cdr: ChangeDetectorRef // Inject ChangeDetectorRef
  ) { }

  ngOnInit() {
    this.authService.user$.pipe(takeUntil(this.destroy$)).subscribe(user => {
      this.user = user;
      this.cdr.detectChanges(); // Force update
    });

    this.loadUrls();
  }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadUrls() {
    this.loadingTable = true;
    this.urlService.getAll().pipe(takeUntil(this.destroy$)).subscribe({
      next: (urls) => {
        this.urls = urls;
        this.loadingTable = false;
        this.cdr.detectChanges(); // Force update immediately
      },
      error: () => {
        this.error = 'Failed to load URLs.';
        this.loadingTable = false;
        this.cdr.detectChanges(); // Force update
      }
    });
  }

  onCreate() {
    if (!this.newUrl) return;
    this.loading = true;
    this.error = '';
    this.urlService.create(this.newUrl).subscribe({
      next: () => {
        this.newUrl = '';
        this.loading = false;
        this.loadUrls();
        this.cdr.detectChanges(); // Force update
      },
      error: (err) => {
        const errors = err.error?.errors;
        this.error = errors ? errors.join(' ') : 'Failed to create short URL.';
        this.loading = false;
        this.cdr.detectChanges(); // Force update
      }
    });
  }

  onDelete(url: ShortUrl) {
    if (confirm('Are you sure?')) {
      this.urlService.delete(url.id).subscribe(() => {
        this.loadUrls();
        this.cdr.detectChanges();
      });
    }
  }

  canDelete(url: ShortUrl): boolean {
    if (!this.user) return false;
    if (this.authService.isAdmin) return true;
    return this.user.userName === url.createdBy;
  }
}
