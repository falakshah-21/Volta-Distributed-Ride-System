Volta is a distributed ride booking system built using microservices architecture.

This project focuses on real world backend engineering concepts such as API Gateway routing, authentication, event driven communication, and distributed service design.

Technology Stack

Backend
.NET 9 Web API  
Ocelot API Gateway  
RabbitMQ with MassTransit  
SQL Server and Entity Framework Core  
JWT Authentication  
Role Based Access Control  
SignalR  

Frontend
Blazor WebAssembly  
LeafletJS for maps  
OSRM API for route distance calculation  

System Architecture

The system is divided into multiple independent services.
Gateway Service handles all incoming requests and routes them to internal services.
Auth Service manages user registration, login, password hashing, JWT generation and role handling.
Ride Service manages booking logic, distance calculation using OSRM API, database storage and publishes ride events.
Driver Service consumes ride events and allows drivers to accept rides.
Contracts library is used for sharing event models between services.

Main Features

Event driven communication using RabbitMQ  
Secure authentication using JWT and BCrypt  
Role based authorization  
Real time notifications using SignalR  
Map based ride booking  
Distributed microservices architecture  

Frontend

The current frontend is implemented using Blazor WebAssembly. A future migration to Next.js is planned for better performance and industry alignment.

Purpose

This project was built to practice and demonstrate distributed systems, microservices architecture, and backend system design concepts in a practical way.
