# Brokkr
**Brokkr is a .NET library with useful tools to build applications**

<b>Current State: BETA</b>

## 📦 Library Components

### 📍 Brokkr.Location
Strongly-typed location handling for file paths, URLs, and other location types.

**Features:**
- Cross-platform path handling (Windows/Unix)
- URL parsing and manipulation
- Type-safe location creation and validation
- JSON serialization support

### 🏛️ Brokkr.DDD
Domain-Driven Design patterns and building blocks.

**Features:**
- Unit of Work pattern implementation
- Saga pattern support for complex workflows
- Change tracking capabilities

### ⚙️ Brokkr.OptionsHelper
Simplified configuration and options validation for .NET applications.

**Features:**
- Simplified usage for IOptions configuration
- Options validation using FluentValidation

### 💾 Brokkr.DDD.FileSystem
File system-based storage implementation for DDD patterns.

**Features:**
- JSON-based entity storage (single and multi-file)
- Unit of Work pattern for file operations

## 🚀 Getting Started

### 📍 Working with Locations (Brokkr.Location)

```csharp
// it can determine the location type from a strings format
var windowsPath = Location.Create(@"C:\Users\John\Documents\file.txt");
var unixPath = Location.Create("/home/john/documents/file.txt");
var url = Location.Create("https://api.example.com/v1/users");

if(windowsPath is AbsoluteWindowsPath)
{
    // do something windows-specific
}

if(unixPath is RelativeLocalPath)
{
    // do something specific if its a relative path
}

// you can make sure that paths are really the type of path you expect
var absoluteWindowsPath = new AbsoluteWindowsPath(@"C:\Projects");
var relativePath = JsonSerializer.Deserialize<RelativeLocalPath>(json);
```

### 🏛️ Saga Pattern (Brokkr.DDD)

The Saga pattern helps coordinate operations across multiple systems with automatic compensation on failure.

```csharp
// Define a saga for order processing
public class OrderCreationSaga : SagaBase
{
    public OrderProcessingSaga(
        IUnitOfWork inventoryContext, // could use a distributed cache like redis
        IUnitOfWork orderContext)     // could use SQL database
    {
        EntityContextMap[typeof(InventoryItem)] = new EntityContext(inventoryContext);
        EntityContextMap[typeof(Order)] = new EntityContext(orderContext);
    }
}

// Usage example
public class OrderService
{
    private readonly OrderCreationSaga _orderCreationSaga;
    
    public OrderService(OrderCreationSaga orderCreationSaga)
    {
        _orderCreationSaga = orderCreationSaga;
    }
    
    public async Task CreateOrder(Order order)
    {
        // Step 1: Reserve inventory with compensation
        saga.AddOperation<InventoryItem>(
            async repository => { // can be async if needed
                var item = await repository.GetById(order.ProductId);
                item.Reserve(order.Quantity);
            },
            // Compensation: Release reserved inventory if later steps fail
            repository => {  // can be async if needed
                var item = repository.GetById(order.ProductId);
                item.ReleaseReservation(order.Quantity);
            });

        // Step 2: Create the new order (no compensation needed)
        saga.AddOperation<Order>(
            repository => {
                repository.Add(new Order(order));
            });

        try
        {
            // Execute all operations - automatically rolls back on failure
            await saga.SaveTrackedChanges();
            Console.WriteLine("Order processed successfully");
        }
        catch (SagaFailedException ex)
        {
            Console.WriteLine($"Order processing failed: {ex.OriginalException.Message}");

            // Compensation was automatically executed
            if (ex.CompensationExceptions.Any())
            {
                Console.WriteLine("Some compensations also failed!");
            }
        }
    }
}
```
