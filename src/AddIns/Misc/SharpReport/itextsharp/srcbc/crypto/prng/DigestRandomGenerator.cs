using System;

using Org.BouncyCastle.Crypto.Digests;

namespace Org.BouncyCastle.Crypto.Prng
{
	/**
	 * Random generation based on the digest with counter. Calling addSeedMaterial will
	 * always increase the entropy of the hash.
	 * <p>
	 * Internal access to the digest is syncrhonized so a single one of these can be shared.
	 * </p>
	 */
	public class DigestRandomGenerator
		: IRandomGenerator
	{
		private long	counter;
		private IDigest	digest;
		private byte[]	state;

		public DigestRandomGenerator(
			IDigest digest)
		{
			this.digest = digest;
			this.state = new byte[digest.GetDigestSize()];
			this.counter = 1;
		}

		public void AddSeedMaterial(
			byte[] inSeed)
		{
			lock (this)
			{
				DigestUpdate(inSeed);
			}
		}

		public void AddSeedMaterial(
			long rSeed)
		{
			lock (this)
			{
				for (int i = 0; i != 8; i++)
				{
					DigestUpdate((byte)rSeed);
//					rSeed >>>= 8;
					rSeed >>= 8;
				}
			}
		}

		public void NextBytes(
			byte[] bytes)
		{
			NextBytes(bytes, 0, bytes.Length);
		}

		public void NextBytes(
			byte[]	bytes,
			int		start,
			int		len)
		{
			lock (this)
			{
				int stateOff = 0;

				DigestDoFinal(state);

				int end = start + len;
				for (int i = start; i < end; ++i)
				{
					if (stateOff == state.Length)
					{
						DigestUpdate(counter++);
						DigestUpdate(state);
						DigestDoFinal(state);
						stateOff = 0;
					}
					bytes[i] = state[stateOff++];
				}

				DigestUpdate(counter++);
				DigestUpdate(state);
			}
		}

		private void DigestUpdate(long seed)
		{
			for (int i = 0; i != 8; i++)
			{
				digest.Update((byte)seed);
//				seed >>>= 8;
				seed >>= 8;
			}
		}

		private void DigestUpdate(byte[] inSeed)
		{
			digest.BlockUpdate(inSeed, 0, inSeed.Length);
		}

		private void DigestDoFinal(byte[] result)
		{
			digest.DoFinal(result, 0);
		}
	}
}
