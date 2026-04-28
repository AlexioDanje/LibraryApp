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
- Extract the `LibraryApp-Windows.zip` file
- Navigate to the `publish-standalone` folder
- Double-click `LibraryApp.exe`

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

## Building from Source

If you have the source code and want to rebuild:

### On Windows:
1. Install .NET 8 SDK: https://dotnet.microsoft.com/download/dotnet/8.0
2. Open Command Prompt in the project folder
3. Run:
   ```cmd
   dotnet publish LibraryApp.vbproj -c Release -r win-x64 --self-contained true -o ./publish
   ```
4. The executable will be in the `publish` folder

### On Linux/Mac (using Docker):
```bash
docker run --rm -v $(pwd):/app -w /app mcr.microsoft.com/dotnet/sdk:8.0 dotnet publish LibraryApp.vbproj -c Release -r win-x64 --self-contained true -o /app/publish
```

## Troubleshooting

### "Can't connect to MySQL server"
- Verify MySQL service is running (Services → MySQL80)
- Check the password in Program.vb matches your MySQL root password
- Ensure MySQL is listening on port 3306

### "Access denied for user 'root'"
- The password in the code doesn't match your MySQL root password
- Update the password in Program.vb and rebuild

### Application won't start
- Make sure all files in the publish-standalone folder are present
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
