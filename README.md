# D365-CE-Complaints-Project
# D365 Complaint Management App

## Overview
This project is a model-driven app built on Microsoft Dynamics 365 and Power Platform to automate complaint management. It includes:
- Logging complaints with geolocation lookup and inspector assignment.
- Round-robin assignment of complaints to inspectors using a custom Dataverse plugin.
- Automated email notifications for inspectors and residents using Power Automate.

## Features
- **Complaint Logging**: Residents can log complaints with their email, details, IP address, and timestamp.
- **Geolocation Lookup**: Captures latitude and longitude from the IP address using [ipgeolocation.io](https://ipgeolocation.io/).
- **Round-Robin Assignment**: A custom plugin assigns complaints to inspectors in a round-robin fashion.
- **Notifications**: Inspectors receive an email when assigned, and residents are notified when their complaint is actioned.
- **Complaint Counts Dashboard**: Visualises complaints per user using embedded Power BI reports.
<img width="953" alt="image" src="https://github.com/user-attachments/assets/1424aa16-0efb-4e9f-b952-65e9a41ce77d" />
- **ALTERNATIVE due to licensing - Complaint Counts Dashboard**: Visualises complaints per user using Charts in views .
- <img width="358" alt="image" src="https://github.com/user-attachments/assets/d3a08382-751a-4882-a30c-07856d1baa13" />


## Prerequisites
- **Dataverse Environment**: Microsoft Power Apps environment with Dataverse enabled.
- **Plugin Registration Tool**: To register custom plugins.
- **SendGrid API Key**: For email notifications.

## Solution Components
1. **Dataverse Tables**:
   - **Complaints**: Stores complaints with fields like Assigned Inspector, Latitude, Longitude, etc.
   - **Inspectors**: Stores active inspectors.
2. **Custom Plugin**:
   - Automates round-robin assignment of complaints to inspectors.
3. **Power Automate Flows**:
   - Sends email notifications for complaint updates.
   - <img width="257" alt="image" src="https://github.com/user-attachments/assets/b5161282-54c5-470c-8087-8aea03122b7f" />

   - <img width="328" alt="image" src="https://github.com/user-attachments/assets/e362cf86-9a2c-48f0-a95a-385ea3163e86" />

4. **Power BI Dashboard**:
   - Visualises complaint trends.
<img width="454" alt="image" src="https://github.com/user-attachments/assets/1557384a-2b71-447f-b4cd-1d12aab84347" />

## How to Import the Solution
1. Download the `.zip` file from this repository.
2. Open the [Power Apps Maker Portal](https://make.powerapps.com/).
3. Navigate to **Solutions** → **Import**.
4. Upload the `.zip` file and follow the prompts.


## Plugin Code
The plugin code used for round-robin assignment is included in the `src` folder. See the [Plugin Code](#plugin-code) section for more details.

## Plugin Code
### Overview
The plugin automates the process of assigning inspectors to complaints in a round-robin manner, ensuring balanced distribution.

### Key Features
- Fetches active inspectors dynamically.
- Tracks the last assigned inspector to determine the next one.

### How to Deploy the Plugin
1. Open the Plugin Registration Tool.
2. Register the plugin assembly (DLL file from the `bin/Debug` folder).
3. Add a new step:
   - **Message**: `Create`
   - **Primary Entity**: `Complaints`
   - **Execution Mode**: `PreOperation` or `PostOperation`
   - **Stage**: `PreOperation`
