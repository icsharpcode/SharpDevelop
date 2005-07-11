using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Remoting.Channels;
using System.Collections;

namespace CustomSinks
{
	class PrivateEventHandlersClientChannelSinkProvider: IClientChannelSinkProvider 
	{
		private IClientChannelSinkProvider nextProvider;

		public PrivateEventHandlersClientChannelSinkProvider(IDictionary properties, ICollection providerData)
		{
		}

		IClientChannelSink IClientChannelSinkProvider.CreateSink(IChannelSender channel, string url, object remoteChannelData)
		{
			IClientChannelSink nextSink = nextProvider.CreateSink(channel, url, remoteChannelData);
			IClientChannelSink thisSink = new PrivateEventHandlersClientChannelSink(nextSink as IClientFormatterSink);
			return thisSink;
		}

		IClientChannelSinkProvider IClientChannelSinkProvider.Next {
			get	{
				return nextProvider;
			}
			set	{
				nextProvider = value;
			}
		}
	}
}
