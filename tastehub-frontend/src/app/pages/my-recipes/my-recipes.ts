import { Component, OnInit, inject, signal } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth';

interface Recipe {
  id: number;
  title: string;
  description: string;
  instructions: string;
  durationInMinutes: number;
  difficulty: number;
  category: number;
  status: number;
  userId: number;
}

@Component({
  selector: 'app-my-recipes',
  imports: [RouterLink],
  templateUrl: './my-recipes.html',
  styleUrl: './my-recipes.scss',
})
export class MyRecipes implements OnInit {
  private http = inject(HttpClient);
  private authService = inject(AuthService);

  private apiUrl = 'http://localhost:5199/api/recipes';

  recipes = signal<Recipe[]>([]);
  isLoading = signal(true);
  errorMessage = signal('');
  successMessage = signal('');
  processingIds = signal<number[]>([]);

  ngOnInit(): void {
    this.loadRecipes();
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

  loadRecipes(): void {
    const headers = this.getHeaders();

    if (!headers) {
      this.errorMessage.set('Tens de iniciar sessão para consultar as tuas receitas.');
      this.isLoading.set(false);
      return;
    }

    this.isLoading.set(true);
    this.errorMessage.set('');

    this.http.get<Recipe[]>(`${this.apiUrl}/my`, { headers }).subscribe({
      next: (data) => {
        this.recipes.set(data);
        this.isLoading.set(false);
      },
      error: () => {
        this.errorMessage.set('Não foi possível carregar as receitas.');
        this.isLoading.set(false);
      },
    });
  }

  getStatus(status: number): string {
    switch (status) {
      case 0:
        return 'Rascunho';
      case 1:
        return 'Pendente de aprovação';
      case 2:
        return 'Aprovada';
      case 3:
        return 'Rejeitada';
      default:
        return 'Desconhecido';
    }
  }

  isProcessing(id: number): boolean {
    return this.processingIds().includes(id);
  }

  private startProcessing(id: number): void {
    this.processingIds.update((ids) => [...ids, id]);
  }

  private stopProcessing(id: number): void {
    this.processingIds.update((ids) => ids.filter((item) => item !== id));
  }

  deleteRecipe(id: number): void {
    if (this.isProcessing(id)) {
      return;
    }

    const confirmed = confirm('Tens a certeza de que queres eliminar esta receita?');

    if (!confirmed) {
      return;
    }

    const headers = this.getHeaders();

    if (!headers) {
      this.errorMessage.set('Tens de iniciar sessão.');
      return;
    }

    this.errorMessage.set('');
    this.successMessage.set('');
    this.startProcessing(id);

    this.http.delete<void>(`${this.apiUrl}/${id}`, { headers }).subscribe({
      next: () => {
        this.recipes.update((recipes) => recipes.filter((recipe) => recipe.id !== id));

        this.successMessage.set('Receita eliminada com sucesso!');

        this.stopProcessing(id);
      },
      error: (error) => {
        this.errorMessage.set(error.error?.message || 'Não foi possível eliminar a receita.');

        this.stopProcessing(id);
      },
    });
  }

  submitRecipe(id: number): void {
    if (this.isProcessing(id)) {
      return;
    }

    const headers = this.getHeaders();

    if (!headers) {
      this.errorMessage.set('Tens de iniciar sessão.');
      return;
    }

    this.errorMessage.set('');
    this.successMessage.set('');
    this.startProcessing(id);

    this.http.post<void>(`${this.apiUrl}/${id}/submit`, {}, { headers }).subscribe({
      next: () => {
        this.recipes.update((recipes) =>
          recipes.map((recipe) => (recipe.id === id ? { ...recipe, status: 1 } : recipe)),
        );

        this.successMessage.set('Receita submetida para aprovação com sucesso!');

        this.stopProcessing(id);
      },
      error: (error) => {
        this.errorMessage.set(error.error?.message || 'Não foi possível submeter a receita.');

        this.stopProcessing(id);
      },
    });
  }
}
