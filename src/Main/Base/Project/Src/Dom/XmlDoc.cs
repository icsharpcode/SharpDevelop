// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Collections.Generic;
using System.Xml;

namespace ICSharpCode.SharpDevelop.Dom
{
	/// <summary>
	/// Description of XmlDoc.
	/// </summary>
	public class XmlDoc : IDisposable
	{
		Dictionary<string, string> xmlDescription = new Dictionary<string, string>();
		Dictionary<string, int>    indexDictionary;
		
		void ReadMembersSection(XmlTextReader reader)
		{
			while (reader.Read()) {
				switch (reader.NodeType) {
					case XmlNodeType.EndElement:
						if (reader.LocalName == "members") {
							return;
						}
						break;
					case XmlNodeType.Element:
						if (reader.LocalName == "member") {
							string memberAttr = reader.GetAttribute(0);
							string innerXml   = reader.ReadInnerXml();
							xmlDescription[memberAttr] = innerXml;
						}
						break;
				}
			}
		}
		
		public string GetDocumentation(string key)
		{
			lock (xmlDescription) {
				if (indexDictionary != null) {
					if (!indexDictionary.ContainsKey(key))
						return null;
				}
				if (xmlDescription.ContainsKey(key))
					return xmlDescription[key];
				if (indexDictionary == null)
					return null;
				return LoadDocumentation(key);
			}
		}
		
		#region Save binary files
		// FILE FORMAT FOR BINARY DOCUMENTATION
		// long  magic = 0x4244636f446c6d58 (identifies file type = 'XmlDocDB')
		// short version = 1              (file version)
		// long  fileDate                 (last change date of xml file in DateTime ticks)
		// int   indexPointer             (points to location where index starts in the file)
		// { string docu }                (all documentation strings as length-prefixed strings)
		// indexPointer points to the start of the following section:
		// {
		//   string key             (documentation key as length-prefixed string)
		//   int    index           (index where the docu string starts in the file)
		// }
		
		const long magic = 0x4244636f446c6d58;
		const short version = 1;
		
		void Save(string fileName, DateTime fileDate)
		{
			using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None)) {
				using (BinaryWriter w = new BinaryWriter(fs)) {
					w.Write(magic);
					w.Write(version);
					w.Write(fileDate.Ticks);
					
					int indexPointerPos = (int)fs.Position;
					w.Write(0); // skip 4 bytes
					int[] indices = new int[xmlDescription.Count];
					
					int i = 0;
					foreach (KeyValuePair<string, string> p in xmlDescription) {
						indices[i++] = (int)fs.Position;
						w.Write(p.Value.Trim());
					}
					
					int indexStart = (int)fs.Position;
					i = 0;
					foreach (KeyValuePair<string, string> p in xmlDescription) {
						w.Write(p.Key);
						w.Write(indices[i++]);
					}
					w.Seek(indexPointerPos, SeekOrigin.Begin);
					w.Write(indexStart);
				}
			}
		}
		#endregion
		
		#region Load binary files
		BinaryReader loader;
		FileStream fs;
		Queue<string> keyCacheQueue;
		const int cacheLength = 150; // number of strings to cache when working in file-mode
		
		bool LoadFromBinary(string fileName, DateTime fileDate)
		{
			indexDictionary = new Dictionary<string, int>();
			keyCacheQueue   = new Queue<string>(cacheLength);
			fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
			int len = (int)fs.Length;
			loader = new BinaryReader(fs);
			if (loader.ReadInt64() != magic) {
				Console.WriteLine("Wrong magic");
				return false;
			}
			if (loader.ReadInt16() != version) {
				Console.WriteLine("Wrong version");
				return false;
			}
			if (loader.ReadInt64() != fileDate.Ticks) {
				Console.WriteLine("Wrong date");
				return false;
			}
			fs.Position = loader.ReadInt32(); // go to start of index
			while (fs.Position < len) {
				string key = loader.ReadString();
				int pos = loader.ReadInt32();
				indexDictionary.Add(key, pos);
			}
			return true;
		}
		
		string LoadDocumentation(string key)
		{
			if (keyCacheQueue.Count > cacheLength - 1) {
				xmlDescription.Remove(keyCacheQueue.Dequeue());
			}
			int pos = indexDictionary[key];
			fs.Position = pos;
			string docu = loader.ReadString();
			xmlDescription.Add(key, docu);
			keyCacheQueue.Enqueue(docu);
			return docu;
		}
		
		public void Dispose()
		{
			if (loader != null) {
				loader.Close();
				fs.Close();
			}
			xmlDescription = null;
			indexDictionary = null;
			keyCacheQueue = null;
			loader = null;
			fs = null;
		}
		#endregion
		
		public static XmlDoc Load(TextReader textReader)
		{
			XmlDoc newXmlDoc = new XmlDoc();
			using (XmlTextReader reader = new XmlTextReader(textReader)) {
				while (reader.Read()) {
					if (reader.IsStartElement()) {
						switch (reader.LocalName) {
							case "members":
								newXmlDoc.ReadMembersSection(reader);
								break;
						}
					}
				}
			}
			return newXmlDoc;
		}
		
		static string MakeTempPath()
		{
			string tempPath = Path.Combine(Path.GetTempPath(), "SharpDevelop/DocumentationCache");
			if (!Directory.Exists(tempPath))
				Directory.CreateDirectory(tempPath);
			return tempPath;
		}
		
		public static XmlDoc Load(string fileName)
		{
			string cacheName = MakeTempPath() + "/" + Path.GetFileNameWithoutExtension(fileName)
				+ "." + fileName.GetHashCode().ToString("x") + ".dat";
			XmlDoc doc;
			if (File.Exists(cacheName)) {
				doc = new XmlDoc();
				if (doc.LoadFromBinary(cacheName, File.GetLastWriteTimeUtc(fileName))) {
					return doc;
				} else {
					doc.Dispose();
					try {
						File.Delete(cacheName);
					} catch {}
				}
			}
			
			using (TextReader textReader = File.OpenText(fileName)) {
				doc = Load(textReader);
			}
			
			if (doc.xmlDescription.Count > cacheLength * 2) {
				DateTime date = File.GetLastWriteTimeUtc(fileName);
				doc.Save(cacheName, date);
				doc.Dispose();
				doc = new XmlDoc();
				doc.LoadFromBinary(cacheName, date);
			}
			return doc;
		}
	}
}
