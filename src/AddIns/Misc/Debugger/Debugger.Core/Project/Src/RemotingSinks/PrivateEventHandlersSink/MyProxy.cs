// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Activation;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
using System.Runtime.Remoting.Services;

namespace CustomSinks
{
	[AttributeUsage(AttributeTargets.Class)]
	public class MyProxyAttribute: ProxyAttribute
	{
		public override MarshalByRefObject CreateInstance(Type type)
		{
			Console.WriteLine("Creating proxy of type " + type.ToString());
			MarshalByRefObject instance = base.CreateInstance(type);
			MyProxy proxy = new MyProxy(type, instance);
			return (MarshalByRefObject)proxy.GetTransparentProxy();
		}
	}

	public class MyProxy: RealProxy
	{
		private MarshalByRefObject realObject;

		public MyProxy(Type type, MarshalByRefObject realObject):base(type)
		{
			this.realObject = realObject;
		}

		public override IMessage Invoke(IMessage msg)
		{
			Console.WriteLine("Proxy called: " + msg.Properties["__MethodName"]);
			if (msg is IConstructionCallMessage) {
				IConstructionCallMessage ctorMsg = (IConstructionCallMessage)msg;

				try {
					RemotingServices.GetRealProxy(realObject).InitializeServerObject(ctorMsg);
				} catch {
				}

				ObjRef objRef = RemotingServices.Marshal(realObject);
				RemotingServices.Unmarshal(objRef);

				MarshalByRefObject transpProxy = (MarshalByRefObject)this.GetTransparentProxy();

				return EnterpriseServicesHelper.CreateConstructionReturnMessage(ctorMsg, transpProxy);
			} else {
				return RemotingServices.ExecuteMessage(realObject, (IMethodCallMessage)msg);
			}
		}
	}
}
