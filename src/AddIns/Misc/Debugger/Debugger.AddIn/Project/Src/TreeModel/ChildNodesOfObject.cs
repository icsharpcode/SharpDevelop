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

namespace Debugger.AddIn.TreeModel
{
	public partial class Util
	{
		public static IEnumerable<AbstractNode> GetChildNodesOfObject(Expression expression, DebugType shownType)
		{
			if (shownType.BaseType != null) {
				yield return new BaseClassNode(expression, shownType.BaseType);
			}
			if (shownType.HasMembers(BindingFlags.NonPublicInstance)) {
				yield return new NonPublicInstanceMembersNode(expression, shownType);
			}
			if (shownType.HasMembers(BindingFlags.Static)) {
				yield return new StaticMembersNode(expression, shownType);
			}
			foreach(Expression childExpr in expression.AppendObjectMembers(shownType, BindingFlags.PublicInstance)) {
				yield return new ExpressionNode(childExpr);
			}
		}
	}
	
	public class BaseClassNode: AbstractNode
	{
		Expression expression;
		DebugType shownType;
		
		public BaseClassNode(Expression expression, DebugType shownType)
		{
			this.expression = expression;
			this.shownType = shownType;
			
			this.Image = DebuggerIcons.ImageList.Images[0]; // Class
			this.Name = StringParser.Parse("${res:MainWindow.Windows.Debug.LocalVariables.BaseClass}");
			this.Type = shownType.FullName;
			this.ChildNodes = Util.GetChildNodesOfObject(expression, shownType);
		}
	}
	
	public class NonPublicInstanceMembersNode: AbstractNode
	{
		Expression expression;
		DebugType shownType;
		
		public NonPublicInstanceMembersNode(Expression expression, DebugType shownType)
		{
			this.expression = expression;
			this.shownType = shownType;
			
			this.Name = StringParser.Parse("${res:MainWindow.Windows.Debug.LocalVariables.PrivateMembers}");
			this.ChildNodes = GetChildNodes();
		}
		
		IEnumerable<AbstractNode> GetChildNodes()
		{
			foreach(Expression childExpr in expression.AppendObjectMembers(shownType, BindingFlags.NonPublicInstance)) {
				yield return new ExpressionNode(childExpr);
			}
		}
	}
	
	public class StaticMembersNode: AbstractNode
	{
		Expression expression;
		DebugType shownType;
		
		public StaticMembersNode(Expression expression, DebugType shownType)
		{
			this.expression = expression;
			this.shownType = shownType;
			
			this.Name = StringParser.Parse("${res:MainWindow.Windows.Debug.LocalVariables.StaticMembers}");
			this.ChildNodes = GetChildNodes();
		}
		
		IEnumerable<AbstractNode> GetChildNodes()
		{
			if (shownType.HasMembers(BindingFlags.NonPublicStatic)) {
				yield return new NonPublicStaticMembersNode(expression, shownType);
			}
			foreach(Expression childExpr in expression.AppendObjectMembers(shownType, BindingFlags.PublicStatic)) {
				yield return new ExpressionNode(childExpr);
			}
		}
	}
	
	public class NonPublicStaticMembersNode: AbstractNode
	{
		Expression expression;
		DebugType shownType;
		
		public NonPublicStaticMembersNode(Expression expression, DebugType shownType)
		{
			this.expression = expression;
			this.shownType = shownType;
			
			this.Name = StringParser.Parse("${res:MainWindow.Windows.Debug.LocalVariables.PrivateStaticMembers}");
			this.ChildNodes = GetChildNodes();
		}
		
		IEnumerable<AbstractNode> GetChildNodes()
		{
			foreach(Expression childExpr in expression.AppendObjectMembers(shownType, BindingFlags.NonPublicStatic)) {
				yield return new ExpressionNode(childExpr);
			}
		}
	}
}
