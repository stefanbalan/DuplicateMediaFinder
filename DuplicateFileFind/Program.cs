using System;
using System.IO;
using System.Threading.Tasks;

namespace DuplicateFileFind
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            if (args.Length < 1)
            {
                PrintUsage();
                return;
            }

            var logic = new Logic();

            var recurse = false;

            var commandArgument = args[0];
            string fileArg;
            FileInfo fi;
            DirectoryInfo di;
            switch (commandArgument[0])
            {
                case 'r':
                    if (args.Length < 2)
                    {
                        PrintUsage();
                        return;
                    }

                    fileArg = args[1];
                    fi = new FileInfo(fileArg);
                    di = new DirectoryInfo(fileArg);

                    if (!di.Exists)
                        return;

                    recurse = commandArgument.Length == 2 && commandArgument[1] == 'r';

                    await logic.AddReference(di, recurse);
                    break;


                case 'l':
                    var directories = commandArgument.Length == 2 && commandArgument[1] == 'd';
                    await logic.List(directories);
                    break;

                case 'd':

                    fileArg = args[1];
                    fi = new FileInfo(fileArg);
                    di = new DirectoryInfo(fileArg);

                    if (!di.Exists)
                        return;
                    recurse = commandArgument.Length == 2 && commandArgument[1] == 'r';

                    await logic.Delete(di, recurse);
                    break;
            }
            Console.WriteLine("___done___");
            Console.ReadKey();
        }

        private static void PrintUsage()
        {
            Console.WriteLine("r - add reference");
        }
    }
}
