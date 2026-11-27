import { Routes } from '@angular/router';
import { SuggestIngredient } from './pages/suggest-ingredient/suggest-ingredient';
import { Home } from './pages/home/home';
import { Login } from './pages/login/login';
import { Register } from './pages/register/register';
import { RecipeDetail } from './pages/recipe-detail/recipe-detail';
import { CreateRecipe } from './pages/create-recipe/create-recipe';
import { MyRecipes } from './pages/my-recipes/my-recipes';
import { EditRecipe } from './pages/edit-recipe/edit-recipe';
import { AdminDashboard } from './pages/admin-dashboard/admin-dashboard';
import { Favorites } from './pages/favorites/favorites';

export const routes: Routes = [
  { path: '', component: Home },
  { path: 'login', component: Login },
  { path: 'register', component: Register },
  { path: 'recipes/create', component: CreateRecipe },
  { path: 'my-recipes', component: MyRecipes },
  { path: 'admin', component: AdminDashboard },
  { path: 'recipes/:id/edit', component: EditRecipe },
  { path: 'recipes/:id', component: RecipeDetail },
  { path: 'favorites', component: Favorites },
  { path: 'ingredients/suggest', component: SuggestIngredient },
  { path: '**', redirectTo: '' },
];
