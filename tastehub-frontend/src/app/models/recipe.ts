
export interface Recipe {
  id: number;
  title: string;
  description: string;
  instructions: string;
  durationInMinutes: number;
  difficulty: number;
  status: number;
  userId: number;
  ingredients: RecipeIngredient[];
}

export interface RecipeIngredient {
  ingredientId: number;
  name: string;
  quantity: number;
  unit: string;
}
