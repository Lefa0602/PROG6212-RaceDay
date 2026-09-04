
-- RaceDay Database Script


-- 1. USERS
CREATE TABLE Users (
    UserId INT IDENTITY(1,1) PRIMARY KEY,
    FirstName NVARCHAR(50) NOT NULL,
    LastName NVARCHAR(50) NOT NULL,
    Email NVARCHAR(100) NOT NULL,
    PasswordHash NVARCHAR(255) NOT NULL,
    Role NVARCHAR(20) NOT NULL,
    CreatedAt DATETIME2 NOT NULL
);

-- 2. EVENTS
CREATE TABLE Events (
    EventId INT IDENTITY(1,1) PRIMARY KEY,
    OrganiserId INT NOT NULL,
    EventName NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500),
    EventDate DATE NOT NULL,
    Location NVARCHAR(200) NOT NULL,
    EventType NVARCHAR(50) NOT NULL,
    CreatedAt DATETIME2 NOT NULL,
    FOREIGN KEY (OrganiserId) REFERENCES Users(UserId)
);

-- 3. CATEGORIES
CREATE TABLE Categories (
    CategoryId INT IDENTITY(1,1) PRIMARY KEY,
    EventId INT NOT NULL,
    CategoryName NVARCHAR(100) NOT NULL,
    DistanceKm DECIMAL(6,2) NOT NULL,
    MaxParticipants INT NOT NULL,
    EntryFee DECIMAL(10,2) NOT NULL,
    FOREIGN KEY (EventId) REFERENCES Events(EventId)
);

-- 4. ENROLMENTS
CREATE TABLE Enrolments (
    EnrolmentId INT IDENTITY(1,1) PRIMARY KEY,
    ParticipantId INT NOT NULL,
    CategoryId INT NOT NULL,
    EnrolmentDate DATETIME2 NOT NULL,
    Status NVARCHAR(20) NOT NULL,
    FOREIGN KEY (ParticipantId) REFERENCES Users(UserId),
    FOREIGN KEY (CategoryId) REFERENCES Categories(CategoryId)
);

-- 5. RESULTS
CREATE TABLE Results (
    ResultId INT IDENTITY(1,1) PRIMARY KEY,
    EnrolmentId INT NOT NULL,
    FinishTime TIME,
    Position INT,
    ResultStatus NVARCHAR(20) NOT NULL,
    RecordedAt DATETIME2 NOT NULL,
    FOREIGN KEY (EnrolmentId) REFERENCES Enrolments(EnrolmentId)
);

-- 6. EVENT ROUTES
CREATE TABLE EventRoutes (
    RouteId INT IDENTITY(1,1) PRIMARY KEY,
    EventId INT NOT NULL,
    RouteName NVARCHAR(100) NOT NULL,
    DistanceKm DECIMAL(6,2) NOT NULL,
    RouteDescription NVARCHAR(500),
    RouteFileUrl NVARCHAR(500),
    FOREIGN KEY (EventId) REFERENCES Events(EventId)
);

-- DATA VALIDATION CONSTRAINTS

ALTER TABLE Users
ADD CONSTRAINT UQ_Users_Email UNIQUE (Email);

ALTER TABLE Users
ADD CONSTRAINT CK_Users_Role
CHECK (Role IN ('Organiser', 'Participant'));

ALTER TABLE Categories
ADD CONSTRAINT CK_Categories_Distance
CHECK (DistanceKm > 0);

ALTER TABLE Categories
ADD CONSTRAINT CK_Categories_MaxParticipants
CHECK (MaxParticipants > 0);

ALTER TABLE Categories
ADD CONSTRAINT CK_Categories_EntryFee
CHECK (EntryFee >= 0);

ALTER TABLE Enrolments
ADD CONSTRAINT CK_Enrolments_Status
CHECK (Status IN ('Confirmed', 'Cancelled', 'Pending'));

ALTER TABLE Results
ADD CONSTRAINT CK_Results_Position
CHECK (Position > 0);

ALTER TABLE Results
ADD CONSTRAINT CK_Results_Status
CHECK (ResultStatus IN ('Finished', 'Disqualified', 'Did Not Finish'));



-- SAMPLE DATA

-- USERS
INSERT INTO Users
    (FirstName, LastName, Email, PasswordHash, Role, CreatedAt)
