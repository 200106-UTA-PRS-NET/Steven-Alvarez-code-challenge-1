using System;

namespace LibraryProject
{
    class Palindrome
    {
        public static void Main(string[] args)
        {
            string string1, rev;
            string1 = "Hannah";
            char[] ch = string1.ToCharArray();

            Array.Reverse(ch);
            rev = new string(ch);

            bool b = string1.Equals(rev, StringComparison.OrdinalIgnoreCase);
            if (b == true)
            {
                Console.WriteLine("" + string1 + " Is a Palindrome!");
            }
            else
            {
                Console.WriteLine("" + string1 + " Is not a Palindrome!");
            }
            Console.Read();

        }
    }
}
