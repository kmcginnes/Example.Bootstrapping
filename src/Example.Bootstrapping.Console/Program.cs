using System;

namespace Example.Bootstrapping.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var app = new Bootstrapper();

            try
            {
                app.Start(args).Wait(TimeSpan.FromMinutes(2));

                System.Console.ReadKey();
            }
            catch (Exception exception)
            {
                typeof(Program).Name.Log().Fatal(exception, "Failed to startup application");
                throw;
            }
        }
    }
}
