using System;

namespace Example.Bootstrapping.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var app = new Bootstrapper();

            app.Start(args).Wait();
        }
    }
}
