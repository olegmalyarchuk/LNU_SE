using System.Text.Json;
using System.Text.Json.Serialization;

class MainCus
{
    public List<Customer> customers { get; set; }
    public List<Operator> operators { get; set; }
    public List<Bill> bills { get; set; }

    public MainCus()
    {
        customers = new List<Customer>();
        operators = new List<Operator>();
        bills = new List<Bill>();
    }

    public static void ReadJson(out MainCus? main, string json = "options.json")
    {
        string jsonString = File.ReadAllText("options.json");
        main = JsonSerializer.Deserialize<MainCus>(jsonString, new JsonSerializerOptions() { WriteIndented = true, IncludeFields = true });
    }
}

class Customer
{
    public int ID { get; set; }
    public string name { get; set; }
    public int age { get; set; }
    public List<Operator> operators { get; set; }
    public List<Bill> bills { get; set; }

    public Customer(int ID, string name, int age, List<Operator> operators, List<Bill> bills)
    {
        this.ID = ID;
        this.name = name;
        this.age = age;
        this.operators = operators;
        this.bills = bills;
    }

    public void talk(int minute, Customer other)
    {
        if (other.operators.Count == 0)
        {
            operators.Add(new Operator(1, 10, 2, 5, 4));
        }

        Bill bill = new Bill(10000);

        if (minute * other.operators.Last().talkingCharge <= bill.limitingAmount)
        {
            bill.add(other.operators.Last().calculateTalkingCost(minute, this));
            bills.Add(bill);
        }

        else
        {
            Console.WriteLine("Limit is exceeded");
        }
    }

    public void message(int quantity, Customer other)
    {
        if (other.operators.Count == 0)
        {
            operators.Add(new Operator(1, 10, 2, 5, 4));
        }

        Bill bill = new Bill(10000);

        if (quantity * other.operators.Last().messageCost <= bill.limitingAmount)
        {
            bill.add(other.operators.Last().calculateMessageCost(quantity, this, other));
            bills.Add(bill);
        }

        else
        {
            Console.WriteLine("Limit is exceeded");
        }
    }

    public void connection(double ammount)
    {
        if (operators.Count == 0)
        {
            operators.Add(new Operator(1, 10, 2, 5, 4));
        }

        Bill bill = new Bill(10000);

        if (ammount * operators.Last().networkCharge <= bill.limitingAmount)
        {
            bill.add(ammount);
            bills.Add(bill);
        }

        else
        {
            Console.WriteLine("Limit is exceeded");
        }
    }
}

class Operator
{
    public int ID { get; set; }
    public double talkingCharge { get; set; }
    public double messageCost { get; set; }
    public double networkCharge { get; set; }
    public int discountRate { get; set; }

    public Operator(int ID, double talkingCharge, double messageCost, double networkCharge, int discountRate)
    {
        this.ID = ID;
        this.talkingCharge = talkingCharge;
        this.messageCost = messageCost;
        this.networkCharge = networkCharge;
        this.discountRate = discountRate;
    }

    public double calculateTalkingCost(int minute, Customer customer)
    {
        if (customer.age < 18 || customer.age > 65)
        {
            this.discountRate = 30;
        }
        return (double)(minute * customer.operators.Last().talkingCharge) / 100 * (100 - discountRate);
    }

    public double calculateMessageCost(int quantity, Customer customer, Customer other)
    {
        if (customer.age < 18 || customer.age > 65)
        {
            this.discountRate = 50;
        }

        if (customer.operators.Last().ID == other.operators.Last().ID)
        {
            this.discountRate = this.discountRate + 25;
        }

        return (double)(quantity * (other.operators.Last().messageCost + customer.operators.Last().messageCost)) / 100.0 * (100 - discountRate);
    }

    public double calculateNetworkCost(double amount)
    {
        return networkCharge * amount;
    }
}

class Bill
{
    public double limitingAmount { get; set; }
    public double currentDebt { get; set; } = 0;

    public Bill(double limitingAmount)
    {
        this.limitingAmount = limitingAmount;
    }

    public bool check(double ammount)
    {
        return (ammount > limitingAmount) ? true : false;
    }

    public void add(double ammount)
    {
        this.currentDebt += ammount;
    }

    public void pay(double ammount)
    {
        this.currentDebt = (this.currentDebt - ammount < 0) ? 0 : this.currentDebt - ammount;
    }

    public void changeTheLimit(double ammount)
    {
        this.limitingAmount = ammount;
    }
}

namespace Program
{
    class Program
    {
        static void outputData(List<Bill> bills)
        {
            int counter = 0;
            foreach(Bill bill in bills)
            {
                counter++;
                Console.WriteLine($"Bill {counter} has debt of {bill.currentDebt}");
            }
        }

        public static void Main()
        {
            MainCus? main = new MainCus();

            //int N, M;

            //Console.WriteLine("Input N value (NUMBER OF CUSTOMERS): \n");

            //N = Convert.ToInt32(Console.ReadLine());

            //Console.WriteLine("Input M value (NUMBER OF OPERATORS): \n");

            //M = Convert.ToInt32(Console.ReadLine());

            MainCus.ReadJson(out main);

            main.customers[0].talk(60, main.customers[1]);

            Console.WriteLine("Talk completed");

            main.customers[0].message(5, main.customers[1]);

            Console.WriteLine("Messages sent");

            main.customers[0].connection(200);

            Console.WriteLine("Connection established");

            outputData(main.customers[0].bills);

            main.customers[0].bills[0].pay(2000);

            Console.WriteLine("Bills payed off");

            outputData(main.customers[0].bills);

            Console.WriteLine($"Active operator: {main.customers[0].operators[1]}");

            Operator temp = main.customers[0].operators[1];

            main.customers[0].operators[1] = main.customers[0].operators[0];

            main.customers[0].operators[0] = temp;

            Console.WriteLine("Operator changed");

            Console.WriteLine($"Active operator: {main.customers[0].operators[1]}");

            Console.WriteLine($"Current first bill limit: {main.customers[0].bills[0].limitingAmount}");

            main.customers[0].bills[0].changeTheLimit(5000);

            Console.WriteLine("Limit has changed");

            Console.WriteLine($"Current first bill limit: {main.customers[0].bills[0].limitingAmount}");

            Console.Read();
        }
    }
}