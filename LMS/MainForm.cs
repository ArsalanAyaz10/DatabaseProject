using LMS;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

public class MainForm : Form
{
    private static readonly string JDBC_URL = "Data Source=DESKTOP-FEPI9CN\\SQLEXPRESS;Initial Catalog=E-Book;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";
    private Library library = new Library();

    [STAThread]
    public static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new MainForm());
    }

    public MainForm()
    {
        CreateAndShowGUI();
        library.Project_Info();
    }

    private void CreateAndShowGUI()
    {
        this.Text = "Book Inventory Management System";
        this.Size = new Size(500, 500);
        this.FormClosing += (sender, e) => SaveBookData();

        var panel = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(10) };

        var addBookButton = new Button { Text = "Add Book", Dock = DockStyle.Top };
        var updateProgressButton = new Button { Text = "Update Progress", Dock = DockStyle.Top };
        var viewStatusButton = new Button { Text = "View Book Status", Dock = DockStyle.Top };
        var DeleteButton = new Button { Text = "Delete Book", Dock = DockStyle.Top };
        var exitButton = new Button { Text = "Exit", Dock = DockStyle.Top };

        var deleteButton = new Button { Text = "Delete Book", Dock = DockStyle.Top };
        

       

        panel.Controls.Add(addBookButton);
        panel.Controls.Add(updateProgressButton);
        panel.Controls.Add(viewStatusButton);
        panel.Controls.Add(deleteButton);
        panel.Controls.Add(exitButton);

        this.Controls.Add(panel);

        addBookButton.Click += (sender, e) => ShowAddBookDialog();
        updateProgressButton.Click += (sender, e) => ShowUpdateProgressDialog();
        viewStatusButton.Click += (sender, e) => ShowViewStatusDialog();
        deleteButton.Click += (sender, e) => ShowDeleteBookDialog();
        exitButton.Click += (sender, e) =>
        {
            SaveBookData();
            Application.Exit();
        };
    }

    private void ShowAddBookDialog()
    {
        var dialog = new Form { Text = "Add Book", Size = new Size(400, 400), FormBorderStyle = FormBorderStyle.FixedDialog };
        var panel = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(10) };

        var nameLabel = new Label { Text = "Book Name:", Dock = DockStyle.Top };
        var nameField = new TextBox { Dock = DockStyle.Top };
        var authorLabel = new Label { Text = "Author Name:", Dock = DockStyle.Top };
        var authorField = new TextBox { Dock = DockStyle.Top };
        var serialLabel = new Label { Text = "Serial Number:", Dock = DockStyle.Top };
        var serialField = new TextBox { Dock = DockStyle.Top };
        var chaptersLabel = new Label { Text = "Chapters Read:", Dock = DockStyle.Top };
        var chaptersField = new TextBox { Dock = DockStyle.Top };
        var pagesLabel = new Label { Text = "Pages Read:", Dock = DockStyle.Top };
        var pagesField = new TextBox { Dock = DockStyle.Top };
        var timeLabel = new Label { Text = "Time Read (minutes):", Dock = DockStyle.Top };
        var timeField = new TextBox { Dock = DockStyle.Top };
        var addButton = new Button { Text = "Add", Dock = DockStyle.Top };

        panel.Controls.Add(nameLabel);
        panel.Controls.Add(nameField);
        panel.Controls.Add(authorLabel);
        panel.Controls.Add(authorField);
        panel.Controls.Add(serialLabel);
        panel.Controls.Add(serialField);
        panel.Controls.Add(chaptersLabel);
        panel.Controls.Add(chaptersField);
        panel.Controls.Add(pagesLabel);
        panel.Controls.Add(pagesField);
        panel.Controls.Add(timeLabel);
        panel.Controls.Add(timeField);
        panel.Controls.Add(addButton);

        dialog.Controls.Add(panel);

        addButton.Click += (sender, e) =>
        {
            if (int.TryParse(pagesField.Text, out int pagesRead) && int.TryParse(chaptersField.Text, out int chaptersRead) && int.TryParse(timeField.Text, out int timeRead))
            {
                var name = nameField.Text;
                var author = authorField.Text;
                var serialNumber = serialField.Text;

                var book = new Book(name, author, serialNumber);
                book.UpdatePages(pagesRead);
                book.UpdateChapter(chaptersRead);
                book.UpdateTime(timeRead);

                library.addBook(book);

                SaveBookData(); // Save data after adding a book
                MessageBox.Show("Book added successfully!");
                dialog.Close();
            }
            else
            {
                MessageBox.Show("Please enter valid numbers for pages read, chapters read, and time read.");
            }
        };

        dialog.ShowDialog(this);
    }

    private void ShowUpdateProgressDialog()
    {
        if (library.getBooks().Count == 0)
        {
            MessageBox.Show("No books added yet!");
            return;
        }

        var dialog = new Form { Text = "Update Progress", Size = new Size(400, 400), FormBorderStyle = FormBorderStyle.FixedDialog };
        var panel = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(10) };

        var selectBookLabel = new Label { Text = "Select Book:", Dock = DockStyle.Top };
        var bookComboBox = new ComboBox { DataSource = library.getBooks(), Dock = DockStyle.Top, DisplayMember = "B_Name" };
        var chaptersLabel = new Label { Text = "Chapters Read:", Dock = DockStyle.Top };
        var chaptersField = new TextBox { Dock = DockStyle.Top };
        var timeLabel = new Label { Text = "Time Read (minutes):", Dock = DockStyle.Top };
        var timeField = new TextBox { Dock = DockStyle.Top };
        var pagesLabel = new Label { Text = "Pages Read:", Dock = DockStyle.Top };
        var pagesField = new TextBox { Dock = DockStyle.Top };
        var updateButton = new Button { Text = "Update", Dock = DockStyle.Top };

        panel.Controls.Add(selectBookLabel);
        panel.Controls.Add(bookComboBox);
        panel.Controls.Add(chaptersLabel);
        panel.Controls.Add(chaptersField);
        panel.Controls.Add(timeLabel);
        panel.Controls.Add(timeField);
        panel.Controls.Add(pagesLabel);
        panel.Controls.Add(pagesField);
        panel.Controls.Add(updateButton);

        dialog.Controls.Add(panel);

        updateButton.Click += (sender, e) =>
        {
            var selectedBook = (Book)bookComboBox.SelectedItem;
            if (selectedBook != null)
            {
                if (int.TryParse(chaptersField.Text, out int chaptersRead) &&
                    int.TryParse(timeField.Text, out int timeRead) &&
                    int.TryParse(pagesField.Text, out int pagesRead))
                {
                    selectedBook.UpdateChapter(chaptersRead);
                    selectedBook.UpdateTime(timeRead);
                    selectedBook.UpdatePages(pagesRead);

                    var dialogResult = MessageBox.Show("Has the book been completed?", "Book Completion", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        selectedBook.MarkCompleted();
                    }

                    SaveBookData(); // Save data after updating progress
                    MessageBox.Show("Progress updated successfully!");
                    dialog.Close();
                }
                else
                {
                    MessageBox.Show("Please enter valid numbers for chapters, time, and pages read.");
                }
            }
        };

        dialog.ShowDialog(this);
    }

    private void ShowViewStatusDialog()
    {
        FetchBookDataFromDatabase();

        if (library.getBooks().Count == 0)
        {
            MessageBox.Show("No books added yet!");
            return;
        }

        var dialog = new Form { Text = "View Book Status", Size = new Size(400, 400), FormBorderStyle = FormBorderStyle.FixedDialog };
        var statusArea = new TextBox { Multiline = true, Dock = DockStyle.Fill, ReadOnly = true, ScrollBars = ScrollBars.Vertical };
        var bookComboBox = new ComboBox { DataSource = library.getBooks(), DisplayMember = "B_Name", Dock = DockStyle.Top };
    

        UpdateStatusText(statusArea); // Update the status text

        dialog.Controls.Add(statusArea);
        dialog.Controls.Add(bookComboBox);
  

        var refreshButton = new Button { Text = "Refresh", Dock = DockStyle.Bottom };
        dialog.Controls.Add(refreshButton);

        refreshButton.Click += (sender, e) =>
        {
            library.clearBooks(); // Clear existing books
            FetchBookDataFromDatabase(); // Fetch updated data
            UpdateStatusText(statusArea); // Update the status text
        };

        dialog.ShowDialog(this);
    }

    private void ShowDeleteBookDialog()
    {
        if (library.getBooks().Count == 0)
        {
            MessageBox.Show("No books added yet!");
            return;
        }

        var dialog = new Form { Text = "Delete Book", Size = new Size(300, 150), FormBorderStyle = FormBorderStyle.FixedDialog };
        var panel = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(10) };

        var selectBookLabel = new Label { Text = "Select Book to Delete:", Dock = DockStyle.Top };
        var bookComboBox = new ComboBox { DataSource = library.getBooks(), DisplayMember = "B_Name", Dock = DockStyle.Top };
        var deleteButton = new Button { Text = "Delete", Dock = DockStyle.Top };

        panel.Controls.Add(selectBookLabel);
        panel.Controls.Add(bookComboBox);
        panel.Controls.Add(deleteButton);

        dialog.Controls.Add(panel);

        deleteButton.Click += (sender, e) =>
        {
            var selectedBook = (Book)bookComboBox.SelectedItem;
            if (selectedBook != null)
            {
                library.removeBook(selectedBook);
                SaveBookData(); // Save data after deletion
                MessageBox.Show("Book deleted successfully!");
                dialog.Close();
            }
            else
            {
                MessageBox.Show("Please select a book to delete.");
            }
        };

        dialog.ShowDialog(this);
    }

    private void UpdateStatusText(TextBox statusArea)
    {
        statusArea.Clear(); // Clear existing text
        foreach (var book in library.getBooks())
        {
            statusArea.AppendText($"Name: {book.getB_Name()}\r\n");
            statusArea.AppendText($"Author: {book.getB_Author()}\r\n");
            statusArea.AppendText($"Serial Number: {book.getB_SNO()}\r\n");
            statusArea.AppendText($"Chapters Read: {book.getChap()}\r\n");
            statusArea.AppendText($"Time Read: {book.getTime()} minutes\r\n");
            statusArea.AppendText($"Pages Read: {book.getPagesRead()}\r\n");
            statusArea.AppendText($"Completion Status: {(book.isB_status() ? "Completed" : "Not Completed")}\r\n\r\n");
        }
    }



    private void FetchBookDataFromDatabase()
    {
     

        using (var connection = new SqlConnection(JDBC_URL))
        {
            connection.Open();
            using (var command = new SqlCommand("SELECT Name, Author, SerialNumber, ChaptersRead, TimeRead, PagesRead FROM BookData", connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var name = reader["Name"].ToString();
                        var author = reader["Author"].ToString();
                        var serialNumber = reader["SerialNumber"].ToString();
                        var chaptersRead = int.Parse(reader["ChaptersRead"].ToString());
                        var timeRead = int.Parse(reader["TimeRead"].ToString());
                        var pagesRead = int.Parse(reader["PagesRead"].ToString());

                        var book = new Book(name, author, serialNumber);
                        book.UpdateChapter(chaptersRead);
                        book.UpdateTime(timeRead);
                        book.UpdatePages(pagesRead);

                        library.addBook(book);
                    }
                }
            }
        }
    }

    private void SaveBookData()
    {
        using (var connection = new SqlConnection(JDBC_URL))
        {
            connection.Open();
            foreach (var book in library.getBooks())
            {
                using (var command = new SqlCommand(@"
                IF EXISTS (SELECT 1 FROM BookData WHERE SerialNumber = @SerialNumber)
                BEGIN
                    UPDATE BookData
                    SET Name = @Name, Author = @Author, ChaptersRead = @ChaptersRead, TimeRead = @TimeRead, PagesRead = @PagesRead
                    WHERE SerialNumber = @SerialNumber
                END
                ELSE
                BEGIN
                    INSERT INTO BookData (Name, Author, SerialNumber, ChaptersRead, TimeRead, PagesRead)
                    VALUES (@Name, @Author, @SerialNumber, @ChaptersRead, @TimeRead, @PagesRead)
                END", connection))
                {
                    command.Parameters.AddWithValue("@Name", book.getB_Name());
                    command.Parameters.AddWithValue("@Author", book.getB_Author());
                    command.Parameters.AddWithValue("@SerialNumber", book.getB_SNO());
                    command.Parameters.AddWithValue("@ChaptersRead", book.getChap());
                    command.Parameters.AddWithValue("@TimeRead", book.getTime());
                    command.Parameters.AddWithValue("@PagesRead", book.getPagesRead());

                    command.ExecuteNonQuery();
                }
            }

            // Delete books that are no longer in memory
            var serialNumbersToDelete = library.getBooks().Select(b => b.getB_SNO()).ToList();
            var serialNumbersToDeleteString = string.Join(",", serialNumbersToDelete.Select(s => $"'{s}'")); // Convert to string format for SQL

            if (!string.IsNullOrEmpty(serialNumbersToDeleteString))
            {
                using (var deleteCommand = new SqlCommand($@"
                DELETE FROM BookData WHERE SerialNumber NOT IN ({serialNumbersToDeleteString})", connection))
                {
                    deleteCommand.ExecuteNonQuery();
                }
            }
        }
    }

}

