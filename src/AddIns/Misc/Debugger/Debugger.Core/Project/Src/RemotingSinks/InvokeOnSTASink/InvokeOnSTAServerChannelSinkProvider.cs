using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Remoting.Channels;
using System.Collections;

namespace CustomSinks
{
	class InvokeOnSTAServerChannelSinkProvider: IServerChannelSinkProvider 
	{
		private IServerChannelSinkProvider nextProvider;

		public InvokeOnSTAServerChannelSinkProvider(IDictionary properties, ICollection providerData)
		{
		}

		IServerChannelSink IServerChannelSinkProvider.CreateSink(IChannelReceiver channel)
		{
			IServerChannelSink nextSink = nextProvider.CreateSink(channel);
			IServerChannelSink thisSink = new InvokeOnSTAServerChannelSink(nextSink);
			return thisSink;
		}

		void IServerChannelSinkProvider.GetChannelData(IChannelDataStore channelData)
		{
		}

		IServerChannelSinkProvider IServerChannelSinkProvider.Next {
			get	{
				return nextProvider;
			}
			set	{
				nextProvider = value;
			}
		}
	}
}
