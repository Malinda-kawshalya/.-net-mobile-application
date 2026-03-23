# FlavorVault (Recipes app)

FlavorVault is a cross-platform .NET MAUI recipes application built with C# and MVVM.
It combines public recipe discovery (TheMealDB), Firebase authentication, Firestore data storage, favorites management, custom recipe authoring, and shopping list workflows in one mobile-first app.

This README is a complete technical reference for the codebase, including architecture, setup, runtime flow, and function-level documentation.

## Table of Contents

1. Project Summary
2. Features
3. Tech Stack
4. Architecture Overview
5. App Startup and Navigation Flow
6. Dependency Injection Map
7. Environment and Configuration
8. Build and Run
9. Folder Structure
10. Interfaces (Full Contract Reference)
11. Services (Function-by-Function)
12. ViewModels (Properties, Commands, Methods)
13. Models (Data Contract Reference)
14. Controls and Converters
15. Views and Code-Behind Responsibilities
16. External API Reference
17. Data Storage Behavior
18. Error Handling Strategy
19. Known Limitations
20. Coursework Submission Notes

## 1. Project Summary

FlavorVault helps users:

- Sign in and register with Firebase Auth
- Search and browse recipes using TheMealDB
- View full recipe details including ingredients and instructions
- Save and remove favorites
- Add recipe ingredients to a shopping list
- Create and edit user recipes
- Toggle light/dark mode

## 2. Features

- Email/password authentication and password reset
- Home dashboard with featured recipes, categories, and recent recipes
- Category browsing and keyword search
- Full recipe details with servings controls and rating display
- Favorites tab synchronized with cloud database
- Shopping list management (toggle, delete, clear completed, clear all)
- Add/edit custom recipes with dynamic ingredient and instruction lists
- Theme persistence via local app preferences
- Cloud fallback behavior when Firebase is not configured

## 3. Tech Stack

- .NET 8 + .NET MAUI
- C#
- CommunityToolkit.Mvvm (observable properties and relay commands)
- CommunityToolkit.Maui
- Newtonsoft.Json
- Firebase Authentication REST API
- Firebase Firestore REST API
- TheMealDB public API

## 4. Architecture Overview

Pattern: MVVM + dependency injection + service abstraction.

- Views: XAML pages and minimal code-behind in `Views/`
- ViewModels: state + command logic in `ViewModels/`
- Interfaces: abstraction contracts in `Interfaces/`
- Services: concrete infrastructure/data/API implementations in `Services/`
- Models: domain and API DTO models in `Models/`
- UI helpers: custom control in `Controls/` and converters in `Converters/`

## 5. App Startup and Navigation Flow

### Startup sequence

1. `MauiProgram.CreateMauiApp()` configures fonts, toolkit, DI registrations.
2. `App` constructor applies saved theme preference (`Preferences.Get("DarkMode", false)`).
3. `App` sets `MainPage = new AppShell()`.
4. `AppShell` registers named routes and exposes TabBar tabs.

### TabBar routes (`AppShell.xaml`)

- `HomePage`
- `FavoritesPage`
- `AddEditRecipePage`
- `ShoppingListPage`
- `ProfilePage`

### Additional registered routes (`AppShell.xaml.cs`)

- `LoginPage`
- `RegisterPage`
- `RecipeListPage`
- `RecipeDetailPage`
- `CategoriesPage`
- `AddEditRecipeDetail` (to `AddEditRecipePage`)

## 6. Dependency Injection Map

### Singleton services

- `HttpClient`
- `IAuthService -> AuthService`
- `INavigationService -> NavigationService`
- `IRecipeApiService -> RecipeApiService`
- `IDatabaseService -> FirestoreService` when Firebase config exists
- `IDatabaseService -> LocalDatabaseService` when Firebase config is missing

### Transient ViewModels

- `LoginViewModel`
- `RegisterViewModel`
- `HomeViewModel`
- `CategoriesViewModel`
- `RecipeListViewModel`
- `RecipeDetailViewModel`
- `AddEditRecipeViewModel`
- `FavoritesViewModel`
- `ShoppingListViewModel`
- `ProfileViewModel`

### Transient pages

- `LoginPage`
- `RegisterPage`
- `HomePage`
- `CategoriesPage`
- `RecipeListPage`
- `RecipeDetailPage`
- `AddEditRecipePage`
- `FavoritesPage`
- `ShoppingListPage`
- `ProfilePage`

