using System;

namespace LMS
{
    public class Book
    {
        public string B_Name { get; set; }
        public string B_Author { get; set; }
        public string B_SNO { get; set; }
        public int Chap { get; set; }
        public int Time { get; set; }
        public bool B_status { get; set; }
        public int PagesRead { get; set; }  // Changed from Summary

        // Book Class Constructor
        public Book(string name, string author, string serialNumber)
        {
            this.B_Author = author;
            this.B_Name = name;
            this.B_SNO = serialNumber;
            this.Chap = 0;
            this.Time = 0;
            this.PagesRead = 0;  // Initialize PagesRead to 0
            this.B_status = false;
        }

        // Updates time, pages read, number of chapters, and completion status
        public void UpdateTime(int timeRead)
        {
            Time += timeRead;
        }

        public void UpdatePages(int pages)
        {
            PagesRead += pages;
        }

        public void UpdateChapter(int chapters)
        {
            Chap += chapters;
        }

        public void MarkCompleted()
        {
            B_status = true;
        }

        // Getter methods for private properties
        public int getTime()
        {
            return Time;
        }

        public string getB_Name()
        {
            return B_Name;
        }

        public string getB_Author()
        {
            return B_Author;
        }

        public string getB_SNO()
        {
            return B_SNO;
        }

        public bool isB_status()
        {
            return B_status;
        }

        public int getPagesRead()
        {
            return this.PagesRead;
        }


        public int getChap()
        {
            return Chap;
        }
    }
}
