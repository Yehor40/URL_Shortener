import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { catchError, Observable, of, tap, finalize, ReplaySubject, map } from 'rxjs';

export interface User {
    email: string;
    userName: string;
    roles: string[];
}

@Injectable({
    providedIn: 'root'
})
export class AuthService {
    private apiUrl = '/api/auth';

    // ReplaySubject(1) ensures late subscribers get the latest state immediately
    private userSubject = new ReplaySubject<User | null>(1);
    user$ = this.userSubject.asObservable();

    private currentUser: User | null = null;
    private initialized = false;

    constructor(private http: HttpClient) {
        this.checkStatus();
    }

    login(email: string, password: string): Observable<User> {
        return this.http.post<User>(`${this.apiUrl}/login`, { email, password }, { withCredentials: true }).pipe(
            tap(user => {
                this.currentUser = user;
                this.userSubject.next(user);
            })
        );
    }

    logout(): void {
        this.http.post(`${this.apiUrl}/logout`, {}, { withCredentials: true }).subscribe(() => {
            this.currentUser = null;
            this.userSubject.next(null);
        });
    }

    checkStatus(): void {
        this.http.get<User>(`${this.apiUrl}/status`, { withCredentials: true }).pipe(
            catchError(() => {
                return of(null);
            }),
            finalize(() => this.initialized = true)
        ).subscribe(user => {
            this.currentUser = user;
            this.userSubject.next(user);
        });
    }

    get userSync(): User | null {
        return this.currentUser;
    }

    get isAuthInitialized(): boolean {
        return this.initialized;
    }

    get isAdmin(): boolean {
        return this.currentUser?.roles.includes('Admin') ?? false;
    }
}
