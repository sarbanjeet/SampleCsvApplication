
# SampleApplication

SampleApplication is a web application built with ASP.NET 6 and AngularJS. This application demonstrates basic features and setups needed to get a web application up and running.

## Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
- [Node.js](https://nodejs.org/en/download/)
- [Angular CLI](https://cli.angular.io/): Run `npm install -g @angular/cli` to install.
- A text editor or an IDE (like Visual Studio or Visual Studio Code)

## Setting Up the Application

1. Clone the repository to your local machine using:
   ```sh
   git clone https://github.com/your-username/SampleApplication.git
   ```

2. Navigate to the project folder:
   ```sh
   cd SampleApplication
   ```

3. Restore the .NET dependencies:
   ```sh
   dotnet restore
   ```

4. Navigate to the ClientApp (Angular) folder and install the npm packages:
   ```sh
   cd ClientApp
   npm install
   ```

## Running the Application

1. Navigate back to the root project directory:
   ```sh
   cd ..
   ```

2. Run the application using the following command:
   ```sh
   dotnet restore SampleApplication.csproj
   dotnet run
   ```

3. Open your web browser and go to [https://localhost:7016](https://localhost:7016) to view the running application.

## Running the Tests

To run the unit tests for the application, navigate to the project directory `SampleApplicationTests` and run:
```sh
dotnet test
```

## Application Structure

- `ClientApp`: Contains the AngularJS client-side application.
- `Controllers`: Contains the ASP.NET controllers for handling API requests.
- `Services`: Contains services for business logic and data access.
- `wwwroot`: Contains static files served by the application.
