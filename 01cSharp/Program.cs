//comments - single line comment
/*
    multi-line comments
*/

//1. Namespace- is logical seperation of the programs or code
using System;
// "using" is a key word or a block. here it is used as a keyword, to bring an existing class level into the current program
// "system" is the name of the library
// 1.1 Predefined namespace - using brings the existing lib intp the current program


//1.2 Custom Namespace
namespace _01cSharp
{
    //1. Types - Primitive types (int, string, char,), Value types, Reference types
    // Types can be - struct, class, interface, delegate, enums
    class Program
    {
        // 3. Type has Type members (variables/fields, methods, properties, events, indexers, etc.)
        // C# Conventions -class, methods => ProperCase, variables => camelCase
        static void Main(string[] args) // "Main" method, is an entry point
            // "static" doesnt need to be called. "void" reurns nothing
        {   
            string name;
            Console.WriteLine("Please enter your name");
            name = Console.ReadLine(); // to take input from user in string form
            //Console.WriteLine("Welcome {0} to Revature!", name); //"Console" is a class within the System namespace, "WriteLine" is a method- always ProperCase
            //Console.WriteLine($"Welcome {name) to Revature"); // string extrapolation with string 
            Console.WriteLine($"Welcome {name} to Revature!");
        }
    }
}
