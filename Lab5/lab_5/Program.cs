using System.Collections.Specialized;
using System.Runtime.CompilerServices;

namespace Program
{
    class Program
    {
        class Piece
        {
            public List<int> position { get; set; }
            public string color { get; set; }
            public int number_of_moves { get; set; }
            public MoveRule move_rule { get; set; }

            public Piece(List<int> position, string color, MoveRule move_rule) 
            {
                this.position = position;
                this.color = color;
                this.number_of_moves = 0;
                this.move_rule = move_rule;
            }

            public void move(List<int> new_position)
            {
                List<List<int>> moves = move_rule.get_all_moves();
                bool state = false;
                if (new_position[0] <= 8 || new_position[1] <= 8)
                {
                    foreach (List<int> move in moves)
                    {
                        if (move[0] == new_position[0] && move[1] == new_position[1])
                        {
                            this.position = new_position;
                            move_rule.position = new_position;
                            number_of_moves++;
                            state = true;
                        }
                    }

                    if (state == false)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\nCan't move there\n");
                        Console.ResetColor();
                    }
                }
            }

            public static void check_position_range(ref List<List<int>> positions, List<int> current_pos)
            {
                List<List<int>> mod_positions = new List<List<int>>(positions);
                foreach (List<int> position in positions)
                {
                    if (position[0] > 7 || position[1] > 7 
                        || position[0] < 0 || position[1] < 0 
                        || (position[0] == current_pos[0] && position[1] == current_pos[1]))
                    {
                        mod_positions.Remove(position);
                    }
                }
                positions = mod_positions;
            }

            public void print_info()
            {
                List<List<int>> moves = move_rule.get_all_moves();

                bool state = true;

                Console.Write($"    ");

                for (int i = 0; i < 8; i++)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write($"{i + 1}   ");
                    Console.ResetColor();
                }

                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.Write($"X");
                Console.ResetColor();

                Console.WriteLine();