## 7. Environment and Configuration

`Helpers/Constants.cs` supports environment-variable driven configuration.

### Required for Firebase features

- `FIREBASE_API_KEY`
- `FIREBASE_PROJECT_ID`

### Constant values used

- `FirebaseAuthUrl = https://identitytoolkit.googleapis.com/v1/accounts`
- `FirestoreBaseUrl = https://firestore.googleapis.com/v1/projects/{0}/databases/(default)/documents`
- `MealDbBaseUrl = https://www.themealdb.com/api/json/v1/1/`

### Firebase configuration gate

`Constants.HasFirebaseConfig` checks that both values are present and not placeholders (`YOUR_...`).

### Secure storage keys

- `auth_token`
- `refresh_token`
- `user_id`
- `user_email`

## 8. Build and Run

### Prerequisites

- .NET 8 SDK
- MAUI workload
- Platform SDKs as needed (Android/iOS/Windows/MacCatalyst)

### Restore workload and packages

```bash
dotnet workload restore
dotnet restore
```

### Build (Windows target)

```bash
dotnet build "Recipes app.csproj" -f net8.0-windows10.0.19041.0
```

### Run (Windows target)

```bash
dotnet run --project "Recipes app.csproj" -f net8.0-windows10.0.19041.0
```

### Target frameworks (`Recipes app.csproj`)

- `net8.0-android`
- `net8.0-ios`
- `net8.0-maccatalyst`
- `net8.0-windows10.0.19041.0` (Windows only condition)

## 9. Folder Structure

```text
Controls/        -> Custom UI controls
Converters/      -> XAML value converters
Helpers/         -> Constants and shared app config
Interfaces/      -> Service contracts
Models/          -> Domain + API models
Platforms/       -> Platform-specific MAUI code
Resources/       -> Styles, images, fonts, icons, splash
Services/        -> API and persistence implementations
ViewModels/      -> MVVM state and command logic
Views/           -> XAML pages and code-behind
```

## 10. Interfaces (Full Contract Reference)

### `IAuthService`

Properties:

- `bool IsSignedIn`
- `string? CurrentUserToken`
- `string? CurrentUserId`
- `string? CurrentUserEmail`

Methods:

- `Task<FirebaseAuthResponse> SignInAsync(string email, string password)`
   - Signs in via Firebase Auth REST (`signInWithPassword`).
- `Task<FirebaseAuthResponse> SignUpAsync(string email, string password, string displayName)`
   - Creates user and updates display name.
- `Task SendPasswordResetAsync(string email)`
   - Sends Firebase password reset email.
- `Task SignOutAsync()`
   - Clears in-memory and secure-stored tokens.
- `Task<bool> TryAutoLoginAsync()`
   - Restores prior session from secure storage.

### `INavigationService`

- `Task GoToAsync(string route)`
- `Task GoToAsync(string route, IDictionary<string, object> parameters)`
- `Task GoBackAsync()`

### `IRecipeApiService`

- `Task<List<Recipe>> SearchRecipesAsync(string query)`
- `Task<List<Recipe>> GetRecipesByCategoryAsync(string category)`
- `Task<Recipe?> GetRecipeByIdAsync(string id)`
- `Task<Recipe?> GetRandomRecipeAsync()`
- `Task<List<Category>> GetCategoriesAsync()`

### `IDatabaseService`

Recipes:

- `Task<List<Recipe>> GetUserRecipesAsync(string userId)`
- `Task<Recipe?> GetRecipeAsync(string recipeId)`
- `Task SaveRecipeAsync(Recipe recipe)`
- `Task UpdateRecipeAsync(Recipe recipe)`
- `Task DeleteRecipeAsync(string recipeId)`

Favorites:

- `Task<List<FavoriteRecipe>> GetFavoritesAsync(string userId)`
- `Task AddFavoriteAsync(FavoriteRecipe favorite)`
- `Task RemoveFavoriteAsync(string favoriteId)`
- `Task<bool> IsFavoriteAsync(string userId, string recipeId)`

Shopping list:

- `Task<List<ShoppingItem>> GetShoppingItemsAsync(string userId)`
- `Task AddShoppingItemAsync(ShoppingItem item)`
- `Task UpdateShoppingItemAsync(ShoppingItem item)`
- `Task DeleteShoppingItemAsync(string itemId)`
- `Task ClearCompletedShoppingItemsAsync(string userId)`

