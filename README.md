# FinancialChat

FinancialChat is a web application that allows users to communicate in real-time through chat rooms. It includes features such as user authentication, message history, and integration with a stock quote bot.

## Features

- Real-time chat using SignalR.
- User authentication with individual accounts.
- Stock quote bot that retrieves and displays stock prices.
- Message persistence in a SQL Server database.
- Hosted on Docker containers for ease of deployment.

## Requirements

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker](https://www.docker.com/get-started)
- [Docker Compose](https://docs.docker.com/compose/)

## Getting Started

### Clone the Repository

```sh
git clone https://github.com/yourusername/FinancialChat.git
cd FinancialChat
```
### Stop Existing Containers (if any):
```sh
docker-compose down
```
This command stops and removes all running containers defined in your docker-compose.yml file.

### Build and Run the Containers:
```sh
docker-compose up --build
```
This command builds the Docker images and starts the containers as defined in your docker-compose.yml file. The --build flag forces a rebuild of the images.

### Run unit tests using the .NET CLI:

```sh
dotnet test
```
This command runs all unit tests in the solution, ensuring that your application is functioning correctly.

