# Clean Architecture Template

What's included in the template?

- SharedKernel project with common Domain-Driven Design abstractions.
- Domain layer with sample entities.
- Application layer with abstractions for:
  - CQRS
  - Example use cases
  - Cross-cutting concerns (logging, validation)
- Infrastructure layer with:
  - Authentication
  - Permission authorization
  - EF Core, PostgreSQL
  - Serilog
- Seq for searching and analyzing structured logs
  - Seq is available at http://localhost:8081 by default
- Testing projects
  - Architecture testing

I'm open to hearing your feedback about the template and what you'd like to see in future iterations.

If you're ready to learn more, check out [**Pragmatic Clean Architecture**](https://www.milanjovanovic.tech/pragmatic-clean-architecture?utm_source=ca-template):

- Domain-Driven Design
- Role-based authorization
- Permission-based authorization
- Distributed caching with Redis
- OpenTelemetry
- Outbox pattern
- API Versioning
- Unit testing
- Functional testing
- Integration testing

# Pandit Tithi Platform - Core Database Schema

Minimal but production-ready relational schema for Hindu pandit booking + tithi event platform. Designed for SQL Server/PostgreSQL with EF Core.


### DevoteeProfiles

| Column         | Type          | Constraints          | Description    |
| -------------- | ------------- | -------------------- | -------------- |
| `Id`           | `Guid`        | PK                   | Profile ID     |
| `UserId`       | `Guid`        | FK→Users.Id, Unique  | Linked user    |
| `FullName`     | `string(100)` | NotNull              | Display name   |
| `AddressLine1` | `string(200)` | NotNull              | Street address |
| `AddressLine2` | `string(200)` | Nullable             | Apt/Unit       |
| `City`         | `string(50)`  | NotNull              | City           |
| `Postcode`     | `string(10)`  | NotNull              | Postal code    |
| `State`        | `string(50)`  | NotNull              | State/Province |
| `Country`      | `string(50)`  | Default("Australia") | Country        |
| `Timezone`     | `string(50)`  | NotNull              | IANA timezone  |

### PanditProfiles

| Column              | Type           | Constraints          | Description                      |
| ------------------- | -------------- | -------------------- | -------------------------------- |
| `Id`                | `Guid`         | PK                   | Profile ID                       |
| `UserId`            | `Guid`         | FK→Users.Id, Unique  | Linked user                      |
| `FullName`          | `string(100)`  | NotNull              | Display name                     |
| `Bio`               | `string(1000)` | Nullable             | Professional bio                 |
| `BaseCity`          | `string(50)`   | NotNull              | Home city                        |
| `Postcode`          | `string(10)`   | NotNull              | Base postcode                    |
| `State`             | `string(50)`   | NotNull              | Base state                       |
| `Country`           | `string(50)`   | Default("Australia") | Base country                     |
| `ServiceRadiusKm`   | `int`          | Default(50)          | Travel radius                    |
| `Languages`         | `jsonb/text`   | Nullable             | `["Nepali", "Hindi", "English"]` |
| `YearsOfExperience` | `int`          | Default(0)           | Experience years                 |
| `IsVerified`        | `bool`         | Default(false)       | Admin verified                   |
| `AverageRating`     | `decimal(2,1)` | Nullable             | 1.0-5.0 aggregate                |

## Pujas, Availability, Bookings

### PujaTypes

| Column                   | Type            | Constraints     | Description                    |
| ------------------------ | --------------- | --------------- | ------------------------------ |
| `Id`                     | `Guid`          | PK              | Puja type ID                   |
| `Name`                   | `string(100)`   | Unique, Indexed | "Griha Pravesh", "Bratabandha" |
| `Description`            | `string(500)`   | Nullable        | Ritual description             |
| `DefaultDurationMinutes` | `int`           | Default(60)     | Typical duration               |
| `DefaultDakshina`        | `decimal(10,2)` | Default(0)      | Base price (AUD)               |
| `IsActive`               | `bool`          | Default(true)   | Availability                   |

### PanditPujaRates

| Column           | Type            | Constraints          | Description           |
| ---------------- | --------------- | -------------------- | --------------------- |
| `Id`             | `Guid`          | PK                   | Rate ID               |
| `PanditId`       | `Guid`          | FK→PanditProfiles.Id | Pandit                |
| `PujaTypeId`     | `Guid`          | FK→PujaTypes.Id      | Puja type             |
| `CustomDakshina` | `decimal(10,2)` | Nullable             | Custom price override |
| `Notes`          | `string(500)`   | Nullable             | Special notes         |

### PanditAvailabilities

| Column        | Type   | Constraints          | Description     |
| ------------- | ------ | -------------------- | --------------- |
| `Id`          | `Guid` | PK                   | Availability ID |
| `PanditId`    | `Guid` | FK→PanditProfiles.Id | Pandit          |
| `DayOfWeek`   | `int`  | 0-6 (Sun-Sat)        | Weekday         |
| `StartTime`   | `time` | NotNull              | Available from  |
| `EndTime`     | `time` | NotNull              | Available until |
| `IsRecurring` | `bool` | Default(true)        | Weekly repeat   |

