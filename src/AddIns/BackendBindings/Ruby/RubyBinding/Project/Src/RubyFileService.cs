// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Text;

namespace ICSharpCode.RubyBinding
{
	public class RubyFileService : IRubyFileService
	{
		public string GetTempFileName()
		{
			return Path.GetTempFileName();
		}
		
		public TextWriter CreateTextWriter(CreateTextWriterInfo createTextWriterInfo)
		{
			return createTextWriterInfo.CreateTextWriter();
		}
		
		public void DeleteFile(string fileName)
		{
			File.Delete(fileName);
		}
	}
}
