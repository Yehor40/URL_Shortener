import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { UrlService, ShortUrl } from '../../services/url.service';

@Component({
  selector: 'app-url-info',
  standalone: true,
  imports: [CommonModule, RouterLink],
  template: `
    <div style="display: flex; justify-content: center; margin-top: 2rem;">
      <div class="glass-card" style="width: 100%; max-width: 600px;">
        <div style="display: flex; justify-content: space-between; align-items: start; margin-bottom: 2rem;">
          <h2 style="margin: 0;">URL Details</h2>
          <a routerLink="/" style="color: var(--text-muted); text-decoration: none;">&larr; Back home</a>
        </div>

        <div *ngIf="error" style="color: #ef4444; text-align: center; padding: 2rem; background: rgba(239, 68, 68, 0.05); border-radius: 0.5rem;">
          {{error}}
        </div>

        <div *ngIf="url && !error" class="details-grid">
          <div class="detail-item">
            <span class="label">Original URL</span>
            <div class="value overflow-clip">{{url.originalUrl}}</div>
          </div>
          <div class="detail-item">
            <span class="label">Short Code</span>
            <div class="value" style="color: var(--primary); font-weight: 700; font-size: 1.2rem;">{{url.shortCode}}</div>
          </div>
          <div class="detail-item">
            <span class="label">Created By</span>
            <div class="value">{{url.createdBy}}</div>
          </div>
          <div class="detail-item">
            <span class="label">Created Date</span>
            <div class="value">{{url.createdAtUtc | date:'medium'}}</div>
          </div>
        </div>

        <div *ngIf="!url && !error" style="text-align: center; padding: 3rem; color: var(--text-muted)">
            Gathering data...
        </div>
      </div>
    </div>
    <style>
      .details-grid {
        display: grid;
        gap: 1.5rem;
      }
      .detail-item {
        border-bottom: 1px solid var(--glass-border);
        padding-bottom: 1rem;
      }
      .label {
        display: block;
        color: var(--text-muted);
        font-size: 0.85rem;
        margin-bottom: 0.25rem;
        text-transform: uppercase;
        letter-spacing: 0.05rem;
      }
      .value {
        font-size: 1.1rem;
        word-break: break-all;
      }
      .overflow-clip {
        max-height: 100px;
        overflow-y: auto;
      }
    </style>
  `
})
export class UrlInfoComponent implements OnInit {
  url: ShortUrl | null = null;
  error = '';

  constructor(
    private route: ActivatedRoute,
    private urlService: UrlService,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit() {
    this.fetchData();
  }

  fetchData() {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.urlService.getById(id).subscribe({
        next: (url) => {
          if (!url) {
            this.error = 'Short entry not found.';
          } else {
            this.url = url;
          }
          this.cdr.detectChanges(); // Force update
        },
        error: () => {
          this.error = 'Access denied or server error.';
          this.cdr.detectChanges(); // Force update
        }
      });
    } else {
      this.error = 'Invalid entry ID.';
      this.cdr.detectChanges();
    }
  }
}
