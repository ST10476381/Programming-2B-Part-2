

## 2. Project Features

The main features of the system include:

- **User registration and login:** Users can create accounts and log in to the system.
- **Authentication:** JWT tokens are used to protect endpoints that require users to be logged in.
- **Role-based access:** The system has two roles, Participants and Organisers, with different permissions.
- **Event management:** Organisers can create and manage their own race events, while users can view available events.
- **Category management:** Organisers can create categories with details such as distance, entry fee, maximum participants, and age restrictions.
- **Participant enrolment:** Participants can enrol in race categories.
- **Enrolment management:** Participants can view their own enrolments, while Organisers can view enrolments for their own events.
- **Race results:** The system is designed to support recording and viewing race results, including finishing times and positions, through the relevant implemented endpoints.
- **API testing:** Swagger and automated unit tests are used to test the system.

## 3. Technologies Used

The following technologies and tools were used:

- C#
- ASP.NET Core Web API
- .NET 8
- Microsoft SQL Server
- Entity Framework Core
- JWT authentication
- Swagger / OpenAPI
- xUnit testing framework
- Visual Studio

## 4. User Roles

### Participant

A Participant can register, log in, view race events, enrol in a race category, and view their own enrolments and results through the relevant endpoints.

### Organiser

An Organiser can register, log in, create and manage their own race events, create categories, and view enrolments for the events they organise. An Organiser should not be allowed to update an event that belongs to another Organiser.

## 5. Database

The project uses a SQL Server database named `RaceDay`.

The database stores information required by the system, including user accounts, race events, categories, enrolments, and results, depending on the tables implemented in the project.

The `EVENT` table stores information such as the event name, date, location, description, event type, distance, and organiser ID.

The `CATEGORY` table stores category information, including the category name, distance, entry fee, maximum number of participants, and minimum and maximum age limits.

Before running the project, SQL Server must be available, and the connection string must point to the correct database.

## 6. How to Run the Project

Follow these steps to run the project in Visual Studio:

1. Open the `RaceDayAPI` solution in Visual Studio.
2. Confirm that the project uses .NET 8.
3. Check the database connection string in `appsettings.json` or the relevant configuration file.
4. Make sure the `RaceDay` database and the required tables exist.
5. Restore any NuGet packages if necessary.
6. Select **Build → Build Solution**.
7. Run the API using the configured launch profile.
8. Open Swagger to view and test the available endpoints.

If the project cannot connect to SQL Server, check the connection string and make sure the database server is running.

## 7. Testing the API with Swagger

Swagger makes it easier to test the API without creating a separate user interface.

For endpoints that require authentication:

1. Register a user through the registration endpoint.
2. Log in using the user's email and password.
3. Copy the JWT token returned by the login response.
4. Click **Authorize** in Swagger.
5. Enter the token in the format required by the Swagger configuration. If it expects a bearer token, enter `Bearer ` followed by the token.
6. Execute the endpoint you want to test.

When testing permissions, use the correct account type. For example, use a Participant account to test participant actions and an Organiser account to test organiser actions.

## 8. Unit Testing

The project includes a separate `RaceDayAPI.Tests` project that uses xUnit to test the API.

The tests cover:

- Successful participant registration.
- Successful login.
- Rejection of an incorrect password.
- Rejection of requests without an authentication token.
- Role-based access restrictions.
- Successful event creation by an Organiser.
- Prevention of event creation by a Participant.
- Prevention of one Organiser from updating another Organiser's event.

At the time of writing, all 10 unit tests passed in my test run. This helped me check that the main authentication and event permission rules were working as expected.

## 9. Example System Workflow

A typical workflow in the system is:

1. A user registers as a Participant or Organiser.
2. The user logs in and receives a JWT token.
3. An Organiser creates a race event.
4. The Organiser creates categories for the event and sets the relevant details and age limits.
5. Participants view the available events and enrol in a suitable category.
6. Participants view their own enrolments.
7. Organisers view enrolments for their own events.
8. Race results can be recorded and viewed through the relevant endpoints implemented in the project.

## 10. Conclusion

Working on RaceDayAPI helped me improve my understanding of developing REST APIs using ASP.NET Core. I gained practical experience with database integration, JWT authentication, role-based access control, Swagger, and unit testing.

The project demonstrates how an API can be used to manage race events, categories, and participant enrolments while restricting access according to user roles.

**Project name:** RaceDayAPI  
**Framework:** .NET 8  
**Programming language:** C#  
**Database:** Microsoft SQL Server  
**API documentation and testing:** Swagger / OpenAPI  
**Unit testing:** xUnit