## 11. Services (Function-by-Function)

### `AuthService`

- `SignInAsync(email, password)`
   - Validates Firebase config.
   - POSTs to `:signInWithPassword`.
   - Deserializes response and stores tokens.
   - Throws on Firebase error payload.
- `SignUpAsync(email, password, displayName)`
   - POSTs to `:signUp`.
   - Calls private `UpdateProfileAsync(idToken, displayName)`.
   - Stores resulting tokens.
- `SendPasswordResetAsync(email)`
   - POSTs `requestType = PASSWORD_RESET` to `:sendOobCode`.
- `SignOutAsync()`
   - Clears local fields and removes secure storage entries.
- `TryAutoLoginAsync()`
   - Reads secure storage values and restores auth state when possible.

Private helpers:

- `UpdateProfileAsync(idToken, displayName)`
- `StoreTokensAsync(authResponse)`
- `EnsureFirebaseConfigured()`

### `NavigationService`

- `GoToAsync(route)` -> `Shell.Current.GoToAsync(route)`
- `GoToAsync(route, parameters)` -> `Shell.Current.GoToAsync(route, parameters)`
- `GoBackAsync()` -> `Shell.Current.GoToAsync("..")`

### `RecipeApiService`

- `SearchRecipesAsync(query)`
   - GET `search.php?s=...`, maps each `MealDbMeal` through `ToRecipe()`.
- `GetRecipesByCategoryAsync(category)`
   - GET `filter.php?c=...`, returns lightweight `Recipe` list.
- `GetRecipeByIdAsync(id)`
   - GET `lookup.php?i=...`, returns full single recipe.
- `GetRandomRecipeAsync()`
   - GET `random.php`, returns one recipe.
- `GetCategoriesAsync()`
   - GET `categories.php`, maps icon and color by category name.

Private helpers:

- `GetCategoryIcon(category)`
- `GetCategoryColor(category)`

### `FirestoreService`

General behavior:

- Uses authenticated `HttpRequestMessage` with bearer token from `IAuthService`.
- Uses Firestore REST document schema with `fields` typing wrappers.
- Uses `EnsureFirebaseConfigured()` guard before operations.

Recipes:

- `GetUserRecipesAsync(userId)`
   - Reads `/users/{userId}/recipes` documents and maps with `DocumentToRecipe`.
- `GetRecipeAsync(recipeId)`
   - Reads one document under authenticated user's recipes collection.
- `SaveRecipeAsync(recipe)`
   - POST create/overwrite with `documentId={recipe.Id}`.
- `UpdateRecipeAsync(recipe)`
   - PATCH update existing recipe document.
- `DeleteRecipeAsync(recipeId)`
   - DELETE recipe document.

Favorites:

- `GetFavoritesAsync(userId)`
   - Reads `/users/{userId}/favorites`.
- `AddFavoriteAsync(favorite)`
   - Writes favorite with `documentId={favorite.RecipeId}`.
- `RemoveFavoriteAsync(favoriteId)`
   - Deletes favorite by document id (typically recipe id).
- `IsFavoriteAsync(userId, recipeId)`
   - Loads favorites and checks membership.

Shopping list:

- `GetShoppingItemsAsync(userId)`
   - Reads `/users/{userId}/shoppingItems`.
- `AddShoppingItemAsync(item)`
   - Ensures `item.Id`, then POSTs document.
- `UpdateShoppingItemAsync(item)`
   - PATCH updates one item document.
- `DeleteShoppingItemAsync(itemId)`
   - DELETE one item document.
- `ClearCompletedShoppingItemsAsync(userId)`
   - Loads items then deletes checked items.

Firestore parsing/serialization helpers:

- `CreateAuthorizedRequest(...)`
- `GetStringField(...)`
- `GetIntField(...)`
- `GetDoubleField(...)`
- `GetBoolField(...)`
- `DocumentToRecipe(...)`
- `RecipeToDocument(...)`
- `DocumentToFavorite(...)`
- `FavoriteToDocument(...)`
- `DocumentToShoppingItem(...)`
- `ShoppingItemToDocument(...)`
- `EnsureFirebaseConfigured()`

### `LocalDatabaseService`

Purpose:

- In-memory fallback when Firebase is not configured.
- Data is process-memory only and resets when app closes.

Method behavior:

- Implements all `IDatabaseService` methods with `List<T>` operations.
- `DeleteRecipeAsync` also removes related favorites and shopping items.
- `GetShoppingItemsAsync` currently returns all items (model has no `UserId`).

