# Hosted Service Library for ASP.NET Core

[![GitHub issues](https://img.shields.io/github/issues/atomatus/dot-net-boot-starter-hosted-service?style=flat-square&color=%232EA043&label=help%20wanted)](https://github.com/atomatus/dot-net-boot-starter-hosted-service)

[![NuGet version (Com.Atomatus.BootStarter)](https://img.shields.io/nuget/v/Com.Atomatus.BootStarter.Hosting.svg?style=flat-square)](https://www.nuget.org/packages/Com.Atomatus.BootStarter.Hosting/)


The `Com.Atomatus.Bootstarter.Hosting` is a Hosted Service library simplifies the implementation of background services in ASP.NET Core applications using the `IHostedService` interface.

It offers an easy way to create and manage hosted services that run alongside the main application engine.

## Table of Contents
- [What is IHostedService?](#what-is-ihostedservice)
- [Library Overview](#library-overview)
- [Why Use This Library?](#why-use-this-library)
- [Getting Started](#getting-started)
- [Examples](#examples)
- [Conclusion](#conclusion)

---

## What is IHostedService?

In ASP.NET Core, `IHostedService` is an interface used to define long-running background tasks that are managed by the application's lifetime. These services can start and stop with the application and are often used for tasks such as caching, data synchronization, and more.

## Library Overview

The `Com.Atomatus.Bootstarter.Hosting` library provides the following classes to facilitate the creation of IHostedService implementations:

1. **HostedService**: This class defines an abstract method that needs to be implemented to execute a task during application startup. It offers a simple way to create custom services that run when the application starts.

2. **TimedHostedService**: Extends HostedService and allows you to create services that execute at regular intervals. You can specify the execution timeout and an optional initial execution timeout. It supports one or more IHostedServiceCallback callbacks.

3. **OneTimedHostedService**: Also extends HostedService, but it executes the IHostedServiceCallback callbacks only once during application startup.

## Why Use This Library?

- **Simplicity**: The library abstracts away the complexities of implementing IHostedService, making it easier for developers to create and manage background tasks.
- **Flexibility**: Whether you need services to run periodically, at startup, or just once, the library covers all these scenarios.
- **Code Reusability**: By encapsulating common patterns in the library, you can easily reuse the code across different projects.

## Getting Started

1. Install the package:
   ```bash
   dotnet add package Com.Atomatus.Bootstarter.Hosting
   ```

---

## Examples

### Using the HostedService Abstract Base Class in ASP.NET Core

The `HostedService` abstract base class simplifies the implementation of `IHostedService` by providing a common execution pattern for background tasks. In this example, we'll demonstrate how to create a class that inherits from `HostedService`, implement the necessary methods, and then register it using the `ServiceCollection` in an ASP.NET Core application.

#### Step 1: Create a Derived HostedService Class

Create a class that inherits from `HostedService` and overrides the `OnBackgroundAsync` method to provide the background execution logic:

```csharp
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

public class MyHostedService : HostedService
{
    protected override async Task OnBackgroundAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            // Your background logic here
            Console.WriteLine("Background task is running...");
            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }
}
```
#### Step 2: Register the HostedService

In your `Startup.cs` or equivalent, register the `MyHostedService` using the `ServiceCollection`:

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        // ... other services
        
        services.AddHostedService<MyHostedService>();
    }
}
```
### Using the TimedHostedService Abstract Base Class in ASP.NET Core

The `TimedHostedService` abstract base class extends the functionality of `HostedService`, providing a structured pattern for implementing timed background execution tasks in an ASP.NET Core application.

#### Step 1: Create a Derived TimedHostedService Class

Create a class that inherits from `TimedHostedService` and implement the necessary `OnWork` method:

```csharp
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;

public class MyTimedService : TimedHostedService
{
    public MyTimedService(TimeSpan dueTime, TimeSpan period) : base(dueTime, period) { }

    public MyTimedService() : this(TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(10)) { }

    protected override void OnWork(CancellationToken token)
    {
        // Timed background logic to be executed at regular intervals
        Console.WriteLine("Timed background task is running...");
    }
}
```
#### Step 2: Register the TimedHostedService

##### Option 1: Basic Registration

In your `Startup.cs` or equivalent, register the `MyTimedService` using the `ServiceCollection`:

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        // ... other services
        
        services.AddHostedService<MyTimedService>();
    }
}

```
##### Option 2: Alternative Registration
You can also use the alternative extension method provided:

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Com.Atomatus.Bootstarter.Hosting; // Include this namespace

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        // ... other services
        
        services.AddTimedHostedService<MyTimedService>(TimeSpan.FromSeconds(10), TimeSpan.FromMinutes(30));
    }
}

```
### Using the OneTimedHostedService Abstract Base Class in ASP.NET Core
The `OneTimedHostedService` abstract base class is designed to execute its logic once during application startup after a specified timeout. It's useful for performing one-time initialization tasks or startup routines that should run when the application starts.

#### Step 1: Create a Derived OneTimedHostedService Class
Create a class that inherits from `OneTimedHostedService` and implement the necessary `OnTimedBackgroundAsync` method:

```csharp
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

public class MyOneTimeService : OneTimedHostedService
{
    public MyOneTimeService() : this(TimeSpan.FromSeconds(10)) { }

    public MyOneTimeService(TimeSpan timeout) : base(timeout) { }

    protected override Task OnTimedBackgroundAsync(CancellationToken stoppingToken)
    {
        // One-time background logic to be executed during application startup
        Console.WriteLine("One-time background task is running...");
        return Task.CompletedTask;
    }
}
```
#### Step 2: Register the OneTimedHostedService

##### Option 1: Basic Registration
In your `Startup.cs` or equivalent, register the `MyOneTimeService` using the `ServiceCollection`:

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        // ... other services
        
        services.AddHostedService<MyOneTimeService>();
    }
}

```

##### Option 2: Alternative Registration
You can also use the alternative extension method provided:

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Com.Atomatus.Bootstarter.Hosting; // Include this namespace

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        // ... other services
        
        services.AddOneTimedHostedService<MyOneTimeService>(TimeSpan.FromSeconds(10));
    }
}

```

### Implementing Timed Hosted Service Callbacks and Anonymous OneTimedHostedService

This demonstration will show how to implement `ITimedHostedServiceCallback` and `ITimedHostedServiceScopedCallback` callbacks, register them, and use the provided extension method to add an anonymous `OneTimedHostedService` to consume these callbacks.

#### Step 1: Implement Timed Hosted Service Callbacks

Use the callback interfaces and their implementations for timed hosted service callbacks:

```csharp
using System.Threading;
using System.Threading.Tasks;
using Com.Atomatus.Bootstarter.Hosting; // Include this namespace

// Implement the callback interfaces
public class MyTimedHostedServiceCallback : ITimedHostedServiceCallback
{
    private readonly ITransientService transientService;

    public MyTimedHostedServiceCallback(ITransientService transientService)
    {
        this.transientService = transientService;
    }

    public async Task InvokeAsync(CancellationToken stoppingToken)
    {
        // Execute logic using the transient service
        await transientService.DoSomethingAsync(stoppingToken);
    }
}

public class MyTimedHostedServiceScopedCallback : ITimedHostedServiceScopedCallback
{
    private readonly IScopeService scopeService;

    public MyTimedHostedServiceScopedCallback(IScopeService scopeService)
    {
        this.scopeService = scopeService;
    }

    public async Task InvokeAsync(CancellationToken stoppingToken)
    {
        // Execute logic using the scoped service
        await scopeService.DoSomethingAsync(stoppingToken);
    }
}
```
#### Step 2: Register Callbacks and Add OneTimedHostedService
In your `Startup.cs` or equivalent, register the callbacks and add the anonymous `OneTimedHostedService` using the extension method:

```csharp
using Microsoft.Extensions.DependencyInjection;
using Com.Atomatus.Bootstarter.Hosting; // Include this namespace

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        // ... other services

        // Register callbacks
        services.AddSingleton<ITimedHostedServiceCallback, MyTimedHostedServiceCallback>();
        services.AddSingleton<ITimedHostedServiceScopedCallback, MyTimedHostedServiceScopedCallback>();

        // Add anonymous TimedHostedService
        services.AddTimedHostedService(
            TimeSpan.Zero,//dueTime
            TimeSpan.FromSeconds(10));//period
    }
}

```
By registering the callbacks and adding the anonymous `TimedHostedService`, you enable the timed execution of the callback logic when the `TimedHostedService` starts.

### Implementing DelayedTimedHostedServiceCallback and Using it in TimedHostedService
This demonstration will guide you through the process of implementing the `DelayedTimedHostedServiceCallback` class and using it in an anonymous `TimedHostedService`.

#### Step 1: Implement DelayedTimedHostedServiceCallback
Create the `DelayedTimedHostedServiceCallback` class as follows:

```csharp
internal class MyDelayedTimedCallback : DelayedTimedHostedServiceCallback
{
    public MyDelayedTimedCallback(TimeSpan delayInterval) : base(delayInterval)
    {
    }

    protected override async Task InvokeAsync(CancellationToken stoppingToken)
    {
        // Logic to be executed after the specified delay interval
        // This will only be executed once after the delay
    }
}
```
#### Step 2: Register the Callback and Add TimedHostedService
In your `Startup.cs` or equivalent, register the callback and add an anonymous `TimedHostedService`:

```csharp
using Microsoft.Extensions.DependencyInjection;
using Com.Atomatus.Bootstarter.Hosting; // Include this namespace

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        // ... other services

        // Register the callback
        services.AddSingleton<ITimedHostedServiceCallback>(provider =>
        {
            // Set the delay interval here
            TimeSpan delayInterval = TimeSpan.FromMinutes(5);
            return new MyDelayedTimedCallback(delayInterval);
        });

        // Add anonymous TimedHostedService
        services.AddTimedHostedService<MyTimedHostedService>(TimeSpan.FromMinutes(1), TimeSpan.FromSeconds(30));
    }
}
```
By registering the DelayedTimedHostedServiceCallback and adding an anonymous TimedHostedService, you ensure that the callback will be executed only after the specified delay interval when the TimedHostedService starts.

---

## Conclusion: 

### Advantages of Presented Hosted Services

In this tutorial, we explored and implemented three different types of hosted services: `HostedService`, `TimedHostedService`, and `OneTimedHostedService`. These hosted services offer a structured and convenient way to perform background tasks, initialization, and timed executions in an ASP.NET Core application. Here are the advantages of using these hosted services:

### 1. **Structured Background Execution:**

The `HostedService` class provides a standardized pattern for implementing background tasks that need to run asynchronously with the application's startup and shutdown lifecycle. It simplifies the process of creating background services that adhere to the IHostedService interface, allowing you to focus on the core logic of your tasks.

### 2. **Regular Timed Execution:**

The `TimedHostedService` class extends the `HostedService` and adds timed execution capabilities. It enables you to define tasks that run at regular intervals, making it suitable for scenarios where you need to perform periodic updates, data synchronization, or other recurring actions.

### 3. **One-Time Initialization:**

The `OneTimedHostedService` class allows you to execute tasks only once during the application's startup. This is particularly useful for performing one-time initialization, startup routines, or other actions that should run just once when the application starts.

### 4. **Delayed and Controlled Callbacks:**

The `DelayedTimedHostedServiceCallback` class adds another layer of control to timed executions. It enables you to execute callbacks with a specified delay interval, ensuring that tasks are triggered only after a certain amount of time has passed.

### 5. **Scalable and Well-Managed Services:**

All the presented hosted services can be easily registered in the ASP.NET Core's DI container, allowing for proper dependency injection and service lifetime management. This ensures a scalable and maintainable architecture for your background tasks.

By leveraging these hosted services, you can improve the efficiency, performance, and maintainability of your ASP.NET Core applications. Whether it's performing background operations, timed tasks, or one-time initialization, these classes provide a structured and organized approach to handling various scenarios in your application's lifecycle.

---

© Atomatus.com. All rights reserved.