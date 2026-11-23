
import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../services/auth';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  imports: [FormsModule],
  templateUrl: './register.html',
  styleUrl: './register.scss',
})
export class Register {

  private authService = inject(AuthService);
  private router = inject(Router);

  name: string = '';
  email: string = '';
  password: string = '';

  errorMessage = signal('');

  onSubmit(): void {
    this.errorMessage.set('');

    this.authService.register(
      this.name,
      this.email,
      this.password
    ).subscribe({
      next: (response) => {
        console.log('Utilizador registado com sucesso!');
        console.log('Novo utilizador:', response);

        this.router.navigate(['/login']);
      },
      error: (error) => {
        if (error.status === 409) {
          this.errorMessage.set('Este email já está registado.');
        } else {
          this.errorMessage.set('Ocorreu um erro ao criar a conta.');
        }
      }
    });
  }
}