## 12. ViewModels (Properties, Commands, Methods)

All ViewModels inherit `BaseViewModel`.

### `BaseViewModel`

Observable properties:

- `IsBusy`
- `Title`
- `IsRefreshing`

Computed property:

- `IsNotBusy => !IsBusy`

Method:

- `OnIsBusyChanged(bool value)` notifies `IsNotBusy`.

### `LoginViewModel`

Observable properties:

- `Email`, `Password`, `RememberMe`, `ErrorMessage`, `HasError`

Commands:

- `LoginCommand` (`LoginAsync`)
   - Validates fields, calls `SignInAsync`, navigates to `//HomePage`.
- `GoToRegisterCommand` (`GoToRegisterAsync`)
   - Navigates to register page.
- `ForgotPasswordCommand` (`ForgotPasswordAsync`)
   - Triggers password reset for entered email.

### `RegisterViewModel`

Observable properties:

- `DisplayName`, `Email`, `Password`, `ConfirmPassword`, `ErrorMessage`, `HasError`

Commands:

- `RegisterCommand` (`RegisterAsync`)
   - Validates name/email/password/confirmation.
   - Calls `SignUpAsync` and navigates to `//HomePage`.
- `GoToLoginCommand` (`GoToLoginAsync`)
   - Navigates back.

### `HomeViewModel`

Observable properties:

- `FeaturedRecipes`
- `Categories`
- `RecentRecipes`
- `SearchText`

Commands:

- `LoadDataCommand` (`LoadDataAsync`)
   - Loads categories (top 8), random featured recipes (5), and chicken recipes (6).
- `SearchCommand` (`SearchAsync`)
   - Opens list page with `SearchQuery` parameter.
- `GoToRecipeDetailCommand` (`GoToRecipeDetailAsync`)
   - Opens recipe details for selected recipe.
- `GoToCategoryCommand` (`GoToCategoryAsync`)
   - Opens recipe list filtered by category.
- `GoToAllCategoriesCommand` (`GoToAllCategoriesAsync`)
   - Opens categories page.

### `CategoriesViewModel`

Observable properties:

- `Categories`

Commands:

- `LoadCategoriesCommand` (`LoadCategoriesAsync`)
   - Loads all categories.
- `SelectCategoryCommand` (`SelectCategoryAsync`)
   - Opens recipe list filtered by selected category.

### `RecipeListViewModel`

Query-bound properties:

- `SearchQuery` from shell query key `SearchQuery`
- `CategoryFilter` from shell query key `Category`

Observable properties:

- `Recipes`
- `SearchQuery`
- `CategoryFilter`
- `IsEmptyState`

Generated partial change handlers:

- `OnSearchQueryChanged(string value)` triggers `PerformSearchAsync(value)`.
- `OnCategoryFilterChanged(string value)` triggers `LoadByCategoryAsync(value)`.

Commands:

- `LoadRecipesCommand` (`LoadRecipesAsync`)
   - Loads by category, search term, or default `chicken`.
- `SearchCommand` (`SearchAsync`)
   - Clears category and re-runs search.
- `GoToRecipeDetailCommand` (`GoToRecipeDetailAsync`)
   - Fetches full recipe details if list item is lightweight then navigates.

Private methods:

- `PerformSearchAsync(query)`
- `LoadByCategoryAsync(category)`

### `RecipeDetailViewModel`

Query-bound property:

- `Recipe` from shell query key `Recipe`

Observable properties:

- `Recipe`
- `Ingredients`
- `Instructions`
- `IsFavorite`
- `ServingsAdjusted`

Generated partial change handler:

- `OnRecipeChanged(Recipe value)`
   - Sets page title, servings, ingredient list, instruction list.
   - Triggers favorite status check.

Commands:

- `ToggleFavoriteCommand` (`ToggleFavoriteAsync`)
   - Adds/removes favorite and syncs recipe data to cloud when adding.
- `AddToShoppingListCommand` (`AddToShoppingListAsync`)
   - Converts all ingredients into shopping items and saves them.
- `GoBackCommand` (`GoBackAsync`)
   - Navigates back.
- `IncrementServingsCommand` (`IncrementServings`)
- `DecrementServingsCommand` (`DecrementServings`)

Private methods:

- `CheckFavoriteStatusAsync()`

