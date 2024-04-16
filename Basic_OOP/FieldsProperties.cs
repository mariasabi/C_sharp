using System;

class User
{
  private string username;
  private int password;
  private int age;
  public string Username //username validation
    {
      get
        {
          return "Username is "+username;
        }
      set
        {
          if(value.Length>=4 && value.Length<=10)
            {
              username=value;
            }
          else
            {
              Console.WriteLine("Invalid username");
            }
        }
    }
  public int Password //writeonly
    {
      set
        {
          password=value;
          Console.WriteLine("Password set");
        }
    }  
  public int Age //readonly
    {
      get
        {
          return age;
        }
    }
  public User(int age) //constructor
    {
      this.age=age;
    }
  
}



public class Program
	{
		public static void Main()
		{
			User user=new User(9);
			user.Username="a";
			Console.WriteLine(user.Username);
			user.Username="abcd";
			Console.WriteLine(user.Username);
			Console.WriteLine("Age is "+user.Age);
			user.Password=123;
		}
	}
