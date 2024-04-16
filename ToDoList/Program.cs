// See https://aka.ms/new-console-template for more information
using System;
Console.WriteLine("Hello!");
string input;
List<string> todo = new List<string>(); 
bool PrintToDo()
{
   bool empty = false;
    if (todo.Count == 0)
    {
        Console.WriteLine("No Todos have been added yet");
        empty = true;
    }
    else
    {
        int j = 1;
        foreach (string st in todo)
        {
            Console.WriteLine($"{j}. {st}");
            j++;
        }
        
    }
    return empty;
}
bool RemoveToDo()
{
    Console.WriteLine("Select the index of the TODO you want to remove: ");
    bool empty=PrintToDo();
    return empty;
}

do
{
    Console.WriteLine("What do you want to do?");
    Console.WriteLine("[S]ee all todos");
    Console.WriteLine("[A]dd a todo");
    Console.WriteLine("[R]emove a todo");
    Console.WriteLine("[E]xit");
    input = Console.ReadLine();
    if (input != "S" && input != "s" && input != "A" && input != "a" && input != "R" && input != "r" && input!="E" && input!="e")
    {
        Console.WriteLine("Incorrect input");
        continue;
    }
    if(input=="S"||input=="s")
    {
        bool empty=PrintToDo();
        if (empty)
            continue;

    }
    else if(input=="A"||input=="a")
    {
        Console.WriteLine("Enter the Todo description: ");
        string desc= Console.ReadLine();
        if(desc=="")
        {
            Console.WriteLine("The description cannot be empty.");
        }
        else if(todo.Contains(desc))
        {
            Console.WriteLine("The description must be unique.");
        }
        else
        {
            todo.Add(desc);
        }
    }
    else if(input=="R"||input=="r")
    {
        string index;
        int remove = 0;
        do
        {
            bool empty=RemoveToDo();
            if(empty)
                break;
            index = Console.ReadLine();
            if (index == "")
            {
                Console.WriteLine("Selected index cannot be empty.");
                continue;
            }
            int no;
            bool isParse = int.TryParse(index, out no);
            if (!isParse || (isParse && (no > todo.Count || no <= 0)))
            {
                Console.WriteLine("The given index is not valid.");
                continue;
            }
            else
            {
                no = no - 1;
                Console.WriteLine("TODO removed: "+todo[no]);
                todo.RemoveAt(no);
                remove = 1;
            }
        } while (remove == 0);
    }
} while (input != "E" && input != "e");