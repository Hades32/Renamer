using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace Rename
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 2 && args.Length != 3 || (args.Length == 3 && args[2] != "-r"))
            {
                displayUsage();
                return;
            }
            bool regexp = MyGlobalMethods.Contains(args,"-r");
            bool dirsToo = MyGlobalMethods.Contains(args, "-d");
            string inPattern = args[0];
            string outPattern = args[1];
            string outfile;
            string shortFile;

            Regex inReg;
            Match match;
            RegexOptions regOpt = RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.ECMAScript;
            //RegExps
            if (regexp)
            {
                inReg = new Regex(inPattern, regOpt);
                if (inReg.GetGroupNumbers().Length <= 1)
                {
                    Console.WriteLine("You have to use at least one group in your Regexp!");
                    return;
                }
            }
            //Wildcards
            //we escape all chars but the wildcard character '*'
            else
            {
                inReg = new Regex(Regex.Escape(inPattern).Replace(@"\*", "(.*)"), regOpt);
            }

            int i = 0;
            while (outPattern.Contains("*"))
            {
                i++;
                outPattern = MyGlobalMethods.ReplaceFirst(outPattern, "*", "\\" + i.ToString());
            }

            foreach (var file in Directory.GetFiles(Directory.GetCurrentDirectory()))
            {
                Rename(outPattern, inReg, file);
            }

            if (dirsToo)
            {
                foreach (var dir in Directory.GetDirectories(Directory.GetCurrentDirectory()))
                {
                    Rename(outPattern, inReg, dir);
                }
            }
        }

        private static void Rename(string outPattern, Regex inReg, string file)
        {
            string outfile;
            Match match;
            String shortFile = Path.GetFileName(file);
            if ((match = inReg.Match(shortFile)).Success)
            {
                Console.Write("Trying to rename file ");
                Console.Write(shortFile);
                Console.Write(" to ");
                try
                {
                    outfile = outPattern;
                    for (int i = match.Groups.Count - 1; i > 0; i--)
                    {
                        outfile = outfile.Replace("\\" + i.ToString(), match.Groups[i].Value);
                    }
                    Console.Write(outfile);

                    //Is suitable for Files, too. ??Is File.Move good for Dirs, too???
                    Directory.Move(file, Path.Combine(Path.GetDirectoryName(file), outfile));

                    Console.WriteLine("\tSuccess");
                }
                catch
                {
                    var oldCol = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\tFailure");
                    Console.ForegroundColor = oldCol;
                }
            }
        }

        static void displayUsage()
        {   
            //TODO: Add path
            var usage =
@"rename InPattern OutPattern (-r) (-d)
    -r : full usage of RegExp in patterns
    -d : rename directories, too

Examples:
    rename *.mp3 *.wav           : changes extension from MP3 to WAV
    rename *.mp3 CD1_*.mp3       : addes CD1_ before every MP3
    rename *xxx*.jpg \2xxx\1.jpg : switches start and end of the filename
    rename .*(\d+).*\.mp3 \1.mp3 : renames all mp3s like *000*.mp3 to 000.mp3
";
            Console.WriteLine(usage);
        }
    }
}
