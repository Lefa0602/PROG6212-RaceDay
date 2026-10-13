# RaceDay

## 1. System Description

RaceDay is a full-stack web-based event management platform designed for South African road running, walking, and cycling events.

The planned system allows organisers to create and manage events, categories, routes, enrolments, and race results. Participants can view events, enter available categories, manage their profiles, and view their results.

The Part 2 website introduces the RaceDay visual identity and a responsive front-end interface for discovering sporting activities and sample events.

**Project status:** The homepage and external CSS stylesheet have been created. Backend integration, authentication, live event data, and event registration are not yet implemented in the current front-end demonstration.

## 2. Technology Stack

- Frontend: HTML5 and CSS3
- Backend: Planned RESTful API
- Database: Microsoft SQL Server
- Database management: SQL Server Management Studio (SSMS)
- Source control: Git and GitHub
- Continuous integration: GitHub Actions
- Development environment: Visual Studio Code
- Browser testing: Google Chrome Developer Tools

## 3. Project Scope

### Part 1: Planning and Database Design

The Part 1 deliverables include:

- Entity Relationship Diagram (ERD)
- SQL Server database schema
- Sample database data
- API endpoint plan
- GitHub repository
- GitHub Actions continuous integration workflow

### Part 2: CSS Styling and Responsive Design

Part 2 focuses on implementing the visual design of the RaceDay website.

The deliverables include:

- An HTML homepage linked to an external CSS stylesheet
- Consistent colours, typography, spacing, and visual styling
- A structured layout using CSS Grid and Flexbox
- Interactive hover and focus states
- Responsive layouts for desktop, tablet, and mobile screens
- Browser testing at different viewport sizes
- Screenshot evidence documenting responsive layouts
- Updated project documentation, changelog, and references

## 4. User Roles

### Organiser

Organisers are responsible for managing events and race information.

Planned capabilities include:

- Create, update, and delete events
- Create and manage event categories
- Add event routes
- View participant enrolments
- Record and update race results

### Participant

Participants use RaceDay to discover events and enter available categories.

Planned capabilities include:

- Register for an account
- Log in
- Manage their profile
- View available events
- View event categories
- Enrol in a category
- Cancel an enrolment
- View race results

These capabilities describe the planned full-stack system and should not be interpreted as confirmation that the backend features are operational.

## 5. Database Design

The RaceDay database is designed around six main entities:

- Users
- Events
- Categories
- Enrolments
- Results
- Event Routes

The database design and relationships are documented in the ERD.

Project database documentation:

- [RaceDay ERD](docs/RaceDay_ERD.png)
- [RaceDay SQL Database Script](docs/RaceDay_Database.sql)

The ERD documents the database entities, primary keys, foreign keys, and relationships between tables.

The SQL script is intended to create the database tables, apply data validation constraints, insert sample data, and provide verification queries.

## 6. API Planning

The planned API endpoints are documented in [API Endpoint Plan](docs/API_Endpoint_Plan.md).

The API is intended to use role-based access control to restrict protected operations to appropriate user roles.

The endpoint plan covers authentication, user profiles, events, categories, enrolments, results, and event routes.

## 7. Website Design and Implementation

### 7.1 Homepage

The RaceDay homepage introduces the platform and provides navigation to its main content sections.

The homepage includes:

- A branded navigation header
- A hero section introducing RaceDay
- Activity categories for running, walking, and cycling
- Sample event cards
- An About RaceDay section
- A call-to-action section
- A footer

The displayed event cards are illustrative sample content. They do not represent verified live events or an operational registration system.

### 7.2 External CSS Stylesheet

The website uses an external stylesheet named `style.css`, linked from `index.html`.

The stylesheet defines:

- A consistent colour palette and reusable CSS variables
- Typography, spacing, borders, and shadows
- A CSS reset for more consistent browser rendering
- Reusable button and card styles
- CSS Grid and Flexbox layouts
- Hover and keyboard-focus states
- Responsive media queries
- Reduced-motion accessibility styling

### 7.3 Responsive Design

The website uses responsive CSS to adapt its layout to different screen widths.

The current stylesheet includes:

- A desktop layout with multi-column sections
- A tablet breakpoint at 900 pixels
- A mobile breakpoint at 600 pixels
- Additional styling for screens up to 360 pixels wide

Relative units, flexible containers, and responsive grid layouts are used to help content fit smaller screens.

The website should be tested at representative desktop, tablet, and mobile viewport sizes before submission.

### 7.4 Accessibility and Usability

The current implementation includes:

- A viewport meta tag
- Semantic HTML sections
- A skip-to-content link
- A labelled main navigation area
- Visible keyboard-focus styling
- Reduced-motion preferences
- Flexible layouts intended to avoid horizontal overflow

These features should be checked during browser testing.

## 8. Repository Structure

