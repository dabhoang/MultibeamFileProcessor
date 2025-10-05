using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MultibeamFileProcessor
{
    public static class UnitTest
    {
        public static void RunTests()
        {
            TestChecksum();
            TestChecksum2();
            TestValidationHappy();
            TestValidationUnhappy();
        }
        public static void TestChecksum()
        {
            Console.WriteLine("Testing Checksum Case 1");
            string testDir = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "UnitTest";
            string testFilePath = testDir + Path.DirectorySeparatorChar + "MultibeamTest.txt";

            byte[] checksum = FileProcessor.ComputeChecksum(testFilePath);
            Console.WriteLine("Actual checksum: ");
            FileProcessor.PrintByteArray(checksum);
            string actualChecksumString = "";
            for (int i = 0; i < checksum.Length; i++)
            {
                actualChecksumString = actualChecksumString + checksum[i].ToString();
            }
            Console.WriteLine(actualChecksumString);

            string expectedString = "21914087156184171229071911219114916522014811066220123214134177246902392511742291702191";
            
            Console.WriteLine("Expected checksum: " + expectedString);
            if (actualChecksumString.Equals(expectedString))
                Console.WriteLine("Pass");
            else Console.WriteLine("Fail");
        }

        public static void TestChecksum2()
        {
            Console.WriteLine("Testing Checksum Case 2");
            string testDir = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "UnitTest";
            string testFilePath = testDir + Path.DirectorySeparatorChar + "csharpext.cs";

            byte[] checksum = FileProcessor.ComputeChecksum(testFilePath);
            Console.WriteLine("Actual checksum: ");
            FileProcessor.PrintByteArray(checksum);
            string actualChecksumString = "";
            for (int i = 0; i < checksum.Length; i++)
            {
                actualChecksumString = actualChecksumString + checksum[i].ToString();
            }
            Console.WriteLine(actualChecksumString);

            string expectedString = "1415317811933416765411739712690150471132161701981761642111251716518831104324";

            Console.WriteLine("Expected checksum: " + expectedString);
            if (actualChecksumString.Equals(expectedString))
                Console.WriteLine("Pass");
            else Console.WriteLine("Fail");
        }
        public static void TestValidationHappy()
        {
            Console.WriteLine("Testing Validation Happy");
            string testDir = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "UnitTest";
            string testFilePath = testDir + Path.DirectorySeparatorChar + "MultibeamTest.txt";
            byte[] checksum = FileProcessor.ComputeChecksum(testFilePath);
            string ext = Path.GetExtension(testFilePath);
            Console.WriteLine("Actual Extension: " + ext);
            bool isValid = FileProcessor.IsValid(checksum, ext);
            Console.WriteLine("Is Valid: " + isValid);
            if (isValid)
                Console.WriteLine("Pass");
            else Console.WriteLine("Fail");
        }

        public static void TestValidationUnhappy()
        {
            Console.WriteLine("Testing Validation Unhappy");
            string testDir = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "UnitTest";
            string testFilePath = testDir + Path.DirectorySeparatorChar + "csharpext.cs";
            byte[] checksum = FileProcessor.ComputeChecksum(testFilePath);
            string ext = Path.GetExtension(testFilePath);
            Console.WriteLine("Actual Extension: " + ext);
            bool isValid = FileProcessor.IsValid(checksum, ext);
            Console.WriteLine("Is Valid: " + isValid);
            if (!isValid)
                Console.WriteLine("Pass");
            else Console.WriteLine("Fail");
        }
    }
}
