# FlavorVault - Recipe App: Comprehensive Project Plan

## App Concept
**FlavorVault** is a feature-rich recipe management application built with .NET MAUI and C#. Users can discover, create, save, and organize recipes. The app includes cloud authentication, cloud database storage, external API integration, local caching, and a polished UI — targeting the **Outstanding (100%)** grade across every rubric criterion.

---

## 1. RUBRIC MAPPING & TARGET GRADES

| Rubric Criterion | Weight | Target | Key Requirements |
|---|---|---|---|
| **UI Usage** | 20% | Outstanding (100) | Comprehensive UI complexity, custom control, polished styling |
| **Version Control** | 15% | Outstanding (100) | 12+ commits, main + dev branches, 2+ feature branches, 2+ merges, 2+ weeks history |
| **MVVM Integration** | 25% | Outstanding (100) | 8+ ViewModels, base ViewModel (ObservableObject), zero code-behind |
| **Software Design Patterns** | 10% | Outstanding (100) | 4+ design patterns |
| **Separation of Concerns** | 10% | Outstanding (100) | Models/ViewModels/Services/Interfaces + extra folders (Converters, Controls, Helpers) |
| **Database Integration** | 10% | Outstanding (100) | All CRUD operations, cloud integration |
| **Cloud Integration** | 10% | Outstanding (100) | 2+ external APIs (auth + database/public API), 4+ endpoints |

---

## 2. COLOR THEME (Food/Recipe Warm Palette)

### Light Mode
| Key | Color | Usage |
|---|---|---|
| **Primary** | `#E85D2C` | Warm orange - buttons, headers, accent |
| **PrimaryDark** | `#C94A1F` | Darker orange - pressed states |
| **Secondary** | `#2E7D32` | Fresh green - success, categories |
| **SecondaryLight** | `#A5D6A7` | Light green - tags, badges |
| **Tertiary** | `#F9A825` | Golden yellow - ratings, stars |
| **Background** | `#FFF8F0` | Warm off-white - page background |
| **Surface** | `#FFFFFF` | White - cards, containers |
| **SurfaceVariant** | `#FFF0E6` | Light peach - alternate cards |
| **OnPrimary** | `#FFFFFF` | White text on primary |
| **OnBackground** | `#2C1810` | Dark brown - body text |
| **OnSurface** | `#3E2723` | Dark brown - card text |
| **Outline** | `#D7CCC8` | Light brown/gray - borders |

### Dark Mode
| Key | Color | Usage |
|---|---|---|
| **Primary** | `#FF8A65` | Soft orange |
| **PrimaryDark** | `#E85D2C` | Medium orange |
| **Secondary** | `#66BB6A` | Medium green |
| **Background** | `#1A1210` | Very dark brown |
| **Surface** | `#2C2220` | Dark brown - cards |
| **OnPrimary** | `#1A1210` | Dark text on primary |
| **OnBackground** | `#F5E6D8` | Light cream - body text |
| **OnSurface** | `#EFEBE9` | Light - card text |

---

## 3. APP ARCHITECTURE & FOLDER STRUCTURE