### Bookings

| Column                | Type             | Constraints           | Description                                   |
| --------------------- | ---------------- | --------------------- | --------------------------------------------- |
| `Id`                  | `Guid`           | PK                    | Booking ID                                    |
| `DevoteeId`           | `Guid`           | FK→DevoteeProfiles.Id | Booker                                        |
| `PanditId`            | `Guid`           | FK→PanditProfiles.Id  | Provider                                      |
| `PujaTypeId`          | `Guid`           | FK→PujaTypes.Id       | Ritual type                                   |
| `RequestedStartUtc`   | `DateTimeOffset` | NotNull               | Requested start                               |
| `RequestedEndUtc`     | `DateTimeOffset` | NotNull               | Requested end                                 |
| `LocalTimezone`       | `string(50)`     | NotNull               | "Australia/Sydney"                            |
| `AddressLine1`        | `string(200)`    | NotNull               | Puja venue                                    |
| `AddressLine2`        | `string(200)`    | Nullable              | Apt/Unit                                      |
| `City`                | `string(50)`     | NotNull               | Venue city                                    |
| `Postcode`            | `string(10)`     | NotNull               | Venue postcode                                |
| `State`               | `string(50)`     | NotNull               | Venue state                                   |
| `SpecialInstructions` | `text`           | Nullable              | Puja notes                                    |
| `Status`              | `string(20)`     | Indexed               | Pending/Accepted/Rejected/Cancelled/Completed |
| `CreatedAt`           | `DateTimeOffset` | NotNull               | Booking created                               |
| `UpdatedAt`           | `DateTimeOffset` | NotNull               | Last status change                            |

### Payments

| Column              | Type             | Constraints    | Description                  |
| ------------------- | ---------------- | -------------- | ---------------------------- |
| `Id`                | `Guid`           | PK             | Payment ID                   |
| `BookingId`         | `Guid`           | FK→Bookings.Id | Linked booking               |
| `Amount`            | `decimal(10,2)`  | NotNull        | Payment amount               |
| `Currency`          | `string(3)`      | Default("AUD") | Currency code                |
| `Status`            | `string(20)`     | Indexed        | Pending/Paid/Failed/Refunded |
| `Provider`          | `string(50)`     | NotNull        | "Stripe", "PayPal"           |
| `ProviderReference` | `string(200)`    | Indexed        | External payment ID          |
| `CreatedAt`         | `DateTimeOffset` | NotNull        | Payment initiated            |
| `UpdatedAt`         | `DateTimeOffset` | NotNull        | Last status change           |

### Reviews

| Column      | Type             | Constraints            | Description       |
| ----------- | ---------------- | ---------------------- | ----------------- |
| `Id`        | `Guid`           | PK                     | Review ID         |
| `BookingId` | `Guid`           | FK→Bookings.Id, Unique | Completed booking |
| `Rating`    | `int`            | 1-5                    | Star rating       |
| `Comment`   | `text`           | Nullable               | Review text       |
| `CreatedAt` | `DateTimeOffset` | NotNull                | Review posted     |

## Tithi and Event Data

### TithiProfiles (per family member)

| Column       | Type          | Constraints           | Description                |
| ------------ | ------------- | --------------------- | -------------------------- |
| `Id`         | `Guid`        | PK                    | Profile ID                 |
| `DevoteeId`  | `Guid`        | FK→DevoteeProfiles.Id | Owner                      |
| `PersonName` | `string(100)` | NotNull               | "Father Sharma"            |
| `Relation`   | `string(50)`  | NotNull               | "Self", "Father", "Mother" |
| `BirthDate`  | `date`        | NotNull               | Gregorian birth date       |
| `BirthTime`  | `time`        | Nullable              | Birth time                 |
| `BirthPlace` | `string(200)` | NotNull               | Birth location             |
| `Timezone`   | `string(50)`  | NotNull               | Birth timezone             |

### TithiEvents (generated per year)

| Column             | Type             | Constraints         | Description                |
| ------------------ | ---------------- | ------------------- | -------------------------- |
| `Id`               | `Guid`           | PK                  | Event ID                   |
| `TithiProfileId`   | `Guid`           | FK→TithiProfiles.Id | Source profile             |
| `Year`             | `int`            | Indexed             | Gregorian year             |
| `GregorianDateUtc` | `DateTimeOffset` | NotNull             | Event date/time            |
| `LocalTimezone`    | `string(50)`     | NotNull             | Display timezone           |
| `TithiNumber`      | `int`            | 1-30                | Tithi index                |
| `TithiName`        | `string(50)`     | NotNull             | "Pratipada", "Dvitiya"     |
| `Paksha`           | `string(20)`     | NotNull             | "Shukla", "Krishna"        |
| `EventType`        | `string(50)`     | NotNull             | "BirthdayPuja", "Shraddha" |
| `GeneratedAt`      | `DateTimeOffset` | NotNull             | Calculation time           |

## Optional Tables (Phase 2)

### Messages (in-app chat)
