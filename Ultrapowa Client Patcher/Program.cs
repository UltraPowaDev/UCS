using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace UCSClientPatcher
{
	internal class Patcher
	{
		internal static byte[] HexToByteArray(string hex)
		{
			return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
		}

		internal static string AssemblyVersion
		{
			get
			{
				return "v" + Assembly.GetExecutingAssembly().GetName().Version.ToString();
			}
		}

		internal static void Main(string[] args)
		{
			string fileName = "libg.so";//Place libg in the folder of this program exe

            byte[] fileBytes = File.ReadAllBytes(fileName);

			/*
			("9bc23206948f104820e347ed47fa92256ca843b72aec503a0982889cd6a7eb38");// CR 1.4.0
			("bbdba8653396d1df84efaea923ecd150d15eb526a46a6c39b53dac974fff3829");// CR 1.5.0
			("0f9fff6d583023c5c739c053581c994dbe37789900ffda312fc97edfd091945f");// CR 1.7.0
			("e330c7916ae0a66f3a90eae97a863ee00ac1dcad058877b1eecfc8fe91c93532");// CR 1.6.0
			*/
			byte[] searchPattern = HexToByteArray("bbdba8653396d1df84efaea923ecd150d15eb526a46a6c39b53dac974fff3829");
            byte[] replacePattern = HexToByteArray("72f1a4a4c48e44da0c42310f800e96624e6dc6a641a9d41c3b5039d8dfadc27e");//CR Patched

            Console.Title = "Ultrapowa Client Patcher " + AssemblyVersion + " - © 2016";
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine(
                @"
      ____ ___.__   __                                              
     |    |   \  |_/  |_____________  ______   ______  _  _______   
     |    |   /  |\   __\_  __ \__  \ \____ \ /  _ \ \/ \/ /\__  \  
     |    |  /|  |_|  |  |  | \// __ \|  |_> >  <_> )     /  / __ \_
     |______/ |____/__|  |__|  (____  /   __/ \____/ \/\_/  (____  /
                                    \/|__|                       \/
                  ");
			Console.ResetColor();
			Console.WriteLine("[UCS]    -> This program is by the Ultrapowa Network development team.");
			Console.WriteLine("[UCS]    -> You can find the source at www.ultrapowa.com and https://github.com/UltraPowaDev/UCS/");
			Console.WriteLine("[UCS]    -> This program can be used with args - Ultrapowa Client Patcher.exe {Binary Name}");

			//Search
            IEnumerable<int> positions = FindPattern(fileBytes, searchPattern);
			if (positions.Count() == 0)
			{
				Console.WriteLine("[UCS] Pattern not found.");
				Console.Read();
				return;
			}

			//Backup
            string backupFileName = fileName + ".bak";
			File.Copy(fileName, backupFileName);
			Console.WriteLine("[UCS] Backup file: {0} -> {1}", fileName, backupFileName);

			foreach (int pos in positions)
			{
				//Replace
                Console.WriteLine("[UCS] Key offset: 0x{0}", pos.ToString("X8"));
				using (BinaryWriter bw = new BinaryWriter(File.Open(fileName, FileMode.Open, FileAccess.Write)))
				{
					bw.BaseStream.Seek(pos, SeekOrigin.Begin);
					bw.Write(replacePattern);
				}

				Console.WriteLine("[UCS] File: {0} patched", fileName);
			}

			Console.Read();
		}

		public static IEnumerable<int> FindPattern(byte[] fileBytes, byte[] searchPattern)
		{
			if ((searchPattern != null) && (fileBytes.Length >= searchPattern.Length))
				for (int i = 0; i < fileBytes.Length - searchPattern.Length + 1; i++)
					if (!searchPattern.Where((data, index) => !fileBytes[i + index].Equals(data)).Any())
						yield return i;
		}
	}
}