```
Recipes app/
├── Controls/                  # Custom controls (e.g., RatingStarsControl, RecipeCard)
├── Converters/                # Value converters (BoolToColor, InverseBool, etc.)
├── Helpers/                   # Utility/helper classes
├── Interfaces/                # Service interfaces (contracts)
│   ├── IAuthService.cs
│   ├── IRecipeService.cs
│   ├── IDatabaseService.cs
│   ├── INavigationService.cs
│   ├── IFavoriteService.cs
│   ├── IShoppingListService.cs
│   └── IApiService.cs
├── Models/                    # Data models / entities
│   ├── Recipe.cs
│   ├── Ingredient.cs
│   ├── Category.cs
│   ├── UserProfile.cs
│   ├── ShoppingItem.cs
│   └── FavoriteRecipe.cs
├── Services/                  # Service implementations
│   ├── AuthService.cs         (Firebase Auth)
│   ├── FirestoreService.cs    (Firebase Firestore - cloud DB)
│   ├── LocalDatabaseService.cs (SQLite - local cache)
│   ├── RecipeApiService.cs    (TheMealDB / Spoonacular API)
│   ├── NavigationService.cs
│   ├── FavoriteService.cs
│   └── ShoppingListService.cs
├── ViewModels/                # All view models
│   ├── BaseViewModel.cs       (inherits ObservableObject - DRY)
│   ├── LoginViewModel.cs
│   ├── RegisterViewModel.cs
│   ├── HomeViewModel.cs
│   ├── RecipeListViewModel.cs
│   ├── RecipeDetailViewModel.cs
│   ├── AddEditRecipeViewModel.cs
│   ├── FavoritesViewModel.cs
│   ├── ProfileViewModel.cs
│   ├── ShoppingListViewModel.cs
│   └── CategoriesViewModel.cs
├── Views/                     # XAML pages (zero code-behind logic)
│   ├── LoginPage.xaml
│   ├── RegisterPage.xaml
│   ├── HomePage.xaml
│   ├── RecipeListPage.xaml
│   ├── RecipeDetailPage.xaml
│   ├── AddEditRecipePage.xaml
│   ├── FavoritesPage.xaml
│   ├── ProfilePage.xaml
│   ├── ShoppingListPage.xaml
│   └── CategoriesPage.xaml
├── Resources/
│   ├── Styles/
│   │   ├── Colors.xaml        (custom recipe-themed palette)
│   │   └── Styles.xaml        (global styles)
│   ├── Fonts/
│   ├── Images/
│   └── Raw/
├── Platforms/
├── App.xaml
├── AppShell.xaml
└── MauiProgram.cs             (DI registration)
```

---

## 4. PAGES & VIEWMODELS (10 Pages, 11 ViewModels → exceeds 8 minimum)

### Page Details

| # | Page | ViewModel | Description |
|---|---|---|---|
| 1 | **LoginPage** | LoginViewModel | Email/password login via Firebase Auth, "Remember Me", navigate to Register |
| 2 | **RegisterPage** | RegisterViewModel | Create account with Firebase Auth, input validation |
| 3 | **HomePage** | HomeViewModel | Dashboard: featured recipes carousel, recipe categories grid, search bar, recent recipes |
| 4 | **CategoriesPage** | CategoriesViewModel | Browse all categories (Breakfast, Lunch, Dinner, Dessert, Vegan, etc.) with icons |
| 5 | **RecipeListPage** | RecipeListViewModel | Filterable/searchable list of recipes from API + user-created, pull-to-refresh |
| 6 | **RecipeDetailPage** | RecipeDetailViewModel | Full recipe view: image, title, rating, cook time, ingredients list, step-by-step instructions, add-to-favorites, add-ingredients-to-shopping-list |
| 7 | **AddEditRecipePage** | AddEditRecipeViewModel | Create/edit recipe form: image picker, title, description, category picker, ingredients (dynamic list), instructions, cook time, servings |
| 8 | **FavoritesPage** | FavoritesViewModel | List of saved/favorite recipes with remove option |
| 9 | **ShoppingListPage** | ShoppingListViewModel | Shopping list from recipe ingredients, check off items, clear completed |
| 10 | **ProfilePage** | ProfileViewModel | User info, app settings (dark mode toggle), logout, account management |

### BaseViewModel (DRY Principle)
```csharp
// Uses CommunityToolkit.Mvvm
public partial class BaseViewModel : ObservableObject
{
    [ObservableProperty] bool isBusy;
    [ObservableProperty] string title;
    [ObservableProperty] bool isRefreshing;

    public bool IsNotBusy => !IsBusy;
}
```

---

## 5. MODELS

