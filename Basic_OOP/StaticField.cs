using System;

class User
{
  private string username;
  private int password;
  private int age;
  public static int ID; //static field
  
  public User() //constructor
    {
      ID++;
      Console.WriteLine("User id is "+ID);
    }
}

public class Program
	{
		public static void Main()
		{
			User u1=new User();
		  User u2=new User();
		  Console.WriteLine(User.ID);
		}
	}
