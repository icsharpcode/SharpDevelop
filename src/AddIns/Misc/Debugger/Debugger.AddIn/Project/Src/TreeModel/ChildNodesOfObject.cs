// <file>
//     <copyright license="BSD-new" see="prj:///COPYING"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;

using ICSharpCode.Core;

using Debugger;
using Debugger.Expressions;

namespace Debugger.AddIn.TreeModel
{
	public partial class Util
	{
		public static IEnumerable<AbstractNode> GetChildNodesOfObject(Expression targetObject, DebugType shownType)
		{
			if (shownType.BaseType != null) {
				yield return new BaseClassNode(targetObject, shownType.BaseType);
			}
			if (shownType.HasMembers(BindingFlags.NonPublicInstance)) {
				yield return new NonPublicInstanceMembersNode(targetObject, shownType);
			}
			if (shownType.HasMembers(BindingFlags.Static)) {
				yield return new StaticMembersNode(targetObject, shownType);
			}
			foreach(Expression childExpr in targetObject.AppendObjectMembers(shownType, BindingFlags.PublicInstance)) {
				yield return Util.CreateNode(childExpr);
			}
		}
	}
	
	public class BaseClassNode: AbstractNode
	{
		Expression targetObject;
		DebugType shownType;
		
		public BaseClassNode(Expression targetObject, DebugType shownType)
		{
			this.targetObject = targetObject;
			this.shownType = shownType;
			
			this.Image = DebuggerIcons.ImageList.Images[0]; // Class
			this.Name = StringParser.Parse("${res:MainWindow.Windows.Debug.LocalVariables.BaseClass}");
			this.Type = shownType.FullName;
			this.ChildNodes = Util.GetChildNodesOfObject(targetObject, shownType);
		}
	}
	
	public class NonPublicInstanceMembersNode: AbstractNode
	{
		Expression targetObject;
		DebugType shownType;
		
		public NonPublicInstanceMembersNode(Expression targetObject, DebugType shownType)
		{
			this.targetObject = targetObject;
			this.shownType = shownType;
			
			this.Name = StringParser.Parse("${res:MainWindow.Windows.Debug.LocalVariables.PrivateMembers}");
			this.ChildNodes = GetChildNodes();
		}
		
		IEnumerable<AbstractNode> GetChildNodes()
		{
			foreach(Expression childExpr in targetObject.AppendObjectMembers(shownType, BindingFlags.NonPublicInstance)) {
				yield return Util.CreateNode(childExpr);
			}
		}
	}
	
	public class StaticMembersNode: AbstractNode
	{
		Expression targetObject;
		DebugType shownType;
		
		public StaticMembersNode(Expression targetObject, DebugType shownType)
		{
			this.targetObject = targetObject;
			this.shownType = shownType;
			
			this.Name = StringParser.Parse("${res:MainWindow.Windows.Debug.LocalVariables.StaticMembers}");
			this.ChildNodes = GetChildNodes();
		}
		
		IEnumerable<AbstractNode> GetChildNodes()
		{
			if (shownType.HasMembers(BindingFlags.NonPublicStatic)) {
				yield return new NonPublicStaticMembersNode(targetObject, shownType);
			}
			foreach(Expression childExpr in targetObject.AppendObjectMembers(shownType, BindingFlags.PublicStatic)) {
				yield return Util.CreateNode(childExpr);
			}
		}
	}
	
	public class NonPublicStaticMembersNode: AbstractNode
	{
		Expression targetObject;
		DebugType shownType;
		
		public NonPublicStaticMembersNode(Expression targetObject, DebugType shownType)
		{
			this.targetObject = targetObject;
			this.shownType = shownType;
			
			this.Name = StringParser.Parse("${res:MainWindow.Windows.Debug.LocalVariables.PrivateStaticMembers}");
			this.ChildNodes = GetChildNodes();
		}
		
		IEnumerable<AbstractNode> GetChildNodes()
		{
			foreach(Expression childExpr in targetObject.AppendObjectMembers(shownType, BindingFlags.NonPublicStatic)) {
				yield return Util.CreateNode(childExpr);
			}
		}
	}
}
