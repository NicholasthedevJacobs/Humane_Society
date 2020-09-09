﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace HumaneSociety
{
    public static class Query
    {        
        static HumaneSocietyDataContext db;

        static Query()
        {
            db = new HumaneSocietyDataContext();
        }

        internal static List<USState> GetStates()
        {
            List<USState> allStates = db.USStates.ToList();       

            return allStates;
        }
            
        internal static Client GetClient(string userName, string password)
        {
            Client client = db.Clients.Where(c => c.UserName == userName && c.Password == password).Single();

            return client;
        }

        internal static List<Client> GetClients()
        {
            List<Client> allClients = db.Clients.ToList();

            return allClients;
        }

        internal static void AddNewClient(string firstName, string lastName, string username, string password, string email, string streetAddress, int zipCode, int stateId)
        {
            Client newClient = new Client();

            newClient.FirstName = firstName;
            newClient.LastName = lastName;
            newClient.UserName = username;
            newClient.Password = password;
            newClient.Email = email;

            Address addressFromDb = db.Addresses.Where(a => a.AddressLine1 == streetAddress && a.Zipcode == zipCode && a.USStateId == stateId).FirstOrDefault();

            // if the address isn't found in the Db, create and insert it
            if (addressFromDb == null)
            {
                Address newAddress = new Address();
                newAddress.AddressLine1 = streetAddress;
                newAddress.City = null;
                newAddress.USStateId = stateId;
                newAddress.Zipcode = zipCode;                

                db.Addresses.InsertOnSubmit(newAddress);
                db.SubmitChanges();

                addressFromDb = newAddress;
            }

            // attach AddressId to clientFromDb.AddressId
            newClient.AddressId = addressFromDb.AddressId;

            db.Clients.InsertOnSubmit(newClient);

            db.SubmitChanges();
        }

        internal static void UpdateClient(Client clientWithUpdates)
        {
            // find corresponding Client from Db
            Client clientFromDb = null;

            try
            {
                clientFromDb = db.Clients.Where(c => c.ClientId == clientWithUpdates.ClientId).Single();
            }
            catch(InvalidOperationException e)
            {
                Console.WriteLine("No clients have a ClientId that matches the Client passed in.");
                Console.WriteLine("No update have been made.");
                return;
            }
            
            // update clientFromDb information with the values on clientWithUpdates (aside from address)
            clientFromDb.FirstName = clientWithUpdates.FirstName;
            clientFromDb.LastName = clientWithUpdates.LastName;
            clientFromDb.UserName = clientWithUpdates.UserName;
            clientFromDb.Password = clientWithUpdates.Password;
            clientFromDb.Email = clientWithUpdates.Email;

            // get address object from clientWithUpdates
            Address clientAddress = clientWithUpdates.Address;

            // look for existing Address in Db (null will be returned if the address isn't already in the Db
            Address updatedAddress = db.Addresses.Where(a => a.AddressLine1 == clientAddress.AddressLine1 && a.USStateId == clientAddress.USStateId && a.Zipcode == clientAddress.Zipcode).FirstOrDefault();

            // if the address isn't found in the Db, create and insert it
            if(updatedAddress == null)
            {
                Address newAddress = new Address();
                newAddress.AddressLine1 = clientAddress.AddressLine1;
                newAddress.City = null;
                newAddress.USStateId = clientAddress.USStateId;
                newAddress.Zipcode = clientAddress.Zipcode;                

                db.Addresses.InsertOnSubmit(newAddress);
                db.SubmitChanges();

                updatedAddress = newAddress;
            }

            // attach AddressId to clientFromDb.AddressId
            clientFromDb.AddressId = updatedAddress.AddressId;
            
            // submit changes
            db.SubmitChanges();
        }
        
        internal static void AddUsernameAndPassword(Employee employee)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.EmployeeId == employee.EmployeeId).FirstOrDefault();

            employeeFromDb.UserName = employee.UserName;
            employeeFromDb.Password = employee.Password;

            db.SubmitChanges();
        }

        internal static Employee RetrieveEmployeeUser(string email, int employeeNumber)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.Email == email && e.EmployeeNumber == employeeNumber).FirstOrDefault();

            if (employeeFromDb == null)
            {
                throw new NullReferenceException(); 
            }
            else
            {
                return employeeFromDb;
            }
        }

        internal static Employee EmployeeLogin(string userName, string password)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.UserName == userName && e.Password == password).FirstOrDefault();

            return employeeFromDb;
        }

        internal static bool CheckEmployeeUserNameExist(string userName)
        {
            Employee employeeWithUserName = db.Employees.Where(e => e.UserName == userName).FirstOrDefault();

            return employeeWithUserName != null;
        }


        //// TODO Items: ////
        
        // TODO: Allow any of the CRUD operations to occur here
        internal static void RunEmployeeQueries(Employee employee, string crudOperation)
        {
            if (crudOperation == "create")
            {
                try 
                {
                    db.Employees.InsertOnSubmit(employee);
                    db.SubmitChanges();
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }
            }
            else if (crudOperation == "delete")
            {
                try
                { 
                    Employee employeeFromDB = db.Employees.Where(a => a.EmployeeId == employee.EmployeeId).FirstOrDefault();
                    db.Employees.DeleteOnSubmit(employee);
                    db.SubmitChanges();
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }
            }
            else if (crudOperation == "read")
            {
                try
                {
                    Employee employeeFromDB = db.Employees.Where(a => a.EmployeeId == employee.EmployeeId).FirstOrDefault();
                    Console.WriteLine(employeeFromDB.FirstName + "\n" + employeeFromDB.LastName + "\n" + employeeFromDB.UserName + "\n" + employeeFromDB.Password + "\n" + employeeFromDB.EmployeeNumber + "\n" + employeeFromDB.Email);
                    db.SubmitChanges();
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }
            }
            else if (crudOperation == "update")
            { 
                try
                {
                    Employee employeeFromDB = db.Employees.Where(a => a.EmployeeId == employee.EmployeeId).FirstOrDefault();
                    employeeFromDB.FirstName = employee.FirstName;
                    employeeFromDB.LastName = employee.LastName;
                    employeeFromDB.UserName = employee.UserName;
                    employeeFromDB.UserName = employee.Password;
                    employeeFromDB.EmployeeNumber = employee.EmployeeNumber;
                    employeeFromDB.Email = employee.Email;
                    db.SubmitChanges();
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }
            }
        }

        // TODO: Animal CRUD Operations
        internal static void AddAnimal(Animal animal)
        {
            try
            {
                db.Animals.InsertOnSubmit(animal);
                db.SubmitChanges();
            }
            catch(Exception e)
            {
                throw new Exception(e.Message);
            }           
        }

        internal static Animal GetAnimalByID(int id)
        {
            try
            {
                Animal animal = db.Animals.Where(a => a.AnimalId == id).FirstOrDefault();
                return animal;
            }
            catch(Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        internal static void UpdateAnimal(int animalId, Dictionary<int, string> updates)
        {

            Animal animal = null;
            try
            {
                animal = db.Animals.Where(a => a.AnimalId == animalId).FirstOrDefault();               
                db.SubmitChanges();
            }
            catch(InvalidOperationException e)
            {
                Console.WriteLine("No animals with that ID exist.");
                Console.WriteLine("No update has been made.");
                return;
            }

            foreach(KeyValuePair<int, string> update in updates)
            {
                switch (updates)
                {
                    case "1":
                        animal.CategoryId = Convert.ToInt32(updates[1]);
                        break;
                }

            }
            //1. Category", "2. Name", "3. Age", "4. Demeanor", "5. Kid friendly", "6. Pet friendly", "7. Weight", "8. ID", "9. Finished" };
            animal.CategoryId = Convert.ToInt32(updates[1]);
            animal.Name = updates[2];
            animal.Age = Convert.ToInt32(updates[3]);
            animal.Demeanor = updates[4];
            animal.KidFriendly = Convert.ToBoolean(updates[5]);
            animal.PetFriendly = Convert.ToBoolean(updates[6]);
            animal.Weight = Convert.ToInt32(updates[7]);
            animal.AnimalId = Convert.ToInt32(updates[8]);//maybe dont need to have this one here.  Id gets auto created.
            db.SubmitChanges();
            


        }
        internal static void RemoveAnimal(Animal animal)
        {
            try
            {
                Animal animalToRemove = db.Animals.Where(a => a.AnimalId == animal.AnimalId).FirstOrDefault();
                db.Animals.DeleteOnSubmit(animal);
                db.SubmitChanges();
            }
            catch(Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        
        // TODO: Animal Multi-Trait Search
        internal static IQueryable<Animal> SearchForAnimalsByMultipleTraits(Dictionary<int, string> updates) // parameter(s)?
        {
            IQueryable<Animal> animals = db.Animals.Where(a => a.CategoryId == Convert.ToInt32(updates[1]) && a.Name == updates[2] && a.Age == Convert.ToInt32(updates[3]) && a.Demeanor == updates[4] && a.KidFriendly == Convert.ToBoolean(updates[5]) && a.PetFriendly == Convert.ToBoolean(updates[6]) && a.Weight == Convert.ToInt32(updates[7]));
            return animals;
        }
         
        // TODO: Misc Animal Things
        internal static int GetCategoryId(string categoryName)
        {
            try
            {
                int categoryID = Convert.ToInt32(db.Categories.Where(a => a.Name == categoryName).Select(a => a.CategoryId));
                return categoryID;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        internal static Room GetRoom(int animalId)
        {
            try
            { 
                Room room = db.Rooms.Where(a => a.AnimalId == animalId).FirstOrDefault();
                return room;
            }
             catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        
        internal static int GetDietPlanId(string dietPlanName)
        {
            try
            {
                int dietPlanID = Convert.ToInt32(db.DietPlans.Where(a => a.Name == dietPlanName).Select(a => a.DietPlanId));
                return dietPlanID;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        // TODO: Adoption CRUD Operations
        internal static void Adopt(Animal animal, Client client)
        {
            try
            {
                client = db.Clients.Where(a => a == client).FirstOrDefault();
                animal = db.Animals.Where(a => a == animal).FirstOrDefault();
                Adoption adoption = db.Adoptions.Where(a => a.ClientId == client.ClientId && a.AnimalId == animal.AnimalId).FirstOrDefault();
                animal.AdoptionStatus = "This animal was adopted by " + client.FirstName + "" + client.LastName;
                adoption.ApprovalStatus = "pending";
                adoption.PaymentCollected = true;
                db.SubmitChanges();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        internal static IQueryable<Adoption> GetPendingAdoptions()
        {
            IQueryable<Adoption> pendingAdoptions = db.Adoptions.Where(a => a.ApprovalStatus == "pending");
            return pendingAdoptions;
        }

        internal static void UpdateAdoption(bool isAdopted, Adoption adoption)
        {
            try
            {
                if (isAdopted == true)
                {
                    adoption.ApprovalStatus = "approved";
                    adoption.PaymentCollected = true;
                }
                else
                {
                    adoption.ApprovalStatus = "pending";
                    adoption.PaymentCollected = false;
                }
                db.SubmitChanges();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }

        internal static void RemoveAdoption(int animalId, int clientId)
        {
            throw new NotImplementedException();
        }

        // TODO: Shots Stuff
        internal static IQueryable<AnimalShot> GetShots(Animal animal)
        {
            IQueryable<AnimalShot> shots = db.AnimalShots.Where(a => a.Animal == animal);
            return shots;
        }

        internal static void UpdateShot(string shotName, Animal animal)
        {
            try
            {
                Shot shot = db.Shots.Where(a => a.Name == shotName).FirstOrDefault();
                AnimalShot animalShot = db.AnimalShots.Where(a => a.AnimalId == animal.AnimalId && a.ShotId == shot.ShotId).FirstOrDefault();
                animalShot.DateReceived = DateTime.Now;
                db.SubmitChanges();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}