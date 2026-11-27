import { Component, OnInit, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { ActivatedRoute, Router } from '@angular/router';
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
}

@Component({
  selector: 'app-edit-recipe',
  imports: [FormsModule],
  templateUrl: './edit-recipe.html',
  styleUrl: './edit-recipe.scss',
})
export class EditRecipe implements OnInit {
  private http = inject(HttpClient);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private authService = inject(AuthService);

  private apiUrl = 'http://localhost:5199/api/recipes';

  recipeId = 0;

  title = '';
  description = '';
  instructions = '';
  durationInMinutes = 30;
  difficulty = 0;
  category = 0;

  isLoading = signal(true);
  isSaving = signal(false);
  errorMessage = signal('');
  successMessage = signal('');

  ngOnInit(): void {
    this.recipeId = Number(this.route.snapshot.paramMap.get('id'));

    if (!Number.isInteger(this.recipeId) || this.recipeId <= 0) {
      this.errorMessage.set('Identificador de receita inválido.');
      this.isLoading.set(false);
      return;
    }

    this.loadRecipe();
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

  loadRecipe(): void {
    const headers = this.getHeaders();

    if (!headers) {
      this.errorMessage.set('Tens de iniciar sessão.');
      this.isLoading.set(false);
      return;
    }

    this.http.get<Recipe>(`${this.apiUrl}/${this.recipeId}`, { headers }).subscribe({
      next: (recipe) => {
        if (recipe.status !== 0) {
          this.errorMessage.set('Esta receita não está em rascunho e não pode ser editada.');
          this.isLoading.set(false);
          return;
        }

        this.title = recipe.title;
        this.description = recipe.description;
        this.instructions = recipe.instructions;
        this.durationInMinutes = recipe.durationInMinutes;
        this.difficulty = recipe.difficulty;
        this.category = recipe.category;

        this.isLoading.set(false);
      },
      error: () => {
        this.errorMessage.set('Não foi possível carregar a receita.');
        this.isLoading.set(false);
      },
    });
  }

  saveRecipe(): void {
    if (this.isSaving() || this.isLoading() || this.errorMessage()) {
      return;
    }

    if (
      this.title.trim().length < 3 ||
      this.title.trim().length > 100 ||
      !this.description.trim() ||
      this.description.length > 2000 ||
      !this.instructions.trim() ||
      !Number.isInteger(this.durationInMinutes) ||
      this.durationInMinutes < 1 ||
      this.durationInMinutes > 1440
    ) {
      this.errorMessage.set('Verifica os campos obrigatórios e o tempo de preparação.');
      return;
    }

    const headers = this.getHeaders();

    if (!headers) {
      this.errorMessage.set('Tens de iniciar sessão.');
      return;
    }

    const updatedRecipe = {
      title: this.title.trim(),
      description: this.description.trim(),
      instructions: this.instructions.trim(),
      durationInMinutes: this.durationInMinutes,
      difficulty: this.difficulty,
      category: this.category,
    };

    this.isSaving.set(true);
    this.successMessage.set('');

    this.http.put<void>(`${this.apiUrl}/${this.recipeId}`, updatedRecipe, { headers }).subscribe({
      next: () => {
        this.isSaving.set(false);
        this.successMessage.set('Receita atualizada com sucesso!');

        this.router.navigate(['/my-recipes']);
      },
      error: (error) => {
        this.isSaving.set(false);
        this.errorMessage.set(error.error?.message || 'Não foi possível atualizar a receita.');
      },
    });
  }

  cancel(): void {
    this.router.navigate(['/my-recipes']);
  }
}
