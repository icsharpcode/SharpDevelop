// <file>
//	 <copyright see="prj:///doc/copyright.txt"/>
//	 <license see="prj:///doc/license.txt"/>
//	 <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//	 <version>$Revision$</version>
// </file>

#pragma warning disable 108, 1591 

namespace Debugger.Interop.CorPub
{
	using System;
	using System.Runtime.CompilerServices;
	using System.Runtime.InteropServices;
	
	[ComImport, CoClass(typeof(CorpubPublishClass)), Guid("9613A0E7-5A68-11D3-8F84-00A0C9B4D50C")]
	public interface CorpubPublish : ICorPublish
	{
	}
}

#pragma warning restore 108, 1591
 
