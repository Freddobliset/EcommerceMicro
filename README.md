# Ecommerce Microservices System

A distributed system for order and warehouse management built with **.NET 9**, orchestrated via **Docker Compose**, and featuring asynchronous communication through **Apache Kafka**.

## System Architecture
The project follows the required 5-layer architecture (WebApi, Business, ClientHttp, Repository, Shared) and consists of the following components:

* **Order Service**: Manages order creation and coordinates with the warehouse.
* **Warehouse Service**: Manages product stock and inventory updates.
* **Apache Kafka**: The message broker used for asynchronous communication (stock updates after an order is placed).
* **SQL Server**: Relational database for persistent data storage.



## 🛠️ Key Features & Technologies
* **Entity Framework Core**: Database management using a **Code-First** approach.
* **Database Migrations**: Full database versioning included within the Repository projects.
* **Kafka Messaging**: Asynchronous communication protocol used to ensure service decoupling.
* **NuGet Local Feed**: Dependencies are managed via local `.nupkg` packages (found in the `NugetLocal` folder) to avoid direct DLL references.
* **Resilience**: Implemented a **Retry Policy** to ensure services wait for SQL Server to be fully initialized before attempting connections.

## 🚦 Getting Started
The system is designed to be "turnkey" using Docker.

1.  Ensure **Docker** is running.
2.  Open your terminal in the project root (where `docker-compose.yml` is located).
3.  Run the following command:
    ```bash
    docker-compose up -d --build
    ```
4.  Wait approximately 30-60 seconds for the databases and the Kafka broker to initialize.

## 📦 Automatic Data Seeding
Upon startup, the system automatically performs:
1.  **Migration**: EF Core migrations are applied to create/update the schema.
2.  **Data Seeding**: If the database is empty, a test product is automatically created ("Prodotto Test Automatico", ID: 1) to allow for immediate end-to-end testing.

## 🔗 Swagger Endpoints
Once the containers are running, you can interact with the services via Swagger:
* **Order Service**: [http://localhost:5001/swagger](http://localhost:5001/swagger)
* **Warehouse Service**: [http://localhost:5002/swagger](http://localhost:5002/swagger)

## 🧪 Testing the Workflow
1.  Verify the initial stock by calling `GET /api/products` on the **Warehouse Service**.
2.  Place an order via `POST /api/orders` on the **Order Service** using **ProductId: 1**.
3.  Refresh the Warehouse Service `GET` call to verify that the stock quantity has been automatically decreased via the Kafka event.
