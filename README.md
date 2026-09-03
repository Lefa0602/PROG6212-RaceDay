# RaceDay

## System Description

RaceDay is a full-stack web-based event management platform designed for South African road running, walking, and cycling events.

The system allows organisers to create and manage events, categories, routes, enrolments, and race results. Participants can view events, enter available categories, manage their profiles, and view their results.

## User Roles

### Organiser

Organisers are responsible for managing events and race information.

Organisers can:
- Create events
- Update events
- Delete events
- Create and manage event categories
- Add event routes
- View participant enrolments
- Record and update race results

### Participant

Participants use RaceDay to find events and enter available categories.

Participants can:
- Register for an account
- Log in
- Manage their profile
- View available events
- View event categories
- Enrol in a category
- Cancel an enrolment
- View race results

## Database Design

The RaceDay database is designed using six main entities:

- Users
- Events
- Categories
- Enrolments
- Results
- Event Routes

The database design and relationships are documented in the ERD.

See:

`docs/RaceDay_ERD.png`

The SQL Server database script and sample data are available at:

`docs/RaceDay_Database.sql`

## API Planning

The planned API endpoints for RaceDay are documented in:

`docs/API_Endpoint_Plan.md`

The API uses role-based access control so that protected operations are restricted to the appropriate user role.

## Repository Structure

```text
Race Day
├── .github
│   └── workflows
│       └── ci.yml
├── docs
│   ├── API_Endpoint_Plan.md
│   ├── RaceDay_Database.sql
│   └── RaceDay_ERD.png
└── README.md