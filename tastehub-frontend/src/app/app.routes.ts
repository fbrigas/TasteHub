
import { Routes } from '@angular/router';
import { Home } from './pages/home/home';
import { RecipeDetail } from './pages/recipe-detail/recipe-detail';
import { Login } from './pages/login/login';
import { Register } from './pages/register/register';

export const routes: Routes = [
  { path: '', component: Home },
  { path: 'recipes/:id', component: RecipeDetail },
  { path: 'login', component: Login },
  { path: 'register', component: Register }
];
