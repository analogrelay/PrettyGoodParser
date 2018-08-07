using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;

namespace pgparser
{
    [Command(Name = Name, Description = "Parses PGP files and displays information from them.")]
    internal class ParseCommand
    {
        public const string Name = "parse";

        [Argument(0, Description = "The file to parse")]
        public string FilePath { get; set; }

        public async Task<int> OnExecuteAsync(CommandLineApplication app, IConsole console)
        {
            // TODO: Consider non-ASCII files
            var lines = new List<string>();
            using (var reader = File.OpenText(FilePath))
            {
                var line = await reader.ReadLineAsync();
                if (!line.StartsWith("-----"))
                {
                    console.Error.WriteLine("The file is not an ASCII-armored PGP file");
                    return 1;
                }

                // Skip blank lines, but stop on null because it means end-of-stream
                while ((line = await reader.ReadLineAsync()) != null && string.IsNullOrWhiteSpace(line))
                {
                }

                // Read non-blank lines and build up the Base64 string
                lines.Add(line.Trim());
                while (!string.IsNullOrWhiteSpace(line = await reader.ReadLineAsync()))
                {
                    if (line.StartsWith("----"))
                    {
                        // End of the file
                        break;
                    }
                    lines.Add(line.Trim());
                }

                // We're done with the file now
            }

            // The last line is a checksum, so take all the lines but the last one
            var content = string.Join(Environment.NewLine, lines.Take(lines.Count - 1));

            // Convert the content to binary
            ReadOnlyMemory<byte> bytes = Convert.FromBase64String(content.ToString());

            // Load the content
            if (OpenPgpParser.TryParse(bytes.Span, out var packet))
            {
                switch(packet)
                {
                    case SignaturePacket signature:
                        console.WriteLine($"{signature.SignatureType} v{signature.Version} Signature Packet (Length: {signature.Length})");
                        console.WriteLine($"Public Key Algorithm: 0x{signature.PublicKeyAlgorithm:X2}");
                        console.WriteLine($"Hash Algorithm: 0x{signature.HashAlgorithm:X2}");
                        console.WriteLine($"Hashed Data Length: {signature.HashedData.Length}");
                        console.WriteLine($"Unhashed Data Length: {signature.UnhashedData.Length}");
                        break;
                    default:
                        console.WriteLine($"{packet.Tag} Packet (Length: {packet.Length})");
                        break;
                }
            }

            return 0;
        }
    }
}