### `AddEditRecipeViewModel`

Query-bound property:

- `EditRecipe` from shell query key `Recipe`

Observable properties:

- `EditRecipe`
- `RecipeName`
- `RecipeDescription`
- `RecipeImageUrl`
- `SelectedCategory`
- `CookTime`
- `PrepTime`
- `Servings`
- `Ingredients`
- `InstructionSteps`
- `NewIngredientName`
- `NewIngredientQuantity`
- `NewIngredientUnit`
- `NewInstructionStep`
- `ErrorMessage`
- `HasError`

Other properties:

- `CategoryOptions` (static list of category labels)

Generated partial change handler:

- `OnEditRecipeChanged(Recipe? value)`
   - Switches to edit mode and pre-populates fields/lists.

Commands:

- `AddIngredientCommand` (`AddIngredient`)
- `RemoveIngredientCommand` (`RemoveIngredient`)
- `AddInstructionCommand` (`AddInstruction`)
- `RemoveInstructionCommand` (`RemoveInstruction`)
- `SaveRecipeCommand` (`SaveRecipeAsync`)
   - Validates required fields and list content.
   - Serializes ingredients/instructions to JSON.
   - Creates or updates recipe in cloud DB when signed in.
- `CancelCommand` (`CancelAsync`)
   - Navigates back.

### `FavoritesViewModel`

Observable properties:

- `FavoriteRecipes`
- `IsEmptyState`

Commands:

- `LoadFavoritesCommand` (`LoadFavoritesAsync`)
   - Loads favorite records then resolves full recipes.
- `RemoveFavoriteCommand` (`RemoveFavoriteAsync`)
   - Removes favorite by recipe id and updates collection.
- `GoToRecipeDetailCommand` (`GoToRecipeDetailAsync`)
   - Opens recipe detail.

### `ShoppingListViewModel`

Observable properties:

- `ShoppingItems`
- `IsEmptyState`
- `CheckedCount`
- `TotalCount`

Commands:

- `LoadItemsCommand` (`LoadItemsAsync`)
   - Loads shopping list and updates counts.
- `ToggleItemCommand` (`ToggleItemAsync`)
   - Flips checked state, saves update, refreshes item in collection.
- `DeleteItemCommand` (`DeleteItemAsync`)
   - Deletes one item.
- `ClearCompletedCommand` (`ClearCompletedAsync`)
   - Deletes checked items.
- `ClearAllCommand` (`ClearAllAsync`)
   - Confirms and deletes all items.

Private method:

- `UpdateCounts()`

### `ProfileViewModel`

Observable properties:

- `DisplayName`
- `Email`
- `IsDarkMode`
- `MemberSince`
- `IsLoggedIn`

Commands:

- `LoadProfileCommand` (`LoadProfileAsync`)
   - Loads auth user details and dark mode preference.
- `LogoutCommand` (`LogoutAsync`)
   - Confirms, signs out, navigates to login page.
- `GoToLoginCommand` (`GoToLoginAsync`)
   - Navigates to login page.
- `EditProfileCommand` (`EditProfileAsync`)
   - Placeholder alert.

Generated partial change handler:

- `OnIsDarkModeChanged(bool value)`
   - Persists preference and updates `Application.Current.UserAppTheme`.

## 13. Models (Data Contract Reference)

### `Recipe`

- `Id`
- `Title`
- `Description`
- `ImageUrl`
- `Category`
- `IngredientsJson`
- `InstructionsJson`
- `CookTimeMinutes`
- `PrepTimeMinutes`
- `Servings`
- `Rating`
- `CreatedBy`
- `CreatedAt`
- `IsFromApi`
- `Ingredients` (runtime parsed)
- `Instructions` (runtime parsed)

### `Ingredient`

- `Name`
- `Quantity`
- `Unit`
- `ToString()` formats output based on available quantity/unit.

### `Category`

- `Id`
- `Name`
- `Icon`
- `Color`
- `ImageUrl`
- `RecipeCount`

### `FavoriteRecipe`

- `Id`
- `RecipeId`
- `UserId`
- `SavedAt`

### `ShoppingItem`

- `Id`
- `Name`
- `Quantity`
- `Unit`
- `IsChecked`
- `RecipeId`
- `RecipeTitle`

### `UserProfile`

- `UserId`
- `DisplayName`
- `Email`
- `AvatarUrl`
- `JoinedDate`

### Firebase auth DTOs

