// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

namespace Debugger.Wrappers.CorDebug
{
	using System;
	using System.Collections;
	using System.Reflection;
	using System.Runtime.Remoting;
	using System.Runtime.Remoting.Proxies;
	using System.Runtime.Remoting.Messaging;
	
	
	public class WrapperProxy : RealProxy, IRemotingTypeInfo
	{
		Hashtable wrappingClassCache = new Hashtable();
		
		MarshalByRefObject wrappedComObject;
		
		public WrapperProxy(MarshalByRefObject objectToWrapp) :
			base (typeof(MarshalByRefObject))
		{
			wrappedObject = objectToWrapp;
		}
		
		void GetWrappingClass(Type interfaceType)
		{
			object cachedItem = wrappingClassCache[interfaceType];
			if (cachedItem != null) {
				return cachedItem;
			}
			
			interfaceType.GetCustomAttributes(typeof(WrappingClassAttribute), false);
			
			// TODO
			
			Type wrappingClassType;
			
			object wrappingClass = Activator.CreateInstance(wrappingClassType, wrappedComObject);
			
			wrappingClassCache.Add(interfaceType, wrappingClass);
		}
		
		public override IMessage Invoke(IMessage msg)
		{
			Type interfaceType = (msg as IMethodCallMessage).MethodBase.ReflectedType;
			
			MarshalByRefObject wrappingClass = GetWrappingClass(wrappingClassType);
			
			return RemotingServices.ExecuteMessage(wrappingClass, msg as IMethodCallMessage);
		}
		
		public string TypeName {
			get {
				return wrappedObject.GetType().FullName;
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public bool CanCastTo(Type type, object o)
		{
			Wrapper wrappedObject = o as Wrapper;
			
			if (wrappedObject == null) {
				return false;
			}
			
			if (!type.IsSubclassOf(typeof(Wrapper))) {
				return false;
			}
			
			try {
				Activator.CreateInstance(type, wrappedObject.WrappedObject);
				return true;
			}
			catch {
				return false;
			}
		}
	}
}
