using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Remoting.Channels;
using System.Windows.Forms;
using System.Runtime.Remoting.Messaging;
using System.IO;
using System.Threading;
using System.Reflection;

namespace CustomSinks
{
	class PrivateEventHandlersClientChannelSink : BaseChannelSinkWithProperties, IClientFormatterSink 
	{
		IClientFormatterSink nextSink;

		public PrivateEventHandlersClientChannelSink(IClientFormatterSink nextSink)
		{
			this.nextSink = nextSink;
		}


		// IClientChannelSink Members

		void IClientChannelSink.AsyncProcessRequest(IClientChannelSinkStack sinkStack, IMessage msg, ITransportHeaders headers, Stream stream)
		{
			nextSink.AsyncProcessRequest(sinkStack, msg, headers, stream);
		}

		void IClientChannelSink.AsyncProcessResponse(IClientResponseChannelSinkStack sinkStack, object state, ITransportHeaders headers, Stream stream)
		{
			nextSink.AsyncProcessResponse(sinkStack, state, headers, stream);
		}

		Stream IClientChannelSink.GetRequestStream(IMessage msg, ITransportHeaders headers)
		{
			return nextSink.GetRequestStream(msg, headers);
		}

		IClientChannelSink IClientChannelSink.NextChannelSink {
			get {
				return nextSink;
			}
		}

		void IClientChannelSink.ProcessMessage(IMessage msg, ITransportHeaders requestHeaders, Stream requestStream, out ITransportHeaders responseHeaders, out Stream responseStream)
		{
			nextSink.ProcessMessage(msg, requestHeaders, requestStream, out responseHeaders, out responseStream);
		}

		// IMessageSink Members

		IMessageCtrl IMessageSink.AsyncProcessMessage(IMessage msg, IMessageSink replySink)
		{
			return nextSink.AsyncProcessMessage(msg, replySink);
		}

		IMessageSink IMessageSink.NextSink {
			get {
				return nextSink;
			}
		}

		IMessage IMessageSink.SyncProcessMessage(IMessage msg)
		{
			Console.WriteLine("Remoting message:" + msg.Properties["__MethodName"].ToString());
			MethodCall methodCall = new MethodCall(msg);
			if (methodCall.ArgCount > 0 && methodCall.Args[0] is Delegate) {
				Delegate realDelegate = methodCall.Args[0] as Delegate;
				Type type = realDelegate.GetType();
				EventForwarder eventForwarder = new EventForwarder(realDelegate);
				MethodInfo proxyMethod = typeof(EventForwarder).GetMethod("ForwardEvent" + realDelegate.Method.GetParameters().Length);
				Delegate proxyDelegate = Delegate.CreateDelegate(type, eventForwarder, proxyMethod);
				methodCall.Args[0] = proxyDelegate;
			}

			return nextSink.SyncProcessMessage(methodCall);
		}

		// IChannelSinkBase Members

		System.Collections.IDictionary IChannelSinkBase.Properties {
			get {
				return this.Properties;
			}
		}
}
}
