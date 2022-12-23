using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SimpleComponents;

namespace Machine
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            string[] paths = new string[]
            {
                //"Add.hack",
                //"Add01To2.hack",
                //"Copy1To0.hack",
                //"Max.hack",
                //"MaxL.hack",
                //"Product.hack",
                //"ScreenExample.hack",
                //"SimpleLoop.hack",
                "Sum100To200.hack",
                //"TestJumping.hack",

            };
            foreach(string path in paths)
            {
                RunProgram(path);
            }
            
        }

        private static void RunProgram(string FileName)
        {
            Machine16 machine = new Machine16(false, true);
            machine.Code.LoadFromFile(@"BinaryCode\"+FileName);
            machine.Data[0] = 100;
            machine.Data[1] = 15;
            DateTime dtStart = DateTime.Now;
            machine.Reset();
            for (int i = 0; i < 200; i++)
            {
                machine.CPU.PrintState();
                Console.WriteLine();
                Clock.ClockDown();
                Clock.ClockUp();
            }

            Console.WriteLine("Done "+FileName+" "+ (DateTime.Now - dtStart).TotalSeconds);
            Console.ReadLine();
        }
    }
}
