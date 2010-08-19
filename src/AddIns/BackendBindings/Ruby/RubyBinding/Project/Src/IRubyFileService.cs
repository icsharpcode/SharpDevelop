// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Text;
using ICSharpCode.UnitTesting;

namespace ICSharpCode.RubyBinding
{
	public interface IRubyFileService : IFileSystem
	{
		string GetTempFileName();
		TextWriter CreateTextWriter(CreateTextWriterInfo createTextWriterInfo);
		void DeleteFile(string fileName);
	}
}
