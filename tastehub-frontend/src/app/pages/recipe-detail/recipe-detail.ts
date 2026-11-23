import { RouterLink } from '@angular/router';
import { Component, OnInit, inject, ChangeDetectorRef } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { RecipeService } from '../../services/recipe';
import { Recipe } from '../../models/recipe';

@Component({
  selector: 'app-recipe-detail',
  templateUrl: './recipe-detail.html',
  imports: [RouterLink],
  styleUrl: './recipe-detail.scss',
})
export class RecipeDetail implements OnInit {
  private route = inject(ActivatedRoute);
  private recipeService = inject(RecipeService);
  private cdr = inject(ChangeDetectorRef);

  recipe: Recipe | null = null;

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));

    if (!Number.isInteger(id) || id <= 0) {
      console.error('ID da receita inválido');
      return;
    }

    this.recipeService.getRecipeById(id).subscribe({
      next: (data) => {
        this.recipe = data;
        this.cdr.markForCheck();
      },
      error: (error) => {
        console.error('Erro ao obter receita:', error);
      },
    });
  }
}
