import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth';

@Component({
  selector: 'app-suggest-ingredient',
  imports: [FormsModule, RouterLink],
  templateUrl: './suggest-ingredient.html',
  styleUrl: './suggest-ingredient.scss',
})
export class SuggestIngredient {
  private http = inject(HttpClient);
  private authService = inject(AuthService);

  ingredientName = '';

  isSaving = signal(false);
  successMessage = signal('');
  errorMessage = signal('');

  suggestIngredient(): void {
    if (this.isSaving()) {
      return;
    }

    const name = this.ingredientName.trim();

    if (name.length < 2 || name.length > 100) {
      this.errorMessage.set('O nome deve ter entre 2 e 100 caracteres.');
      return;
    }

    const token = this.authService.getToken();

    if (!token) {
      this.errorMessage.set('Tens de iniciar sessão para sugerir ingredientes.');
      return;
    }

    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`,
    });

    this.isSaving.set(true);
    this.errorMessage.set('');
    this.successMessage.set('');

    this.http.post('http://localhost:5199/api/ingredients', { name }, { headers }).subscribe({
      next: () => {
        this.isSaving.set(false);
        this.ingredientName = '';
        this.successMessage.set('Ingrediente sugerido com sucesso! Aguarda aprovação.');
      },
      error: (error) => {
        this.isSaving.set(false);

        this.errorMessage.set(
          typeof error.error?.message === 'string'
            ? error.error.message
            : 'Não foi possível sugerir o ingrediente.',
        );
      },
    });
  }
}
