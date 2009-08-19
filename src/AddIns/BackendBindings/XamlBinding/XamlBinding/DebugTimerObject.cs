// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.NRefactory;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.XmlEditor;

#pragma warning disable 169

namespace ICSharpCode.XamlBinding
{
	class DebugTimerObject : IDisposable
	{
		Stopwatch watch;
		string text;
		
		public DebugTimerObject(string text)
		{
			#if DEBUG
			this.watch = new Stopwatch();
			this.text = text;
			this.watch.Start();
			#endif
		}
		
		public void Dispose()
		{
			#if DEBUG
			this.watch.Stop();
			
			Core.LoggingService.Info(text + " took " + watch.ElapsedMilliseconds + "ms");
			#endif
		}
	}
}

#pragma warning restore 169