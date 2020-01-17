using System;


namespace PizzaBox.Client
{
    class Program
    {
        /* start repository calls  
        static void Main(string[] args)
          {
              Console.WriteLine("Hello World!");
          } */

        private static OrderRepository orderRepos = new OrderRepository();

        static StoreRepository storeRepos = new StoreRepository();

        private static CustomerRepository custRepos = new CustomerRepository();

        private static ManagerRepository managRepos = new ManagerRepository();


        static void Main()
        {
            Console.Clear();

            Start();
        }

        static void Exit()
        {
            Console.WriteLine("Now exiting...");
        }

        static void Start()
        {
            Console.WriteLine("!!Welcome!!");
            Console.WriteLine("1. Login");
            Console.WriteLine("2. Register New Account");
            Console.WriteLine("3. Manager Login");
            Console.WriteLine("4. Exit");
            Console.WriteLine("Please choose an option to continue or exit :)");

            int input = ValidateUserInput();

            switch (input)
            {
                case 1:
                    Login();
                    break;
                case 2:
                    Register();
                    break;
                case 3:
                    ManagerLogin();
                    break;
                case 4:
                    Exit();
                    break;
                default:
                    Start();
                    break;
            }
        }

        public static void Login()
        {
            Console.Clear();

            CustomerDetails CurrentUser = new CustomerDetails(); //should be valid class instance when CustomerDetails.cs is done

            Console.WriteLine("Input Email:");
            CurrentUser.Email = Console.ReadLine();

            Console.WriteLine("Input Password:");
            CurrentUser.Password = Console.ReadLine();

            if (custRepos.Read(CurrentUser) == null)
            {
                Console.Clear();
                Console.WriteLine("Email or Password invalid, please try again");
                Start();
                return;
            }

            Console.Clear();
            Console.WriteLine("....Login Succesful");
            CustDash(custRepos.Read(CurrentUser));
            //Customer Dashboard call using Customer details Repository, <'CustDash'( 'custRepos'.Read(CurrentUser))>
            // ^^ should be valid when Customer Dashboard and Customer Repository is finished
        }

        public static void Register()
        {
            Console.Clear();

            CustomerDetails NewU = new CustomerDetails();

            Console.WriteLine("Enter First Name: ");
            NewU.FirstName = Console.ReadLine();
            Console.WriteLine("Enter Last Name: ");
            NewU.LastName = Console.ReadLine();
            Console.WriteLine("Enter Email: ");
            NewU.Email = Console.ReadLine();
            Console.WriteLine("Enter Password: ");
            NewU.Password = Console.ReadLine();
            Console.WriteLine("Enter Address: ");
            NewU.Address = Console.ReadLine();

            if (custRepos.Read(NewU) != null)
            {
                Console.Clear();
                Console.WriteLine("This email is currently already registers to user. Please try again.");

                Start();
            }

            custRepos.Create(NewU);
            Console.WriteLine(".... Registration Successful for {0}", NewU.FirstName);
            CustDash(NewU);
        }

        public static void ManagerLogin()
        {
            Console.Clear();

            ManagerDetails CurrentUser = new ManagerDetails();

            Console.WriteLine("Enter Manager Email: ");
            string email = Console.ReadLine();
            Console.WriteLine("Enter Manager Password: ");
            string pass = Console.ReadLine();

            CurrentUser = managRepos.managerList.Find(x => x.Email == email && x.Password == pass); // should be fine when establish manager repository and manager list
            //managerList ^^
            if (CurrentUser == null)
            {
                Console.Clear();
                Console.WriteLine("There is not a manager associated with this Email or Password. Please Try again ");
                Start();
                return;
            }

            // invoke currentStore from store repository to associate manager to their store.
            CurrentUser.CurrentStore = storeRepos.Read(new StoreDetails("MizzaPizza", "Lubbock, TX"));

            Console.Clear();
            Console.WriteLine("....Login Succesful");
            ManagDash(CurrentUser);
        }

        public static void CustDash(CustomerDetails NewU)
        {
            Console.Clear();
            Console.WriteLine("Welcome {0}", NewU.FirstName);
            Console.WriteLine("1. Would you like to start an order?");
            Console.WriteLine("2. Would you like to view order history?");
            Console.WriteLine("3. Or logout?");
            Console.WriteLine("Please choose an option to continue or log out");

            int input = ValidateUserInput(); //might want to change char to int  for cases and ^^

            switch (input)
            {
                case 1:
                    StartOrder(NewU);
                    break;
                case 2:
                    CustOrderHistory(NewU);
                    break;
                case 3:
                    Console.WriteLine("Thanks for stopping bye {0}", NewU.FirstName);
                    break;
                default:
                    CustDash(NewU);
                    break;
            }
        }

