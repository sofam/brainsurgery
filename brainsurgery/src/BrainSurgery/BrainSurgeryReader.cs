using System;
using System.Collections.Generic;
using System.IO;

// Data structure to be able to read back a MegaHAL brain
namespace BrainSurgery
{

    class Tree
    {
        public ulong Usage;
        public ushort Symbol;
        public ushort Branch;

        public ushort Count;
        public List<Tree> Nodes;

    }

    class BrainString
    {
        public ushort Length;
        public System.String Word;

    }

    class DictionaryEntry
    {
        public ulong Size;
        public BrainString Entry;
        public ushort Index;
    }

    class Model
    {
        public char Order;
        public Tree Forward;
        public Tree Backward;
        public Tree Context;

        public List<String> ModelDictionary;
    }

    class BrainLoader
    {
        char[] Cookie = { 'M', 'e', 'g', 'a', 'H', 'A', 'L', 'v', '8' };

        private String Filename;

        private Model BrainModel;
        public void LoadBrain(String filename)
        {
            BrainModel = new Model();
            byte[] FileCookie = new byte[Cookie.Length];
            var file = File.ReadAllBytes(filename);
            var ms = new MemoryStream(file);
            ms.Read(FileCookie, 0, Cookie.Length);
            var c1 = new String(Cookie);
            var c2 = System.Text.Encoding.GetEncoding("ISO-8859-1").GetString(FileCookie, 0, Cookie.Length);

            if (c2 != c1)
            {
                Console.WriteLine("File {0} is not a MegaHAL brain Cookie is: {1}, FileCookie is: {2}", filename, Cookie.ToString(), FileCookie.ToString());
            }
            Console.WriteLine("File {0} is a MegaHAL brain Cookie is: {1}, FileCookie is: {2}", filename, c1, c2);



            Console.WriteLine("Position: {0}", ms.Position);
            BrainModel.Order = Convert.ToChar(ms.ReadByte());
            Console.WriteLine("Read {0}", BrainModel.Order);
            Console.WriteLine("Position: {0}", ms.Position);
            BrainModel.Forward = new Tree();
            LoadTree(ms, BrainModel.Forward);
            BrainModel.Backward = new Tree();
            LoadTree(ms, BrainModel.Backward);
            BrainModel.ModelDictionary = new List<String>();
            LoadDictionary(ms, BrainModel.ModelDictionary);
            BrainModel.ModelDictionary.Sort();
            foreach (var item in BrainModel.ModelDictionary)
            {
                Console.WriteLine("{0} {1}",item, BrainModel.ModelDictionary.IndexOf(item));

            }
            while (true)
            {
                var itemNumber = Convert.ToInt32(Console.ReadLine());
                Console.WriteLine(itemNumber);
                Console.WriteLine(BrainModel.ModelDictionary[itemNumber]);

            }

        }

        public void LoadTree(MemoryStream ms, Tree tree)
        {
            var level = 0;

            byte[] symbol = new byte[sizeof(ushort)];
            byte[] usage = new byte[sizeof(ulong)];
            byte[] count = new byte[sizeof(ushort)];
            byte[] branch = new byte[sizeof(ushort)];
            ms.Read(symbol, 0, sizeof(ushort));
            ms.Read(usage, 0, sizeof(ulong));
            ms.Read(count, 0, sizeof(ushort));
            ms.Read(branch, 0, sizeof(ushort));
            tree.Symbol = BitConverter.ToUInt16(symbol, 0); //Convert.ToUInt16(symbol);
            tree.Usage = BitConverter.ToUInt64(usage, 0);
            tree.Count = BitConverter.ToUInt16(count, 0);
            tree.Branch = BitConverter.ToUInt16(branch, 0);



            if (tree.Branch == 0) return;
            tree.Nodes = new List<Tree>();

            for (var i = 0; i < tree.Branch; ++i)
            {
                ++level;
                tree.Nodes.Add(new Tree());
                LoadTree(ms, tree.Nodes[tree.Nodes.Count - 1]);
                --level;

            }
            if (level == 0) return;
        }

        public void LoadDictionary(MemoryStream ms, List<String> dictionary)
        {
            int size;
            byte[] sizeByte = new byte[sizeof(ulong)];
            ms.Read(sizeByte, 0, sizeof(ulong));

            size = BitConverter.ToInt32(sizeByte, 0);
            Console.WriteLine("Size of dictionary: {0}", size);
            for (var i = 0; i < size; ++i)
            {
                LoadWord(ms, dictionary);
            }
        }

        public void LoadWord(MemoryStream ms, List<String> dictionary)
        {
            byte[] sizeWordLength = new byte[sizeof(byte)];
            byte wordLength = 0;
            BrainString word = new BrainString();
            DictionaryEntry dictionaryEntry = new DictionaryEntry();

            ms.Read(sizeWordLength, 0, sizeof(byte));

            wordLength = sizeWordLength[0];
            Console.WriteLine("Word length: {0}", wordLength);

            byte[] wordBuffer = new byte[wordLength];

            
            ms.Read(wordBuffer, 0, Convert.ToInt16(wordLength));
            word.Word = System.Text.Encoding.GetEncoding("ISO-8859-1").GetString(wordBuffer);
            Console.WriteLine("{0}", word.Word);
            word.Length = wordLength;
            
            dictionaryEntry.Entry = new BrainString();
            dictionaryEntry.Entry.Word = word.Word;
            dictionaryEntry.Entry.Length = word.Length;
            dictionary.Add(word.Word);
        }
    }

}