```csharp
public class Recipe
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string ImageUrl { get; set; }
    public string Category { get; set; }
    public List<Ingredient> Ingredients { get; set; }
    public List<string> Instructions { get; set; }
    public int CookTimeMinutes { get; set; }
    public int PrepTimeMinutes { get; set; }
    public int Servings { get; set; }
    public double Rating { get; set; }
    public string CreatedBy { get; set; }       // User ID
    public DateTime CreatedAt { get; set; }
    public bool IsFromApi { get; set; }         // true if from external API
}

public class Ingredient
{
    public string Name { get; set; }
    public string Quantity { get; set; }
    public string Unit { get; set; }
}

public class Category
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Icon { get; set; }
    public string Color { get; set; }
}

public class UserProfile
{
    public string UserId { get; set; }
    public string DisplayName { get; set; }
    public string Email { get; set; }
    public string AvatarUrl { get; set; }
    public DateTime JoinedDate { get; set; }
}

public class ShoppingItem
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Quantity { get; set; }
    public string Unit { get; set; }
    public bool IsChecked { get; set; }
    public string RecipeId { get; set; }    // which recipe it came from
}

public class FavoriteRecipe
{
    public string Id { get; set; }
    public string RecipeId { get; set; }
    public string UserId { get; set; }
    public DateTime SavedAt { get; set; }
}
```

---

## 6. SERVICES & INTERFACES

| Interface | Implementation | Purpose |
|---|---|---|
| `IAuthService` | `AuthService` | Firebase Authentication (login, register, logout, get current user) |
| `IRecipeService` | `RecipeApiService` | External recipe API calls (TheMealDB free API) |
| `IDatabaseService` | `FirestoreService` | Cloud Firestore for user recipes, favorites, shopping list |
| `ILocalDatabaseService` | `LocalDatabaseService` | SQLite local cache for offline access |
| `INavigationService` | `NavigationService` | Shell navigation abstraction |
| `IFavoriteService` | `FavoriteService` | Manage favorite recipes (uses IDatabaseService) |
| `IShoppingListService` | `ShoppingListService` | Manage shopping list items |

---

## 7. SOFTWARE DESIGN PATTERNS (6 patterns → exceeds 4 minimum)

| # | Pattern | Where Used |
|---|---|---|
| 1 | **MVVM** | Entire app architecture - Views bind to ViewModels |
| 2 | **Repository Pattern** | Services abstract data access (IRecipeService, IDatabaseService) |
| 3 | **Dependency Injection** | MauiProgram.cs registers all services; constructor injection throughout |
| 4 | **Singleton Pattern** | Services registered as singletons in DI container |
| 5 | **Observer Pattern** | INotifyPropertyChanged via ObservableObject, ObservableCollection for lists |
| 6 | **Strategy Pattern** | Different data sources (API vs local DB vs cloud DB) behind same interface |

---

## 8. DATABASE INTEGRATION

### Local Database (SQLite)
- **NuGet Package**: `sqlite-net-pcl`
- **Purpose**: Offline cache for recipes, shopping list persistence
- **CRUD Operations**:
  - **C**reate: Save new recipe locally, add shopping item
  - **R**ead: Get all cached recipes, get shopping list
  - **U**pdate: Edit recipe, toggle shopping item checked state
  - **D**elete: Remove cached recipe, clear shopping list items

### Cloud Database (Firebase Firestore)
- **NuGet Package**: Custom REST API wrapper or `Plugin.CloudFirestore` alternative
- **Purpose**: Cloud sync for user-created recipes, favorites, user profile
- **CRUD Operations**:
  - **C**reate: Save user recipe to cloud, add favorite
  - **R**ead: Fetch user's recipes, fetch favorites list
  - **U**pdate: Edit cloud recipe, update user profile
  - **D**elete: Remove recipe from cloud, remove favorite

---

## 9. CLOUD INTEGRATION (3 APIs → exceeds 2 minimum)

