using System;
using MPM.Core.Protocols;
using MPM.Data;
using MPM.Data.Repository;
using MPM.Net;
using MPM.Net.Protocols.Minecraft;
using MPM.Types;

namespace MPM.CLI {
	public class CLIProtocolResolver : IProtocolResolver {
		public CLIProtocolResolver(IHashRepository hashRepository) {
		}

		private IHashRepository HashRepository { get; }

		public IArchResolver GetArchResolver() => new MetaArchInstaller();

		public byte[] Resolve(string protocol, string path, Hash hash) {
			switch (protocol) {
				default:
					throw new NotSupportedException($"Protocol {protocol} is not supported by {nameof(CLIProtocolResolver)}");
			}
		}
	}
}
