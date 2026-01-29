import { Routes } from '@angular/router';
import { UrlTableComponent } from './components/url-table/url-table.component';
import { LoginComponent } from './components/login/login.component';
import { UrlInfoComponent } from './components/url-info/url-info.component';

export const routes: Routes = [
    { path: '', component: UrlTableComponent },
    { path: 'login', component: LoginComponent },
    { path: 'info/:id', component: UrlInfoComponent },
    { path: '**', redirectTo: '' }
];
