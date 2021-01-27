using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace UMLGenerator.Libraries
{
    public static class PlantUMLMethods
    {
        public static async Task<string> GetRemoteSVG(string content)
        {
            var client = new HttpClient();
            content = Uri.UnescapeDataString(content);
            var bytes = Encode(content);
            var str = $"http://plantuml.com/plantuml/svg/{bytes}";
            return await client.GetStringAsync(str);
        }

        public static async Task<string> GetRemoteSVG(string content, CancellationToken cancellationToken)
        {
            var client = new HttpClient();
            content = Uri.UnescapeDataString(content);
            var bytes = Encode(content);
            var str = $"http://plantuml.com/plantuml/svg/{bytes}";
            return await client.GetStringAsync(str, cancellationToken);
        }

        public static async Task<string> GetLocalSVG(string content, CancellationToken cancellationToken)
        {
            string tempPath = Path.GetTempFileName();
            File.WriteAllText(tempPath, content);

            Process p = new Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.FileName = "cmd.exe";
            string pathToPlantUML = $"{Directory.GetCurrentDirectory()}\\Resources\\PlantUML\\plantuml.jar";
            p.StartInfo.Arguments = $"/C type {tempPath} | java -jar {pathToPlantUML} -pipe -tsvg";
            p.Start();
            string output = await p.StandardOutput.ReadToEndAsync();
            p.WaitForExit();
            if (File.Exists(tempPath))
                File.Delete(tempPath);
            return output;
        }

        public static Bitmap GetLocalPNG(string content, CancellationToken cancellationToken)
        {
            string tempPath = Path.GetTempFileName();
            File.WriteAllText(tempPath, content);

            Process p = new Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.FileName = "cmd.exe";
            string pathToPlantUML = $"{Directory.GetCurrentDirectory()}\\Resources\\PlantUML\\plantuml.jar";
            p.StartInfo.Arguments = $"/C type {tempPath} | java -DPLANTUML_LIMIT_SIZE=8192 -jar {pathToPlantUML} -pipe -tpng";
            p.Start();
            var output = new Bitmap(p.StandardOutput.BaseStream);
            p.WaitForExit();
            if (File.Exists(tempPath))
                File.Delete(tempPath);
            return output;
        }

        private static string Encode(string content)
        {
            using (var output = new MemoryStream())
            {
                using (var writer = new StreamWriter(new DeflateStream(output, CompressionLevel.Optimal), Encoding.UTF8))
                    writer.Write(content);
                return Encode(output.ToArray());
            }
        }

        private static string Encode(IReadOnlyList<byte> bytes)
        {
            var length = bytes.Count;
            var s = new StringBuilder();
            for (var i = 0; i < length; i += 3)
            {
                var b1 = bytes[i];
                var b2 = i + 1 < length ? bytes[i + 1] : (byte)0;
                var b3 = i + 2 < length ? bytes[i + 2] : (byte)0;
                s.Append(Append3Bytes(b1, b2, b3));
            }
            return s.ToString();
        }

        private static char[] Append3Bytes(byte b1, byte b2, byte b3)
        {
            var c1 = b1 >> 2;
            var c2 = (b1 & 0x3) << 4 | b2 >> 4;
            var c3 = (b2 & 0xF) << 2 | b3 >> 6;
            var c4 = b3 & 0x3F;
            return new[]
            {
            EncodeByte((byte) (c1 & 0x3F)),
            EncodeByte((byte) (c2 & 0x3F)),
            EncodeByte((byte) (c3 & 0x3F)),
            EncodeByte((byte) (c4 & 0x3F))
         };
        }

        private static char EncodeByte(byte b)
        {
            var ascii = Encoding.ASCII;
            if (b < 10)
                return ascii.GetChars(new[] { (byte)(48 + b) })[0];
            b -= 10;
            if (b < 26)
                return ascii.GetChars(new[] { (byte)(65 + b) })[0];
            b -= 26;
            if (b < 26)
                return ascii.GetChars(new[] { (byte)(97 + b) })[0];
            b -= 26;
            if (b == 0)
                return '-';
            if (b == 1)
                return '_';
            return '?';
        }

    }
}
