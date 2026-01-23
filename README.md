# Volta - Distributed Ride Booking System

Volta is a distributed ride booking system built using **microservices architecture**.  
This project focuses on real-world backend engineering concepts such as **API Gateway routing, authentication, event-driven communication, and distributed service design**.

## Technology Stack

**Backend:**
- .NET 10 Web API
- Ocelot API Gateway
- RabbitMQ with MassTransit
- SQL Server + Entity Framework Core
- JWT Authentication
- Role-Based Access Control
- SignalR

**Frontend:**
- Blazor WebAssembly
- LeafletJS for maps
- OSRM API for route distance calculation

## System Architecture

- The system is divided into multiple independent services.
- **Gateway Service:** Handles all incoming requests and routes them to internal services.
- **Auth Service:** Manages user registration, login, password hashing, JWT generation, and role handling.
- **Ride Service:** Manages booking logic, distance calculation (using OSRM API), database storage, and publishes ride events.
- **Driver Service:** Consumes ride events and allows drivers to accept rides.
- **Contracts library:** Shared event models between services.

## Main Features

- Event-driven communication using RabbitMQ
- Secure authentication using JWT and BCrypt
- Role-based authorization
- Real-time notifications using SignalR
- Map-based ride booking
- Distributed microservices architecture

## Frontend

- Current frontend: Blazor WebAssembly  
- Future plan: Migrate to **Next.js** for better performance and industry alignment

## Purpose

This project was built to **practice and demonstrate distributed systems, microservices architecture, and backend system design concepts** in a practical way.

## Future Plans

- Improve frontend performance with Next.js
- Optimize event-driven communication and scalability
