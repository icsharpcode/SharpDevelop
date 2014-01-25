// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
