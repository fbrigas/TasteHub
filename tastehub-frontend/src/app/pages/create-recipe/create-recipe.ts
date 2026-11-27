import { Component, OnInit, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth';

interface RecipeIngredient {
  ingredientId: number;
  quantity: number;
  unit: string;
}

interface NewRecipeIngredient {
  name: string;
  quantity: number;
  unit: string;
}

interface Ingredient {
  id: number;
  name: string;
}

@Component({
  selector: 'app-create-recipe',
  imports: [FormsModule, RouterLink],
  templateUrl: './create-recipe.html',
  styleUrl: './create-recipe.scss',
})
export class CreateRecipe implements OnInit {
  private http = inject(HttpClient);
  private authService = inject(AuthService);

  private ingredientsUrl = 'http://localhost:5199/api/ingredients';

  private recipesUrl = 'http://localhost:5199/api/recipes';

  title = '';
  description = '';
  instructions = '';
  durationInMinutes = 30;
  difficulty = 0;
  category = 0;

  availableIngredients: Ingredient[] = [];

  ingredients: RecipeIngredient[] = [];
  newIngredients: NewRecipeIngredient[] = [];

  newIngredientId = 0;
  newQuantity = 100;
  newUnit = 'g';

  suggestedName = '';
  suggestedQuantity = 100;
  suggestedUnit = 'g';

  errorMessage = '';
  saveMessage = signal('');
  isSaving = signal(false);

  ngOnInit(): void {
    this.loadIngredients();
  }

  loadIngredients(): void {
    this.http.get<Ingredient[]>(this.ingredientsUrl).subscribe({
      next: (data) => {
        this.availableIngredients = data;
      },
      error: () => {
        this.errorMessage = 'Não foi possível carregar os ingredientes.';
      },
    });
  }

  addIngredient(): void {
    this.errorMessage = '';

    const exists = this.availableIngredients.some((item) => item.id === this.newIngredientId);

    const alreadyAdded = this.ingredients.some(
      (item) => item.ingredientId === this.newIngredientId,
    );

    if (!exists) {
      this.errorMessage = 'Seleciona um ingrediente válido.';
      return;
    }

    if (alreadyAdded) {
      this.errorMessage = 'Este ingrediente já foi adicionado à receita.';
      return;
    }

    if (!Number.isFinite(this.newQuantity) || this.newQuantity <= 0 || !this.newUnit.trim()) {
      this.errorMessage = 'Indica uma quantidade e uma unidade válidas.';
      return;
    }

    this.ingredients.push({
      ingredientId: this.newIngredientId,
      quantity: this.newQuantity,
      unit: this.newUnit.trim(),
    });

    this.newIngredientId = 0;
    this.newQuantity = 100;
    this.newUnit = 'g';
  }

  addNewIngredient(): void {
    this.errorMessage = '';

    const name = this.suggestedName.trim();

    if (name.length < 2 || name.length > 100) {
      this.errorMessage = 'O nome deve ter entre 2 e 100 caracteres.';
      return;
    }

    if (
      !Number.isFinite(this.suggestedQuantity) ||
      this.suggestedQuantity <= 0 ||
      !this.suggestedUnit.trim()
    ) {
      this.errorMessage = 'Indica uma quantidade e uma unidade válidas.';
      return;
    }

    const normalizedName = name.toLocaleLowerCase();

    const alreadyExists = this.availableIngredients.some(
      (item) => item.name.trim().toLocaleLowerCase() === normalizedName,
    );

    const alreadySuggested = this.newIngredients.some(
      (item) => item.name.trim().toLocaleLowerCase() === normalizedName,
    );

    if (alreadyExists || alreadySuggested) {
      this.errorMessage = 'Este ingrediente já existe ou já foi adicionado.';
      return;
    }

    this.newIngredients.push({
      name,
      quantity: this.suggestedQuantity,
      unit: this.suggestedUnit.trim(),
    });

    this.suggestedName = '';
    this.suggestedQuantity = 100;
    this.suggestedUnit = 'g';
  }

  removeIngredient(index: number): void {
    this.ingredients.splice(index, 1);
  }

  removeNewIngredient(index: number): void {
    this.newIngredients.splice(index, 1);
  }

  getIngredientName(id: number): string {
    return (
      this.availableIngredients.find((item) => item.id === id)?.name ?? 'Ingrediente desconhecido'
    );
  }

  saveRecipe(): void {
    if (this.isSaving()) {
      return;
    }

    const token = this.authService.getToken();

    if (!token) {
      this.saveMessage.set('Tens de iniciar sessão para criar uma receita.');
      return;
    }

    if (
      this.title.trim().length < 3 ||
      this.title.trim().length > 100 ||
      !this.description.trim() ||
      this.description.trim().length > 2000 ||
      !this.instructions.trim() ||
      !Number.isInteger(this.durationInMinutes) ||
      this.durationInMinutes < 1 ||
      this.durationInMinutes > 1440 ||
      this.ingredients.length + this.newIngredients.length === 0
    ) {
      this.saveMessage.set('Preenche os campos obrigatórios e adiciona pelo menos um ingrediente.');
      return;
    }

    const recipe = {
      title: this.title.trim(),
      description: this.description.trim(),
      instructions: this.instructions.trim(),
      durationInMinutes: this.durationInMinutes,
      difficulty: this.difficulty,
      category: this.category,
      ingredients: this.ingredients,
      newIngredients: this.newIngredients,
    };

    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`,
    });

    this.isSaving.set(true);
    this.saveMessage.set('');

    this.http.post(this.recipesUrl, recipe, { headers }).subscribe({
      next: () => {
        this.isSaving.set(false);
        this.saveMessage.set(
          'Receita guardada como rascunho! ' +
            'Os novos ingredientes ficaram pendentes de aprovação.',
        );

        this.ingredients = [];
        this.newIngredients = [];

        this.title = '';
        this.description = '';
        this.instructions = '';
        this.durationInMinutes = 30;
        this.difficulty = 0;
        this.category = 0;
      },
      error: (error) => {
        this.isSaving.set(false);

        this.saveMessage.set(
          typeof error.error?.message === 'string'
            ? error.error.message
            : 'Não foi possível guardar a receita.',
        );
      },
    });
  }
}
