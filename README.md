# MyGalleryService

**MyGalleryService** is the backend API service for the Next Photo Gallery application. It is built using .NET 8.0 and provides endpoints for serving image data, metadata, and other gallery-related functionality. The service integrates with a MySQL database and utilizes several packages to manage image processing, entity framework, and API documentation.

## Live API Documentation

You can access the live version of the **photo gallery** at [https://gallery.buraksaglik.com/](https://gallery.buraksaglik.com/).

The API documentation for **MyGalleryService** is available via [Swagger](https://your-api-docs-url-here) once the service is up and running.

## GitHub Repositories

- **Frontend (Next.js)**: [https://github.com/sglkbrk/next-photo-gallery](https://github.com/sglkbrk/next-photo-gallery)
- **Backend (.NET Service)**: [https://github.com/sglkbrk/MyGalleryService](https://github.com/sglkbrk/MyGalleryService)

## Technologies Used

### Core Libraries & Frameworks:

- **.NET 8.0**: The backend is built using the latest .NET 8 framework, offering powerful performance improvements and new features.
- **Microsoft.AspNetCore.OpenApi (8.0.8)**: API documentation via Swagger.
- **Microsoft.EntityFrameworkCore (8.0.10)**: ORM for database access, using MySQL.
- **Pomelo.EntityFrameworkCore.MySql (8.0.2)**: MySQL provider for Entity Framework Core.
- **SixLabors.ImageSharp (3.1.5)**: Image processing library for image resizing, format conversion, etc.
- **Swashbuckle.AspNetCore (6.4.0)**: Automatically generates API documentation for your RESTful service.

### Image Processing:

- **SixLabors.ImageSharp.Drawing (2.1.4)**: Used for image manipulation tasks like resizing and drawing shapes on images.
- **System.Drawing.Common (8.0.10)**: Provides common graphics functionality.

### Database:

- **Pomelo.EntityFrameworkCore.MySql (8.0.2)**: Entity Framework Core provider for MySQL.
- **Microsoft.EntityFrameworkCore.SqlServer (8.0.10)**: (Optional, if using SQL Server as an alternative database engine).
- **MySql.Data (9.1.0)**: MySQL connector for .NET.
- **MySql.EntityFrameworkCore (8.0.8)**: Another package for MySQL support in EF Core.

## Features

- **Image Management**: Upload, retrieve, and manipulate images in various formats and sizes.
- **Database Integration**: Uses Entity Framework Core to interact with MySQL or SQL Server databases.
- **API Documentation**: Exposes a full Swagger UI for easy exploration of API endpoints.
- **Image Resizing**: Resize and process images dynamically using ImageSharp.
- **Test Coverage**: Includes unit tests for various service layers, using xUnit and Coverlet for code coverage.

## 🛠 Installation

To get started with this project, follow these steps:

1. Clone the repository:

   ```bash
   git clone https://github.com/sglkbrk/MyGalleryService.git
   ```

2. Navigate to the project directory:

   ```bash
   cd MyGalleryService
   ```

3. Install the dependencies:

   ```bash
   dotnet restore
   ```

4. Configure the Database

   You need to configure your database connection string in the `appsettings.json` file.

   The connection string specifies the details for connecting to your database.

   #### Example (`appsettings.json`):

   json
   {
   "ConnectionStrings": {"MyGalleryDb": "server=localhost;database=mygallery;user=myuser;password=mypassword;"}
   }

5. Build and Run the Service

   ```bash
   dotnet run

   ```

6. Apply Database Migrations
   ```bash
   dotnet ef database update
   ```

```

```
