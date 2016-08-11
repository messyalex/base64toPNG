using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace base64toPNGConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Decoder decoder = new base64toPNGConsole.Decoder();
        }
    }

    public class Decoder
    {
        private string desktopPath;
        private string sourceDir;
        private string destinationDir;

        public Decoder()
        {
            desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            Console.WriteLine("Source directory name in desktop: ");
            sourceDir = Console.ReadLine();
            Console.WriteLine("Destination directory name in desktop: ");
            destinationDir = Console.ReadLine();

            string[] files = Directory.GetFiles(Path.Combine(desktopPath, sourceDir), "*.html", SearchOption.TopDirectoryOnly);

            foreach (string file in files)
            {
                int lastDirectoryPos = file.LastIndexOf("\\") + 1;
                int extentionPos = file.IndexOf(".html");
                int i = int.Parse(file.Substring(lastDirectoryPos, extentionPos - lastDirectoryPos));

                string contents = getSourceString(i);

                if (contents == null)
                {
                    Console.WriteLine(i.ToString() + " already exists");
                    continue;
                }

                string base64 = extractImage(i, contents);

                SaveImageFile(i, base64);

                Console.WriteLine(i.ToString() + " finished");
            }

            Console.ReadLine();
        }

        public string getSourceString(int pageNo)
        {
            string sourcePath = Path.Combine(Path.Combine(desktopPath, sourceDir), pageNo.ToString() + ".html");

            string destPath = Path.Combine(Path.Combine(desktopPath, destinationDir), pageNo.ToString() + ".png");

            if (File.Exists(destPath))
                return null;

            return File.ReadAllText(sourcePath);
        }

        public string extractImage(int pageNo, string pageString)
        {
            int sectionPos = pageString.IndexOf("id=\"page" + pageNo);
            //Console.WriteLine("sectionPos: " + sectionPos);
            string backgroundImageAttr = ";base64,";
            int imageStart = pageString.IndexOf(backgroundImageAttr, sectionPos) + backgroundImageAttr.Length;
            //Console.WriteLine("imageStart: " + imageStart);
            int imageEnd = pageString.IndexOf(">", imageStart);
            //Console.WriteLine("imageEnd: " + imageEnd);
            int imageLength = imageEnd - imageStart;

            string base64 = pageString.Substring(imageStart, imageLength);
            base64 = base64.Replace("background-repeat: no-repeat", "");
            base64 = base64.Replace("\"", "");
            base64 = base64.Replace(")", "");
            base64 = base64.Replace(";", "");
            base64 = base64.Replace("&quot", "");
            base64 = base64.Replace(" ", "");

            //Console.WriteLine(base64);

            return base64;
        }

        public void SaveImageFile(int pageNo, string base64Image)
        {

            string destinationImgPath = Path.Combine(Path.Combine(desktopPath, destinationDir), pageNo.ToString() + ".png");

            //Console.WriteLine("trying " + pageNo.ToString() + " ...");
            //Console.ReadLine();

            var bytes = Convert.FromBase64String(base64Image);
            using (var imageFile = new FileStream(destinationImgPath, FileMode.Create))
            {
                imageFile.Write(bytes, 0, bytes.Length);
                imageFile.Flush();
            }
        }

    }
}