                for (int i = 0; i < 8; i++)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write($"{i + 1}   ");
                    Console.ResetColor();
                    for (int j = 0; j < 8; j++)
                    {
                        foreach (List<int> move in moves)
                        {
                            if (i == move[0] && j == move[1])
                            {
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.Write("+   ");
                                Console.ResetColor();
                                state = false;
                            }
                        }

                        if (state == true)
                        {
                            if (i == position[0] && j == position[1])
                            {
                                Console.ForegroundColor = ConsoleColor.Blue;
                                Console.Write("$   ");
                                Console.ResetColor();
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.White;
                                Console.Write("*   ");
                                Console.ResetColor();
                            }
                        }

                        state = true;
                    }
                    Console.WriteLine();
                }
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.Write($"Y");
                Console.ResetColor();
                Console.WriteLine();
            }
        }

        abstract class MoveRule
        {
            public List<int> position;
            public virtual List<List<int>> get_all_moves()
            {
                return new List<List<int>>();
            }
        }

        class Knight : MoveRule
        {
            public Knight(List<int> position)
            {
                this.position = position;
            }

            public override List<List<int>> get_all_moves()
            {
                List<List<int>> moves = new List<List<int>>();

                moves.Add(new List<int>() { position[0] + 1, position[1] - 2 });
                moves.Add(new List<int>() { position[0] - 1, position[1] - 2 });

                moves.Add(new List<int>() { position[0] + 2, position[1] - 1 });
                moves.Add(new List<int>() { position[0] - 2, position[1] - 1 });

                moves.Add(new List<int>() { position[0] + 2, position[1] + 1 });
                moves.Add(new List<int>() { position[0] - 2, position[1] + 1 });

                moves.Add(new List<int>() { position[0] + 1, position[1] + 2 });
                moves.Add(new List<int>() { position[0] - 1, position[1] + 2 });

                Piece.check_position_range(ref moves, this.position);

                return moves;
            }
        }

        class Bishop : MoveRule
        {
            public Bishop(List<int> position)
            {
                this.position = position;
            }

            public override List<List<int>> get_all_moves()
            {
                List<List<int>> moves = new List<List<int>>();

                int x = position[0], y = position[1];

                for (int i = 0; i < 8; i++)
                {
                    moves.Add(new List<int>() { x++, y++ });
                }

                x = position[0];
                y = position[1];

                for (int i = 0; i < 8; i++)
                {
                    moves.Add(new List<int>() { x++, y-- });
                }

                x = position[0];
                y = position[1];

                for (int i = 0; i < 8; i++)
                {
                    moves.Add(new List<int>() { x--, y++ });
                }

                x = position[0];
                y = position[1];

                for (int i = 0; i < 8; i++)
                {
                    moves.Add(new List<int>() { x--, y-- });
                }

                Piece.check_position_range(ref moves, this.position);

                return moves;
            }
        }

        class King : MoveRule
        {
            public King(List<int> position)
            {
                this.position = position;
            }

            public override List<List<int>> get_all_moves()
            {
                List<List<int>> moves = new List<List<int>>();

                moves.Add(new List<int>() { position[0] + 1, position[1] });

                moves.Add(new List<int>() { position[0] - 1, position[1] });

                moves.Add(new List<int>() { position[0], position[1] + 1 });

                moves.Add(new List<int>() { position[0], position[1] - 1 });

                moves.Add(new List<int>() { position[0] + 1, position[1] + 1 });

                moves.Add(new List<int>() { position[0] + 1, position[1] - 1 });

                moves.Add(new List<int>() { position[0] - 1, position[1] + 1 });

                moves.Add(new List<int>() { position[0] - 1, position[1] - 1 });

                Piece.check_position_range(ref moves, this.position);

                return moves;
            }
        }

        class Rook : MoveRule
        {
            public Rook(List<int> position)
            {
                this.position = position;
            }

            public override List<List<int>> get_all_moves()
            {
                List<List<int>> moves = new List<List<int>>();

                int x = position[0], y = position[1];

                for (int i = 0; i < 8; i++)
                {
                    moves.Add(new List<int>() { x++, y });
                }

                x = position[0];
                y = position[1];

                for (int i = 0; i < 8; i++)
                {
                    moves.Add(new List<int>() { x--, y });
                }

                x = position[0];
                y = position[1];

                for (int i = 0; i < 8; i++)
                {
                    moves.Add(new List<int>() { x, y++ });
                }

                x = position[0];
                y = position[1];

                for (int i = 0; i < 8; i++)
                {
                    moves.Add(new List<int>() { x, y-- });
                }

                Piece.check_position_range(ref moves, this.position);

                return moves;
            }
        }

        static void Main(string[] args)
        {
            List<int> position = new List<int>();
            MoveRule move_rule;
            Piece figure;

            int option;
            string option_str;

            Console.WriteLine("Enter position of your figure (x axis) on board [between 1 - 8] ");

            option = Convert.ToInt32(Console.ReadLine());

            if (option < 1 || option > 8)
            {
                Console.WriteLine("Wrong x axis position value");
                return;
            }

            position.Add(option - 1);

            Console.WriteLine("Enter position of your figure (y axis) on board [between 1 - 8] ");

            option = Convert.ToInt32(Console.ReadLine());

            if (option < 1 || option > 8)
            {
                Console.WriteLine("Wrong y axis position value");
                return;
            }

            position.Add(option - 1);

            Console.WriteLine("Enter figure you want to create " +
                "[0] - Knight, [1] - Bishop, [2] - King, [3] - Rook");

            option = Convert.ToInt32(Console.ReadLine());

            switch (option)
            {
                case 0:
                    move_rule = new Knight(position);
                    break;
                case 1:
                    move_rule = new Bishop(position);
                    break;
                case 2:
                    move_rule = new King(position);
                    break;
                case 3:
                    move_rule = new Rook(position);
                    break;
                default:
                    Console.WriteLine("Wrong figure choosen");
                    return;
            }

            Console.WriteLine("Enter figure color you want to create" +
                " [0] - White, [1] - Black: ");

            option = Convert.ToInt32(Console.ReadLine());

            switch (option)
            {
                case 0:
                    option_str = "white";
                    break;
                case 1:
                    option_str = "black";
                    break;
                default:
                    Console.WriteLine("Wrong color picked");
                    return;
            }

            figure = new Piece(position, option_str, move_rule);

            int x, y;

            for (; ; )
            {
                figure.print_info();
                Console.WriteLine($"Moves done: {figure.number_of_moves}");
                Console.WriteLine($"Color: {figure.color}");
                Console.WriteLine("Enter x value to move: ");
                x = Convert.ToInt32(Console.ReadLine());
                Console.WriteLine("Enter y value to move: ");
                y = Convert.ToInt32(Console.ReadLine());
                figure.move(new List<int>() { y - 1, x - 1 });
            }
        }
    }
}