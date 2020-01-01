using System;
using dotnetCampus.DotNETBuild;

namespace dotnetCampus.IPC.WCF.Build
{
    class Program : Compiler
    {
        static void Main(string[] args)
        {
            var program = new Program();
            program.Compile();
        }

        public override void Compile()
        {
            Nuget.Restore();
            MsBuild.Build();
        }
    }
}
