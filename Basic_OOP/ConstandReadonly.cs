using System;

class User
{

  public const int AGE=18; //const
  public static int currentID; //static field
  public readonly int ID; //readonly
  public User() 
    {
      currentID++;
      ID=currentID;
    }
}

public class Program
	{
		public static void Main()
		{
			User u1=new User();
		  User u2=new User();
		  Console.WriteLine(u1.ID);
		  Console.WriteLine(u2.ID);
		  Console.WriteLine(User.currentID);
		  Console.WriteLine(User.AGE);
		}
	}
