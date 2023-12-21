using System.ComponentModel;
using System.Text.Json;

namespace Program
{
    class Program
    {
        interface IPort
        {
            void incomingShip(Ship s);
            void outgoingShip(Ship s);
        }

        interface IShip
        {
            bool sailTo(Port p);
            void reFuel(double newFuel);
            bool load(Container count);
            bool unLoad(Container count);
        }

        class MainJson
        {
            public List<Container> containers { get; set; }
            public List<Ship> ships { get; set; }
            public List<Port> ports { get; set; }

            public MainJson()
            {
                containers = new List<Container>();
                ships = new List<Ship>();
                ports = new List<Port>();
            }

            public static void ReadJson(out MainJson? main, string json = "options.json")
            {
                string jsonString = File.ReadAllText("options.json");
                main = JsonSerializer.Deserialize<MainJson>(jsonString, new JsonSerializerOptions() { WriteIndented = true, IncludeFields = true });
            }

            public void WriteJson()
            {

            }
        }

        class Ship : IShip
        {
            public int ID { get; set; }
            public double fuel { get; set; }
            public Port currentPort { get; set; }
            public int totalWeightCapacity { get; set; }
            public int maxNumberOfAllContainers { get; set; }
            public int maxNumberOfHeavyContainers { get; set; }
            public int maxNumberOfRefrigeratedContainers { get; set; }
            public int maxNumberOfLiquidContainers { get; set; }
            public double fuelConsumptionPerKM { get; set; }

            public Ship(int ID, double fuel, Port currentPort, int totalWeightCapacity, int maxNumberOfAllContainers,
                        int maxNumberOfHeavyContainers, int maxNumberOfRefrigeratedContainers, int maxNumberOfLiquidContainers,
                        double fuelConsumptionPerKM)
            {
                this.ID = ID;
                this.fuel = fuel;
                this.currentPort = currentPort;
                this.totalWeightCapacity = totalWeightCapacity;
                this.maxNumberOfAllContainers = maxNumberOfAllContainers;
                this.maxNumberOfHeavyContainers = maxNumberOfHeavyContainers;
                this.maxNumberOfLiquidContainers = maxNumberOfLiquidContainers;
                this.maxNumberOfRefrigeratedContainers = maxNumberOfRefrigeratedContainers;
                this.fuelConsumptionPerKM = fuelConsumptionPerKM;
            }

            public bool load(Container cont)
            {
                if (Convert.ToInt32(cont.weight) <= 3000)
                {
                    if (cont.weight.Last() == 'R' || cont.weight.Last() == 'L')
                    {
                        return false;
                    }
                    BasicContainer container = new BasicContainer(cont.ID, cont.weight);
                    currentPort.containers.Add(container);
                }
                else
                {
                    if (cont.weight.Last() == 'R')
                    {
                        RefrigiratedContainer container = new RefrigiratedContainer(cont.ID, cont.weight);
                        currentPort.containers.Add(container);
                    }
                    else if (cont.weight.Last() == 'L')
                    {
                        LiquidContainer container = new LiquidContainer(cont.ID, cont.weight);
                        currentPort.containers.Add(container);
                    }
                    else
                    {
                        HeavyContainer container = new HeavyContainer(cont.ID, cont.weight);
                        currentPort.containers.Add(container);
                    }
                }
                return true;
            }

            public void reFuel(double newFuel)
            {
                this.fuel += newFuel;
            }

            public bool sailTo(Port p)
            {
                bool res = (this.currentPort.ID == p.ID);
                return res;
            }

            public bool unLoad(Container cont)
            {
                currentPort.containers.Remove(currentPort.containers.Where(el => el.ID == cont.ID).Single());
                return true;
            }
        }

        class Port : IPort
        {
            public int ID { get; set; }
            public double latitude { get; set; }
            public double longitude { get; set; }
            public List<Container> containers { get; set; }
            public List<Ship> history { get; set; }
            public List<Ship> current { get; set; }

