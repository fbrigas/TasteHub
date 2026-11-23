import { Component, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../services/auth';

@Component({
  selector: 'app-login',
  imports: [FormsModule],
  templateUrl: './login.html',
  styleUrl: './login.scss',
})
export class Login {
  private authService = inject(AuthService);
  private router = inject(Router);

  email: string = '';
  password: string = '';
  errorMessage = signal('');

  onSubmit(): void {
    this.errorMessage.set('');

    this.authService.login(this.email, this.password).subscribe({
      next: (response) => {
        this.authService.setToken(response.token);
        this.authService.setUser(response.user);

        console.log('Login realizado com sucesso!');
        console.log('Utilizador:', response.user);
        console.log('Token guardado:', this.authService.getToken() !== null);

        this.router.navigate(['/']);
        
      },
      error: (error) => {
        if (error.status === 401) {
          this.errorMessage.set('Email ou palavra-passe incorretos.');
        } else {
          this.errorMessage.set('Ocorreu um erro ao iniciar sessão.');
        }
      },
    });
  }
}
