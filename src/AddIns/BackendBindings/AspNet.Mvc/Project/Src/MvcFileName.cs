// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;

namespace ICSharpCode.AspNet.Mvc
{
	public class MvcFileName
	{
		string folder = String.Empty;
		
		public string Folder {
			get {
				EnsureFolderIsNotNull();
				return folder;
			}
			set { folder = value; }
		}
		
		void EnsureFolderIsNotNull()
		{
			folder = ConvertToEmptyStringIfNull(folder);
		}

		public override string ToString()
		{
			return GetPath();
		}
		
		public string GetPath()
		{
			string fileName = GetFileName();
			return Path.Combine(Folder, fileName);
		}
		
		public virtual string GetFileName()
		{
			return String.Empty;
		}

		protected static string ConvertToEmptyStringIfNull(string text)
		{
			if (text != null) {
				return text;
			}
			return String.Empty;
		}
		
		public static string GetLowerCaseFileExtension(string fileName)
		{
			if (fileName != null) {
				return Path.GetExtension(fileName).ToLowerInvariant();
			}
			return String.Empty;
		}
	}
}
