# RaceDay API Endpoint Plan
| POST | /api/auth/register | Register new user | Public | FirstName, LastName, Email, Password, Role | Created user |

| POST | /api/auth/register | Register new user | Public |
| POST | /api/auth/login | Login user | Public | Email, Password | JWT token |
| GET | /api/users/profile | View own profile | Participant/Organiser | None | User profile |
| PUT | /api/users/profile | Update own profile | Participant/Organiser | FirstName, LastName, Email | Updated profile |
| GET | /api/events | View all events | Public | None | List of events |
| GET | /api/events/{id} | View one event | Public | None | Event details |
| POST | /api/events | Create a new event | Organiser | EventName, Description, EventDate, Location, EventType | Created event |
| PUT | /api/events/{id} | Update an event | Organiser | EventName, Description, EventDate, Location, EventType | Updated event |
| DELETE | /api/events/{id} | Delete an event | Organiser | None | Success message |
| GET | /api/categories | View all categories | Public | None | Category list |
| GET | /api/categories/{id} | View one category | Public | None | Category details |
| POST | /api/categories | Create a new category | Organiser | CategoryName, DistanceKm, MaxParticipants, EntryFee | Created category |
| PUT | /api/categories/{id} | Update a category | Organiser | CategoryName, DistanceKm, MaxParticipants, EntryFee | Updated category |
| GET | /api/enrolments | View enrolments | Participant/Organiser | None | Enrolment list |
| POST | /api/enrolments | Enter a participant into a category | Participant | CategoryId | New enrolment |
| DELETE | /api/enrolments/{id} | Cancel an enrolment | Participant | None | Success message |
| GET | /api/results | View all results | Participant/Organiser | None | Result list |
| GET | /api/results/{id} | View one result | Participant/Organiser | None | Result details |
| POST | /api/results | Record a race result | Organiser | EnrolmentId, FinishTime, Position, ResultStatus | Created result |
| PUT | /api/results/{id} | Update a race result | Organiser | FinishTime, Position, ResultStatus | Updated result |
| GET | /api/routes | View event routes | Public | None | Route list |
| POST | /api/routes | Add an event route | Organiser | RouteName, DistanceKm, RouteDescription, RouteFileUrl | Created route |