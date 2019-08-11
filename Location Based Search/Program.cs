using System;

namespace Location_Based_Search
{
	class Program
	{
		static void Main(string[] args)
		{
			Search search = new Search();
			search.NewSearch();
			
			Console.WriteLine("Press any key to exit...");
			Console.ReadLine();
		}
	}
}