        public static void ManagDash(ManagerDetails manag)
        {
            Console.WriteLine("Welcome {0} from {1}", manag.FirstName, manag.CurrentStore);
            Console.WriteLine("1. View your store orders");
            Console.WriteLine("2. Change your current store");
            Console.WriteLine("3.Or logout");
            Console.Write("Plese choose an option to continue");

            int input = ValidateUserInput();

            switch (input)
            {
                case 1:
                    ViewStoreOrders(manag);
                    break;
                case 2:
                    ChangeCurrentStore(manag);
                    break;
                case 3:
                    Console.WriteLine("GoodBye {0}", manag.FirstName);
                    Start();
                    break;
                default:
                    ManagDash(manag);
                    break;
            }
        }
        public static void StartOrder(CustomerDetails NewU)
        {

            Console.Clear();

            if (AllowTwoHrPd(NewU))
            {
                CustDash(NewU);
                return;
            }

            StoreDetails S = new StoreDetails();

            OrderDetails O = new OrderDetails();

            S = ChooseStore();

            if (AllowTwentyFourHrPd(S, NewU))
            {
                CustDash(NewU);
                return;
            }
            Console.WriteLine("You chose to order from {0}", S);


            O = CreateOrder();

            if (O == null)
            {
                Console.WriteLine("...An error occured");
                CustDash(NewU);
                return;
            }
            O.StoreName = S.Name;
            O.CustID = NewU.Id;

            S.AddOrder(O);
            storeRepos.Update(S);
            NewU.AddOrder(O);

            orderRepos.Create(O);
            custRepos.Update(NewU);

            Console.Clear();
            Console.WriteLine("Your order has been successfully placed :)");
            O.PrintOrder();
            CustDash(NewU);
        }

        public static bool AllowTwoHrPd(CustomerDetails NewU)
        {
            if (NewU.Orders.Count > 0)
            {
                TimeSpan now = DateTime.Now - NewU.Orders[NewU.Orders.Count - 1].OrderDate;

                if (now < new TimeSpan(0, 2, 0, 0))
                {
                    now = new TimeSpan(2, 0, 0) - now;
                    Console.WriteLine("...An error occured :/ ...\n too soon to order again...\n must wait {0} minutes to order again.", now);
                    return true;
                }
            }
            return false;
        }

        public static bool AllowTwentyFourHrPd(StoreDetails S, CustomerDetails NewU)
        {
            OrderDetails O = NewU.Orders.Find(x => x.storename == S.Name);

            if (O != null)
            {
                TimeSpan now = DateTime.Now - O.OrderDate;

                if (now < new TimeSpan(0, 24, 0, 0) && S.Name == O.storename)
                {
                    now = new TimeSpan(24, 0, 0) - now;
                    Console.WriteLine("...An error occured :/ ...\n too soon to order again...\n must wait {0} hours to order from {1} again.", now, S);
                    CustDash(NewU);
                    return true;
                }
            }
            return false;
        }

        public static StoreDetails ChooseStore()
        {
            Console.Clear();
            Console.Clear();
            Console.WriteLine("Stores: ");
            Console.WriteLine("1. MizzaPizza   Lubbock, TX");
            Console.WriteLine("2. Ranger Pizza   Arlington, TX");
            Console.WriteLine("3. Cowboy Pizza   Dallas, TX");
            Console.Write("Please Select a Store from the above options: ");

            int input = ValidateUserInput();

            switch (input)
            {
                case 1:
                    return storeRepos.Read(new StoreDetails("MizzaPizza   Lubbock, TX"));
                case 2:
                    return storeRepos.Read(new StoreDetails("Ranger Pizza   Arlington, TX"));
                case 3:
                    return storeRepos.Read(new StoreDetails("Cowboy Pizza   Dallas, TX"));
                default:
                    ChooseStore();
                    break;
            }
            return ChooseStore();
        }

