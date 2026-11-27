import { Component, OnInit, inject, signal } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth';
import { Recipe } from '../../models/recipe';

@Component({
  selector: 'app-recipe-detail',
  imports: [RouterLink],
  templateUrl: './recipe-detail.html',
  styleUrl: './recipe-detail.scss',
})
export class RecipeDetail implements OnInit {
  private route = inject(ActivatedRoute);
  private http = inject(HttpClient);
  private authService = inject(AuthService);

  private apiUrl = 'http://localhost:5199/api/recipes';
  private favoritesUrl = 'http://localhost:5199/api/favorites';

  recipe = signal<Recipe | null>(null);
  isLoading = signal(true);
  errorMessage = signal('');

  // Favoritos
  isFavorite = signal(false);
  isLoadingFavorite = signal(false);
  isSavingFavorite = signal(false);
  favoriteMessage = signal('');

  backUrl = '/';
  backText = '← Voltar às receitas';

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    const origin = this.route.snapshot.queryParamMap.get('from');

    if (origin === 'admin') {
      this.backUrl = '/admin';
      this.backText = '← Voltar às receitas em aprovação';
    } else if (origin === 'my-recipes') {
      this.backUrl = '/my-recipes';
      this.backText = '← Voltar às minhas receitas';
    } else if (origin === 'favorites') {
      this.backUrl = '/favorites';
      this.backText = '← Voltar aos meus favoritos';
    }

    if (!Number.isInteger(id) || id <= 0) {
      this.errorMessage.set('Identificador de receita inválido.');
      this.isLoading.set(false);
      return;
    }

    this.loadRecipe(id);
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

  isLoggedIn(): boolean {
    return !!this.authService.getToken();
  }

  loadRecipe(id: number): void {
    const headers = this.getHeaders() ?? new HttpHeaders();

    this.isLoading.set(true);
    this.errorMessage.set('');

    this.http.get<Recipe>(`${this.apiUrl}/${id}`, { headers }).subscribe({
      next: (data) => {
        this.recipe.set(data);
        this.isLoading.set(false);

        // Só consultar favoritos em receitas aprovadas
        if (this.isLoggedIn() && data.status === 2) {
          this.checkFavorite(id);
        }
      },
      error: (error) => {
        this.isLoading.set(false);

        if (error.status === 401) {
          this.errorMessage.set('A tua sessão expirou. Inicia sessão novamente.');
        } else if (error.status === 403) {
          this.errorMessage.set('Não tens permissão para consultar esta receita.');
        } else if (error.status === 404) {
          this.errorMessage.set('Receita não encontrada ou sem permissão de acesso.');
        } else {
          this.errorMessage.set('Não foi possível carregar a receita.');
        }
      },
    });
  }

  checkFavorite(recipeId: number): void {
    const headers = this.getHeaders();

    if (!headers) {
      return;
    }

    this.isLoadingFavorite.set(true);
    this.favoriteMessage.set('');

    this.http
      .get<{ isFavorite: boolean }>(`${this.favoritesUrl}/${recipeId}`, { headers })
      .subscribe({
        next: (response) => {
          this.isFavorite.set(response.isFavorite);
          this.isLoadingFavorite.set(false);
        },
        error: () => {
          this.isLoadingFavorite.set(false);
          this.favoriteMessage.set('Não foi possível verificar os favoritos.');
        },
      });
  }

  toggleFavorite(recipeId: number): void {
    if (this.isSavingFavorite() || this.isLoadingFavorite()) {
      return;
    }

    const headers = this.getHeaders();

    if (!headers) {
      this.favoriteMessage.set('Tens de iniciar sessão para guardar favoritos.');
      return;
    }

    this.isSavingFavorite.set(true);
    this.favoriteMessage.set('');

    if (this.isFavorite()) {
      this.http.delete<void>(`${this.favoritesUrl}/${recipeId}`, { headers }).subscribe({
        next: () => {
          this.isFavorite.set(false);
          this.isSavingFavorite.set(false);
          this.favoriteMessage.set('Receita removida dos favoritos.');
        },
        error: (error) => {
          this.isSavingFavorite.set(false);
          this.favoriteMessage.set(
            error.error?.message ?? 'Não foi possível remover dos favoritos.',
          );
        },
      });
    } else {
      this.http.post<void>(`${this.favoritesUrl}/${recipeId}`, {}, { headers }).subscribe({
        next: () => {
          this.isFavorite.set(true);
          this.isSavingFavorite.set(false);
          this.favoriteMessage.set('Receita adicionada aos favoritos!');
        },
        error: (error) => {
          this.isSavingFavorite.set(false);
          this.favoriteMessage.set(
            error.error?.message ?? 'Não foi possível adicionar aos favoritos.',
          );
        },
      });
    }
  }
}
