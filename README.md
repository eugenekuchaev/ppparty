# ppparty
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

## Overview
The ppparty app is an event-centered social media platform designed to help people find events based on their location and interests. It aims to connect event-goers with exciting and relevant experiences while providing event creators a platform to promote their events and find interested participants.

## Key Features
— **User Registration and Profile:** Create an account, customize your profile with name, location, interests, about section, contacts, and avatar.<br>
— **Account Security:** Update your email address and password.<br>
— **Friend Connections:** Add other users as friends on the platform.<br>
— **Real-Time Chat:** Engage in real-time conversations with friends.<br>
— **Event Recomendations:** Users can discover personalized event recommendations based on their location and interests.<br>
— **Event Invitations:** Invite friends to events you're participating in or hosting.<br>
— **Event Creation and Management:** Create, edit, or cancel events with notifications to participants.<br>
— **Event Participation:** Join or cancel participation in existing events.<br>
— **Advanced Search:** Search events and users by different parameters.

## Technologies Used
— **Backend:** ASP.NET Core 6<br>
— **Frontend:** Angular 15.1.6 with Bootstrap 5<br>
— **Database:** PostgreSQL with Entity Framework Core 6<br>
— **Real-Time Messaging:** SignalR<br>
— **Authentication/Authorization:** ASP.NET Core Identity<br>
— **Photo Uploading:** Cloudinary

## Installation and Setup
### Backend Setup
Make sure you have installed ASP.NET Core 6 or a higher version on your machine.<br>
Install the latest version of PostgreSQL. You can use a Docker image for convenience.<br>
Obtain a free Cloudinary account. You will need the cloud name, API key, and API secret.<br>
Configure .NET in your system to work with https.

### Environment Configuration
Set the following environmental variables in your operating system:<br>
DatabaseDefaultConnection: The connection string for the PostgreSQL database.<br>
IdentityTokenKey: Create your own key, which will be used by ASP.NET Core Identity for authentication functionality.<br>
CloudinaryCloudName: Your Cloudinary cloud name.<br>
CloudinaryApiKey: Your Cloudinary API key.<br>
CloudinaryApiSecret: Your Cloudinary API secret.

### Frontend Setup
Install Angular 15.1.6 (other versions may not work correctly) on your machine.<br>
Ensure that you have compatible versions of Node.js and npm installed.<br>
Configure this particular Angular app to work with https.

### Accessing the App
Run `dotnet run` in the 'API' folder and `ng serve` in the 'client' folder.<br>
Access the app at [https://localhost:4200](https://localhost:4200).

## Notes
The Angular app is poorly written and only serves as a demonstration of the API functionality in a convenient manner. Please feel free to contribute new pull requests and raise issues. You are also welcome to use this repository or any of its components as needed. Optimized only for 1920x1080 screen resolution.
