import { Component, OnInit, inject, ChangeDetectorRef } from '@angular/core';
import { RouterLink } from '@angular/router';
import { RecipeService } from '../../services/recipe';
import { Recipe } from '../../models/recipe';

@Component({
  selector: 'app-home',
  templateUrl: './home.html',
  imports: [RouterLink],
  styleUrl: './home.scss',
})
export class Home implements OnInit {
  private cdr = inject(ChangeDetectorRef);
  private recipeService = inject(RecipeService);

  recipes: Recipe[] = [];

  ngOnInit(): void {
    this.recipeService.getRecipes().subscribe({
      next: (data) => {
        this.recipes = data;
        this.cdr.markForCheck();
      },
      error: (error) => {
        console.error('Erro ao obter receitas:', error);
      },
    });
  }
}
