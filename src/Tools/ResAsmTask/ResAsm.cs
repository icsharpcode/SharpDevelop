// ResAsm.cs
// Copyright (c) 2001 Mike Krueger
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

using System;
using System.Collections;
using System.Drawing;
using System.Resources;
using System.IO;
using System.Text;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Runtime.Serialization.Formatters.Binary;

namespace ResAsmTask
{
	public class ResAsm
	{
		static string ConvertIllegalChars(string str)
		{
			StringBuilder newString = new StringBuilder();
			for (int i = 0; i < str.Length; ++i) {
				switch (str[i]) {
					case '\r':
						break;
					case '\n':
						newString.Append("\\n");
						break;
					case '"':
						newString.Append("\\\"");
						break;
					case '\\':
						newString.Append("\\\\");
						break;
					default:
						newString.Append(str[i]);
						break;
				}
			}
			return newString.ToString();
		}
		
		/// <remarks>
		/// Builds resource files out of the ResAsm format
		/// </remarks>
		public static void Assemble(string inputFileName, string outputFileName)
		{
			int linenr = 0;
			try {
				StreamReader reader      = new StreamReader(inputFileName, new UTF8Encoding());
				
				ResourceWriter rw = new ResourceWriter(outputFileName);
				linenr = 0;
				while (true) {
					string line = reader.ReadLine();
					linenr++;
					if (line == null) {
						break;
					}
					line = line.Trim();
					// skip empty or comment lines
					if (line.Length == 0 || line[0] == '#') {
						continue;
					}
					
					// search for a = char
					int idx = line.IndexOf('=');
					if (idx < 0) {
						Console.WriteLine("error in file " + inputFileName + " at line " + linenr);
						continue;
					}
					string key = line.Substring(0, idx).Trim();
					string val = line.Substring(idx + 1).Trim();
					object entryval = null;
					
					if (val[0] == '"') { // case 1 : string value 
						val = val.Trim(new char[] {'"'});
						StringBuilder tmp = new StringBuilder();
						for (int i = 0; i < val.Length; ++i) {
							switch (val[i]) { // handle the \ char 
								case '\\':
										++i;
										if (i < val.Length)
										switch (val[i]) {
											case '\\':
												tmp.Append('\\');
												break;
											case 'n':
												tmp.Append('\n');
												break;
											case '\"':
												tmp.Append('\"');
												break;
										}
										break;
								default:
									tmp.Append(val[i]);
									break;
							}
						}
						entryval = tmp.ToString();
					} else { // case 2 : no string value -> load resource
						entryval = LoadResource(val);
					}
					rw.AddResource(key, entryval);
					
				}
				rw.Generate();
				rw.Close();
				reader.Close();
			} catch (Exception e) {
				Console.WriteLine("Error in line " + linenr);
				Console.WriteLine("Error while processing " + inputFileName + " :");
				Console.WriteLine(e.ToString());
			}
		}
		
		/// <remarks>
		/// Loads a file. 
		/// </remarks>
		/// <returns>
		/// An object representation of the file (for a bitmap a Bitmap,
		/// for a Icon an Icon and so on), the fall back is a byte array
		/// </returns>
		static object LoadResource(string name)
		{
			switch (Path.GetExtension(name).ToUpper()) {
				case ".CUR":
					return new Cursor(name);
				case ".ICO":
					return new Icon(name);
				default:
					// try to read a bitmap
					try { 
						return new Bitmap(name); 
					} catch {}
					
					// try to read a serialized object
					try {
						Stream r = File.Open(name, FileMode.Open);
						try {
							BinaryFormatter c = new BinaryFormatter();
							object o = c.Deserialize(r);
							r.Close();
							return o;
						} catch { r.Close(); }
					} catch { }
					
					// finally try to read a byte array
					try {
						FileStream s = new FileStream(name, FileMode.Open);
						BinaryReader r = new BinaryReader(s);
						Byte[] d = new Byte[(int) s.Length];
						d = r.ReadBytes((int) s.Length);
						s.Close();
						return d;
					} catch (Exception e) { 
						MessageBox.Show(e.Message, "Can't load resource", MessageBoxButtons.OK); 
					}
				break;
			}
			return null;
		}
	}
}