            public Port(int ID, double latitude, double longitude)
            {
                this.ID = ID;
                this.latitude = latitude;
                this.longitude = longitude;
                this.containers = new List<Container>();
                this.history = new List<Ship>();
                this.current = new List<Ship>();
            }

            public void incomingShip(Ship s)
            {
                current.Add(s);
            }

            public void outgoingShip(Ship s)
            {
                current.Remove(s);
                foreach (Ship el in history)
                {
                    if (el.ID == s.ID)
                    {
                        return;
                    }
                }
                history.Add(s);
            }
            public double getDistance(Port other)
            {
                double rlat1 = Math.PI * this.latitude / 180;
                double rlat2 = Math.PI * other.latitude / 180;
                double theta = this.longitude - other.longitude;
                double rtheta = Math.PI * theta / 180;
                double dist = Math.Sin(rlat1) * Math.Sin(rlat2) + Math.Cos(rlat1) * Math.Cos(rlat2) * Math.Cos(rtheta);
                dist = Math.Acos(dist);
                dist = dist * 180 / Math.PI;
                dist = dist * 60 * 1.1515;
                return dist * 1.609344;
            }
        }

        class Container
        {
            public int ID { get; set; }
            virtual public string weight { get; set; }

            public Container(int ID, string weight)
            {
                this.ID = ID;
                this.weight = weight;
            }

            virtual public double consumption() { return 0; }
            virtual public bool equals(Container other) { return true; }
        }

        class BasicContainer : Container
        {
            public BasicContainer(int ID, string weight) : base(ID, weight) { }
            public override double consumption()
            {
                return Convert.ToDouble(this.weight) * 2.5;
            }
            public override bool equals(Container other)
            {
                bool res = (typeof(BasicContainer) == typeof(Container)) ? true : false;
                return res;
            }

        }

        class HeavyContainer : Container
        {
            public HeavyContainer(int ID, string weight) : base(ID, weight) { }
            public override double consumption()
            {
                return Convert.ToDouble(this.weight) * 3.0;
            }
            public override bool equals(Container other)
            {
                bool res = (typeof(HeavyContainer) == typeof(Container)) ? true : false;
                return res;
            }
        }

        class RefrigiratedContainer : HeavyContainer
        {
            public RefrigiratedContainer(int ID, string weight) : base(ID, weight) { }
            public override double consumption()
            {
                return Convert.ToDouble(this.weight) * 5.0;
            }
            public override bool equals(Container other)
            {
                bool res = (typeof(RefrigiratedContainer) == typeof(Container)) ? true : false;
                return res;
            }

        }

        class LiquidContainer : HeavyContainer
        {
            public LiquidContainer(int ID, string weight) : base(ID, weight) { }
            public override double consumption()
            {
                return Convert.ToDouble(this.weight) * 4.0;
            }
            public override bool equals(Container other)
            {
                bool res = (typeof(LiquidContainer) == typeof(Container)) ? true : false;
                return res;
            }
        }

