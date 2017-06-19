using System;

namespace bf
{
    class Program
    {
        static void Main(string[] args)
        {
            int[] memory = new int[UInt16.MaxValue];
            for (int i = 0; i < memory.Length; i++)
            {
                memory[i] = 0;
            }
            int p = 0;
            string cmd = "";
            bool run = true;
            int depth;
            ConsoleKeyInfo cki;
            Random rnd = new Random();
            Console.WriteLine("Enter h for help.");
            while (true)
            {
                run = true;
                Console.Write($"{p}# ");
                cmd = Console.ReadLine();
                if (cmd == "q")
                {
                    break;
                } else if (cmd == "h")
                {
                    Console.WriteLine(@"Commands:
    p   -   Move the pointer one cell to the left. if the pointer gets below 
            zero, then it cycles to the last position of the memory.
    n   -   Move the pointer one cell to the right. if the pointer gets above 
            the available memory, then it becomes zero.
    i   -   Increment the value of the cell appointed.
    d   -   Decrement the value of the cell appointed.
    ()  -   Repeat the code inside the parenthesis until the appointed value 
            becomes zero.
    w   -   Write the character in the cell appointed the the default output.
    r   -   Read one character from the default input to the appointed cell.
    $   -   Read everything from the default input to the memory until it 
            find a new line.
    'c  -   Write the character 'c' to the appointed cell.
    ""  -   Write everything between the double quotes to the memory.
    ;   -   Write the numeric value of the appointed cell to the default 
            output.
    ^   -   Jump the pointer back to the first cell.
    j   -   Jump the code to the position contained in the current cell.
    c   -   Clear all the memory.
    %   -   If the current cell has a value greater than zero, generates a 
            random number between zero, and that value, otherwise generates a
            number between zero and INT_MAX.
    \n  -   Write the number 'n' to the current cell.
    
For compatibility with brainfuck, the commands '<>+-[]' are also accepted 
instead of 'pnid()'.

The default memory size is UInt16.MaxSize.

The Interpreter accept the following commands:

    h   -   Show this help.
    q   -   Exit the interpreter.

If the execution enters a loop or a deadlock, you can break out of it by 
pressing ESC.
                        ");
                }
                for (int i = 0; i < cmd.Length; i++)
                {
                    if (!run)
                    {
                        break;
                    }
                    if (Console.KeyAvailable)
                    {
                        cki = Console.ReadKey(true);
                        if (cki.Key == ConsoleKey.Escape)
                        {
                            Console.WriteLine("User aborted.");
                            break;
                        }
                    }

                    switch (cmd[i])
                    {
                        case '>':
                        case 'n':
                            p++;
                            if (p >= memory.Length)
                            {
                                p = 0;
                            }
                            break;
                        case '<':
                        case 'p':
                            p--;
                            if (p < 0)
                            {
                                p = memory.Length - 1;
                            }
                            break;
                        case '+':
                        case 'i':
                            memory[p]++;
                            break;
                        case '-':
                        case 'd':
                            memory[p]--;
                            break;
                        case '.':
                        case 'w':
                            Console.Write((char)memory[p]);
                            break;
                        case ',':
                        case 'r':
                            cki = Console.ReadKey();
                            if (cki.Key == ConsoleKey.Escape)
                            {
                                run = false;
                                break;
                            }
                            memory[p] = (int)cki.KeyChar;
                            break;
                        case '$':
                            while (true)
                            {
                                if (p >= memory.Length)
                                {
                                    p = 0;
                                }
                                cki = Console.ReadKey(true);
                                if (cki.Key == ConsoleKey.Enter)
                                {
                                    Console.WriteLine("");
                                    break;
                                }
                                memory[p++] = (int)cki.KeyChar;
                                Console.Write(cki.KeyChar);
                            }
                            break;

                        case '\'':
                            if (i + 1 < cmd.Length)
                            {
                                memory[p] = (int)cmd[i + 1];
                            } else
                            {
                                Console.WriteLine("MALFORMED character.");
                                run = false;
                            }
                            i++;
                            break;
                        case '"':
                            while (true)
                            {
                                i++;
                                if (i >= cmd.Length)
                                {
                                    Console.WriteLine("STRING terminator not found.");
                                    run = false;
                                    break;
                                }
                                if (cmd[i] == '"')
                                {
                                    break;
                                }
                                memory[p] = (int)cmd[i];
                                p++;
                                if (p >= memory.Length)
                                {
                                    p = 0;
                                }
                            }
                            break;
                        case ';':
                            Console.Write(memory[p]);
                            break;
                        case '[':
                        case '(':
                            if (memory[p] == 0)
                            {
                                depth = 1;
                                while (depth > 0)
                                {
                                    i++;
                                    if (i >= cmd.Length)
                                    {
                                        Console.WriteLine("LOOP failed to found it's terminator.");
                                        run = false;
                                        break;
                                    }
                                    char c = cmd[i];
                                    if (c == '[' || c == '(')
                                    {
                                        depth++;
                                    }
                                    else if (c == ']' || c == ')')
                                    {
                                        depth--;
                                    }
                                }
                            }
                            break;
                        case ']':
                        case ')':
                            depth = 1;
                            while (depth > 0)
                            {
                                i--;
                                if (i < 0)
                                {
                                    Console.WriteLine("LOOP failed to found it's beginning");
                                    run = false;
                                    break;
                                }
                                char c = cmd[i];
                                if (c == '[' || c == '(')
                                {
                                    depth--;
                                }
                                else if (c == ']' || c == ')')
                                {
                                    depth++;
                                }
                            }
                            i--;
                            break;
                        case '^':
                            p = 0;
                            break;
                        case 'j':
                            i = memory[p];
                            if (i < 0)
                            {
                                i = 0;
                            } else if (i >= cmd.Length)
                            {
                                Console.WriteLine("JUMP exceeds the memory size.");
                                run = false;
                            }
                            break;
                        case 'c':
                            for (int j = 0; j < memory.Length; j++)
                            {
                                memory[j] = 0;
                            }
                            break;
                        case '%':
                            memory[p] = rnd.Next(0, memory[p] + 1 > 0 ? memory[p] + 1 : int.MaxValue);
                            break;
                        case '\\':
                            string arg = cmd.Substring(i + 1);
                            if (arg == "")
                            {
                                Console.WriteLine("Expected character code.");
                                run = false;
                                break;
                            }
                            string code = "";
                            for (int j = 0; j < arg.Length; j++)
                            {
                                if (char.IsDigit(arg[j]) || (j == 0 && arg[j] == '-'))
                                {
                                    code += arg[j];
                                } else
                                {
                                    break;
                                }
                            }
                            if (code.Length == 0)
                            {
                                Console.WriteLine("Expected numeric character code.");
                                run = false;
                                break;
                            }
                            memory[p] = int.Parse(code);
                            break;
                    }
                }
                Console.WriteLine("");
            }
        }
    }
}
