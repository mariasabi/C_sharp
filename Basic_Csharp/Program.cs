//string userChoice = Console.ReadLine();
//Console.WriteLine("User input:" + userChoice);
//userChoice = "ABC";
//int a = 5;
//Console.WriteLine(a + "hi");

//var choice=Console.ReadLine();
//bool isLong = IsLong(choice);
//if(isLong)
//{
//    Console.WriteLine("User input is long");
//}
//else
//{
//    Console.WriteLine("User input is short");
//}
//bool IsLong(string choice)
//{
//    return choice.Length > 10;
//}

//Console.WriteLine("Provide a no: ");
//string userInput=Console.ReadLine();
//int no=int.Parse(userInput);
//Console.WriteLine(no);

//string ConvertPointsToGrade(int pt)
//{
//    switch(pt)
//    {
//        case 10:
//        case 9:
//            return "A";
//        case 8:
//        case 7:
//        case 6:
//            return "B";

//        case 5:
//        case 4:
//        case 3:
//            return "C";
//        case 2:
//        case 1:
//            return "D";

//        case 0:
//            return  "E";

//        default:
//            return "!";
//    }
//}
//Console.WriteLine("Enter points(0-10): ");
//string userInput = Console.ReadLine();
//int no = int.Parse(userInput);
//Console.WriteLine(ConvertPointsToGrade(no));


//List<int> GetOnlyPositive(int[] numbers, out int countPositve)
//{
//    countPositve = 0;
//    var result = new List<int>();
//    foreach (int number in numbers)
//    {
//        if (number > 0)
//        {
//            result.Add(number);
//            countPositve++;
//        }
//    }
//    return result;
//}
//var numbers = new[] { 10, -8, 2, 12, -17 };
//int countPositive = 0;
//var onlyPositive = GetOnlyPositive(numbers, out countPositive);
//foreach (var pos in onlyPositive)
//{
//    Console.WriteLine(pos);
//}
//Console.WriteLine("Count of positive: " + countPositive);

//bool isParseSuccess;
//do
//{
//    Console.WriteLine("Enter no: ");
//    var input = Console.ReadLine();
//    isParseSuccess = int.TryParse(input, out var value);
//    if (isParseSuccess)
//    {
//        Console.WriteLine("Number is " + value);
//    }
//    else
//    {
//        Console.WriteLine("Not successful");
//    }
//} while (!isParseSuccess);


List<string> GetOnlyUpperCaseWords(List<string> words)
{
    List<string> result = new List<string>();
    for (int i = 0; i < words.Count; i++)
    {

        if (words[i].All(char.IsUpper))
        {
            if (!result.Contains(words[i]))
            {
                result.Add(words[i]);
            }
        }
    }
    return result;
}
List<string> words = new List<string>();
Console.WriteLine("Enter list elements (enter 'stop' to stop): ");
string st;
do
{
    st = Console.ReadLine();
    words.Add(st);
} while (st != "stop");

List<string> result = GetOnlyUpperCaseWords(words);
Console.WriteLine("Upper case words: ");
foreach (var word in result)
{
    Console.WriteLine(word);
}