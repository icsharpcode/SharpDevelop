// <file>
//     <copyright license="BSD-new" see="prj:///COPYING"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision: 2813 $</version>
// </file>

using System;


namespace Debugger.AddIn.TreeModel
{
	/// <summary>
	/// Represents a row in a TreeViewAdv that contains an IDebugeeException
	/// </summary>
	public class ExceptionNode : AbstractNode
	{
		private IDebugeeException exception;
		private ExceptionNode innerException = null;
		
		public static ExceptionNode Create(Debugger.Exception exception) {
			try {
				return new ExceptionNode(exception);
			} catch (GetValueException e) {
				// TODO: Fix this
				throw;
				//return new ErrorNode(expression, e);
			}
		}
		
		private ExceptionNode(IDebugeeException exception){
			this.exception = exception;
			this.Image = DebuggerIcons.ImageList.Images[0]; // Class
			this.Name = "Unhandled Exception";
			this.Type = exception.Type;
			this.Text = exception.Message;
			if (exception.InnerException != null) {
				innerException = new ExceptionNode(exception.InnerException);
			}
		}
		
		
		ExceptionNode InnerException {
			get { return innerException; }
		}
	}
}