- `FirebaseAuthResponse`: `IdToken`, `Email`, `RefreshToken`, `ExpiresIn`, `LocalId`, `Registered`, `DisplayName`
- `FirebaseErrorResponse`: `Error`
- `FirebaseError`: `Code`, `Message`

### TheMealDB DTOs

- `MealDbResponse`
- `MealDbCategoriesResponse`
- `MealDbCategory`
- `MealDbFilterResponse`
- `MealDbFilterItem`
- `MealDbMeal` with `ToRecipe()` conversion method

## 14. Controls and Converters

### `RatingStarsControl`

Custom control inheriting `HorizontalStackLayout`.

Bindable properties:

- `Rating` (double)
- `MaxStars` (int)
- `StarSize` (double)
- `StarColor` (Color)
- `EmptyStarColor` (Color)

Methods:

- `OnRatingChanged(...)` static property changed callback
- `UpdateStars()` renders full/partial/empty stars plus numeric value

### `FavoriteIconConverter`

- `Convert(...)`: bool -> heart icon
- `ConvertBack(...)`: heart icon -> bool

### `InverseBoolConverter`

- `Convert(...)`: negates bool
- `ConvertBack(...)`: negates bool

### `StringNotEmptyConverter`

- `Convert(...)`: returns true when string is non-empty/non-whitespace
- `ConvertBack(...)`: returns placeholder string when true, empty otherwise

## 15. Views and Code-Behind Responsibilities

Code-behind remains intentionally minimal.

- Constructor injection sets `BindingContext` to ViewModel.
- Some pages run initial load commands in `OnAppearing`.

`OnAppearing` command triggers:

- `HomePage` -> `LoadDataCommand` (if not loaded)
- `CategoriesPage` -> `LoadCategoriesCommand` (if collection empty)
- `RecipeListPage` -> `LoadRecipesCommand`
- `FavoritesPage` -> `LoadFavoritesCommand`
- `ShoppingListPage` -> `LoadItemsCommand`
- `ProfilePage` -> `LoadProfileCommand`

Pages without additional code-behind behavior:

- `LoginPage`
- `RegisterPage`
- `RecipeDetailPage`
- `AddEditRecipePage`

## 16. External API Reference

### TheMealDB

Base URL: `https://www.themealdb.com/api/json/v1/1/`

Used endpoints:

- `GET search.php?s={query}`
- `GET filter.php?c={category}`
- `GET lookup.php?i={id}`
- `GET random.php`
- `GET categories.php`

### Firebase Authentication

Base URL: `https://identitytoolkit.googleapis.com/v1/accounts`

Used endpoints:

- `POST :signInWithPassword?key=...`
- `POST :signUp?key=...`
- `POST :sendOobCode?key=...`
- `POST :update?key=...`

### Firebase Firestore

Base URL format:

`https://firestore.googleapis.com/v1/projects/{projectId}/databases/(default)/documents`

Used collections:

- `/users/{userId}/recipes`
- `/users/{userId}/favorites`
- `/users/{userId}/shoppingItems`

## 17. Data Storage Behavior

### Cloud mode (Firebase configured)

- Auth state uses secure storage keys from `Constants`.
- Recipe/favorite/shopping list operations use Firestore REST.

### Local fallback mode (Firebase not configured)

- `IDatabaseService` resolves to `LocalDatabaseService`.
- Data stored in memory lists only.
- Data resets on app restart.

## 18. Error Handling Strategy

- Services generally catch exceptions and return safe fallback values where possible.
- Auth methods surface Firebase error messages for UI display.
- ViewModels present user-facing alerts/messages for operation failures.
- `IsBusy` and `IsRefreshing` patterns protect from duplicate operations.

## 19. Known Limitations

- `RememberMe` exists in `LoginViewModel` but is not currently used in logic.
- `UserProfile` model is present but profile editing is currently placeholder-only.
- Local shopping list mode is not user-scoped (no `UserId` field in `ShoppingItem`).
- Firestore document writes do not currently enforce transaction semantics.
- Auto-login method (`TryAutoLoginAsync`) exists but startup flow currently sets `AppShell` directly.

## 20. Coursework Submission Notes

For final module submission, include:

- A PDF containing:
   - YouTube link to an approximately 8-minute demonstration video
   - Coventry GitHub repository link (in the 6004CMD organization)

If repository-link inclusion is mandated by the brief, ensure that requirement is satisfied before final submission.
