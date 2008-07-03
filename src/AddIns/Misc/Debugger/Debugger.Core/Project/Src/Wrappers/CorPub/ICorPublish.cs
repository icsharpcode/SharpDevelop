// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

#pragma warning disable 1591

namespace Debugger.Core.Wrappers.CorPub
{
	using System;
	using System.Runtime.InteropServices;
	using Debugger.Wrappers;
	
	public partial class ICorPublish
	{
		private Debugger.Interop.CorPub.CorpubPublishClass corpubPublishClass;

		public ICorPublish()
		{
			corpubPublishClass = new Debugger.Interop.CorPub.CorpubPublishClass();
		}
		
		public ICorPublishProcess GetProcess(int id) 
		{
			Debugger.Interop.CorPub.ICorPublishProcess process;
			this.corpubPublishClass.GetProcess((uint)id, out  process);
			return ICorPublishProcess.Wrap(process);
		}
	}
}

#pragma warning restore 1591
