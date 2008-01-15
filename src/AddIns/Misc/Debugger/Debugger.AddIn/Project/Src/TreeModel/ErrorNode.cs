// <file>
//     <copyright license="BSD-new" see="prj:///COPYING"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;

using Debugger;
using Debugger.Expressions;

namespace Debugger.AddIn.TreeModel
{
	public class ErrorNode: AbstractNode
	{
		Expression targetObject;
		GetValueException error;
		
		public Expression TargetObject {
			get { return targetObject; }
		}
		
		public GetValueException Error {
			get { return error; }
		}
		
		public ErrorNode(Expression targetObject, GetValueException error)
		{
			this.targetObject = targetObject;
			this.error = error;
			
			this.Name = targetObject.CodeTail;
			this.Text = error.Error;
		}
	}
}
