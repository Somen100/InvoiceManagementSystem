Invoice Management Web API Application

Description

This is a .NET Web API application built using Visual Studio 2022. The application provides endpoints for managing resources and performing CRUD operations.
It is designed to be extendable and easily configurable.

Prerequisites

Before running this application, ensure you have the following installed:

.NET SDK (Version 6.0 or later)

Visual Studio 2022 (Community/Professional/Enterprise) with the following workloads installed:

ASP.NET and web development

SQL Server (if the application connects to a database)

Postman or any REST client (for testing API endpoints)

Getting Started

1. Clone or Extract the Application

If received as a zip file, extract the contents to a directory on your local machine.

2. Open the Project

Open Visual Studio.

Click File > Open > Project/Solution.

Select the .sln file from the extracted folder.

3. Restore Dependencies

Open the Package Manager Console or use the terminal.

Run the following command to restore NuGet packages:

dotnet restore

4. Update Configuration

Open the appsettings.json file and update the following settings:

Connection Strings: Ensure the database connection string is correct.

API Keys or Secrets: Add any required keys for external services.

Example:

  "ConnectionStrings": {
    "DbConnection": "Data Source=VICTUS\\SQLEXPRESS; Initial Catalog=InvoiceDb; Trusted_connection=true; TrustServerCertificate=true;"
  },

5. Build the Application

In Visual Studio, click Build > Build Solution (or press Ctrl+Shift+B).

6. Run the Application

Press F5 or click Start in Visual Studio to run the application.

The API will be hosted locally, typically at https://localhost:<port>.

Testing the API

Using Postman:

Import the API collection or manually add endpoints.

Test each endpoint with the required parameters and headers.

Swagger UI:

The application includes Swagger for API documentation.

Navigate to https://localhost:<port>/swagger to view and test the endpoints.

Deployment

To deploy the application:

Publish:

Right-click the project in Solution Explorer and select Publish.

Choose the deployment target (e.g., Folder, Azure, IIS).

Zip Published Files:

If deploying manually, zip the published folder and share it with the deployment team.

Troubleshooting

Common Issues

Dependency Errors:

Ensure all NuGet packages are restored.

Database Connection Issues:

Verify the connection string in appsettings.json.

Ensure the database server is running.

Port Conflicts:

Change the port in launchSettings.json if another application is using the same port.

Logs

The application uses Serilog for logging.

Logs are stored in:

A text file (default configuration).

A database (if audit logging is enabled).

Contact

For questions or issues, contact the developer team at:

Email: somensingh763@gmail.com

Phone: +918076399589



