// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;

namespace ICSharpCode.SharpDevelop.BuildWorker
{
	static class BinaryReaderExtensions
	{
		public static string ReadNullableString(this BinaryReader reader)
		{
			if (reader.ReadBoolean())
				return reader.ReadString();
			else
				return null;
		}
		
		public static void WriteNullableString(this BinaryWriter writer, string s)
		{
			writer.Write(s != null);
			if (s != null)
				writer.Write(s);
		}
		
		public static DateTime ReadDateTime(this BinaryReader reader)
		{
			return new DateTime(reader.ReadInt64());
		}
		
		public static void WriteDateTime(this BinaryWriter writer, DateTime date)
		{
			writer.Write(date.Ticks);
		}
		
		public static void WriteInt32(this BinaryWriter writer, int val)
		{
			writer.Write(val);
		}
	}
}
