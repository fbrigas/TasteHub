import { ComponentFixture, TestBed } from '@angular/core/testing';
import { SuggestIngredient } from './suggest-ingredient';

describe('SuggestIngredient', () => {
  let component: SuggestIngredient;
  let fixture: ComponentFixture<SuggestIngredient>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SuggestIngredient],
    }).compileComponents();

    fixture = TestBed.createComponent(SuggestIngredient);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
