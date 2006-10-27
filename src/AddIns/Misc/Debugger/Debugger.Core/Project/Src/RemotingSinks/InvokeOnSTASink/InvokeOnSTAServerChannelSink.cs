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
using System.Windows.Forms;

namespace CustomSinks
{
	class InvokeOnSTAServerChannelSink: BaseChannelSinkWithProperties, IServerChannelSink
	{
		IServerChannelSink nextSink;

		public InvokeOnSTAServerChannelSink(IServerChannelSink nextSink)
		{
			this.nextSink = nextSink;
		}

		void IServerChannelSink.AsyncProcessResponse(IServerResponseChannelSinkStack sinkStack, object state, System.Runtime.Remoting.Messaging.IMessage msg, ITransportHeaders headers, System.IO.Stream stream)
		{
			nextSink.AsyncProcessResponse(sinkStack, state, msg, headers, stream);
		}

		System.IO.Stream IServerChannelSink.GetResponseStream(IServerResponseChannelSinkStack sinkStack, object state, System.Runtime.Remoting.Messaging.IMessage msg, ITransportHeaders headers)
		{
			return nextSink.GetResponseStream(sinkStack, state, msg, headers);
		}

		IServerChannelSink IServerChannelSink.NextChannelSink {
			get { 
				return nextSink; 
			}
		}

		ServerProcessing IServerChannelSink.ProcessMessage(IServerChannelSinkStack sinkStack, System.Runtime.Remoting.Messaging.IMessage requestMsg, ITransportHeaders requestHeaders, System.IO.Stream requestStream, out System.Runtime.Remoting.Messaging.IMessage responseMsg, out ITransportHeaders responseHeaders, out System.IO.Stream responseStream)
		{
			if (Application.OpenForms.Count > 0) {
				IMessage outResponseMsg = null;
				ITransportHeaders outResponseHeaders = null;
				Stream outResponseStream = null;
				ServerProcessing returnValue = ServerProcessing.Complete;
				Application.OpenForms[0].Invoke(new EventHandler(delegate
				{
					returnValue = nextSink.ProcessMessage(sinkStack, requestMsg, requestHeaders, requestStream, out outResponseMsg, out outResponseHeaders, out outResponseStream);
				}));
				responseMsg = outResponseMsg;
				responseHeaders = outResponseHeaders;
				responseStream = outResponseStream;
				return returnValue;
			} else {
				return nextSink.ProcessMessage(sinkStack, requestMsg, requestHeaders, requestStream, out responseMsg, out responseHeaders, out responseStream);
			}
		}

		System.Collections.IDictionary IChannelSinkBase.Properties {
			get {
				return this.Properties;
			}
		}
	}
}
