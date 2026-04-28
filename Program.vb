' ============================================================
' Library Management System - VB.NET
' Database: MySQL (LibraryDB)
' Table: Books (BookID, Title, Author, ISBN, Year, Genre)
' Controls: TextBox, ComboBox, Button, ListBox
' ============================================================

Imports System.Windows.Forms
Imports System.Data
Imports MySql.Data.MySqlClient

Public Class LibraryForm
    Inherits Form

    ' -------------------------------------------------------
    ' CONTROLS DECLARATION
    ' -------------------------------------------------------
    Private txtTitle    As New TextBox()
    Private txtAuthor   As New TextBox()
    Private txtISBN     As New TextBox()
    Private txtYear     As New TextBox()
    Private cmbGenre    As New ComboBox()
    Private lstBooks    As New ListBox()

    Private btnAdd      As New Button()
    Private btnDisplay  As New Button()
    Private btnUpdate   As New Button()
    Private btnDelete   As New Button()

    Private lblTitle    As New Label()
    Private lblAuthor   As New Label()
    Private lblISBN     As New Label()
    Private lblYear     As New Label()
    Private lblGenre    As New Label()
    Private lblList     As New Label()

    ' -------------------------------------------------------
    ' DATABASE CONNECTION STRING
    ' Update Server/Port/User/Password as needed
    ' -------------------------------------------------------
    Private Const connString As String =
        "Server=localhost;Port=3306;Database=LibraryDB;" &
        "User Id=root;Password=yourpassword;"

    ' -------------------------------------------------------
    ' ARRAY: Temporarily holds book titles before populating
    '        the ListBox (satisfies Array requirement)
    ' -------------------------------------------------------
    Private bookTitlesArray() As String

    ' -------------------------------------------------------
    ' ENTRY POINT
    ' -------------------------------------------------------
    <STAThread>
    Public Shared Sub Main()
        Application.EnableVisualStyles()
        Application.SetCompatibleTextRenderingDefault(False)
        Application.Run(New LibraryForm())
    End Sub

    ' -------------------------------------------------------
    ' CONSTRUCTOR: Build UI & wire events
    ' -------------------------------------------------------
    Public Sub New()
        InitializeComponents()
        SetupDatabase()
    End Sub

    ' -------------------------------------------------------
    ' UI LAYOUT
    ' -------------------------------------------------------
    Private Sub InitializeComponents()
        Me.Text = "Library Management System"
        Me.Size = New Drawing.Size(780, 560)
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.BackColor = Drawing.Color.FromArgb(245, 248, 255)

        ' --- Group box for inputs ---
        Dim grpInput As New GroupBox()
        grpInput.Text = "Book Details"
        grpInput.Location = New Drawing.Point(15, 10)
        grpInput.Size = New Drawing.Size(440, 210)
        grpInput.Font = New Drawing.Font("Segoe UI", 9, Drawing.FontStyle.Bold)

        ' Helper: create a label
        Dim MakeLabel = Function(txt As String, x As Integer, y As Integer) As Label
                            Dim l As New Label()
                            l.Text = txt
                            l.Location = New Drawing.Point(x, y)
                            l.Size = New Drawing.Size(75, 20)
                            l.Font = New Drawing.Font("Segoe UI", 9)
                            Return l
                        End Function

        ' Helper: create a textbox
        Dim MakeTextBox = Function(x As Integer, y As Integer, w As Integer) As TextBox
                              Dim t As New TextBox()
                              t.Location = New Drawing.Point(x, y)
                              t.Size = New Drawing.Size(w, 24)
                              t.Font = New Drawing.Font("Segoe UI", 9)
                              Return t
                          End Function

        ' Label + TextBox rows
        lblTitle  = MakeLabel("Title:",  10, 28)
        txtTitle  = MakeTextBox(90, 25, 330)

        lblAuthor = MakeLabel("Author:", 10, 60)
        txtAuthor = MakeTextBox(90, 57, 330)

        lblISBN   = MakeLabel("ISBN:",   10, 92)
        txtISBN   = MakeTextBox(90, 89, 200)

        lblYear   = MakeLabel("Year:",   10, 124)
        txtYear   = MakeTextBox(90, 121, 100)

        lblGenre  = MakeLabel("Genre:",  10, 156)

        ' ComboBox for Genre
        cmbGenre.Location = New Drawing.Point(90, 153)
        cmbGenre.Size     = New Drawing.Size(180, 24)
        cmbGenre.Font     = New Drawing.Font("Segoe UI", 9)
        cmbGenre.DropDownStyle = ComboBoxStyle.DropDownList
        cmbGenre.Items.AddRange(New String() {
            "Fiction", "Non-Fiction", "Science",
            "Biography", "History", "Technology",
            "Children", "Romance", "Mystery", "Other"
        })

        grpInput.Controls.AddRange(New Control() {
            lblTitle, txtTitle, lblAuthor, txtAuthor,
            lblISBN, txtISBN, lblYear, txtYear,
            lblGenre, cmbGenre
        })

        ' --- Buttons panel ---
        Dim grpButtons As New GroupBox()
        grpButtons.Text = "Actions"
        grpButtons.Location = New Drawing.Point(15, 230)
        grpButtons.Size = New Drawing.Size(440, 70)
        grpButtons.Font = New Drawing.Font("Segoe UI", 9, Drawing.FontStyle.Bold)

        Dim MakeButton = Function(txt As String, x As Integer,
                                  bg As Drawing.Color) As Button
                             Dim b As New Button()
                             b.Text = txt
                             b.Location = New Drawing.Point(x, 25)
                             b.Size = New Drawing.Size(95, 32)
                             b.BackColor = bg
                             b.ForeColor = Drawing.Color.White
                             b.FlatStyle = FlatStyle.Flat
                             b.Font = New Drawing.Font("Segoe UI", 9,
                                                       Drawing.FontStyle.Bold)
                             b.FlatAppearance.BorderSize = 0
                             Return b
                         End Function

        btnAdd     = MakeButton("Add Book",      8,   Drawing.Color.FromArgb(46, 139, 87))
        btnDisplay = MakeButton("Display Books", 112, Drawing.Color.FromArgb(30, 120, 200))
        btnUpdate  = MakeButton("Update Book",   216, Drawing.Color.FromArgb(204, 120, 0))
        btnDelete  = MakeButton("Delete Book",   320, Drawing.Color.FromArgb(180, 40, 40))

        grpButtons.Controls.AddRange(New Control() {
            btnAdd, btnDisplay, btnUpdate, btnDelete
        })

        ' --- ListBox panel ---
        Dim grpList As New GroupBox()
        grpList.Text = "Book List"
        grpList.Location = New Drawing.Point(470, 10)
        grpList.Size = New Drawing.Size(285, 470)
        grpList.Font = New Drawing.Font("Segoe UI", 9, Drawing.FontStyle.Bold)

        lstBooks.Location = New Drawing.Point(8, 22)
        lstBooks.Size = New Drawing.Size(268, 438)
        lstBooks.Font = New Drawing.Font("Segoe UI", 8.5)
        lstBooks.HorizontalScrollbar = True
        grpList.Controls.Add(lstBooks)

        ' Add all groups to form
        Me.Controls.AddRange(New Control() {grpInput, grpButtons, grpList})

        ' --- Wire events ---
        AddHandler btnAdd.Click,     AddressOf AddBook
        AddHandler btnDisplay.Click, AddressOf DisplayBooks
        AddHandler btnUpdate.Click,  AddressOf UpdateBook
        AddHandler btnDelete.Click,  AddressOf DeleteBook
        AddHandler lstBooks.SelectedIndexChanged, AddressOf LoadSelectedBook
    End Sub

    ' -------------------------------------------------------
    ' DATABASE SETUP: Create DB and Table if not existing
    ' -------------------------------------------------------
    Private Sub SetupDatabase()
        Try
            ' Connect without specifying the database first
            Dim setupConn As String =
                "Server=localhost;Port=3306;User Id=root;Password=yourpassword;"

            Using conn As New MySqlConnection(setupConn)
                conn.Open()

                ' Create database
                Dim cmdDB As New MySqlCommand(
                    "CREATE DATABASE IF NOT EXISTS LibraryDB;", conn)
                cmdDB.ExecuteNonQuery()

                ' Use the database and create the Books table
                Dim cmdTable As New MySqlCommand(
                    "USE LibraryDB; " &
                    "CREATE TABLE IF NOT EXISTS Books (" &
                    "  BookID INT PRIMARY KEY AUTO_INCREMENT, " &
                    "  Title  VARCHAR(200) NOT NULL, " &
                    "  Author VARCHAR(150) NOT NULL, " &
                    "  ISBN   VARCHAR(20), " &
                    "  Year   INT, " &
                    "  Genre  VARCHAR(50)" &
                    ");", conn)
                cmdTable.ExecuteNonQuery()
            End Using
        Catch ex As Exception
            MessageBox.Show("DB Setup Error: " & ex.Message,
                            "Database Error", MessageBoxButtons.OK,
                            MessageBoxIcon.Warning)
        End Try
    End Sub

    ' -------------------------------------------------------
    ' HELPER: Clear all input fields
    ' -------------------------------------------------------
    Private Sub ClearInputs()
        txtTitle.Clear()
        txtAuthor.Clear()
        txtISBN.Clear()
        txtYear.Clear()
        cmbGenre.SelectedIndex = -1
    End Sub

    ' -------------------------------------------------------
    ' HELPER: Validate required inputs
    ' -------------------------------------------------------
    Private Function ValidateInputs() As Boolean
        If String.IsNullOrWhiteSpace(txtTitle.Text) Then
            MessageBox.Show("Please enter the book title.",
                            "Validation", MessageBoxButtons.OK,
                            MessageBoxIcon.Warning)
            txtTitle.Focus()
            Return False
        End If
        If String.IsNullOrWhiteSpace(txtAuthor.Text) Then
            MessageBox.Show("Please enter the author name.",
                            "Validation", MessageBoxButtons.OK,
                            MessageBoxIcon.Warning)
            txtAuthor.Focus()
            Return False
        End If
        Return True
    End Function

    ' -------------------------------------------------------
    ' FUNCTION 1: AddBook
    ' Inserts a new record into the Books table
    ' -------------------------------------------------------
    Private Sub AddBook(sender As Object, e As EventArgs)
        If Not ValidateInputs() Then Return

        Try
            Using conn As New MySqlConnection(connString)
                conn.Open()

                Dim sql As String =
                    "INSERT INTO Books (Title, Author, ISBN, Year, Genre) " &
                    "VALUES (@Title, @Author, @ISBN, @Year, @Genre)"

                Using cmd As New MySqlCommand(sql, conn)
                    cmd.Parameters.AddWithValue("@Title",  txtTitle.Text.Trim())
                    cmd.Parameters.AddWithValue("@Author", txtAuthor.Text.Trim())
                    cmd.Parameters.AddWithValue("@ISBN",   txtISBN.Text.Trim())

                    ' Parse Year safely
                    Dim yr As Integer = 0
                    Integer.TryParse(txtYear.Text.Trim(), yr)
                    cmd.Parameters.AddWithValue("@Year", If(yr > 0, yr, DBNull.Value))

                    cmd.Parameters.AddWithValue("@Genre",
                        If(cmbGenre.SelectedIndex >= 0,
                           cmbGenre.SelectedItem.ToString(),
                           "Other"))

                    cmd.ExecuteNonQuery()
                End Using
            End Using

            MessageBox.Show("Book added successfully!",
                            "Success", MessageBoxButtons.OK,
                            MessageBoxIcon.Information)
            ClearInputs()
            DisplayBooks(Nothing, Nothing)  ' Refresh list

        Catch ex As Exception
            MessageBox.Show("Error adding book: " & ex.Message,
                            "Database Error", MessageBoxButtons.OK,
                            MessageBoxIcon.Error)
        End Try
    End Sub

    ' -------------------------------------------------------
    ' FUNCTION 2: DisplayBooks
    ' Retrieves all records and populates the ListBox
    ' Uses an array as intermediate storage (Array requirement)
    ' -------------------------------------------------------
    Private Sub DisplayBooks(sender As Object, e As EventArgs)
        Try
            Dim dataList As New List(Of String)()  ' Dynamic list first

            Using conn As New MySqlConnection(connString)
                conn.Open()

                Dim sql As String =
                    "SELECT BookID, Title, Author, ISBN, Year, Genre " &
                    "FROM Books ORDER BY Title ASC"

                Using cmd As New MySqlCommand(sql, conn)
                    Using reader As MySqlDataReader = cmd.ExecuteReader()
                        While reader.Read()
                            ' Format: [ID] Title - Author (Year) [Genre]
                            Dim entry As String =
                                String.Format("[{0}] {1} - {2} ({3}) [{4}]",
                                    reader("BookID").ToString(),
                                    reader("Title").ToString(),
                                    reader("Author").ToString(),
                                    If(IsDBNull(reader("Year")), "N/A",
                                       reader("Year").ToString()),
                                    reader("Genre").ToString())
                            dataList.Add(entry)
                        End While
                    End Using
                End Using
            End Using

            ' *** Convert List to Array before populating ListBox ***
            bookTitlesArray = dataList.ToArray()

            ' Populate ListBox from the array
            lstBooks.Items.Clear()
            For Each bookEntry As String In bookTitlesArray
                lstBooks.Items.Add(bookEntry)
            Next

            If lstBooks.Items.Count = 0 Then
                lstBooks.Items.Add("(No books found)")
            End If

        Catch ex As Exception
            MessageBox.Show("Error retrieving books: " & ex.Message,
                            "Database Error", MessageBoxButtons.OK,
                            MessageBoxIcon.Error)
        End Try
    End Sub

    ' -------------------------------------------------------
    ' HELPER: Extract BookID from selected ListBox item
    ' Format: "[ID] Title - ..."
    ' -------------------------------------------------------
    Private Function GetSelectedBookID() As Integer
        If lstBooks.SelectedIndex < 0 Then Return -1
        Dim item As String = lstBooks.SelectedItem.ToString()
        If Not item.StartsWith("[") Then Return -1
        Dim closeBracket As Integer = item.IndexOf("]")
        If closeBracket < 0 Then Return -1
        Dim idStr As String = item.Substring(1, closeBracket - 1)
        Dim id As Integer = -1
        Integer.TryParse(idStr, id)
        Return id
    End Function

    ' -------------------------------------------------------
    ' EVENT: Load selected book's data into input fields
    ' -------------------------------------------------------
    Private Sub LoadSelectedBook(sender As Object, e As EventArgs)
        Dim bookID As Integer = GetSelectedBookID()
        If bookID < 0 Then Return

        Try
            Using conn As New MySqlConnection(connString)
                conn.Open()
                Dim sql As String =
                    "SELECT Title, Author, ISBN, Year, Genre " &
                    "FROM Books WHERE BookID = @ID"

                Using cmd As New MySqlCommand(sql, conn)
                    cmd.Parameters.AddWithValue("@ID", bookID)
                    Using reader As MySqlDataReader = cmd.ExecuteReader()
                        If reader.Read() Then
                            txtTitle.Text  = reader("Title").ToString()
                            txtAuthor.Text = reader("Author").ToString()
                            txtISBN.Text   = reader("ISBN").ToString()
                            txtYear.Text   = If(IsDBNull(reader("Year")), "",
                                               reader("Year").ToString())

                            Dim genre As String = reader("Genre").ToString()
                            cmbGenre.SelectedIndex = cmbGenre.Items.IndexOf(genre)
                        End If
                    End Using
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Error loading book details: " & ex.Message,
                            "Database Error", MessageBoxButtons.OK,
                            MessageBoxIcon.Error)
        End Try
    End Sub

    ' -------------------------------------------------------
    ' FUNCTION 3: UpdateBook
    ' Updates the selected book record in the database
    ' -------------------------------------------------------
    Private Sub UpdateBook(sender As Object, e As EventArgs)
        Dim bookID As Integer = GetSelectedBookID()
        If bookID < 0 Then
            MessageBox.Show("Please select a book from the list to update.",
                            "No Selection", MessageBoxButtons.OK,
                            MessageBoxIcon.Warning)
            Return
        End If

        If Not ValidateInputs() Then Return

        Dim confirm As DialogResult =
            MessageBox.Show("Update the selected book?",
                            "Confirm Update",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question)
        If confirm <> DialogResult.Yes Then Return

        Try
            Using conn As New MySqlConnection(connString)
                conn.Open()

                Dim sql As String =
                    "UPDATE Books SET " &
                    "  Title  = @Title, " &
                    "  Author = @Author, " &
                    "  ISBN   = @ISBN, " &
                    "  Year   = @Year, " &
                    "  Genre  = @Genre " &
                    "WHERE BookID = @ID"

                Using cmd As New MySqlCommand(sql, conn)
                    cmd.Parameters.AddWithValue("@Title",  txtTitle.Text.Trim())
                    cmd.Parameters.AddWithValue("@Author", txtAuthor.Text.Trim())
                    cmd.Parameters.AddWithValue("@ISBN",   txtISBN.Text.Trim())

                    Dim yr As Integer = 0
                    Integer.TryParse(txtYear.Text.Trim(), yr)
                    cmd.Parameters.AddWithValue("@Year",
                        If(yr > 0, CObj(yr), DBNull.Value))

                    cmd.Parameters.AddWithValue("@Genre",
                        If(cmbGenre.SelectedIndex >= 0,
                           cmbGenre.SelectedItem.ToString(), "Other"))
                    cmd.Parameters.AddWithValue("@ID", bookID)

                    cmd.ExecuteNonQuery()
                End Using
            End Using

            MessageBox.Show("Book updated successfully!",
                            "Success", MessageBoxButtons.OK,
                            MessageBoxIcon.Information)
            ClearInputs()
            DisplayBooks(Nothing, Nothing)

        Catch ex As Exception
            MessageBox.Show("Error updating book: " & ex.Message,
                            "Database Error", MessageBoxButtons.OK,
                            MessageBoxIcon.Error)
        End Try
    End Sub

    ' -------------------------------------------------------
    ' FUNCTION 4: DeleteBook
    ' Removes the selected book record from the database
    ' -------------------------------------------------------
    Private Sub DeleteBook(sender As Object, e As EventArgs)
        Dim bookID As Integer = GetSelectedBookID()
        If bookID < 0 Then
            MessageBox.Show("Please select a book from the list to delete.",
                            "No Selection", MessageBoxButtons.OK,
                            MessageBoxIcon.Warning)
            Return
        End If

        Dim confirm As DialogResult =
            MessageBox.Show("Are you sure you want to DELETE this book? " &
                            "This action cannot be undone.",
                            "Confirm Delete",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Warning)
        If confirm <> DialogResult.Yes Then Return

        Try
            Using conn As New MySqlConnection(connString)
                conn.Open()

                Dim sql As String = "DELETE FROM Books WHERE BookID = @ID"
                Using cmd As New MySqlCommand(sql, conn)
                    cmd.Parameters.AddWithValue("@ID", bookID)
                    cmd.ExecuteNonQuery()
                End Using
            End Using

            MessageBox.Show("Book deleted successfully!",
                            "Success", MessageBoxButtons.OK,
                            MessageBoxIcon.Information)
            ClearInputs()
            DisplayBooks(Nothing, Nothing)

        Catch ex As Exception
            MessageBox.Show("Error deleting book: " & ex.Message,
                            "Database Error", MessageBoxButtons.OK,
                            MessageBoxIcon.Error)
        End Try
    End Sub

End Class
' ============================================================
' END OF FILE
' ============================================================
