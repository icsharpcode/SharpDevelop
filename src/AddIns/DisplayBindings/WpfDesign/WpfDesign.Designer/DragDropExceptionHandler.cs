// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision: 2573 $</version>
// </file>

using System;

namespace ICSharpCode.WpfDesign.Designer
{
	/// <summary>
	/// When the designer is hosted in a Windows.Forms application, exceptions in
	/// drag'n'drop handlers are silently ignored.
	/// Applications hosting the designer should specify a delegate to their own exception handling
	/// method. The default is Environment.FailFast.
	/// </summary>
	public static class DragDropExceptionHandler
	{
		public static Action<Exception> HandleException = delegate(Exception ex) {
			Environment.FailFast(ex.ToString());
		};
	}
}
