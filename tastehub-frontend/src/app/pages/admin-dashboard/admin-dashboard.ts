import { Component, OnInit, inject, signal } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth';

interface Recipe {
  id: number;
  title: string;
  description: string;
  durationInMinutes: number;
  status: number;
  userId: number;
}

interface Ingredient {
  id: number;
  name: string;
}

@Component({
  selector: 'app-admin-dashboard',
  imports: [RouterLink],
  templateUrl: './admin-dashboard.html',
  styleUrl: './admin-dashboard.scss',
})
export class AdminDashboard implements OnInit {
  private http = inject(HttpClient);
  private authService = inject(AuthService);

  private recipesUrl = 'http://localhost:5199/api/recipes';
  private ingredientsUrl = 'http://localhost:5199/api/ingredients';

  recipes = signal<Recipe[]>([]);
  ingredients = signal<Ingredient[]>([]);

  isLoading = signal(true);
  isLoadingIngredients = signal(true);

  errorMessage = signal('');
  ingredientError = signal('');
  successMessage = signal('');

  processingIds = signal<number[]>([]);
  processingIngredientIds = signal<number[]>([]);

  ngOnInit(): void {
    this.loadRecipes();
    this.loadIngredients();
  }

  private getHeaders(): HttpHeaders | null {
    const user = this.authService.getUser();
    const token = this.authService.getToken();

    if (!token || user?.role !== 'Admin') {
      return null;
    }

    return new HttpHeaders({
      Authorization: `Bearer ${token}`,
    });
  }

  loadRecipes(): void {
    const headers = this.getHeaders();

    if (!headers) {
      this.errorMessage.set('Acesso reservado a administradores.');
      this.isLoading.set(false);
      return;
    }

    this.isLoading.set(true);
    this.errorMessage.set('');

    this.http.get<Recipe[]>(this.recipesUrl, { headers }).subscribe({
      next: (data) => {
        this.recipes.set(data.filter((recipe) => recipe.status === 1));
        this.isLoading.set(false);
      },
      error: () => {
        this.errorMessage.set('Não foi possível carregar as receitas.');
        this.isLoading.set(false);
      },
    });
  }

  loadIngredients(): void {
    const headers = this.getHeaders();

    if (!headers) {
      this.ingredientError.set('Acesso reservado a administradores.');
      this.isLoadingIngredients.set(false);
      return;
    }

    this.isLoadingIngredients.set(true);
    this.ingredientError.set('');

    this.http.get<Ingredient[]>(`${this.ingredientsUrl}/pending`, { headers }).subscribe({
      next: (data) => {
        this.ingredients.set(data);
        this.isLoadingIngredients.set(false);
      },
      error: () => {
        this.ingredientError.set('Não foi possível carregar os ingredientes pendentes.');
        this.isLoadingIngredients.set(false);
      },
    });
  }

  isProcessing(id: number): boolean {
    return this.processingIds().includes(id);
  }

  isProcessingIngredient(id: number): boolean {
    return this.processingIngredientIds().includes(id);
  }

  private processRecipe(id: number, action: 'approve' | 'reject'): void {
    if (this.isProcessing(id)) {
      return;
    }

    const headers = this.getHeaders();

    if (!headers) {
      this.errorMessage.set('Acesso reservado a administradores.');
      return;
    }

    this.errorMessage.set('');
    this.successMessage.set('');
    this.processingIds.update((ids) => [...ids, id]);

    this.http.post<void>(`${this.recipesUrl}/${id}/${action}`, {}, { headers }).subscribe({
      next: () => {
        this.recipes.update((recipes) => recipes.filter((recipe) => recipe.id !== id));

        this.successMessage.set(
          action === 'approve' ? 'Receita aprovada com sucesso!' : 'Receita rejeitada com sucesso!',
        );

        this.processingIds.update((ids) => ids.filter((item) => item !== id));
      },
      error: (error) => {
        this.errorMessage.set(
          typeof error.error?.message === 'string'
            ? error.error.message
            : 'Não foi possível processar a receita.',
        );

        this.processingIds.update((ids) => ids.filter((item) => item !== id));
      },
    });
  }

  approveRecipe(id: number): void {
    this.processRecipe(id, 'approve');
  }

  rejectRecipe(id: number): void {
    this.processRecipe(id, 'reject');
  }

  private processIngredient(id: number, action: 'approve' | 'reject'): void {
    if (this.isProcessingIngredient(id)) {
      return;
    }

    const headers = this.getHeaders();

    if (!headers) {
      this.ingredientError.set('Acesso reservado a administradores.');
      return;
    }

    this.ingredientError.set('');
    this.successMessage.set('');

    this.processingIngredientIds.update((ids) => [...ids, id]);

    this.http.post<void>(`${this.ingredientsUrl}/${id}/${action}`, {}, { headers }).subscribe({
      next: () => {
        this.ingredients.update((ingredients) =>
          ingredients.filter((ingredient) => ingredient.id !== id),
        );

        this.successMessage.set(
          action === 'approve'
            ? 'Ingrediente aprovado com sucesso!'
            : 'Ingrediente rejeitado com sucesso!',
        );

        this.processingIngredientIds.update((ids) => ids.filter((item) => item !== id));
      },
      error: (error) => {
        this.ingredientError.set(
          typeof error.error?.message === 'string'
            ? error.error.message
            : 'Não foi possível processar o ingrediente.',
        );

        this.processingIngredientIds.update((ids) => ids.filter((item) => item !== id));
      },
    });
  }

  approveIngredient(id: number): void {
    this.processIngredient(id, 'approve');
  }

  rejectIngredient(id: number): void {
    this.processIngredient(id, 'reject');
  }
}
