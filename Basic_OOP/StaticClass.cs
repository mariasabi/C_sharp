using System;

//static class
static class Utility
{
  public static void ColorfulWrite(string message, ConsoleColor color)
    {
      Console.WriteLine(message+" written in "+color);
    }
}


class User
{
  public string uname;
  public User(string username) 
    {
      uname=username;
    }
}

public class Program
	{
		public static void Main()
		{
			User u1=new User("Joe");
		  Utility.ColorfulWrite(u1.uname,ConsoleColor.DarkRed);
		}
	}
