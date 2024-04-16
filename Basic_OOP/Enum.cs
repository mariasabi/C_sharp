using System;
//enumeration
enum Race
{
  Earthling,
  Marsian
}

class User
{

  public string uname;
  public Race race;
  public readonly int HEIGHT;
  public User(string username,Race race) 
    {
      uname=username;
      this.race=race;
      if(race==Race.Marsian)
        {
          HEIGHT=100;
        }
      else if(race==Race.Earthling)
        {
          HEIGHT=180;
        }
    }
}

public class Program
	{
		public static void Main()
		{
			User u1=new User("Joe",Race.Earthling);
		  Console.WriteLine(u1.HEIGHT);
		}
	}
