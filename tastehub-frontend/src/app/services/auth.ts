
import { Injectable, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

// Resposta da API após o login
export interface LoginResponse {
  token: string;
  user: {
    id: number;
    name: string;
    email: string;
    role: string;
  };
}

// Resposta da API após o registo
export interface RegisterResponse {
  id: number;
  name: string;
  email: string;
}

@Injectable({
  providedIn: 'root',
})
export class AuthService {

  private http = inject(HttpClient);
  private apiUrl = 'http://localhost:5199/api/users';

  // Dados da sessão
  private token: string | null = null;
  private currentUser = signal<LoginResponse['user'] | null>(null);

  // Login do utilizador
  login(email: string, password: string): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(
      `${this.apiUrl}/login`,
      { email, password }
    );
  }

  // Registo de um novo utilizador
  register(name: string, email: string, password: string): Observable<RegisterResponse> {
    return this.http.post<RegisterResponse>(
      `${this.apiUrl}/register`,
      { name, email, password }
    );
  }

  // Guardar o token JWT
  setToken(token: string): void {
    this.token = token;
  }

  // Obter o token JWT
  getToken(): string | null {
    return this.token;
  }

  // Guardar os dados do utilizador
  setUser(user: LoginResponse['user']): void {
    this.currentUser.set(user);
  }

  // Obter os dados do utilizador
  getUser(): LoginResponse['user'] | null {
    return this.currentUser();
  }

  // Terminar sessão
  logout(): void {
    this.token = null;
    this.currentUser.set(null);
  }
}
