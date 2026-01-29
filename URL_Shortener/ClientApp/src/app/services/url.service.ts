import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface ShortUrl {
    id: string;
    originalUrl: string;
    shortCode: string;
    createdBy: string;
    createdAtUtc: string;
}

@Injectable({
    providedIn: 'root'
})
export class UrlService {
    private apiUrl = '/api/urls';

    constructor(private http: HttpClient) { }

    getAll(): Observable<ShortUrl[]> {
        return this.http.get<ShortUrl[]>(this.apiUrl, { withCredentials: true });
    }

    getById(id: string): Observable<ShortUrl> {
        return this.http.get<ShortUrl>(`${this.apiUrl}/${id}`, { withCredentials: true });
    }

    create(originalUrl: string): Observable<ShortUrl> {
        return this.http.post<ShortUrl>(this.apiUrl, { originalUrl }, { withCredentials: true });
    }

    delete(id: string): Observable<void> {
        return this.http.delete<void>(`${this.apiUrl}/${id}`, { withCredentials: true });
    }
}