### API 1: Firebase Authentication (Required - Auth/Authorization)
- **Endpoints**:
  1. `POST signInWithPassword` — Login with email/password
  2. `POST signUp` — Register new account
  3. `POST sendPasswordResetEmail` — Password reset
  4. `GET getAccountInfo` — Get current user details
- **Features**: Token management, secure storage of credentials

### API 2: Firebase Firestore (Cloud Database)
- **Endpoints**:
  1. `POST /documents` — Create recipe/favorite/shopping item
  2. `GET /documents/recipes` — Fetch user recipes
  3. `PATCH /documents/recipes/{id}` — Update recipe
  4. `DELETE /documents/recipes/{id}` — Delete recipe
  5. `GET /documents/favorites` — Fetch favorites
- **Features**: Real-time data, user-scoped collections

### API 3: TheMealDB API (Free Public API)
- **Base URL**: `https://www.themealdb.com/api/json/v1/1/`
- **Endpoints**:
  1. `search.php?s={name}` — Search recipes by name
  2. `filter.php?c={category}` — Filter by category
  3. `lookup.php?i={id}` — Get recipe details by ID
  4. `random.php` — Get random recipe (for featured section)
  5. `categories.php` — List all categories
- **Features**: Free, no API key for basic tier, rich recipe data with images

### Total Working Endpoints: 12+ (exceeds 4 minimum)

---

## 10. UI COMPLEXITY & CUSTOM CONTROLS

### UI Elements Used
- `CollectionView` with custom `DataTemplate` (recipe cards)
- `CarouselView` (featured recipes on home page)
- `SearchBar` with real-time filtering
- `RefreshView` (pull-to-refresh)
- `TabBar` with `Shell` navigation (bottom tabs)
- `SwipeView` (swipe to delete/favorite)
- `Border` with rounded corners for cards
- `Shadow` effects on cards
- `Picker` for category selection
- `Stepper` for servings adjustment
- `ActivityIndicator` for loading states
- `Frame` / `Border` styled cards
- `Image` with aspect fill for recipe photos
- `Grid` complex layouts
- `FlexLayout` for tags/badges
- Dark mode support via `AppThemeBinding`
- Custom fonts (Google Fonts)
- Animations (fade in, scale)

### Custom Control: RatingStarsControl
A reusable star-rating control:
```xml
<controls:RatingStarsControl Rating="{Binding Recipe.Rating}" MaxStars="5" StarSize="24" />
```
- Displays 1-5 stars (filled/half/empty)
- Bindable `Rating` property
- Customizable star color and size
- Reusable across RecipeDetailPage and RecipeCard

### Custom Control: RecipeCardView (Optional bonus)
A reusable recipe card component with image, title, category badge, cook time, and rating.

---

## 11. NUGET PACKAGES REQUIRED

| Package | Purpose |
|---|---|
| `CommunityToolkit.Mvvm` | ObservableObject, RelayCommand, source generators |
| `CommunityToolkit.Maui` | UI helpers, converters, behaviors |
| `sqlite-net-pcl` | Local SQLite database |
| `SQLitePCLRaw.bundle_green` | SQLite native bindings |
| `Newtonsoft.Json` or `System.Text.Json` | JSON deserialization for API responses |
| `Microsoft.Maui.Controls` | Core MAUI (already included) |

---

## 12. VERSION CONTROL STRATEGY

### Branch Structure
```
main (release)
 └── development
      ├── feature/authentication
      ├── feature/recipe-browsing
      ├── feature/favorites-shopping
      ├── feature/cloud-integration
      └── feature/ui-polish
```

### Commit Strategy (Target: 15-20+ commits over 3-4 weeks)
| Week | Commits | Description |
|---|---|---|
| **Week 1** | 3-4 commits | Project setup, folder structure, models, base ViewModel, color theme |
| **Week 2** | 4-5 commits | Views + ViewModels (Login, Register, Home, RecipeList, RecipeDetail) |
| **Week 3** | 4-5 commits | Cloud integration (Firebase Auth, Firestore, TheMealDB API), database |
| **Week 4** | 4-5 commits | Remaining pages (Favorites, Shopping, Profile), custom controls, polish |

