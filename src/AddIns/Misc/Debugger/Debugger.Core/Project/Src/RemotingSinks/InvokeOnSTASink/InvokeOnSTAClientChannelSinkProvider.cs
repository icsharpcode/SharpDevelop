// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Runtime.Remoting.Channels;

namespace CustomSinks
{
	class InvokeOnSTAClientChannelSinkProvider: IClientChannelSinkProvider 
	{
		private IClientChannelSinkProvider nextProvider;

		public InvokeOnSTAClientChannelSinkProvider(IDictionary properties, ICollection providerData)
		{
		}

		IClientChannelSink IClientChannelSinkProvider.CreateSink(IChannelSender channel, string url, object remoteChannelData)
		{
			IClientChannelSink nextSink = nextProvider.CreateSink(channel, url, remoteChannelData);
			IClientChannelSink thisSink = new InvokeOnSTAClientChannelSink(nextSink as IClientFormatterSink);
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