VALUES
    ('John', 'Mokoena', 'john@raceday.co.za', 'HASH001', 'Organiser', '2026-09-01 08:00:00'),
    ('Sarah', 'Naidoo', 'sarah@raceday.co.za', 'HASH002', 'Participant', '2026-09-01 08:30:00'),
    ('David', 'Smith', 'david@raceday.co.za', 'HASH003', 'Participant', '2026-09-01 09:00:00');


-- EVENTS
INSERT INTO Events
    (OrganiserId, EventName, Description, EventDate, Location, EventType, CreatedAt)
VALUES
    (1, 'Cape Town Road Race', 'Annual road running event', '2026-10-10', 'Cape Town', 'Running', '2026-09-01 10:00:00'),
    (1, 'Johannesburg Cycle Challenge', 'Road cycling event', '2026-11-15', 'Johannesburg', 'Cycling', '2026-09-01 10:30:00');


-- CATEGORIES
INSERT INTO Categories
    (EventId, CategoryName, DistanceKm, MaxParticipants, EntryFee)
VALUES
    (1, '10 km Run', 10.00, 500, 150.00),
    (1, '21 km Half Marathon', 21.10, 300, 250.00),
    (2, '50 km Cycle', 50.00, 400, 200.00);


-- ENROLMENTS
INSERT INTO Enrolments
    (ParticipantId, CategoryId, EnrolmentDate, Status)
VALUES
    (2, 1, '2026-09-02 10:00:00', 'Confirmed'),
    (3, 2, '2026-09-02 10:30:00', 'Confirmed'),
    (2, 3, '2026-09-02 11:00:00', 'Confirmed');


-- RESULTS
INSERT INTO Results
    (EnrolmentId, FinishTime, Position, ResultStatus, RecordedAt)
VALUES
    (1, '00:52:35', 25, 'Finished', '2026-10-10 12:00:00'),
    (2, '01:48:20', 18, 'Finished', '2026-10-10 12:30:00');


-- EVENT ROUTES
INSERT INTO EventRoutes
    (EventId, RouteName, DistanceKm, RouteDescription, RouteFileUrl)
VALUES
    (1, 'Cape Town 10 km Route', 10.00, 'Road route through Cape Town', 'https://example.com/routes/cape-town-10km'),
    (2, 'Johannesburg 50 km Route', 50.00, 'Road cycling route through Johannesburg', 'https://example.com/routes/joburg-50km');
  
-- DATABASE VERIFICATION QUERIES

-- View all users
SELECT * FROM Users;

-- View all events
SELECT * FROM Events;

-- View all categories
SELECT * FROM Categories;

-- View all enrolments
SELECT * FROM Enrolments;

-- View all results
SELECT * FROM Results;

-- View all event routes
SELECT * FROM EventRoutes;  
-- ============================================
-- DATABASE RELATIONSHIP SUMMARY
-- ============================================

-- Users can organise many events.
-- Events belong to one organiser.

-- Each event can contain one or more categories.
-- Categories belong to one event.

-- Participants can have many enrolments.
-- Each enrolment belongs to one participant and one category.

-- An enrolment can have zero or one race result.
-- Each result belongs to one enrolment.

-- Each event can have zero or more routes.
-- Each route belongs to one event.

-- RELATIONSHIP VERIFICATION QUERY

SELECT
    u.FirstName,
    u.LastName,
    e.EventName,
    c.CategoryName,
    en.Status AS EnrolmentStatus
FROM Enrolments en
INNER JOIN Users u
    ON en.ParticipantId = u.UserId
INNER JOIN Categories c
    ON en.CategoryId = c.CategoryId
INNER JOIN Events e
    ON c.EventId = e.EventId;
    -- ============================================
-- RESULTS VERIFICATION QUERY
-- ============================================

SELECT
    u.FirstName,
    u.LastName,
    e.EventName,
    c.CategoryName,
    r.FinishTime,
    r.Position,
    r.ResultStatus
FROM Results r
INNER JOIN Enrolments en
    ON r.EnrolmentId = en.EnrolmentId
INNER JOIN Users u
    ON en.ParticipantId = u.UserId
INNER JOIN Categories c
    ON en.CategoryId = c.CategoryId
INNER JOIN Events e
    ON c.EventId = e.EventId;