### Merge Strategy
1. `feature/authentication` → `development` (PR merge)
2. `feature/recipe-browsing` → `development` (PR merge)
3. `feature/favorites-shopping` → `development` (PR merge)
4. `feature/cloud-integration` → `development` (PR merge)
5. `development` → `main` (release merge at milestones)

---

## 13. DETAILED IMPLEMENTATION ROADMAP

### Phase 1: Foundation (Week 1)
- [ ] Create folder structure (Models, ViewModels, Views, Services, Interfaces, Controls, Converters, Helpers)
- [ ] Set up color theme in `Colors.xaml`
- [ ] Install NuGet packages (CommunityToolkit.Mvvm, CommunityToolkit.Maui, sqlite-net-pcl)
- [ ] Create `BaseViewModel` with ObservableObject
- [ ] Define all Model classes (Recipe, Ingredient, Category, UserProfile, ShoppingItem, FavoriteRecipe)
- [ ] Define all Service interfaces
- [ ] Set up `AppShell.xaml` with TabBar navigation
- [ ] Register services in `MauiProgram.cs` with DI
- [ ] Initialize Git: `main` → `development` branch → `feature/authentication` branch
- [ ] Commits: initial setup, models, base architecture

### Phase 2: Core UI & MVVM (Week 2)
- [ ] Build `LoginPage` + `LoginViewModel`
- [ ] Build `RegisterPage` + `RegisterViewModel`
- [ ] Build `HomePage` + `HomeViewModel` (featured carousel, categories grid, search)
- [ ] Build `RecipeListPage` + `RecipeListViewModel` (CollectionView with recipe cards)
- [ ] Build `RecipeDetailPage` + `RecipeDetailViewModel` (full recipe view)
- [ ] Build `CategoriesPage` + `CategoriesViewModel`
- [ ] Create `RatingStarsControl` custom control
- [ ] Create value converters (BoolToColorConverter, InverseBoolConverter, CookTimeFormatConverter)
- [ ] Merge `feature/authentication` → `development`
- [ ] Commits: login/register UI, home page, recipe pages

### Phase 3: Cloud & Database (Week 3)
- [ ] Implement `AuthService` (Firebase REST API authentication)
- [ ] Implement `RecipeApiService` (TheMealDB API integration)
- [ ] Implement `FirestoreService` (Firebase Firestore REST API)
- [ ] Implement `LocalDatabaseService` (SQLite local cache)
- [ ] Wire up services to ViewModels
- [ ] Build `AddEditRecipePage` + `AddEditRecipeViewModel` (CRUD for user recipes)
- [ ] Test all CRUD operations (local + cloud)
- [ ] Merge `feature/recipe-browsing` → `development`
- [ ] Merge `feature/cloud-integration` → `development`
- [ ] Commits: Firebase auth, API integration, database, CRUD

### Phase 4: Remaining Features & Polish (Week 4)
- [ ] Build `FavoritesPage` + `FavoritesViewModel`
- [ ] Build `ShoppingListPage` + `ShoppingListViewModel`
- [ ] Build `ProfilePage` + `ProfileViewModel` (settings, dark mode, logout)
- [ ] Polish UI: animations, loading states, error handling, empty states
- [ ] Ensure ALL code-behind is empty (only InitializeComponent + BindingContext)
- [ ] Final testing on Windows (and Android emulator if possible)
- [ ] Merge `feature/favorites-shopping` → `development`
- [ ] Merge `development` → `main` (release)
- [ ] Commits: favorites, shopping list, profile, final polish

