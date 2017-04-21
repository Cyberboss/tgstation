using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace DDClickRelayTest
{
	[ServiceContract]
	public interface IStringReverser
	{
		[OperationContract]
		string ReverseString(string value);
	}

	class Program
	{
		static void Main(string[] args)
		{
			System.Diagnostics.Debugger.Launch();
			ChannelFactory<IStringReverser> httpFactory =
			  new ChannelFactory<IStringReverser>(
				new BasicHttpBinding(),
				new EndpointAddress(
				  "http://localhost:8000/Reverse"));

			ChannelFactory<IStringReverser> pipeFactory =
			  new ChannelFactory<IStringReverser>(
				new NetNamedPipeBinding(),
				new EndpointAddress(
				  "net.pipe://localhost/PipeReverse"));

			IStringReverser httpProxy =
			  httpFactory.CreateChannel();

			IStringReverser pipeProxy =
			  pipeFactory.CreateChannel();
			while (true)
			{
				string str = Console.ReadLine();
				Console.WriteLine("http: " + httpProxy.ReverseString(str));
				Console.WriteLine("pipe: " + pipeProxy.ReverseString(str));
			}
		}
	}
}