
using System;
using HentaiUtility.Actions;

namespace HentaiUtility
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Utility program:\n" +
                              "0) Exit\n" +
                              "1) Hash images\n" +
                              "2) Find duplicates\n" +
                              "3) Construct similarity map");

            while (true)
            {
                var option = Console.ReadLine();
                switch (option)
                {
                    case "0":
                        Environment.Exit(1);
                        break;
                    case "1":
                        ImageHashing.Hash();
                        break;
                    case "2":
                        DuplicatesFinder.Find();
                        break;
                }
            }
        }
    }
}