        public static OrderDetails CreateOrder()
        {
            Console.Write("How many pizzas would you like to order? ");

            int input = ValidateUserInput();

            if (input == -1 || input == 0 || input > 100)
            {
                if (input > 100)
                    Console.WriteLine("Can't order more than 100 pizzas");
                return null;
            }
            /// here should be the SQL DB Part
            List<PizzaDetails> pList = new List<PizzaDetails>();
            for (int i = 0; i < input; i++)
            {
                PizzaDetails newPizza = CreatePizza();
                pList.Add(newPizza);
                Console.WriteLine("Your pizza is a {0}", newPizza);
            }

            OrderDetails O = new OrderDetails(pList);

            if (O.OrderTool >= 250.00)
            {
                Console.Clear();
                Console.WriteLine("Apologies, your order cannot exceed $250");
                return null;
            }
            return O;
        }

        static PizzaDetails CreatePizza()
        {
            Console.WriteLine("1. Large Pepperoni Pizza..   .   .   . $10.99");
            Console.WriteLine("2. Large Cheese Pizza..  .   .   . $9.99");
            Console.WriteLine("3. Upside-Down Pizza..  .   .   . $250.00");
            Console.WriteLine("4. Create your own pizza..   .   .   . $4.99-$10.99");
            Console.Write("Which kind of pizza would you like to order? ");

            int input = ValidateUserInput();

            switch (input)
            {
                case 1:
                    return new PizzaDetails("Large", "Thick", "Pepperoni");
                case 2:
                    return new PizzaDetails("Large", "Hand-tossed", "Cheese");
                case 3:
                    return new PizzaDetails("Upside-Down", "Hand-tossed", "Pepperoni");
                case 4:
                    return CustomPizza();
                default:
                    return CreatePizza();
            }
        }
        static PizzaDetails CustomPizza()
        {
            PizzaDetails NewPizza = new PizzaDetails();

            NewPizza.Size = ChooseSize();
            NewPizza.Crust = ChooseCrust();
            NewPizza.Topping = ChooseTopping();

            return NewPizza;
        }

        static string ChooseSize()
        {
            Console.WriteLine("Sizessssss: ");
            Console.WriteLine("1. Small..   .   .   . $4.99");
            Console.WriteLine("2. Medium..  .   .   . $7.99");
            Console.WriteLine("3. Large..   .   .   . $9.99");
            Console.Write("Choose the size of pizza you would like to order: ");

            int input = ValidateUserInput();

            switch (input)
            {
                case 1:
                    return "Small";
                case 2:
                    return "Medium";
                case 3:
                    return "Large";
                default:
                    ChooseSize();
                    break;
            }
            return ChooseSize();
        }


        static string ChooseCrust()
        {
            Console.WriteLine("Crustsssss: ");
            Console.WriteLine("1. Hand-tossed");
            Console.WriteLine("2. Thin");
            Console.WriteLine("3. Cheese-stuffed");
            Console.Write("Choose what kinda of crust would you like? ");

            int input = ValidateUserInput();

            switch (input)
            {
                case 1:
                    return "Hand-tossed";
                case 2:
                    return "Thin";
                case 3:
                    return "Cheese-stuffed";
                default:
                    ChooseCrust();
                    break;
            }
            return ChooseCrust();
        }
        static string ChooseTopping()

        {
            Console.WriteLine("ToppingSssss: ");
            Console.WriteLine("1. Pepperoni");
            Console.WriteLine("2. Veggies");
            Console.WriteLine("3. Sausage");
            Console.Write("Please choose a topping: ");

            int input = ValidateUserInput();

            switch (input)
            {
                case 1:
                    return "Pepperoni";
                case 2:
                    return "Veggies";
                case 3:
                    return "Sausage";
                default:
                    ChooseTopping();
                    break;
            }
            return ChooseTopping();
        }

        static void ViewStoreOrders(ManagerDetails manag)
        {
            Console.Clear();
            manag.CurrentStore.ViewStoreOrders();
            ManagDash(manag);
        }

        static void ChangeCurrentStore(ManagerDetails manag)
        {
            manag.CurrentStore = ChooseStore();
            ManagDash(manag);
        }

        static StoreDetails SelectStore()
        { 
            return new StoreDetails("Ranger Pizza", "Dallas, TX");
        }

        public static int ValidateUserInput()

        {
            int input;
            try
            {
                input = Convert.ToInt32(Console.ReadLine());
            }
            catch (Exception exc)
            {
                var a = exc.Data;
                Console.WriteLine("Invalid input. Please Try Again :)");
                return -1;
            }
            return input;
        }
    }
}
