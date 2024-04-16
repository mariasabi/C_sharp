// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello!\nInput the first number:");
string sa=Console.ReadLine();
int a=int.Parse(sa);
Console.WriteLine("Input the second number:");
string ba = Console.ReadLine();
int b = int.Parse(ba);
Console.WriteLine("What do you want to do with those numbers?");
Console.WriteLine("[A]dd");
Console.WriteLine("[S]ubtract");
Console.WriteLine("[M]ultiply");
string choice = Console.ReadLine();
if(choice=="A"||choice=="a")
{
    Console.WriteLine($"{a} + {b} = {a + b}");
}
else if(choice=="S" || choice == "s")
{
    Console.WriteLine($"{a} - {b} = {a - b}");
}
else if(choice=="M" || choice == "m")
{
    Console.WriteLine($"{a} * {b} = {a * b}");
}
else
{
    Console.WriteLine("Invalid option");
}