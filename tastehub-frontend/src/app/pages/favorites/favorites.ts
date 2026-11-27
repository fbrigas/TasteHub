import { Component, OnInit, inject, signal } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth';
import { Recipe } from '../../models/recipe';

@Component({
  selector: 'app-favorites',
  imports: [RouterLink],
  templateUrl: './favorites.html',
  styleUrl: './favorites.scss',
})
export class Favorites implements OnInit {
  private http = inject(HttpClient);
  private authService = inject(AuthService);

  private apiUrl = 'http://localhost:5199/api/favorites';

  recipes = signal<Recipe[]>([]);
  isLoading = signal(true);
  errorMessage = signal('');
  successMessage = signal('');
  removingIds = signal<number[]>([]);

  ngOnInit(): void {
    this.loadFavorites();
  }

  private getHeaders(): HttpHeaders | null {
    const token = this.authService.getToken();

    if (!token) {
      return null;
    }

    return new HttpHeaders({
      Authorization: `Bearer ${token}`,
    });
  }

  loadFavorites(): void {
    const headers = this.getHeaders();

    if (!headers) {
      this.errorMessage.set('Tens de iniciar sessão para consultar os favoritos.');
      this.isLoading.set(false);
      return;
    }

    this.isLoading.set(true);
    this.errorMessage.set('');

    this.http.get<Recipe[]>(this.apiUrl, { headers }).subscribe({
      next: (data) => {
        this.recipes.set(data);
        this.isLoading.set(false);
      },
      error: () => {
        this.errorMessage.set('Não foi possível carregar os favoritos.');
        this.isLoading.set(false);
      },
    });
  }

  removeFavorite(recipeId: number): void {
    if (this.removingIds().includes(recipeId)) {
      return;
    }

    const headers = this.getHeaders();

    if (!headers) {
      this.errorMessage.set('Tens de iniciar sessão.');
      return;
    }

    this.errorMessage.set('');
    this.successMessage.set('');
    this.removingIds.update((ids) => [...ids, recipeId]);

    this.http.delete<void>(`${this.apiUrl}/${recipeId}`, { headers }).subscribe({
      next: () => {
        this.recipes.update((recipes) => recipes.filter((recipe) => recipe.id !== recipeId));

        this.removingIds.update((ids) => ids.filter((id) => id !== recipeId));

        this.successMessage.set('Receita removida dos favoritos.');
      },
      error: () => {
        this.removingIds.update((ids) => ids.filter((id) => id !== recipeId));

        this.errorMessage.set('Não foi possível remover a receita.');
      },
    });
  }
}
