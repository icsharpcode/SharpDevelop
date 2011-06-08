// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Management.Automation.Host;

namespace PackageManagement.Cmdlets.Tests.Helpers
{
	public class FakeCommandRuntime : ICommandRuntime
	{
		public List<object> ObjectsPassedToWriteObject = new List<object>();
		public object FirstObjectPassedToWriteObject {
			get { return ObjectsPassedToWriteObject[0]; }
		}
		
		public PSHost Host {
			get {
				throw new NotImplementedException();
			}
		}
		
		public PSTransactionContext CurrentPSTransaction {
			get {
				throw new NotImplementedException();
			}
		}
		
		public void WriteDebug(string text)
		{
			throw new NotImplementedException();
		}
		
		public void WriteError(ErrorRecord errorRecord)
		{
			throw new NotImplementedException();
		}
		
		public void WriteObject(object sendToPipeline)
		{
			ObjectsPassedToWriteObject.Add(sendToPipeline);
		}
		
		public bool EnumerateCollectionPassedToWriteObject;
		
		public void WriteObject(object sendToPipeline, bool enumerateCollection)
		{
			EnumerateCollectionPassedToWriteObject = enumerateCollection;
			ObjectsPassedToWriteObject.Add(sendToPipeline);
		}
		
		public void WriteProgress(ProgressRecord progressRecord)
		{
			throw new NotImplementedException();
		}
		
		public void WriteProgress(long sourceId, ProgressRecord progressRecord)
		{
			throw new NotImplementedException();
		}
		
		public void WriteVerbose(string text)
		{
			throw new NotImplementedException();
		}
		
		public void WriteWarning(string text)
		{
			throw new NotImplementedException();
		}
		
		public void WriteCommandDetail(string text)
		{
			throw new NotImplementedException();
		}
		
		public bool ShouldProcess(string target)
		{
			throw new NotImplementedException();
		}
		
		public bool ShouldProcess(string target, string action)
		{
			throw new NotImplementedException();
		}
		
		public bool ShouldProcess(string verboseDescription, string verboseWarning, string caption)
		{
			throw new NotImplementedException();
		}
		
		public bool ShouldProcess(string verboseDescription, string verboseWarning, string caption, out ShouldProcessReason shouldProcessReason)
		{
			throw new NotImplementedException();
		}
		
		public bool ShouldContinue(string query, string caption)
		{
			throw new NotImplementedException();
		}
		
		public bool ShouldContinue(string query, string caption, ref bool yesToAll, ref bool noToAll)
		{
			throw new NotImplementedException();
		}
		
		public bool TransactionAvailable()
		{
			throw new NotImplementedException();
		}
		
		public void ThrowTerminatingError(ErrorRecord errorRecord)
		{
		}
	}
}
