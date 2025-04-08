# WhatsMyRecipe 🍳


A modern recipe management application that helps you organize, create, and share your favorite culinary creations with beautiful visuals and intuitive controls.

![HomeView](https://github.com/user-attachments/assets/db9ff0a4-ab08-4f20-b369-2f95f49e9660)

## Technologies Used 🛠️

- **Backend**: ASP.NET Core 8.0
- **Frontend**: Bootstrap 5, Font Awesome
- **Database**: Entity Framework Core
- **Authentication**: ASP.NET Core Identity
- **Hosting**: Ready for Azure/Windows/Linux deployment
- **Other**: jQuery Validation, HTML5, CSS3

## Features Overview ✨

### 🔐 Secure Authentication
login and register pages with full validation.

![Login](https://github.com/user-attachments/assets/5e2995b5-0edd-4790-a1bc-e655c6b86d0a)
![Register](https://github.com/user-attachments/assets/147c0f04-ee9a-4975-9131-2e96688b901d)

### 🗂️ Category Management
Organize recipes into intuitive categories with cover images.

![IndexForCategory](https://github.com/user-attachments/assets/0f5fcbad-1fcc-497e-80e8-c614f899142a)

### 🍽️ Recipe Management
Full CRUD operations with rich text formatting and image support.

#### Recipe Listing
![IndexForRecipe](https://github.com/user-attachments/assets/b5b87541-ecff-4bb8-a79e-be40537c4e8a)

#### Recipe Operations
| Create Recipe | Edit Recipe | Delete Recipe |
|--------------|------------|--------------|
| ![CreatingRecipe](https://github.com/user-attachments/assets/09104b9c-3ff7-4e22-8f21-4e380d619822)
|![EditDeleteRecipe](https://github.com/user-attachments/assets/09b178cb-ee1a-417f-abe2-921b38af16a4)|

## Key Features

- **UI**: Dark/light mode with responsive design
- **Image Support**: Upload recipe and category images
- **Formatted Content**: Special formatting for ingredients and instructions
- **User-Friendly**: Intuitive navigation and controls
- **Secure**: Proper authentication and authorization

## Future Improvements 🚀

- 📱 **Mobile App** (React Native/Flutter)
- 🗓️ **Meal Schedule Planner** with calendar view
- 🛒 **Automated Shopping Lists** from meal plans
- 📊 **Nutritional Information** calculator
- 👨‍🍳 **Chef Mode** with step-by-step cooking timer
- 🌍 **Recipe Sharing** with social media integration
- ⭐ **Rating System** for community recipes
- 🤖 **AI Suggestions** for recipe variations

## Installation & Setup

```bash
# Clone the repository
git clone https://github.com/dryzzen/WhatsMyRecipe.git

# Navigate to project directory
cd WhatsMyRecipe

# Restore dependencies
dotnet restore

# Apply database migrations
dotnet ef database update

# Run the application
dotnet run
