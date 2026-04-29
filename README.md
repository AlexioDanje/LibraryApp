# Library Management System

A Windows desktop application for managing library books built with VB.NET and MySQL.

## Features
- Add new books with title, author, ISBN, year, and genre
- Display all books in a searchable list
- Update existing book information
- Delete books from the database
- Automatic database and table creation

## Requirements
- Windows 10/11
- MySQL Server 8.0 or higher

## MySQL Setup Instructions

### 1. Install MySQL Server
Download and install MySQL Server from: https://dev.mysql.com/downloads/mysql/

During installation:
- Choose "Developer Default" or "Server only"
- Set root password to: `yourpassword` (or change it in the code)
- Keep default port: 3306
- Complete the installation

### 2. Verify MySQL is Running
Open Command Prompt and run:
```cmd
mysql -u root -p
```
Enter password: `yourpassword`

If connected successfully, type `exit` to quit.

### 3. Run the Application

**First time setup:**
1. Make sure MySQL Server is installed and running
2. Build the application (see "Building from Source" section below)
3. Navigate to the `publish` folder created by the build
4. Double-click `LibraryApp.exe`

The application will automatically:
- Create the `LibraryDB` database
- Create the `Books` table
- Be ready to use

## Changing MySQL Password

If you want to use a different MySQL password:

1. Open `Program.vb` in a text editor
2. Find line 38:
   ```vb
   "User Id=root;Password=yourpassword;"
   ```
3. Change `yourpassword` to your actual MySQL root password
4. Rebuild the application (see Building section)

## Setup and Building from Source

### Using an IDE (Visual Studio) on Windows

1. **Clone the repository**
   - Open your terminal or command prompt
   - Run: `git clone <repository-url>` (replace `<repository-url>` with the actual URL)
   - Navigate to the cloned project folder

2. **Open in Visual Studio**
   - Open Visual Studio
   - Select "Open a project or solution"
   - Navigate to the cloned folder and select `LibraryApp.vbproj`

3. **Build the Application**
   - In Visual Studio, ensure the solution configuration is set to `Release`
   - Right-click the project in the Solution Explorer and select **Publish**
   - Choose a target (Folder) and configure it for a Windows self-contained deployment (e.g., target runtime `win-x64`)
   - Click **Publish** to build the `.exe`

4. **Run the Application**
   - Go to the publish folder you configured
   - Double-click `LibraryApp.exe`

### Using Command Line on Windows

1. **Clone the repository**
   - Open Command Prompt
   - Run: `git clone <repository-url>`
   - Navigate to the project folder

2. **Install .NET 8 SDK**
   - Download from: https://dotnet.microsoft.com/download/dotnet/8.0
   - Run the installer and follow the prompts

3. **Build the Application**
   - Run this command in the project directory:
   ```cmd
   dotnet publish LibraryApp.vbproj -c Release -r win-x64 --self-contained true -o ./publish
   ```

4. **Run the Application**
   - Go to the `publish` folder
   - Double-click `LibraryApp.exe`

## Troubleshooting

### "Can't connect to MySQL server"
- Verify MySQL service is running (Services → MySQL80)
- Check the password in Program.vb matches your MySQL root password
- Ensure MySQL is listening on port 3306

### "Access denied for user 'root'"
- The password in the code doesn't match your MySQL root password
- Update the password in Program.vb and rebuild

### Application won't start
- Make sure all files in the publish folder are present (don't move just the .exe)
- Try running as Administrator

## Database Schema

The application creates this table structure:

```sql
Database: LibraryDB
Table: Books
- BookID (INT, Primary Key, Auto Increment)
- Title (VARCHAR(200), NOT NULL)
- Author (VARCHAR(150), NOT NULL)
- ISBN (VARCHAR(20))
- Year (INT)
- Genre (VARCHAR(50))
```

## Support

For issues or questions, contact the developer.
