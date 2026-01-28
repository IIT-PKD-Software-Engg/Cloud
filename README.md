# Cloud Service Library for Azure Blob Storage

This is a reusable C# cloud service library designed to provide secure and structured access to Azure Blob Storage for a distributed software system.

This module exposes a clean abstraction over common storage operations and is intended to be consumed by multiple client components through a unified interface.

This repository is part of the Software Lab Project, designed to support and manage cloud-based applications and services.


## Overview

The Cloud Service Library acts as a centralized backend service for managing cloud storage operations such as file upload, download, update, deletion, configuration retrieval, and JSON-based search.

It is designed with modularity and extensibility in mind, enabling different system components to interact with cloud storage without direct dependency on the underlying cloud provider.


## Features

- Upload, download, update, delete, and list files in cloud storage
- Secure access is enforced using Azure IAM (RBAC) and SAS-based authentication
- Unified `CloudService` abstraction implementing the `ICloud` interface
- JSON file search based on key-value pairs
- Structured API responses using `ServiceResponse<T>`
- Integrated logging using `Microsoft.Extensions.Logging`
- Designed for use in a multi-module distributed system


##  Architecture Overview

## Technologies Used

- C#
- .NET Core
- Azure Functions (HTTP-triggered)
- Azure Blob Storage

## Getting Started

### Prerequisites

- .NET Core / .NET 5+ SDK
- An Azure Storage account
- Valid SAS token for the target container


### Basic Usage


```csharp
var cloudService = new CloudService(
    baseUrl: "https://secloudapp-2024.azurewebsites.net/api",
    team: "your-container-name",
    sasToken: "<SAS_TOKEN>",
    httpClient: new HttpClient(),
    logger: logger
);

await cloudService.UploadAsync("example.txt", stream, "text/plain");

```
## Documentation

Detailed technical documentation for the Cloud Service Library is available in the `docs` directory.

- **API Reference** (`docs/api-reference.md`)
Complete reference for all cloud operations including uplaod, download, update, delete, list, configuration retrieval, and JSON search APIs. Each function includes method signatures, purpose, features, return types, example responses, and usage examples.

- **Configuration Guide** (`docs/configuration.md`)
Explains required configuration pararmeters such as base URL, container name, SAS token usage, and best practices for secure access.

-- **Architecture Overvie** (`docs/architecture.md`)
Describes the layered design of the cloud module, service abstraction, interfaces, and third-party cloud provider integration.

## Error Handling & Logging

All operations return a standardized ServiceResponse<T> object containing:
- Operation success status
- Optional data payload
- Error message (if applicable)

The library integrates with Microsoft.Extensions.Logging to provide:
- Informational logs for successful operations
- Error logs for failures
- Debug logs for detailed execution trailing


## Project Context

This module was developed as part of final-year Software Engineering project focused on building a distributed, modular system with clearly defined service boundaries.

The cloud service was designed to support multiple independent client modules through a common reusable interface.


## License

This project is intended for academic and educational use.

## Authors
- **Pranav Rao**
- **Arnav Kadu**
