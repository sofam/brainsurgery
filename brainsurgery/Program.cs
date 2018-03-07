using System;

namespace BrainSurgery
{
    class Program
    {
        static void Main(string[] args)
        {
            var brainLoader = new BrainLoader();
            Console.WriteLine("byte {0}", sizeof(byte));
            Console.WriteLine("char {0}", sizeof(char));
            brainLoader.LoadBrain("data/megahal.brn");
            Console.WriteLine("Hello World!");
        }
    }
}
