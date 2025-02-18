# Real Estate Microservices Project

## Architecture Overview

### System Architecture

```mermaid
graph TD
    Client[Client Applications] -->|HTTP/HTTPS| AG[API Gateway :5000]

    subgraph Logging
        AG -->|Logs| SEQ[Seq Server :5341]
        BS -->|Logs| SEQ
        PS -->|Logs| SEQ
        PMS -->|Logs| SEQ
        NS -->|Logs| SEQ
    end

    subgraph Services
        AG -->|Route| BS[Booking Service :80]
        AG -->|Route| PS[Payment Service :80]
        AG -->|Route| PMS[Property Service :80]
        AG -->|Route| NS[Notification Service :80]
    end

    subgraph Database
        BS -->|Data| SQL[(SQL Server :1433)]
        PS -->|Data| SQL
        PMS -->|Data| SQL
        NS -->|Data| SQL
    end

    subgraph Docker Network
        SEQ -.->|realestate-network| NET[Internal Network]
        SQL -.->|realestate-network| NET
        BS -.->|realestate-network| NET
        PS -.->|realestate-network| NET
        PMS -.->|realestate-network| NET
        NS -.->|realestate-network| NET
        AG -.->|realestate-network| NET
    end

    classDef gateway fill:#f96,stroke:#333,stroke-width:2px
    classDef service fill:#58f,stroke:#333,stroke-width:2px
    classDef database fill:#6b5,stroke:#333,stroke-width:2px
    classDef logging fill:#f6c,stroke:#333,stroke-width:2px
    classDef network fill:#ccc,stroke:#333,stroke-width:1px,stroke-dasharray: 5 5

    class AG gateway
    class BS,PS,PMS,NS service
    class SQL database
    class SEQ logging
    class NET network
```

### Request Flow

```mermaid
sequenceDiagram
    participant C as Client
    participant AG as API Gateway
    participant BS as Booking Service
    participant PS as Payment Service
    participant NS as Notification Service
    participant SQL as SQL Server
    participant SEQ as Seq Server

    C->>+AG: POST /booking/create
    AG->>+BS: Forward Request
    BS->>+SQL: Create Booking
    SQL-->>-BS: Booking Created
    BS->>+PS: Process Payment
    PS->>+SQL: Save Payment
    SQL-->>-PS: Payment Saved
    PS-->>-BS: Payment Processed
    BS->>+NS: Send Confirmation
    NS->>+SQL: Save Notification
    SQL-->>-NS: Notification Saved
    NS-->>-BS: Notification Sent
    BS-->>-AG: Booking Complete
    AG-->>-C: Response

    Note over AG,SEQ: All services log to Seq
```

## Services

### API Gateway

- Entry point for all client requests
- Routes requests to appropriate microservices
- Handles cross-cutting concerns
- Port: 5000

### Booking Service

- Manages property bookings
- Handles booking lifecycle
- Integrates with Payment and Notification services
- Internal Port: 80

### Payment Service

- Processes payments
- Manages payment status
- Handles refunds
- Internal Port: 80

### Property Service

- Manages property listings
- Handles property search
- Manages property availability
- Internal Port: 80

### Notification Service

- Sends notifications
- Manages notification preferences
- Supports multiple channels (email, SMS)
- Internal Port: 80

## Infrastructure

### Logging (Seq)

- Centralized logging
- Real-time log aggregation
- Structured logging support
- Port: 5341

### Database

- SQL Server
- Separate databases per service
- Port: 1433

### Network

- Docker network: realestate-network
- Internal service communication
- Isolated network for security

## Getting Started

### Prerequisites

- Docker
- .NET 8.0 SDK
- SQL Server
- Seq

### Running the Project

1. Clone the repository
2. Navigate to the root directory
3. Run `docker-compose up`
4. Access the API Gateway at `http://localhost:5000`
5. Access Seq dashboard at `http://localhost:5341`

## Technologies

- .NET 8.0
- Entity Framework Core
- YARP Reverse Proxy
- Serilog
- Polly
- Docker
- SQL Server
- Seq

## Features

- Microservices Architecture
- Service Discovery
- Centralized Logging
- Resilience Patterns
- Docker Containerization
- API Gateway Pattern
- Circuit Breaker Pattern
- Structured Logging