The project contains the following documented files and folders. The website files and screenshot folder are added as part of Part 2.

```text
Race Day/
├── .github/
│   └── workflows/
│       └── ci.yml
├── docs/
│   ├── API_Endpoint_Plan.md
│   ├── RaceDay_Database.sql
│   └── RaceDay_ERD.png
├── screenshots/
│   ├── desktop-view.png
│   ├── tablet-view.png
│   └── mobile-view.png
├── index.html
├── style.css
├── PROG6212 Planning Guide.html
└── README.md
```

The screenshot filenames and website files listed above should match the actual repository contents before submission.

## 9. Setup and Verification

### 9.1 Website

1. Clone the RaceDay repository from GitHub.
2. Open the project folder in Visual Studio Code.
3. Open `index.html` in a web browser.
4. Confirm that the homepage displays correctly.
5. Confirm that `style.css` is located alongside `index.html`.
6. Resize the browser or use Chrome Developer Tools to test desktop, tablet, and mobile layouts.
7. Check navigation links, card layouts, text wrapping, and horizontal overflow.
8. Capture and save screenshots of the tested layouts.

### 9.2 Database

1. Open SQL Server Management Studio.
2. Open `docs/RaceDay_Database.sql`.
3. Execute the script according to its database setup instructions.
4. Verify that the database, tables, constraints, and sample data have been created successfully.
5. View the ERD in `docs/RaceDay_ERD.png`.
6. Review the API endpoint plan in `docs/API_Endpoint_Plan.md`.

Database setup and verification are separate from opening and testing the front-end website.

## 10. Responsive Testing Evidence


The website was tested at desktop, tablet, and mobile screen sizes to check the layout, navigation, readability, and responsiveness.

**Desktop view**

![Desktop view](screenshots/desktop-view.png)

**Tablet view**

![Tablet view](screenshots/tablet-view.png)

**Mobile view**

![Mobile view](screenshots/mobile-view.png)

## 11. Continuous Integration

GitHub Actions is used to validate the RaceDay repository structure.

The workflow is located at `.github/workflows/ci.yml`.

The workflow checks for the required project documentation and database files. Its actual checks should be reviewed to confirm whether it also validates the new website files.


## 12. Changelog

This changelog records changes made during the project. Entries should be updated as corrections and improvements are completed.

| Version | Changes |
|---|---|
| Part 1 | Created the project planning documentation, ERD, SQL database script, API endpoint plan, GitHub repository, and CI workflow. |
| Part 2 – Initial implementation | Created `index.html` as the RaceDay homepage and linked it to the external `style.css` stylesheet. |
| Part 2 – Styling | Added reusable CSS variables, base styles, typography, buttons, card designs, colours, shadows, and layout styling. |
| Part 2 – Responsive design | Added CSS Grid and Flexbox layouts, tablet and mobile media queries, relative units, keyboard-focus styles, and reduced-motion styling. |
| Part 2 – Testing | Captured desktop, tablet, and mobile screenshots and checked the website at representative viewport sizes. Record any issues found and the fixes made here. |
| Part 2 – Documentation | Updated the README to document the website, stylesheet, responsive design, testing evidence, and project changes. |

### Part 1 Feedback Corrections

For each correction, document:

- The issue identified in the feedback
- The file or project component changed
- The specific correction made
- How the correction was verified

## 13. References

The following resources support the HTML, CSS, responsive design, and accessibility techniques used in this project.

1. MDN Web Docs. HTML: HyperText Markup Language. https://developer.mozilla.org/en-US/docs/Web/HTML
2. MDN Web Docs. CSS: Cascading Style Sheets. https://developer.mozilla.org/en-US/docs/Web/CSS
3. MDN Web Docs. CSS media queries. https://developer.mozilla.org/en-US/docs/Web/CSS/CSS_media_queries/Using_media_queries
4. MDN Web Docs. CSS Grid Layout. https://developer.mozilla.org/en-US/docs/Web/CSS/CSS_grid_layout
5. MDN Web Docs. CSS Flexible Box Layout. https://developer.mozilla.org/en-US/docs/Web/CSS/CSS_flexible_box_layout
6. MDN Web Docs. Responsive images. https://developer.mozilla.org/en-US/docs/Web/HTML/Guides/Responsive_images
7. GitHub Docs. About GitHub and repositories. https://docs.github.com/en/repositories

Accessed: 12 October 2026.

## 14. SQL Demonstration Video

A demonstration video explaining the RaceDay SQL database script and its execution in SQL Server Management Studio is available at:

**YouTube:** https://youtu.be/f_h5BXToo8g

## 15. AI Use Disclosure

AI tools were used to assist with understanding requirements, explaining HTML and CSS concepts, and reviewing project documentation. The student is responsible for reviewing, testing, understanding, and adapting the code and documentation submitted for assessment.