        static void Main()
        {
            int actionID;
            MainJson? main = new MainJson();
            MainJson.ReadJson(out main);

            foreach (Ship el in main.ships)
            {
                foreach (Port p in main.ports)
                {
                    if (el.currentPort != null)
                    {
                        if (el.currentPort.ID == p.ID)
                        {
                            p.current.Add(el);
                        }
                    }
                }
            }

            int ID;
            int port;
            int targetPort;
            bool state;
            double refuel;

            for (; ; )
            {
                Console.WriteLine("Pick action ID - \n[1] (incomingShip); [2] (outgoingShip); [3] (getDistance);\n" +
                                  "[4] (container consumption); [5] (container equals);\n" +
                                  "[6] (sailTo); [7] (refuel); [8] (load); [9] (unLoad);\n" +
                                  "[0] (save json);\n");
                actionID = Convert.ToInt32(Console.ReadLine());
                switch (actionID)
                {
                    case 0:
                        File.WriteAllText("output.json", JsonSerializer.Serialize(main.ports));
                        break;
                    case 1:
                        Console.WriteLine("Available ships");
                        foreach (Ship s in main.ships)
                        {
                            Console.Write("\n[ID:" + s.ID + "] ");
                            if (s.currentPort != null)
                            {
                                Console.Write("current port ID: " + s.currentPort.ID);
                            }
                            Console.Write("\n\n");
                        }
                        Console.WriteLine("Select ship ID to assign to port: ");
                        ID = Convert.ToInt32(Console.ReadLine());
                        foreach (Port p in main.ports)
                        {
                            Console.Write("\n[ID:" + p.ID + "]");
                            Console.Write("\n\n");
                        }
                        Console.WriteLine("Select port to assign: ");
                        port = Convert.ToInt32(Console.ReadLine());
                        main.ports[port].incomingShip(main.ships[ID]);
                        Console.WriteLine("New ships state in choosen port: ");
                        Console.WriteLine("Port [" + port + "] - ships:");
                        foreach (Ship s in main.ports[port].current)
                        {
                            Console.WriteLine("\t[ID:" + s.ID + "]\n");
                        }
                        break;
                    case 2:
                        Console.WriteLine("Available ships");
                        foreach (Ship s in main.ships)
                        {
                            Console.Write("\n[ID:" + s.ID + "] ");
                            if (s.currentPort != null)
                            {
                                Console.Write("current port ID: " + s.currentPort.ID);
                            }
                            Console.Write("\n\n");
                        }
                        Console.WriteLine("Select ship ID to move out from port: ");
                        ID = Convert.ToInt32(Console.ReadLine());
                        foreach (Port p in main.ports)
                        {
                            Console.Write("\n[ID:" + p.ID + "]");
                            Console.Write("\n\n");
                        }
                        main.ports[main.ships[ID].currentPort.ID].outgoingShip(main.ships[ID]);
                        Console.WriteLine("New ships current state in choosen port: ");
                        Console.WriteLine("Port [" + main.ships[ID].currentPort.ID + "] - ships:");
                        foreach (Ship s in main.ports[main.ships[ID].currentPort.ID].current)
                        {
                            Console.WriteLine("\t[ID:" + s.ID + "]\n");
                        }
                        Console.WriteLine("New ships history state in choosen port: ");
                        Console.WriteLine("Port [" + main.ships[ID].currentPort.ID + "] - ships:");
                        foreach (Ship s in main.ports[main.ships[ID].currentPort.ID].history)
                        {
                            Console.WriteLine("\t[ID:" + s.ID + "]\n");
                        }
                        break;
                    case 3:
                        Console.WriteLine("Available ports");
                        foreach (Port p in main.ports)
                        {
                            Console.Write("\n[ID:" + p.ID + "]");
                            Console.Write("\n\n");
                        }
                        Console.WriteLine("Choose port initial ID:");
                        port = Convert.ToInt32(Console.ReadLine());
                        Console.WriteLine("Choose port target ID:");
                        targetPort = Convert.ToInt32(Console.ReadLine());
                        Console.WriteLine("Distance between ports in kilometers: " + main.ports[port].getDistance(main.ports[targetPort]) + "\n");
                        break;
                    case 4:
                        Console.WriteLine("Choose port:");
                        port = Convert.ToInt32(Console.ReadLine());
                        Console.WriteLine("Available containers");
                        foreach (Ship s in main.ships)
                        {
                            foreach (Container cont in s.currentPort.containers)
                            {
                                Console.WriteLine("[ID:" + cont.ID + "]" + " with weigth " + cont.weight + "\n");
                            }
                        }
                        Console.WriteLine("Choose container ID to check:");
                        ID = Convert.ToInt32(Console.ReadLine());
                        Console.WriteLine("Current container consumption: " + main.ports[port].containers[ID].consumption() + "\n\n");
                        break;
                    case 5:
                        Console.WriteLine("Choose port:");
                        port = Convert.ToInt32(Console.ReadLine());
                        Console.WriteLine("Available containers");
                        foreach (Ship s in main.ships)
                        {
                            foreach (Container cont in s.currentPort.containers)
                            {
                                Console.WriteLine("[ID:" + cont.ID + "]" + " with weigth " + cont.weight + "\n");
                            }
                        }
                        Console.WriteLine("Choose container ID initial:");
                        ID = Convert.ToInt32(Console.ReadLine());
                        Console.WriteLine("Choose container ID target:");
                        targetPort = Convert.ToInt32(Console.ReadLine());
                        bool val = main.ports[port].containers[ID].equals(main.ports[port].containers[targetPort]) == true;
                        Console.WriteLine("Containers equal state: " + val);
                        break;
                    case 6:
                        Console.WriteLine("Available ships");
                        foreach (Ship s in main.ships)
                        {
                            Console.WriteLine("[ID:" + s.ID + "]\n");
                        }
                        Console.WriteLine("Pick ship to check:");
                        ID = Convert.ToInt32(Console.ReadLine());
                        Console.WriteLine("Available Ports");
                        foreach (Port p in main.ports)
                        {
                            Console.WriteLine("[ID:" + p.ID + "]\n");
                        }
                        Console.WriteLine("Pick port to check:");
                        port = Convert.ToInt32(Console.ReadLine());
                        state = main.ships[ID].sailTo(main.ports[port]);
                        if (state == true)
                        {
                            Console.WriteLine("Ship sails to that port");
                        }
                        else
                        {
                            Console.WriteLine("Ship doesn't sail to that port");
                        }
                        break;
                    case 7:
                        Console.WriteLine("Available ships");
                        foreach (Ship s in main.ships)
                        {
                            Console.WriteLine("[ID:" + s.ID + "]" + " with fuel: " + s.fuel + "\n");
                        }
                        Console.WriteLine("Enter ship ID to refuel:");
                        ID = Convert.ToInt32(Console.ReadLine());
                        Console.WriteLine("Enter refuel value:");
                        refuel = Convert.ToInt32(Console.ReadLine());
                        main.ships[ID].reFuel(refuel);
                        Console.WriteLine("Ship was refueled");
                        Console.WriteLine("[ID:" + main.ships[ID] + "]" + " with fuel: " + main.ships[ID] + "\n");
                        break;
                    case 8:
                        Console.WriteLine("Available ships");
                        foreach (Ship s in main.ships)
                        {
                            Console.WriteLine("[ID:" + s.ID + "]\n");
                        }
                        Console.WriteLine("Enter ship ID to load:");
                        ID = Convert.ToInt32(Console.ReadLine());
                        Console.WriteLine("Available containers");
                        foreach (Container cont in main.containers)
                        {
                            Console.WriteLine("[ID:" + cont.ID + "]" + " with weight " + cont.weight + "\n");
                        }
                        Console.WriteLine("Pick container ID to load:");
                        port = Convert.ToInt32(Console.ReadLine());
                        state = main.ships[ID].load(main.containers[port]);
                        if (state == true)
                        {
                            Console.WriteLine("Container was loaded");
                        }
                        else
                        {
                            Console.WriteLine("Container wasn't loaded");
                        }
                        break;
                    case 9:
                        Console.WriteLine("Available ships");
                        Console.WriteLine("Available containers");
                        foreach (Ship s in main.ships)
                        {
                            foreach (Container cont in s.currentPort.containers)
                            {
                                Console.WriteLine("[ID-ship:]" + s.ID + "], " + "[ID-cont:" + cont.ID + "]" + " with weight " + cont.weight + "\n");
                            }
                        }
                        Console.WriteLine("Enter ship ID to unload:");
                        ID = Convert.ToInt32(Console.ReadLine());
                        Console.WriteLine("Pick container ID to unload:");
                        port = Convert.ToInt32(Console.ReadLine());
                        state = main.ships[ID].unLoad(main.containers[port]);
                        if (state == true)
                        {
                            Console.WriteLine("Container was unloaded");
                        }
                        else
                        {
                            Console.WriteLine("Container wasn't unloaded");
                        }
                        break;
                    default:
                        break;
                }
            }

            Console.Read();
        }
    }
}