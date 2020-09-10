using System;
using System.Collections.Generic;
using System.Data.Common;
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
        internal static void RunEmployeeQueries(Employee employee, string crudOperation)
        {
            if (crudOperation == "create")
            {
                CreateEmployeeQueries(employee, crudOperation);
            }
            else if (crudOperation == "delete")
            {
                DeleteEmployeeQueries(employee, crudOperation);
            }
            else if (crudOperation == "read")
            {
                ReadEmployeeQueries(employee, crudOperation);
            }
            else if (crudOperation == "update")
            {
                UpdateEmployeeQueries(employee, crudOperation);
            }
            else
            {
                Console.WriteLine("Please enther valid input.");
                //RunEmployeeQueries(employee, crudOperation);   **SHOULD WE INCLUDE RECURSION HERE?**NEVIN
            }
        }
        internal static void CreateEmployeeQueries(Employee employee, string crudOperation)
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
        internal static void DeleteEmployeeQueries(Employee employee, string crudOperation)
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
        internal static void ReadEmployeeQueries(Employee employee, string crudOperation)
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
        internal static void UpdateEmployeeQueries(Employee employee, string crudOperation)
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
            catch(InvalidCastException e)
            {
                Console.WriteLine("No animals with that ID exist.");
                Console.WriteLine("No update has been made.");
                return;
            }
            foreach(KeyValuePair<int, string> update in updates)
            {
                switch (update.Key)
                {
                    case 1:
                        animal.CategoryId = Convert.ToInt32(update.Value);
                        break;
                    case 2:
                        animal.Name = update.Value;
                        break;
                    case 3:
                        animal.Age = Convert.ToInt32(update.Value);
                        break;
                    case 4:
                        animal.Demeanor = update.Value;
                        break;
                    case 5:
                        animal.KidFriendly = Convert.ToBoolean(update.Value);
                        break;
                    case 6:
                        animal.PetFriendly = Convert.ToBoolean(update.Value);
                        break;
                    case 7:
                        animal.Weight = Convert.ToInt32(update.Value);
                        break;
                    default:
                        Console.WriteLine("Enter an appropriate value.");
                        break;
                }
            }
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
        internal static IQueryable<Animal> SearchForAnimalsByMultipleTraits(Dictionary<int, string> updates) // parameter(s)?
        {
            IQueryable<Animal> animalSearch = db.Animals;
            foreach (KeyValuePair<int, string> update in updates)
            {
                switch (update.Key)
                {
                    case 1:
                        animalSearch = animalSearch.Where(a => a.CategoryId == Convert.ToInt32(update.Value));
                        break;
                    case 2:
                        animalSearch = animalSearch.Where(a => a.Name == update.Value);
                        break;
                    case 3:
                        animalSearch = animalSearch.Where(a => a.Age == Convert.ToInt32(update.Value));
                        break;
                    case 4:
                        animalSearch = animalSearch.Where(a => a.Demeanor == update.Value);
                        break;
                    case 5:
                        animalSearch = animalSearch.Where(a => a.KidFriendly == Convert.ToBoolean(update.Value));
                        break;
                    case 6:
                        animalSearch = animalSearch.Where(a => a.PetFriendly == Convert.ToBoolean(update.Value));
                        break;
                    case 7:
                        animalSearch = animalSearch.Where(a => a.Weight == Convert.ToInt32(update.Value));
                        break; 
                    default:
                        Console.WriteLine("Enter an appropriate value.");
                        break;
                }
            }
            return animalSearch;
        }
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
            try
            {
                Adoption adoption = db.Adoptions.Where(c => c.AnimalId == animalId && c.ClientId == clientId).FirstOrDefault();
                db.Adoptions.DeleteOnSubmit(adoption);
                db.SubmitChanges();
            }
            catch(Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        internal static IQueryable<AnimalShot> GetShots(Animal animal)
        {
            IQueryable<AnimalShot> shots = db.AnimalShots.Where(a => a.Animal == animal);
            return shots;
        }
        internal static void UpdateShot(string shotName, Animal animal)
        {
            try
            {
                AnimalShot animalShot = new AnimalShot();
                animalShot.AnimalId = animal.AnimalId;
                Shot shot = db.Shots.Where(a => a.Name == shotName).FirstOrDefault();
                //AnimalShot animalShots = db.AnimalShots.Where(a => a.AnimalId == animal.AnimalId && a.ShotId == shot.ShotId).FirstOrDefault();
                animalShot.ShotId = shot.ShotId;
                animalShot.DateReceived = DateTime.Now;

                db.AnimalShots.InsertOnSubmit(animalShot);
                db.SubmitChanges();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        } 
    }
}