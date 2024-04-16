using System;

class Point
{
  public int x;
  public int y;
  public int z;
  public Point()
  {
    
  }
  public Point(int x,int Y)
    {
      this.x=x;
      y=Y;
      z=5;
    }
}

public class Program
	{
		public static void Main()
		{
			Point pt=new Point(4,6);
			Console.WriteLine($"{pt.x},{pt.y},{pt.z}");
			
			Point p=new Point();
			p.x=10;
			p.y=2;
			p.z=8;
			Console.WriteLine($"{p.x},{p.y},{p.z}");
		}
	}

