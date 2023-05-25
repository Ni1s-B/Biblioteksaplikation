using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace Biblioteksupgift
{
    class Library
    {
        private List<User> users = new List<User>(); // Lista för att laga användarkonton
        private List<Book> books = new List<Book>(); // Lista för att lagra böcker
        private string userFilePath = "users.txt"; // Filvägen till textfilen där användarnas konton sparas
        private string bookFilePath = "books.txt";// Filvägen till textfilen där böckerna sparas

        public void Start()
        {
            Console.WriteLine("!! Bibliotekshanteringssystem !!");

            while (true)//val för användaren att välja mellan.
            {
                Console.WriteLine("\nMeny:");
                Console.WriteLine("1. Logga in");
                Console.WriteLine("2. Skapa konto");
                Console.WriteLine("3. Sök böcker");
                Console.WriteLine("4. Byt lösenord");
                Console.WriteLine("5. Låna bok");
                Console.WriteLine("6. Återlämna bok");
                Console.WriteLine("0. Avsluta");

                Console.Write("Välj ett alternativ: ");
                int val = Convert.ToInt32(Console.ReadLine());
                //kallar på funktioner beroende på vad användaren har valt
                switch (val)
                {
                    case 1:
                        LoggaIn();
                        break;
                    case 2:
                        SkapaKonto();
                        break;
                    case 3:
                        SokBocker();
                        break;
                    case 4:
                        BytLosenord();
                        break;
                    case 5:
                        LanaBok();
                        break;
                    case 6:
                        AterlamnaBok();
                        break;
                    case 0:
                        Console.WriteLine("avslutar programmet...");
                        return;
                    default:
                        Console.WriteLine("Ogiltigt val. Försök igen.");
                        break;
                }
            }
        }

        private void LoggaIn()
        {
            Console.Write("Användarnamn: ");
            string username = Console.ReadLine();

            Console.Write("Lösenord: ");
            string password = Console.ReadLine();

            User user = users.FirstOrDefault(u => u.Username == username && u.Password == password);

            if (user != null)
            {
                Console.WriteLine("Inloggning lyckades!");
            }
            else
            {
                Console.WriteLine("Fel användarnamn eller lösenord. Försök igen.");
            }
        }

        private void SkapaKonto()
        {
            Console.Write("Ange användarnamn: ");
            string username = Console.ReadLine();

            Console.Write("Ange lösenord: ");
            string password = Console.ReadLine();

            User existingUser = users.FirstOrDefault(u => u.Username == username);

            if (existingUser != null)
            {
                Console.WriteLine("Användarnamnet är redan upptaget. Välj ett annat användarnamn.");
            }
            else
            {
                User newUser = new User(username, password);
                users.Add(newUser);
                Console.WriteLine("Kontot har skapats.");

                // Spara nya anvndares inlog i en ´txtfil
                using (StreamWriter writer = new StreamWriter(userFilePath, true))
                {
                    writer.WriteLine($"{username},{password}");
                }
            }
        }

        private void SokBocker()
        {
            Console.WriteLine("Sök böcker");
            Console.WriteLine("1. Sök efter titel");
            Console.WriteLine("2. Sök efter författare");
            Console.WriteLine("3. Sök efter ISBN");
            Console.Write("Välj sökalternativ: ");
            int val = Convert.ToInt32(Console.ReadLine());

            Console.Write("Ange sökterm: ");
            string searchTerm = Console.ReadLine();

            List<Book> searchResults;

            switch (val)
            {
                case 1:
                    searchResults = books.Where(b => b.Title.ToLower().Contains(searchTerm.ToLower())).ToList();
                    break;
                case 2:
                    searchResults = books.Where(b => b.Author.ToLower().Contains(searchTerm.ToLower())).ToList();
                    break;
                case 3:
                    searchResults = books.Where(b => b.ISBN.ToLower() == searchTerm.ToLower()).ToList();
                    break;
                default:
                    Console.WriteLine("Ogiltigt val. Återgår till huvudmenyn.");
                    return;
            }

            if (searchResults.Count > 0)
            {
                Console.WriteLine("Sökresultat:");
                foreach (Book book in searchResults)
                {
                    Console.WriteLine($"- Titel: {book.Title}, Författare: {book.Author}, ISBN: {book.ISBN}");
                }
            }
            else
            {
                Console.WriteLine("Inga matchande böcker hittades.");
            }
        }

        private void BytLosenord()
        {
            Console.WriteLine("Byt lösenord");

            Console.Write("Ange användarnamn: ");
            string användarnamn = Console.ReadLine();

            Console.Write("Ange nuvarande lösenord: ");
            string nuvarandeLösenord = Console.ReadLine();

            User användare = users.FirstOrDefault(u => u.Username == användarnamn && u.Password == nuvarandeLösenord);

            if (användare != null)
            {
                Console.Write("Ange nytt lösenord: ");
                string nyttLösenord = Console.ReadLine();

                användare.Password = nyttLösenord; // Uppdaterarar användarens lösernord

                // Uppdaterar lösenordet i användarfilen(users.txt)
                UpdateUserPasswordInFile(användarnamn, nyttLösenord);

                Console.WriteLine("Lösenordet har ändrats.");
            }
            else
            {
                Console.WriteLine("Fel användarnamn eller lösenord. Försök igen.");
            }
        }

        private void UpdateUserPasswordInFile(string användarnamn, string nyttLösenord)
        {
            // Läser in användardata från filen
            List<string> userData = File.ReadAllLines(userFilePath).ToList();

            // Hitta raden för användaren och uppdatera lösenordet
            for (int i = 0; i < userData.Count; i++)
            {
                string[] parts = userData[i].Split(',');
                string username = parts[0];

                if (username == användarnamn)
                {
                    // Uppdaterar lösenordet i raden
                    userData[i] = $"{username},{nyttLösenord}";
                    break;
                }
            }

            // Skriv tillbaka uppdaterad användardata till filen
            File.WriteAllLines(userFilePath, userData);
        }



        private void LanaBok()
        {
            Console.WriteLine("Låna bok");

            Console.Write("Ange användarnamn: ");
            string username = Console.ReadLine();

            Console.Write("Ange lösenord: ");
            string password = Console.ReadLine();

            User user = users.FirstOrDefault(u => u.Username == username && u.Password == password);

            if (user != null)
            {
                Console.WriteLine("Inloggning lyckades!");

                Console.Write("Ange ISBN för boken att låna: ");
                string isbn = Console.ReadLine();

                Book book = books.FirstOrDefault(b => b.ISBN == isbn);

                if (book != null)
                {
                    user.BorrowedBooks.Add(book); // Lägger till boken i användarens låneböcker
                    Console.WriteLine("Boken har lånats.");
                }
                else
                {
                    Console.WriteLine("Boken hittades inte.");
                }
            }
            else
            {
                Console.WriteLine("Fel användarnamn eller lösenord. Försök igen.");
            }
        }


        private void AterlamnaBok()
        {
            Console.WriteLine("Återlämna bok");

            Console.Write("Ange användarnamn: ");
            string username = Console.ReadLine();

            Console.Write("Ange lösenord: ");
            string password = Console.ReadLine();

            User user = users.FirstOrDefault(u => u.Username == username && u.Password == password);

            if (user != null)
            {
                Console.WriteLine("Inloggning lyckades!");

                Console.Write("Ange ISBN för boken att lämna tillbaka: ");
                string isbn = Console.ReadLine();

                Book book = user.BorrowedBooks.FirstOrDefault(b => b.ISBN == isbn);

                if (book != null)
                {
                    user.BorrowedBooks.Remove(book); // Tar bort boken från användarens lånade böcker ifall den har lånat den boken
                    Console.WriteLine("Boken har lämnats tillbaka.");
                }
                else
                {
                    Console.WriteLine("Du har inte lånat denna bok.");
                }
            }
            else
            {
                Console.WriteLine("Fel användarnamn eller lösenord. Försök igen.");
            }
        }


        public void LoadUsersFromFile()
        {
            if (File.Exists(userFilePath))
            {
                using (StreamReader reader = new StreamReader(userFilePath))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] parts = line.Split(',');
                        string username = parts[0];
                        string password = parts[1];

                        User user = new User(username, password);
                        users.Add(user);
                    }
                }
            }
        }

        public void LoadBooksFromFile()
        {
            if (File.Exists(bookFilePath))
            {
                using (StreamReader reader = new StreamReader(bookFilePath))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] parts = line.Split(',');
                        string title = parts[0];
                        string author = parts[1];
                        string isbn = parts[2];

                        Book book = new Book(title, author, isbn);
                        books.Add(book);
                    }
                }
            }
        }
    }

    class User
    {
        public string Username { get; }
        public string Password { get; set; }
        public List<Book> BorrowedBooks { get; } // Listar för att lagra de bööker användaren har lånat


        public User(string username, string password)
        {
            Username = username;
            Password = password;
            BorrowedBooks = new List<Book>(); // Skapar en tom lista för lånade böcker

        }
    }

    class Book
    {
        public string Title { get; }
        public string Author { get; }
        public string ISBN { get; }

        public Book(string title, string author, string isbn)
        {
            Title = title;
            Author = author;
            ISBN = isbn;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Library library = new Library();
            library.LoadUsersFromFile();
            library.LoadBooksFromFile();
            library.Start();
        }
    }
}
