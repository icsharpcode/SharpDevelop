// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.ComponentModel;
using System.Reflection;

using NoGoop.ObjBrowser.TreeNodes;
using NoGoop.ObjBrowser.Types;
using NoGoop.Util;

namespace NoGoop.ObjBrowser
{
	public class TypeHandlerManager
	{
		public class TypeHandlerInfo
		{
			internal string _name;
			internal Type   _handlerType;
			internal Type   _typeHandled;    
			internal bool   _enabled;
			
			internal TypeHandlerInfo(string name, Type typeHandled, Type handlerType)
			{
				_name = name;
				_typeHandled = typeHandled;
				_handlerType = handlerType;
				_enabled = true;
			}
			
			public override String ToString()
			{
				return _typeHandled.ToString();
			}
			
			public string Name {
				get	{
					return _name;
				}
			}
			
			public Type HandledType {
				get {
					return _typeHandled;
				}
			}
			
			public bool Enabled {
				get {
					return _enabled;
				}
				set {
					_enabled = value;
				}
			}
		}
		
		protected ArrayList       _typeHandlers;
		static TypeHandlerManager _typeHandlerManager;
		
		internal TypeHandlerManager()
		{
			_typeHandlerManager = this;
			_typeHandlers = new ArrayList();
			_typeHandlers.Add(new TypeHandlerInfo("IEnumerator", typeof(IEnumerator), typeof(IEnumeratorTypeHandler)));
			_typeHandlers.Add(new TypeHandlerInfo("IList", typeof(IList), typeof(IListTypeHandler)));
			_typeHandlers.Add(new TypeHandlerInfo("EventHandlerList", typeof(EventHandlerList), typeof(EventHandlerListTypeHandler)));
		}
		
		public static TypeHandlerManager Instance {
			get {
				if (_typeHandlerManager == null) {
					_typeHandlerManager = new TypeHandlerManager();
				}
				return _typeHandlerManager;
			}
		}
		
		public ICollection GetTypeHandlers()
		{
			return _typeHandlers;
		}
		
		internal BaseTypeHandler GetTypeHandler(Type type, ObjectTreeNode node)
		{
			TypeHandlerInfo thInfo = null;
			if (type == null)
				return null;
			foreach (TypeHandlerInfo t in _typeHandlers) {
				if (t._typeHandled.IsAssignableFrom(type)) {
					thInfo = t;
					break;
				}
			}
			if (thInfo == null)
				return null;
			// We assume there is only one constructor, 
			// which takes the type handler info and the node
			ConstructorInfo ci = thInfo._handlerType.
				GetConstructors(ReflectionHelper.ALL_BINDINGS)[0];
			Object[] parameters = new Object[] { thInfo, node };
			return (BaseTypeHandler)ci.Invoke(parameters);
		}
	}
}
