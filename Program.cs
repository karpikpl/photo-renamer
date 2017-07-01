using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace PhotoRename
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("hello");
            string directory = args.Length > 0 ? args[0] : Assembly.GetExecutingAssembly().GetName().CodeBase;
            var dir = new DirectoryInfo(directory);
            Console.WriteLine("Processing files in {0}", directory);

            var files = dir.GetFiles("*.jpg");
            Regex regex = new Regex("\\b\\d{4}-\\d{2}-\\d{2}_\\d{2}-\\d{2}-\\d{2}_.*");

            foreach (var file in files)
            {
                if (regex.IsMatch(file.Name))
                {
                    Console.WriteLine("Skipping {0} because it was already renamed", file.Name);
                }

                Console.WriteLine("Processing {0}", file.Name);
                byte[] dateTakenBytes = null;

                using (var bmp = System.Drawing.Image.FromFile(file.FullName))
                {
                    if (bmp.PropertyItems.All(p => p.Id != 36867))
                    {
                        Console.WriteLine("{0} doesn't have 'date taken' data - skipping", file.Name);
                        continue;
                    }
                    dateTakenBytes = bmp.GetPropertyItem(36867).Value;
                }

                var dateTakenString = Encoding.ASCII.GetString(dateTakenBytes);
                var dateTaken = DateTime.ParseExact(dateTakenString, "yyyy:MM:dd HH:mm:ss\0", CultureInfo.InvariantCulture);

                Console.WriteLine("Photo {0} was taken {1}", file.Name, dateTaken);

                var newName = dateTaken.ToString("yyyy-MM-dd_HH-mm-ss") + "_" + file.Name;


                File.Move(file.FullName, Path.Combine(file.DirectoryName, newName));

                Console.WriteLine("{0} -> {1}", file.Name, newName);
            }

            Console.WriteLine(directory);
        }
    }
}