### Phase 5: Submission Prep (Final Days)
- [ ] Verify README (app name + background/motivation, under 700 words)
- [ ] Record 8-minute demo video (voice + all screens + cloud evidence)
- [ ] Upload video to YouTube
- [ ] Final commit and push to GitHub
- [ ] Create PDF with: YouTube link + GitHub repository link
- [ ] Include cloud provider dashboard screenshots in video

---

## 14. SECURITY CONSIDERATIONS (Module Learning Outcome 3)
- Store Firebase API keys in a separate config (not hardcoded)
- Use `SecureStorage` for storing auth tokens
- Implement input validation on all forms
- HTTPS for all API calls (Firebase and TheMealDB use HTTPS)
- User data scoped by authenticated user ID in Firestore
- Implement token refresh for Firebase Auth

---

## 15. CROSS-PLATFORM USABILITY (Module Learning Outcome 4)
- Use Shell navigation for consistent UX across platforms
- Responsive layouts with Grid/FlexLayout
- Platform-specific adjustments via `OnPlatform` where needed
- Test on Windows and Android

---

## 16. KEY CHECKLIST FOR OUTSTANDING GRADE

### UI Usage (20%) ✅
- [ ] 10+ different UI controls used
- [ ] Complex layouts (Grid, CollectionView, CarouselView)
- [ ] Custom `RatingStarsControl`
- [ ] Consistent styling and color theme
- [ ] Dark mode support
- [ ] Animations and visual polish

### Version Control (15%) ✅
- [ ] README with app name + motivation (< 700 words)
- [ ] 15+ commits spaced over 3-4 weeks
- [ ] `main` + `development` branches
- [ ] 2+ feature branches
- [ ] 2+ merges from feature→development→main

### MVVM Integration (25%) ✅
- [ ] 11 ViewModels (exceeds 8 minimum)
- [ ] `BaseViewModel` using `ObservableObject`
- [ ] Zero code-behind (only InitializeComponent)
- [ ] All logic in ViewModels via Commands and data binding

### Design Patterns (10%) ✅
- [ ] MVVM
- [ ] Repository Pattern
- [ ] Dependency Injection
- [ ] Singleton Pattern
- [ ] Observer Pattern
- [ ] Strategy Pattern (6 total, exceeds 4 minimum)

### Separation of Concerns (10%) ✅
- [ ] Models/ folder
- [ ] ViewModels/ folder
- [ ] Views/ folder (or pages in Views)
- [ ] Services/ folder
- [ ] Interfaces/ folder
- [ ] Converters/ folder
- [ ] Controls/ folder
- [ ] Helpers/ folder

### Database Integration (10%) ✅
- [ ] SQLite local database with full CRUD
- [ ] Firebase Firestore cloud database with full CRUD
- [ ] All 4 CRUD operations demonstrated

### Cloud Integration (10%) ✅
- [ ] Firebase Authentication (auth/authorization)
- [ ] Firebase Firestore (cloud database)
- [ ] TheMealDB API (public API)
- [ ] 12+ endpoints total (exceeds 4 minimum)
- [ ] Dashboard evidence in video

---

## 17. VIDEO DEMO SCRIPT (8 minutes)

| Time | Content |
|---|---|
| 0:00-0:30 | Introduction: app name, purpose, tech stack |
| 0:30-1:30 | Login/Register screens + Firebase Auth demo |
| 1:30-3:00 | Home page: featured recipes, categories, search |
| 3:00-4:00 | Browse recipes from TheMealDB API |
| 4:00-5:00 | Recipe detail page with rating, ingredients |
| 5:00-5:45 | Create/Edit recipe (CRUD demo) |
| 5:45-6:15 | Favorites management |
| 6:15-6:45 | Shopping list functionality |
| 6:45-7:15 | Profile page, dark mode toggle |
| 7:15-7:45 | Show Firebase Console/dashboard evidence |
| 7:45-8:00 | Summary and conclusion |

---

*This plan targets the Outstanding (100%) mark in every rubric category. Follow the implementation roadmap phase by phase, committing regularly to demonstrate consistent development progress.*
