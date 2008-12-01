// <file>
//     <copyright license="BSD-new" see="prj:///COPYING"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System.Collections;
using System.Collections.Generic;
using Debugger.Expressions;
using Debugger.MetaData;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;

namespace Debugger.AddIn.TreeModel
{
	public partial class Utils
	{
		public static IEnumerable<AbstractNode> GetChildNodesOfObject(Expression targetObject, DebugType shownType)
		{
			BindingFlags Flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Field | BindingFlags.GetProperty;
			if (shownType.BaseType != null) {
				yield return new BaseClassNode(targetObject, shownType.BaseType);
			}
			if (shownType.HasMembers(NonPublicInstanceMembersNode.Flags)) {
				yield return new NonPublicInstanceMembersNode(targetObject, shownType);
			}
			if (shownType.HasMembers(StaticMembersNode.Flags) ||
			    shownType.HasMembers(NonPublicStaticMembersNode.Flags)) 
			{
				yield return new StaticMembersNode(targetObject, shownType);
			}
			DebugType iListType = shownType.GetInterface(typeof(IList).FullName);
			if (iListType != null) {
				yield return new IListNode(targetObject, iListType);
			}
			foreach(Expression childExpr in targetObject.AppendObjectMembers(shownType, Flags)) {
				yield return ValueNode.Create(childExpr);
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
			
			this.Image = IconService.GetBitmap("Icons.16x16.Class");
			this.Name = StringParser.Parse("${res:MainWindow.Windows.Debug.LocalVariables.BaseClass}");
			this.Type = shownType.FullName;
			if (shownType.FullName == "System.Object") {
				this.ChildNodes = null;
			} else {
				this.ChildNodes = Utils.GetChildNodesOfObject(targetObject, shownType);
			}
		}
	}
	
	public class NonPublicInstanceMembersNode: AbstractNode
	{
		public static BindingFlags Flags = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Field | BindingFlags.GetProperty;
		
		Expression targetObject;
		DebugType shownType;
		
		public NonPublicInstanceMembersNode(Expression targetObject, DebugType shownType)
		{
			this.targetObject = targetObject;
			this.shownType = shownType;
			
			this.Name = StringParser.Parse("${res:MainWindow.Windows.Debug.LocalVariables.NonPublicMembers}");
			this.ChildNodes = GetChildNodes();
		}
		
		IEnumerable<AbstractNode> GetChildNodes()
		{
			foreach(Expression childExpr in targetObject.AppendObjectMembers(shownType, Flags)) {
				yield return ValueNode.Create(childExpr);
			}
		}
	}
	
	public class StaticMembersNode: AbstractNode
	{
		public static BindingFlags Flags = BindingFlags.Public | BindingFlags.Static | BindingFlags.Field | BindingFlags.GetProperty;
		
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
			if (shownType.HasMembers(NonPublicStaticMembersNode.Flags)) {
				yield return new NonPublicStaticMembersNode(targetObject, shownType);
			}
			foreach(Expression childExpr in targetObject.AppendObjectMembers(shownType, Flags)) {
				yield return ValueNode.Create(childExpr);
			}
		}
	}
	
	public class NonPublicStaticMembersNode: AbstractNode
	{
		public static BindingFlags Flags = BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Field | BindingFlags.GetProperty;
		
		Expression targetObject;
		DebugType shownType;
		
		public NonPublicStaticMembersNode(Expression targetObject, DebugType shownType)
		{
			this.targetObject = targetObject;
			this.shownType = shownType;
			
			this.Name = StringParser.Parse("${res:MainWindow.Windows.Debug.LocalVariables.NonPublicStaticMembers}");
			this.ChildNodes = GetChildNodes();
		}
		
		IEnumerable<AbstractNode> GetChildNodes()
		{
			foreach(Expression childExpr in targetObject.AppendObjectMembers(shownType, Flags)) {
				yield return ValueNode.Create(childExpr);
			}
		}
	}
}
