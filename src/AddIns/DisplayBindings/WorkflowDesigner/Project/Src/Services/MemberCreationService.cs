// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Russell Wilkins" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Workflow.ComponentModel.Design;
using ICSharpCode.Core;

namespace WorkflowDesigner
{
	/// <summary>
	/// Description of MemberCreationService.
	/// </summary>
	public class MemberCreationService : IMemberCreationService
	{
		public MemberCreationService()
		{
		}
		
		public void CreateField(string className, string fieldName, Type fieldType, Type[] genericParameterTypes, System.CodeDom.MemberAttributes attributes, System.CodeDom.CodeSnippetExpression initializationExpression, bool overwriteExisting)
		{
			throw new NotImplementedException();
		}
		
		public void CreateProperty(string className, string propertyName, Type propertyType, System.Workflow.ComponentModel.Compiler.AttributeInfo[] attributes, bool emitDependencyProperty, bool isMetaProperty, bool isAttached, Type ownerType, bool isReadOnly)
		{
			throw new NotImplementedException();
		}
		
		public void CreateEvent(string className, string eventName, Type eventType, System.Workflow.ComponentModel.Compiler.AttributeInfo[] attributes, bool emitDependencyProperty)
		{
			throw new NotImplementedException();
		}
		
		public void UpdateTypeName(string oldClassName, string newClassName)
		{
			LoggingService.DebugFormatted("UpdateTypeName(oldClassName={0}, newClassName={1})", oldClassName, newClassName);
			//throw new NotImplementedException();
		}
		
		public void UpdateBaseType(string className, Type baseType)
		{
			throw new NotImplementedException();
		}
		
		public void UpdateProperty(string className, string oldPropertyName, Type oldPropertyType, string newPropertyName, Type newPropertyType, System.Workflow.ComponentModel.Compiler.AttributeInfo[] attributes, bool emitDependencyProperty, bool isMetaProperty)
		{
			throw new NotImplementedException();
		}
		
		public void UpdateEvent(string className, string oldEventName, Type oldEventType, string newEventName, Type newEventType, System.Workflow.ComponentModel.Compiler.AttributeInfo[] attributes, bool emitDependencyProperty, bool isMetaProperty)
		{
			throw new NotImplementedException();
		}
		
		public void RemoveProperty(string className, string propertyName, Type propertyType)
		{
			throw new NotImplementedException();
		}
		
		public void RemoveEvent(string className, string eventName, Type eventType)
		{
			throw new NotImplementedException();
		}
		
		public void ShowCode(System.Workflow.ComponentModel.Activity activity, string methodName, Type delegateType)
		{
			throw new NotImplementedException();
		}
		
		public void ShowCode()
		{
			throw new NotImplementedException();
		}
		
	}
}
