// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Messaging;

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
			Console.WriteLine("Remoting call:" + msg.Properties["__MethodName"]);
			MethodCall methodCall = new MethodCall(msg);
			if (methodCall.ArgCount == 1 && methodCall.Args[0] is Delegate) {
				Delegate realDelegate = methodCall.Args[0] as Delegate;
				methodCall.Args[0] = new EventForwarder(realDelegate).ProxyDelegate;
			}

			AsyncMessageResponseSink responseSink = new AsyncMessageResponseSink();

			// Send the message
			nextSink.AsyncProcessMessage(methodCall, responseSink);

			return responseSink.WaitForResponse();
		}

		// IChannelSinkBase Members

		System.Collections.IDictionary IChannelSinkBase.Properties {
			get {
				return this.Properties;
			}
		}
}
}
