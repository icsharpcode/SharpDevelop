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
using System.Runtime.Serialization;
using System.Security.Permissions;

/// <summary>
/// This Class throws the Standart SharpReport Error
/// </summary>
/// <remarks>
/// 	created by - Forstmeier Peter
/// 	created on - 15.01.2005 09:39:32
/// </remarks>
/// 
namespace ICSharpCode.Reports.Core {
	[Serializable()]
	public class ReportException : Exception {
		
		
		string errorMessage = String.Empty;
		public ReportException():base(){
			
		}
		public ReportException(string errorMessage) :base (errorMessage){
			this.errorMessage = errorMessage;
		}
		public ReportException(string errorMessage,
		                            Exception exception):base (errorMessage,exception){
			
		}
		
		protected ReportException(SerializationInfo info,
		                               StreamingContext context) : base(info, context){
			// Implement type-specific serialization constructor logic.
		}
		
		public string ErrorMessage {
			get {
				return errorMessage;
			}
		}
		
		[SecurityPermissionAttribute(SecurityAction.Demand,
		                             SerializationFormatter = true)]

		public override void GetObjectData(SerializationInfo info, StreamingContext context){
			if (info == null) {
				throw new ArgumentNullException("info");
			}
			info.AddValue("errorMessage", this.errorMessage);
			base.GetObjectData(info, context);
		}
		
	}
}
