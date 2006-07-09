// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

using Debugger.Wrappers.CorDebug;

namespace Debugger
{
	public class Variable: RemotingObjectBase, IExpirable
	{
		string name;
		PersistentValue pValue;
		bool isStatic;
		bool isPublic;
		
		public event EventHandler<PersistentValueEventArgs> ValueChanged {
			add {
				pValue.ValueChanged += value;
			}
			remove {
				pValue.ValueChanged -= value;
			}
		}
		
		public event EventHandler Expired {
			add {
				pValue.Expired += value;
			}
			remove {
				pValue.Expired -= value;
			}
		}
		
		public bool HasExpired {
			get {
				return pValue.HasExpired;
			}
		}
		
		public NDebugger Debugger {
			get {
				return pValue.Debugger;
			}
		}
		
		public virtual string Name {
			get{ 
				return name; 
			}
		}
		
		public PersistentValue PersistentValue {
			get {
				return pValue;
			}
		}
		
		public bool IsStatic {
			get {
				return isStatic;
			}
		}
		
		public bool IsPublic {
			get {
				return isPublic;
			}
		}
		
		public Value Value {
			get {
				return pValue.Value;
			}
		}
		
		public bool MayHaveSubVariables {
			get {
				return pValue.Value.MayHaveSubVariables;
			}
		}
		
		public VariableCollection SubVariables {
			get {
				return pValue.Value.SubVariables;
			}
		}
		
		public Variable(string name, PersistentValue pValue):this(name, false, true, pValue)
		{
			
		}
		
		public Variable(string name, bool isStatic, bool isPublic, PersistentValue pValue)
		{
			this.name = name;
			this.isStatic = isStatic;
			this.isPublic = isPublic;
			this.pValue = pValue;
			
			if (name.StartsWith("<") && name.Contains(">") && name != "<Base class>") {
				string middle = name.TrimStart('<').Split('>')[0]; // Get text between '<' and '>'
				if (middle != "") {
					this.name = middle;
				}
			}
		}
	}
}
