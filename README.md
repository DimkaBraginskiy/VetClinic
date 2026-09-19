## Vet Clinic Management System

The Vet Clinic Management System is a robust web-based application designed to streamline appointment scheduling, clinic administration, and patient management[cite: 2]. Built as a final project for the Polsko-Japońska Akademia Technik Komputerowych (PJATK), the platform enables customers to seamlessly book and manage veterinary services across multiple appointment modes while allowing clinic staff to coordinate shifts and treatments[cite: 2].

## Core Features

*   **Role-Based Access Control:** The system supports two primary user roles: Customers and Veterinarians (categorized further as Doctors or Surgeons based on treatment competencies)[cite: 2].
*   **Dynamic Appointment Scheduling:** Customers can schedule two types of appointments: Treatments (e.g., Checkup, Vaccine, Surgery, Eyesight, Chiropractic) and Consultations[cite: 2].
*   **Flexible Appointment Modes:** Appointments can be conducted Online (with auto-generated meeting links), At Home (requiring a validated customer address), or In Clinic (assigned to specific cabinets within a selected clinic)[cite: 2].
*   **Dynamic Mode Switching:** An appointment's mode can be dynamically changed after creation (e.g., switching from Online to In Clinic) provided valid data is supplied before the appointment starts[cite: 2].
*   **Animal Management:** Customers can register, manage, and remove multiple pet profiles tied to their accounts[cite: 2].
*   **Loyalty & Discount System:** Total appointment prices can be reduced by applying promotional discount codes or redeeming accumulated loyalty points, where one loyalty point equates to 10% of a currency unit[cite: 2].
*   **Veterinarian Administration:** Clinic staff can manage their available shifts, list their assigned appointments, and update the treatments they are authorized to perform[cite: 2].

## Technology Stack

*   **Backend:** Developed in C# utilizing the .NET 9.0 framework, ensuring robust support for object-oriented paradigms and type-safe data access[cite: 2].
*   **Database & Persistence:** SQLite is employed as a self-contained, lightweight relational database engine to consistently persist all application data[cite: 2].
*   **ORM:** Entity Framework Core maps C# domain classes to the SQLite database, translating LINQ queries into database operations and managing inheritance structures (e.g., table-per-hierarchy mapping for appointment subtypes)[cite: 2].
*   **Frontend:** The user interface is built with React, delivering a responsive single-page application (SPA) that guides users through a step-by-step scheduling flow and communicates with the C# backend via HTTP[cite: 2].

## System Architecture & Design

The application follows a client-server architecture with a strict separation between the React-based frontend and the C# business logic layer[cite: 2]. Domain entities are structured using object-oriented principles, utilizing abstract classes and dynamic inheritance to manage the varying requirements of different appointment modes[cite: 2]. Appointments are governed by a state machine, progressing through strictly defined statuses: Scheduled, In Progress, Completed, and Cancelled[cite: 2]. The design also incorporates derived attributes, such as auto-generated appointment titles, and pre-defined enumerations for consistent data validation across animal species and treatment types[cite: 2].
