using System;
using ZebraUDP;

namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {

            var zplCmd = new ZplCmdBuilder();

            var zplCode = zplCmd.LabelLength(100)
                                .LabelHome(5, 50)
                                .FieldOrigin(2, 2, 2)
                                .FieldBlock("C")
                                .FieldSeparator()
                                .EndZPL()
                                .GetZplCode();
            TryFlow(() =>
            {
                var zebra = new ZebraPrinter("10.201.19.25", 6101);
                zebra.Send(zplCode);
            });

            Console.WriteLine("Send Zpl Code Done");


        }

        protected static void TryFlow(Action action)
        {
            try
            {
                action?.Invoke();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"例外事件發生 ex.Message={ex.Message}");
                Console.WriteLine($"例外事件發生 ex.StackTrace={ex.StackTrace}");
            }
            finally
            {
            }
        }
    }
}
