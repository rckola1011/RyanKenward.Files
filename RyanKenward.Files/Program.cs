using System;
using RyanKenward.Files.Business;

namespace RyanKenward.Files
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            if (args.Length != 3)
                throw new ArgumentException("Please provide 3 command line arguments in the form of: search directory, search phrase, and output file name.");

            new SearchManager().Search(args[0], args[1], args[2]);
        }
    }
}
