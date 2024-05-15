using System;
using System.Collections.Generic;
using System.Data.SqlClient;


namespace LMS
{
    public class Library : i_inventory
    {
        private List<Book> books_data;

        public Library()
        {
            books_data = new List<Book>();
        }

        // Polymorphism used here, overriding
        public void Project_Info()
        {
            Console.WriteLine("\nThis is A Personal Book Inventory Management System!!\n");
        }

        // Adds book to List
        public void addBook(Book book)
        {
            books_data.Add(book);
        }

        public List<Book> getBooks()
        {
            return books_data;
        }

        // Fetches book according to index number
        public Book getBookByIndex(int index)
        {
            return books_data[index];
        }

        // Calculate total time
        public int getTotalTimeRead()
        {
            int totalTime = 0;
            foreach (var book in books_data)
            {
                totalTime += book.getTime();
            }
            return totalTime;
        }
    }

}