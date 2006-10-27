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
using System.Threading;
using System.Windows.Forms;

namespace CustomSinks
{
	class InvokeOnSTAClientChannelSink : BaseChannelSinkWithProperties, IClientFormatterSink 
	{
		IClientFormatterSink nextSink;

		public InvokeOnSTAClientChannelSink(IClientFormatterSink nextSink)
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
			IMessage response = null;
			Thread thread = new Thread(new ThreadStart(delegate {
				response = nextSink.SyncProcessMessage(msg);
			}));

			thread.Start();

			ThreadPriority oldPriority = Thread.CurrentThread.Priority;
			Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;
			while (thread.IsAlive) Application.DoEvents();
			Thread.CurrentThread.Priority = oldPriority;

			return response;
		}

		// IChannelSinkBase Members

		System.Collections.IDictionary IChannelSinkBase.Properties {
			get {
				return this.Properties;
			}
		}
}
}
