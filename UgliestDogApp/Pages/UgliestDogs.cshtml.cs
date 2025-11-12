using Microsoft.AspNetCore.Mvc;
   using Microsoft.AspNetCore.Mvc.RazorPages;
   using Microsoft.AspNetCore.Mvc.Rendering;
   using Microsoft.Data.Sqlite;
   using System.Collections.Generic;

   public class UgliestDogsModel : PageModel
   {
      //List of dogs to populate the dorpdown menu
       public List<SelectListItem> DogList { get; set; }
       public Dog SelectedDog { get; set; }

      //Handle get requests 
       public void OnGet()
       {
           LoadDogList();
       }

      //Handle post requests
       public void OnPost(string selectedDog)
       {
           LoadDogList();
           //If dog was selected, retrieve details from the db
           if (!string.IsNullOrEmpty(selectedDog))
           {
               SelectedDog = GetDogById(int.Parse(selectedDog));
           }
       }

       private void LoadDogList()
       {
           DogList = new List<SelectListItem>();

           //Connect to the SQLite database
           using (var connection = new SqliteConnection("Data Source=UgliestDogs.db"))
           {
               connection.Open();
               var command = connection.CreateCommand();
               command.CommandText = "SELECT Id, Name FROM Dogs";
               using (var reader = command.ExecuteReader())
               {
                   while (reader.Read())
                   {
                       DogList.Add(new SelectListItem
                       {
                           Value = reader.GetInt32(0).ToString(),
                           Text = reader.GetString(1)
                       });
                   }
               }
           }
       }

      //Retrieves full details of a dog by its ID
       private Dog GetDogById(int id)
       {
           using (var connection = new SqliteConnection("Data Source=UgliestDogs.db"))
           {
               connection.Open();
               var command = connection.CreateCommand();
               command.CommandText = "SELECT * FROM Dogs WHERE Id = @Id";
               command.Parameters.AddWithValue("@Id", id);
               using (var reader = command.ExecuteReader())
               {
                   if (reader.Read())
                   {
                       return new Dog
                       {
                           Id = reader.GetInt32(0),
                           Name = reader.GetString(1),
                           Breed = reader.GetString(2),
                           Year = reader.GetInt32(3),
                           ImageFileName = reader.GetString(4)
                       };
                   }
               }
           }
           return null; //If no dog was found 
       }
   }

   public class Dog
   {
       public int Id { get; set; }
       public string Name { get; set; }
       public string Breed { get; set; }
       public int Year { get; set; }
       public string ImageFileName { get; set; }

   }
