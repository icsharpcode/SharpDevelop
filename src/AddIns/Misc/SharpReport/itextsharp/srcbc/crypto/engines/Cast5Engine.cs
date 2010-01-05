using System;

using Org.BouncyCastle.Crypto.Parameters;

namespace Org.BouncyCastle.Crypto.Engines
{
    /**
    * A class that provides CAST key encryption operations,
    * such as encoding data and generating keys.
    *
    * All the algorithms herein are from the Internet RFC's
    *
    * RFC2144 - Cast5 (64bit block, 40-128bit key)
    * RFC2612 - CAST6 (128bit block, 128-256bit key)
    *
    * and implement a simplified cryptography interface.
    */
    public class Cast5Engine
		: IBlockCipher
    {
        internal const int M32 = unchecked((int) 0xffffffff);

        internal static readonly int[]
            S1 = {
                     unchecked((int) 0x30fb40d4), unchecked((int) 0x9fa0ff0b), unchecked((int) 0x6beccd2f), unchecked((int) 0x3f258c7a), unchecked((int) 0x1e213f2f), unchecked((int) 0x9c004dd3), unchecked((int) 0x6003e540), unchecked((int) 0xcf9fc949),
                     unchecked((int) 0xbfd4af27), unchecked((int) 0x88bbbdb5), unchecked((int) 0xe2034090), unchecked((int) 0x98d09675), unchecked((int) 0x6e63a0e0), unchecked((int) 0x15c361d2), unchecked((int) 0xc2e7661d), unchecked((int) 0x22d4ff8e),
                     unchecked((int) 0x28683b6f), unchecked((int) 0xc07fd059), unchecked((int) 0xff2379c8), unchecked((int) 0x775f50e2), unchecked((int) 0x43c340d3), unchecked((int) 0xdf2f8656), unchecked((int) 0x887ca41a), unchecked((int) 0xa2d2bd2d),
                     unchecked((int) 0xa1c9e0d6), unchecked((int) 0x346c4819), unchecked((int) 0x61b76d87), unchecked((int) 0x22540f2f), unchecked((int) 0x2abe32e1), unchecked((int) 0xaa54166b), unchecked((int) 0x22568e3a), unchecked((int) 0xa2d341d0),
                     unchecked((int) 0x66db40c8), unchecked((int) 0xa784392f), unchecked((int) 0x004dff2f), unchecked((int) 0x2db9d2de), unchecked((int) 0x97943fac), unchecked((int) 0x4a97c1d8), unchecked((int) 0x527644b7), unchecked((int) 0xb5f437a7),
                     unchecked((int) 0xb82cbaef), unchecked((int) 0xd751d159), unchecked((int) 0x6ff7f0ed), unchecked((int) 0x5a097a1f), unchecked((int) 0x827b68d0), unchecked((int) 0x90ecf52e), unchecked((int) 0x22b0c054), unchecked((int) 0xbc8e5935),
                     unchecked((int) 0x4b6d2f7f), unchecked((int) 0x50bb64a2), unchecked((int) 0xd2664910), unchecked((int) 0xbee5812d), unchecked((int) 0xb7332290), unchecked((int) 0xe93b159f), unchecked((int) 0xb48ee411), unchecked((int) 0x4bff345d),
                     unchecked((int) 0xfd45c240), unchecked((int) 0xad31973f), unchecked((int) 0xc4f6d02e), unchecked((int) 0x55fc8165), unchecked((int) 0xd5b1caad), unchecked((int) 0xa1ac2dae), unchecked((int) 0xa2d4b76d), unchecked((int) 0xc19b0c50),
                     unchecked((int) 0x882240f2), unchecked((int) 0x0c6e4f38), unchecked((int) 0xa4e4bfd7), unchecked((int) 0x4f5ba272), unchecked((int) 0x564c1d2f), unchecked((int) 0xc59c5319), unchecked((int) 0xb949e354), unchecked((int) 0xb04669fe),
                     unchecked((int) 0xb1b6ab8a), unchecked((int) 0xc71358dd), unchecked((int) 0x6385c545), unchecked((int) 0x110f935d), unchecked((int) 0x57538ad5), unchecked((int) 0x6a390493), unchecked((int) 0xe63d37e0), unchecked((int) 0x2a54f6b3),
                     unchecked((int) 0x3a787d5f), unchecked((int) 0x6276a0b5), unchecked((int) 0x19a6fcdf), unchecked((int) 0x7a42206a), unchecked((int) 0x29f9d4d5), unchecked((int) 0xf61b1891), unchecked((int) 0xbb72275e), unchecked((int) 0xaa508167),
                     unchecked((int) 0x38901091), unchecked((int) 0xc6b505eb), unchecked((int) 0x84c7cb8c), unchecked((int) 0x2ad75a0f), unchecked((int) 0x874a1427), unchecked((int) 0xa2d1936b), unchecked((int) 0x2ad286af), unchecked((int) 0xaa56d291),
                     unchecked((int) 0xd7894360), unchecked((int) 0x425c750d), unchecked((int) 0x93b39e26), unchecked((int) 0x187184c9), unchecked((int) 0x6c00b32d), unchecked((int) 0x73e2bb14), unchecked((int) 0xa0bebc3c), unchecked((int) 0x54623779),
                     unchecked((int) 0x64459eab), unchecked((int) 0x3f328b82), unchecked((int) 0x7718cf82), unchecked((int) 0x59a2cea6), unchecked((int) 0x04ee002e), unchecked((int) 0x89fe78e6), unchecked((int) 0x3fab0950), unchecked((int) 0x325ff6c2),
                     unchecked((int) 0x81383f05), unchecked((int) 0x6963c5c8), unchecked((int) 0x76cb5ad6), unchecked((int) 0xd49974c9), unchecked((int) 0xca180dcf), unchecked((int) 0x380782d5), unchecked((int) 0xc7fa5cf6), unchecked((int) 0x8ac31511),
                     unchecked((int) 0x35e79e13), unchecked((int) 0x47da91d0), unchecked((int) 0xf40f9086), unchecked((int) 0xa7e2419e), unchecked((int) 0x31366241), unchecked((int) 0x051ef495), unchecked((int) 0xaa573b04), unchecked((int) 0x4a805d8d),
                     unchecked((int) 0x548300d0), unchecked((int) 0x00322a3c), unchecked((int) 0xbf64cddf), unchecked((int) 0xba57a68e), unchecked((int) 0x75c6372b), unchecked((int) 0x50afd341), unchecked((int) 0xa7c13275), unchecked((int) 0x915a0bf5),
                     unchecked((int) 0x6b54bfab), unchecked((int) 0x2b0b1426), unchecked((int) 0xab4cc9d7), unchecked((int) 0x449ccd82), unchecked((int) 0xf7fbf265), unchecked((int) 0xab85c5f3), unchecked((int) 0x1b55db94), unchecked((int) 0xaad4e324),
                     unchecked((int) 0xcfa4bd3f), unchecked((int) 0x2deaa3e2), unchecked((int) 0x9e204d02), unchecked((int) 0xc8bd25ac), unchecked((int) 0xeadf55b3), unchecked((int) 0xd5bd9e98), unchecked((int) 0xe31231b2), unchecked((int) 0x2ad5ad6c),
                     unchecked((int) 0x954329de), unchecked((int) 0xadbe4528), unchecked((int) 0xd8710f69), unchecked((int) 0xaa51c90f), unchecked((int) 0xaa786bf6), unchecked((int) 0x22513f1e), unchecked((int) 0xaa51a79b), unchecked((int) 0x2ad344cc),
                     unchecked((int) 0x7b5a41f0), unchecked((int) 0xd37cfbad), unchecked((int) 0x1b069505), unchecked((int) 0x41ece491), unchecked((int) 0xb4c332e6), unchecked((int) 0x032268d4), unchecked((int) 0xc9600acc), unchecked((int) 0xce387e6d),
                     unchecked((int) 0xbf6bb16c), unchecked((int) 0x6a70fb78), unchecked((int) 0x0d03d9c9), unchecked((int) 0xd4df39de), unchecked((int) 0xe01063da), unchecked((int) 0x4736f464), unchecked((int) 0x5ad328d8), unchecked((int) 0xb347cc96),
                     unchecked((int) 0x75bb0fc3), unchecked((int) 0x98511bfb), unchecked((int) 0x4ffbcc35), unchecked((int) 0xb58bcf6a), unchecked((int) 0xe11f0abc), unchecked((int) 0xbfc5fe4a), unchecked((int) 0xa70aec10), unchecked((int) 0xac39570a),
                     unchecked((int) 0x3f04442f), unchecked((int) 0x6188b153), unchecked((int) 0xe0397a2e), unchecked((int) 0x5727cb79), unchecked((int) 0x9ceb418f), unchecked((int) 0x1cacd68d), unchecked((int) 0x2ad37c96), unchecked((int) 0x0175cb9d),
                     unchecked((int) 0xc69dff09), unchecked((int) 0xc75b65f0), unchecked((int) 0xd9db40d8), unchecked((int) 0xec0e7779), unchecked((int) 0x4744ead4), unchecked((int) 0xb11c3274), unchecked((int) 0xdd24cb9e), unchecked((int) 0x7e1c54bd),
                     unchecked((int) 0xf01144f9), unchecked((int) 0xd2240eb1), unchecked((int) 0x9675b3fd), unchecked((int) 0xa3ac3755), unchecked((int) 0xd47c27af), unchecked((int) 0x51c85f4d), unchecked((int) 0x56907596), unchecked((int) 0xa5bb15e6),
                     unchecked((int) 0x580304f0), unchecked((int) 0xca042cf1), unchecked((int) 0x011a37ea), unchecked((int) 0x8dbfaadb), unchecked((int) 0x35ba3e4a), unchecked((int) 0x3526ffa0), unchecked((int) 0xc37b4d09), unchecked((int) 0xbc306ed9),
                     unchecked((int) 0x98a52666), unchecked((int) 0x5648f725), unchecked((int) 0xff5e569d), unchecked((int) 0x0ced63d0), unchecked((int) 0x7c63b2cf), unchecked((int) 0x700b45e1), unchecked((int) 0xd5ea50f1), unchecked((int) 0x85a92872),
                     unchecked((int) 0xaf1fbda7), unchecked((int) 0xd4234870), unchecked((int) 0xa7870bf3), unchecked((int) 0x2d3b4d79), unchecked((int) 0x42e04198), unchecked((int) 0x0cd0ede7), unchecked((int) 0x26470db8), unchecked((int) 0xf881814c),
                     unchecked((int) 0x474d6ad7), unchecked((int) 0x7c0c5e5c), unchecked((int) 0xd1231959), unchecked((int) 0x381b7298), unchecked((int) 0xf5d2f4db), unchecked((int) 0xab838653), unchecked((int) 0x6e2f1e23), unchecked((int) 0x83719c9e),
                     unchecked((int) 0xbd91e046), unchecked((int) 0x9a56456e), unchecked((int) 0xdc39200c), unchecked((int) 0x20c8c571), unchecked((int) 0x962bda1c), unchecked((int) 0xe1e696ff), unchecked((int) 0xb141ab08), unchecked((int) 0x7cca89b9),
                     unchecked((int) 0x1a69e783), unchecked((int) 0x02cc4843), unchecked((int) 0xa2f7c579), unchecked((int) 0x429ef47d), unchecked((int) 0x427b169c), unchecked((int) 0x5ac9f049), unchecked((int) 0xdd8f0f00), unchecked((int) 0x5c8165bf)
                 },
            S2 =
        {
            unchecked((int) 0x1f201094), unchecked((int) 0xef0ba75b), unchecked((int) 0x69e3cf7e), unchecked((int) 0x393f4380), unchecked((int) 0xfe61cf7a), unchecked((int) 0xeec5207a), unchecked((int) 0x55889c94), unchecked((int) 0x72fc0651),
            unchecked((int) 0xada7ef79), unchecked((int) 0x4e1d7235), unchecked((int) 0xd55a63ce), unchecked((int) 0xde0436ba), unchecked((int) 0x99c430ef), unchecked((int) 0x5f0c0794), unchecked((int) 0x18dcdb7d), unchecked((int) 0xa1d6eff3),
            unchecked((int) 0xa0b52f7b), unchecked((int) 0x59e83605), unchecked((int) 0xee15b094), unchecked((int) 0xe9ffd909), unchecked((int) 0xdc440086), unchecked((int) 0xef944459), unchecked((int) 0xba83ccb3), unchecked((int) 0xe0c3cdfb),
            unchecked((int) 0xd1da4181), unchecked((int) 0x3b092ab1), unchecked((int) 0xf997f1c1), unchecked((int) 0xa5e6cf7b), unchecked((int) 0x01420ddb), unchecked((int) 0xe4e7ef5b), unchecked((int) 0x25a1ff41), unchecked((int) 0xe180f806),
            unchecked((int) 0x1fc41080), unchecked((int) 0x179bee7a), unchecked((int) 0xd37ac6a9), unchecked((int) 0xfe5830a4), unchecked((int) 0x98de8b7f), unchecked((int) 0x77e83f4e), unchecked((int) 0x79929269), unchecked((int) 0x24fa9f7b),
            unchecked((int) 0xe113c85b), unchecked((int) 0xacc40083), unchecked((int) 0xd7503525), unchecked((int) 0xf7ea615f), unchecked((int) 0x62143154), unchecked((int) 0x0d554b63), unchecked((int) 0x5d681121), unchecked((int) 0xc866c359),
            unchecked((int) 0x3d63cf73), unchecked((int) 0xcee234c0), unchecked((int) 0xd4d87e87), unchecked((int) 0x5c672b21), unchecked((int) 0x071f6181), unchecked((int) 0x39f7627f), unchecked((int) 0x361e3084), unchecked((int) 0xe4eb573b),
            unchecked((int) 0x602f64a4), unchecked((int) 0xd63acd9c), unchecked((int) 0x1bbc4635), unchecked((int) 0x9e81032d), unchecked((int) 0x2701f50c), unchecked((int) 0x99847ab4), unchecked((int) 0xa0e3df79), unchecked((int) 0xba6cf38c),
            unchecked((int) 0x10843094), unchecked((int) 0x2537a95e), unchecked((int) 0xf46f6ffe), unchecked((int) 0xa1ff3b1f), unchecked((int) 0x208cfb6a), unchecked((int) 0x8f458c74), unchecked((int) 0xd9e0a227), unchecked((int) 0x4ec73a34),
            unchecked((int) 0xfc884f69), unchecked((int) 0x3e4de8df), unchecked((int) 0xef0e0088), unchecked((int) 0x3559648d), unchecked((int) 0x8a45388c), unchecked((int) 0x1d804366), unchecked((int) 0x721d9bfd), unchecked((int) 0xa58684bb),
            unchecked((int) 0xe8256333), unchecked((int) 0x844e8212), unchecked((int) 0x128d8098), unchecked((int) 0xfed33fb4), unchecked((int) 0xce280ae1), unchecked((int) 0x27e19ba5), unchecked((int) 0xd5a6c252), unchecked((int) 0xe49754bd),
            unchecked((int) 0xc5d655dd), unchecked((int) 0xeb667064), unchecked((int) 0x77840b4d), unchecked((int) 0xa1b6a801), unchecked((int) 0x84db26a9), unchecked((int) 0xe0b56714), unchecked((int) 0x21f043b7), unchecked((int) 0xe5d05860),
            unchecked((int) 0x54f03084), unchecked((int) 0x066ff472), unchecked((int) 0xa31aa153), unchecked((int) 0xdadc4755), unchecked((int) 0xb5625dbf), unchecked((int) 0x68561be6), unchecked((int) 0x83ca6b94), unchecked((int) 0x2d6ed23b),
            unchecked((int) 0xeccf01db), unchecked((int) 0xa6d3d0ba), unchecked((int) 0xb6803d5c), unchecked((int) 0xaf77a709), unchecked((int) 0x33b4a34c), unchecked((int) 0x397bc8d6), unchecked((int) 0x5ee22b95), unchecked((int) 0x5f0e5304),
            unchecked((int) 0x81ed6f61), unchecked((int) 0x20e74364), unchecked((int) 0xb45e1378), unchecked((int) 0xde18639b), unchecked((int) 0x881ca122), unchecked((int) 0xb96726d1), unchecked((int) 0x8049a7e8), unchecked((int) 0x22b7da7b),
            unchecked((int) 0x5e552d25), unchecked((int) 0x5272d237), unchecked((int) 0x79d2951c), unchecked((int) 0xc60d894c), unchecked((int) 0x488cb402), unchecked((int) 0x1ba4fe5b), unchecked((int) 0xa4b09f6b), unchecked((int) 0x1ca815cf),
            unchecked((int) 0xa20c3005), unchecked((int) 0x8871df63), unchecked((int) 0xb9de2fcb), unchecked((int) 0x0cc6c9e9), unchecked((int) 0x0beeff53), unchecked((int) 0xe3214517), unchecked((int) 0xb4542835), unchecked((int) 0x9f63293c),
            unchecked((int) 0xee41e729), unchecked((int) 0x6e1d2d7c), unchecked((int) 0x50045286), unchecked((int) 0x1e6685f3), unchecked((int) 0xf33401c6), unchecked((int) 0x30a22c95), unchecked((int) 0x31a70850), unchecked((int) 0x60930f13),
            unchecked((int) 0x73f98417), unchecked((int) 0xa1269859), unchecked((int) 0xec645c44), unchecked((int) 0x52c877a9), unchecked((int) 0xcdff33a6), unchecked((int) 0xa02b1741), unchecked((int) 0x7cbad9a2), unchecked((int) 0x2180036f),
            unchecked((int) 0x50d99c08), unchecked((int) 0xcb3f4861), unchecked((int) 0xc26bd765), unchecked((int) 0x64a3f6ab), unchecked((int) 0x80342676), unchecked((int) 0x25a75e7b), unchecked((int) 0xe4e6d1fc), unchecked((int) 0x20c710e6),
            unchecked((int) 0xcdf0b680), unchecked((int) 0x17844d3b), unchecked((int) 0x31eef84d), unchecked((int) 0x7e0824e4), unchecked((int) 0x2ccb49eb), unchecked((int) 0x846a3bae), unchecked((int) 0x8ff77888), unchecked((int) 0xee5d60f6),
            unchecked((int) 0x7af75673), unchecked((int) 0x2fdd5cdb), unchecked((int) 0xa11631c1), unchecked((int) 0x30f66f43), unchecked((int) 0xb3faec54), unchecked((int) 0x157fd7fa), unchecked((int) 0xef8579cc), unchecked((int) 0xd152de58),
            unchecked((int) 0xdb2ffd5e), unchecked((int) 0x8f32ce19), unchecked((int) 0x306af97a), unchecked((int) 0x02f03ef8), unchecked((int) 0x99319ad5), unchecked((int) 0xc242fa0f), unchecked((int) 0xa7e3ebb0), unchecked((int) 0xc68e4906),
            unchecked((int) 0xb8da230c), unchecked((int) 0x80823028), unchecked((int) 0xdcdef3c8), unchecked((int) 0xd35fb171), unchecked((int) 0x088a1bc8), unchecked((int) 0xbec0c560), unchecked((int) 0x61a3c9e8), unchecked((int) 0xbca8f54d),
            unchecked((int) 0xc72feffa), unchecked((int) 0x22822e99), unchecked((int) 0x82c570b4), unchecked((int) 0xd8d94e89), unchecked((int) 0x8b1c34bc), unchecked((int) 0x301e16e6), unchecked((int) 0x273be979), unchecked((int) 0xb0ffeaa6),
            unchecked((int) 0x61d9b8c6), unchecked((int) 0x00b24869), unchecked((int) 0xb7ffce3f), unchecked((int) 0x08dc283b), unchecked((int) 0x43daf65a), unchecked((int) 0xf7e19798), unchecked((int) 0x7619b72f), unchecked((int) 0x8f1c9ba4),
            unchecked((int) 0xdc8637a0), unchecked((int) 0x16a7d3b1), unchecked((int) 0x9fc393b7), unchecked((int) 0xa7136eeb), unchecked((int) 0xc6bcc63e), unchecked((int) 0x1a513742), unchecked((int) 0xef6828bc), unchecked((int) 0x520365d6),
            unchecked((int) 0x2d6a77ab), unchecked((int) 0x3527ed4b), unchecked((int) 0x821fd216), unchecked((int) 0x095c6e2e), unchecked((int) 0xdb92f2fb), unchecked((int) 0x5eea29cb), unchecked((int) 0x145892f5), unchecked((int) 0x91584f7f),
            unchecked((int) 0x5483697b), unchecked((int) 0x2667a8cc), unchecked((int) 0x85196048), unchecked((int) 0x8c4bacea), unchecked((int) 0x833860d4), unchecked((int) 0x0d23e0f9), unchecked((int) 0x6c387e8a), unchecked((int) 0x0ae6d249),
            unchecked((int) 0xb284600c), unchecked((int) 0xd835731d), unchecked((int) 0xdcb1c647), unchecked((int) 0xac4c56ea), unchecked((int) 0x3ebd81b3), unchecked((int) 0x230eabb0), unchecked((int) 0x6438bc87), unchecked((int) 0xf0b5b1fa),
            unchecked((int) 0x8f5ea2b3), unchecked((int) 0xfc184642), unchecked((int) 0x0a036b7a), unchecked((int) 0x4fb089bd), unchecked((int) 0x649da589), unchecked((int) 0xa345415e), unchecked((int) 0x5c038323), unchecked((int) 0x3e5d3bb9),
            unchecked((int) 0x43d79572), unchecked((int) 0x7e6dd07c), unchecked((int) 0x06dfdf1e), unchecked((int) 0x6c6cc4ef), unchecked((int) 0x7160a539), unchecked((int) 0x73bfbe70), unchecked((int) 0x83877605), unchecked((int) 0x4523ecf1)
        },
        S3 =
    {
        unchecked((int) 0x8defc240), unchecked((int) 0x25fa5d9f), unchecked((int) 0xeb903dbf), unchecked((int) 0xe810c907), unchecked((int) 0x47607fff), unchecked((int) 0x369fe44b), unchecked((int) 0x8c1fc644), unchecked((int) 0xaececa90),
        unchecked((int) 0xbeb1f9bf), unchecked((int) 0xeefbcaea), unchecked((int) 0xe8cf1950), unchecked((int) 0x51df07ae), unchecked((int) 0x920e8806), unchecked((int) 0xf0ad0548), unchecked((int) 0xe13c8d83), unchecked((int) 0x927010d5),
        unchecked((int) 0x11107d9f), unchecked((int) 0x07647db9), unchecked((int) 0xb2e3e4d4), unchecked((int) 0x3d4f285e), unchecked((int) 0xb9afa820), unchecked((int) 0xfade82e0), unchecked((int) 0xa067268b), unchecked((int) 0x8272792e),
        unchecked((int) 0x553fb2c0), unchecked((int) 0x489ae22b), unchecked((int) 0xd4ef9794), unchecked((int) 0x125e3fbc), unchecked((int) 0x21fffcee), unchecked((int) 0x825b1bfd), unchecked((int) 0x9255c5ed), unchecked((int) 0x1257a240),
        unchecked((int) 0x4e1a8302), unchecked((int) 0xbae07fff), unchecked((int) 0x528246e7), unchecked((int) 0x8e57140e), unchecked((int) 0x3373f7bf), unchecked((int) 0x8c9f8188), unchecked((int) 0xa6fc4ee8), unchecked((int) 0xc982b5a5),
        unchecked((int) 0xa8c01db7), unchecked((int) 0x579fc264), unchecked((int) 0x67094f31), unchecked((int) 0xf2bd3f5f), unchecked((int) 0x40fff7c1), unchecked((int) 0x1fb78dfc), unchecked((int) 0x8e6bd2c1), unchecked((int) 0x437be59b),
        unchecked((int) 0x99b03dbf), unchecked((int) 0xb5dbc64b), unchecked((int) 0x638dc0e6), unchecked((int) 0x55819d99), unchecked((int) 0xa197c81c), unchecked((int) 0x4a012d6e), unchecked((int) 0xc5884a28), unchecked((int) 0xccc36f71),
        unchecked((int) 0xb843c213), unchecked((int) 0x6c0743f1), unchecked((int) 0x8309893c), unchecked((int) 0x0feddd5f), unchecked((int) 0x2f7fe850), unchecked((int) 0xd7c07f7e), unchecked((int) 0x02507fbf), unchecked((int) 0x5afb9a04),
        unchecked((int) 0xa747d2d0), unchecked((int) 0x1651192e), unchecked((int) 0xaf70bf3e), unchecked((int) 0x58c31380), unchecked((int) 0x5f98302e), unchecked((int) 0x727cc3c4), unchecked((int) 0x0a0fb402), unchecked((int) 0x0f7fef82),
        unchecked((int) 0x8c96fdad), unchecked((int) 0x5d2c2aae), unchecked((int) 0x8ee99a49), unchecked((int) 0x50da88b8), unchecked((int) 0x8427f4a0), unchecked((int) 0x1eac5790), unchecked((int) 0x796fb449), unchecked((int) 0x8252dc15),
        unchecked((int) 0xefbd7d9b), unchecked((int) 0xa672597d), unchecked((int) 0xada840d8), unchecked((int) 0x45f54504), unchecked((int) 0xfa5d7403), unchecked((int) 0xe83ec305), unchecked((int) 0x4f91751a), unchecked((int) 0x925669c2),
        unchecked((int) 0x23efe941), unchecked((int) 0xa903f12e), unchecked((int) 0x60270df2), unchecked((int) 0x0276e4b6), unchecked((int) 0x94fd6574), unchecked((int) 0x927985b2), unchecked((int) 0x8276dbcb), unchecked((int) 0x02778176),
        unchecked((int) 0xf8af918d), unchecked((int) 0x4e48f79e), unchecked((int) 0x8f616ddf), unchecked((int) 0xe29d840e), unchecked((int) 0x842f7d83), unchecked((int) 0x340ce5c8), unchecked((int) 0x96bbb682), unchecked((int) 0x93b4b148),
        unchecked((int) 0xef303cab), unchecked((int) 0x984faf28), unchecked((int) 0x779faf9b), unchecked((int) 0x92dc560d), unchecked((int) 0x224d1e20), unchecked((int) 0x8437aa88), unchecked((int) 0x7d29dc96), unchecked((int) 0x2756d3dc),
        unchecked((int) 0x8b907cee), unchecked((int) 0xb51fd240), unchecked((int) 0xe7c07ce3), unchecked((int) 0xe566b4a1), unchecked((int) 0xc3e9615e), unchecked((int) 0x3cf8209d), unchecked((int) 0x6094d1e3), unchecked((int) 0xcd9ca341),
        unchecked((int) 0x5c76460e), unchecked((int) 0x00ea983b), unchecked((int) 0xd4d67881), unchecked((int) 0xfd47572c), unchecked((int) 0xf76cedd9), unchecked((int) 0xbda8229c), unchecked((int) 0x127dadaa), unchecked((int) 0x438a074e),
        unchecked((int) 0x1f97c090), unchecked((int) 0x081bdb8a), unchecked((int) 0x93a07ebe), unchecked((int) 0xb938ca15), unchecked((int) 0x97b03cff), unchecked((int) 0x3dc2c0f8), unchecked((int) 0x8d1ab2ec), unchecked((int) 0x64380e51),
        unchecked((int) 0x68cc7bfb), unchecked((int) 0xd90f2788), unchecked((int) 0x12490181), unchecked((int) 0x5de5ffd4), unchecked((int) 0xdd7ef86a), unchecked((int) 0x76a2e214), unchecked((int) 0xb9a40368), unchecked((int) 0x925d958f),
        unchecked((int) 0x4b39fffa), unchecked((int) 0xba39aee9), unchecked((int) 0xa4ffd30b), unchecked((int) 0xfaf7933b), unchecked((int) 0x6d498623), unchecked((int) 0x193cbcfa), unchecked((int) 0x27627545), unchecked((int) 0x825cf47a),
        unchecked((int) 0x61bd8ba0), unchecked((int) 0xd11e42d1), unchecked((int) 0xcead04f4), unchecked((int) 0x127ea392), unchecked((int) 0x10428db7), unchecked((int) 0x8272a972), unchecked((int) 0x9270c4a8), unchecked((int) 0x127de50b),
        unchecked((int) 0x285ba1c8), unchecked((int) 0x3c62f44f), unchecked((int) 0x35c0eaa5), unchecked((int) 0xe805d231), unchecked((int) 0x428929fb), unchecked((int) 0xb4fcdf82), unchecked((int) 0x4fb66a53), unchecked((int) 0x0e7dc15b),
        unchecked((int) 0x1f081fab), unchecked((int) 0x108618ae), unchecked((int) 0xfcfd086d), unchecked((int) 0xf9ff2889), unchecked((int) 0x694bcc11), unchecked((int) 0x236a5cae), unchecked((int) 0x12deca4d), unchecked((int) 0x2c3f8cc5),
        unchecked((int) 0xd2d02dfe), unchecked((int) 0xf8ef5896), unchecked((int) 0xe4cf52da), unchecked((int) 0x95155b67), unchecked((int) 0x494a488c), unchecked((int) 0xb9b6a80c), unchecked((int) 0x5c8f82bc), unchecked((int) 0x89d36b45),
        unchecked((int) 0x3a609437), unchecked((int) 0xec00c9a9), unchecked((int) 0x44715253), unchecked((int) 0x0a874b49), unchecked((int) 0xd773bc40), unchecked((int) 0x7c34671c), unchecked((int) 0x02717ef6), unchecked((int) 0x4feb5536),
        unchecked((int) 0xa2d02fff), unchecked((int) 0xd2bf60c4), unchecked((int) 0xd43f03c0), unchecked((int) 0x50b4ef6d), unchecked((int) 0x07478cd1), unchecked((int) 0x006e1888), unchecked((int) 0xa2e53f55), unchecked((int) 0xb9e6d4bc),
        unchecked((int) 0xa2048016), unchecked((int) 0x97573833), unchecked((int) 0xd7207d67), unchecked((int) 0xde0f8f3d), unchecked((int) 0x72f87b33), unchecked((int) 0xabcc4f33), unchecked((int) 0x7688c55d), unchecked((int) 0x7b00a6b0),
        unchecked((int) 0x947b0001), unchecked((int) 0x570075d2), unchecked((int) 0xf9bb88f8), unchecked((int) 0x8942019e), unchecked((int) 0x4264a5ff), unchecked((int) 0x856302e0), unchecked((int) 0x72dbd92b), unchecked((int) 0xee971b69),
        unchecked((int) 0x6ea22fde), unchecked((int) 0x5f08ae2b), unchecked((int) 0xaf7a616d), unchecked((int) 0xe5c98767), unchecked((int) 0xcf1febd2), unchecked((int) 0x61efc8c2), unchecked((int) 0xf1ac2571), unchecked((int) 0xcc8239c2),
        unchecked((int) 0x67214cb8), unchecked((int) 0xb1e583d1), unchecked((int) 0xb7dc3e62), unchecked((int) 0x7f10bdce), unchecked((int) 0xf90a5c38), unchecked((int) 0x0ff0443d), unchecked((int) 0x606e6dc6), unchecked((int) 0x60543a49),
        unchecked((int) 0x5727c148), unchecked((int) 0x2be98a1d), unchecked((int) 0x8ab41738), unchecked((int) 0x20e1be24), unchecked((int) 0xaf96da0f), unchecked((int) 0x68458425), unchecked((int) 0x99833be5), unchecked((int) 0x600d457d),
        unchecked((int) 0x282f9350), unchecked((int) 0x8334b362), unchecked((int) 0xd91d1120), unchecked((int) 0x2b6d8da0), unchecked((int) 0x642b1e31), unchecked((int) 0x9c305a00), unchecked((int) 0x52bce688), unchecked((int) 0x1b03588a),
        unchecked((int) 0xf7baefd5), unchecked((int) 0x4142ed9c), unchecked((int) 0xa4315c11), unchecked((int) 0x83323ec5), unchecked((int) 0xdfef4636), unchecked((int) 0xa133c501), unchecked((int) 0xe9d3531c), unchecked((int) 0xee353783)
    },
        S4 =
    {
        unchecked((int) 0x9db30420), unchecked((int) 0x1fb6e9de), unchecked((int) 0xa7be7bef), unchecked((int) 0xd273a298), unchecked((int) 0x4a4f7bdb), unchecked((int) 0x64ad8c57), unchecked((int) 0x85510443), unchecked((int) 0xfa020ed1),
        unchecked((int) 0x7e287aff), unchecked((int) 0xe60fb663), unchecked((int) 0x095f35a1), unchecked((int) 0x79ebf120), unchecked((int) 0xfd059d43), unchecked((int) 0x6497b7b1), unchecked((int) 0xf3641f63), unchecked((int) 0x241e4adf),
        unchecked((int) 0x28147f5f), unchecked((int) 0x4fa2b8cd), unchecked((int) 0xc9430040), unchecked((int) 0x0cc32220), unchecked((int) 0xfdd30b30), unchecked((int) 0xc0a5374f), unchecked((int) 0x1d2d00d9), unchecked((int) 0x24147b15),
        unchecked((int) 0xee4d111a), unchecked((int) 0x0fca5167), unchecked((int) 0x71ff904c), unchecked((int) 0x2d195ffe), unchecked((int) 0x1a05645f), unchecked((int) 0x0c13fefe), unchecked((int) 0x081b08ca), unchecked((int) 0x05170121),
        unchecked((int) 0x80530100), unchecked((int) 0xe83e5efe), unchecked((int) 0xac9af4f8), unchecked((int) 0x7fe72701), unchecked((int) 0xd2b8ee5f), unchecked((int) 0x06df4261), unchecked((int) 0xbb9e9b8a), unchecked((int) 0x7293ea25),
        unchecked((int) 0xce84ffdf), unchecked((int) 0xf5718801), unchecked((int) 0x3dd64b04), unchecked((int) 0xa26f263b), unchecked((int) 0x7ed48400), unchecked((int) 0x547eebe6), unchecked((int) 0x446d4ca0), unchecked((int) 0x6cf3d6f5),
        unchecked((int) 0x2649abdf), unchecked((int) 0xaea0c7f5), unchecked((int) 0x36338cc1), unchecked((int) 0x503f7e93), unchecked((int) 0xd3772061), unchecked((int) 0x11b638e1), unchecked((int) 0x72500e03), unchecked((int) 0xf80eb2bb),
        unchecked((int) 0xabe0502e), unchecked((int) 0xec8d77de), unchecked((int) 0x57971e81), unchecked((int) 0xe14f6746), unchecked((int) 0xc9335400), unchecked((int) 0x6920318f), unchecked((int) 0x081dbb99), unchecked((int) 0xffc304a5),
        unchecked((int) 0x4d351805), unchecked((int) 0x7f3d5ce3), unchecked((int) 0xa6c866c6), unchecked((int) 0x5d5bcca9), unchecked((int) 0xdaec6fea), unchecked((int) 0x9f926f91), unchecked((int) 0x9f46222f), unchecked((int) 0x3991467d),
        unchecked((int) 0xa5bf6d8e), unchecked((int) 0x1143c44f), unchecked((int) 0x43958302), unchecked((int) 0xd0214eeb), unchecked((int) 0x022083b8), unchecked((int) 0x3fb6180c), unchecked((int) 0x18f8931e), unchecked((int) 0x281658e6),
        unchecked((int) 0x26486e3e), unchecked((int) 0x8bd78a70), unchecked((int) 0x7477e4c1), unchecked((int) 0xb506e07c), unchecked((int) 0xf32d0a25), unchecked((int) 0x79098b02), unchecked((int) 0xe4eabb81), unchecked((int) 0x28123b23),
        unchecked((int) 0x69dead38), unchecked((int) 0x1574ca16), unchecked((int) 0xdf871b62), unchecked((int) 0x211c40b7), unchecked((int) 0xa51a9ef9), unchecked((int) 0x0014377b), unchecked((int) 0x041e8ac8), unchecked((int) 0x09114003),
        unchecked((int) 0xbd59e4d2), unchecked((int) 0xe3d156d5), unchecked((int) 0x4fe876d5), unchecked((int) 0x2f91a340), unchecked((int) 0x557be8de), unchecked((int) 0x00eae4a7), unchecked((int) 0x0ce5c2ec), unchecked((int) 0x4db4bba6),
        unchecked((int) 0xe756bdff), unchecked((int) 0xdd3369ac), unchecked((int) 0xec17b035), unchecked((int) 0x06572327), unchecked((int) 0x99afc8b0), unchecked((int) 0x56c8c391), unchecked((int) 0x6b65811c), unchecked((int) 0x5e146119),
        unchecked((int) 0x6e85cb75), unchecked((int) 0xbe07c002), unchecked((int) 0xc2325577), unchecked((int) 0x893ff4ec), unchecked((int) 0x5bbfc92d), unchecked((int) 0xd0ec3b25), unchecked((int) 0xb7801ab7), unchecked((int) 0x8d6d3b24),
        unchecked((int) 0x20c763ef), unchecked((int) 0xc366a5fc), unchecked((int) 0x9c382880), unchecked((int) 0x0ace3205), unchecked((int) 0xaac9548a), unchecked((int) 0xeca1d7c7), unchecked((int) 0x041afa32), unchecked((int) 0x1d16625a),
        unchecked((int) 0x6701902c), unchecked((int) 0x9b757a54), unchecked((int) 0x31d477f7), unchecked((int) 0x9126b031), unchecked((int) 0x36cc6fdb), unchecked((int) 0xc70b8b46), unchecked((int) 0xd9e66a48), unchecked((int) 0x56e55a79),
        unchecked((int) 0x026a4ceb), unchecked((int) 0x52437eff), unchecked((int) 0x2f8f76b4), unchecked((int) 0x0df980a5), unchecked((int) 0x8674cde3), unchecked((int) 0xedda04eb), unchecked((int) 0x17a9be04), unchecked((int) 0x2c18f4df),
        unchecked((int) 0xb7747f9d), unchecked((int) 0xab2af7b4), unchecked((int) 0xefc34d20), unchecked((int) 0x2e096b7c), unchecked((int) 0x1741a254), unchecked((int) 0xe5b6a035), unchecked((int) 0x213d42f6), unchecked((int) 0x2c1c7c26),
        unchecked((int) 0x61c2f50f), unchecked((int) 0x6552daf9), unchecked((int) 0xd2c231f8), unchecked((int) 0x25130f69), unchecked((int) 0xd8167fa2), unchecked((int) 0x0418f2c8), unchecked((int) 0x001a96a6), unchecked((int) 0x0d1526ab),
        unchecked((int) 0x63315c21), unchecked((int) 0x5e0a72ec), unchecked((int) 0x49bafefd), unchecked((int) 0x187908d9), unchecked((int) 0x8d0dbd86), unchecked((int) 0x311170a7), unchecked((int) 0x3e9b640c), unchecked((int) 0xcc3e10d7),
        unchecked((int) 0xd5cad3b6), unchecked((int) 0x0caec388), unchecked((int) 0xf73001e1), unchecked((int) 0x6c728aff), unchecked((int) 0x71eae2a1), unchecked((int) 0x1f9af36e), unchecked((int) 0xcfcbd12f), unchecked((int) 0xc1de8417),
        unchecked((int) 0xac07be6b), unchecked((int) 0xcb44a1d8), unchecked((int) 0x8b9b0f56), unchecked((int) 0x013988c3), unchecked((int) 0xb1c52fca), unchecked((int) 0xb4be31cd), unchecked((int) 0xd8782806), unchecked((int) 0x12a3a4e2),
        unchecked((int) 0x6f7de532), unchecked((int) 0x58fd7eb6), unchecked((int) 0xd01ee900), unchecked((int) 0x24adffc2), unchecked((int) 0xf4990fc5), unchecked((int) 0x9711aac5), unchecked((int) 0x001d7b95), unchecked((int) 0x82e5e7d2),
        unchecked((int) 0x109873f6), unchecked((int) 0x00613096), unchecked((int) 0xc32d9521), unchecked((int) 0xada121ff), unchecked((int) 0x29908415), unchecked((int) 0x7fbb977f), unchecked((int) 0xaf9eb3db), unchecked((int) 0x29c9ed2a),
        unchecked((int) 0x5ce2a465), unchecked((int) 0xa730f32c), unchecked((int) 0xd0aa3fe8), unchecked((int) 0x8a5cc091), unchecked((int) 0xd49e2ce7), unchecked((int) 0x0ce454a9), unchecked((int) 0xd60acd86), unchecked((int) 0x015f1919),
        unchecked((int) 0x77079103), unchecked((int) 0xdea03af6), unchecked((int) 0x78a8565e), unchecked((int) 0xdee356df), unchecked((int) 0x21f05cbe), unchecked((int) 0x8b75e387), unchecked((int) 0xb3c50651), unchecked((int) 0xb8a5c3ef),
        unchecked((int) 0xd8eeb6d2), unchecked((int) 0xe523be77), unchecked((int) 0xc2154529), unchecked((int) 0x2f69efdf), unchecked((int) 0xafe67afb), unchecked((int) 0xf470c4b2), unchecked((int) 0xf3e0eb5b), unchecked((int) 0xd6cc9876),
        unchecked((int) 0x39e4460c), unchecked((int) 0x1fda8538), unchecked((int) 0x1987832f), unchecked((int) 0xca007367), unchecked((int) 0xa99144f8), unchecked((int) 0x296b299e), unchecked((int) 0x492fc295), unchecked((int) 0x9266beab),
        unchecked((int) 0xb5676e69), unchecked((int) 0x9bd3ddda), unchecked((int) 0xdf7e052f), unchecked((int) 0xdb25701c), unchecked((int) 0x1b5e51ee), unchecked((int) 0xf65324e6), unchecked((int) 0x6afce36c), unchecked((int) 0x0316cc04),
        unchecked((int) 0x8644213e), unchecked((int) 0xb7dc59d0), unchecked((int) 0x7965291f), unchecked((int) 0xccd6fd43), unchecked((int) 0x41823979), unchecked((int) 0x932bcdf6), unchecked((int) 0xb657c34d), unchecked((int) 0x4edfd282),
        unchecked((int) 0x7ae5290c), unchecked((int) 0x3cb9536b), unchecked((int) 0x851e20fe), unchecked((int) 0x9833557e), unchecked((int) 0x13ecf0b0), unchecked((int) 0xd3ffb372), unchecked((int) 0x3f85c5c1), unchecked((int) 0x0aef7ed2)
    },
        S5 =
    {
        unchecked((int) 0x7ec90c04), unchecked((int) 0x2c6e74b9), unchecked((int) 0x9b0e66df), unchecked((int) 0xa6337911), unchecked((int) 0xb86a7fff), unchecked((int) 0x1dd358f5), unchecked((int) 0x44dd9d44), unchecked((int) 0x1731167f),
        unchecked((int) 0x08fbf1fa), unchecked((int) 0xe7f511cc), unchecked((int) 0xd2051b00), unchecked((int) 0x735aba00), unchecked((int) 0x2ab722d8), unchecked((int) 0x386381cb), unchecked((int) 0xacf6243a), unchecked((int) 0x69befd7a),
        unchecked((int) 0xe6a2e77f), unchecked((int) 0xf0c720cd), unchecked((int) 0xc4494816), unchecked((int) 0xccf5c180), unchecked((int) 0x38851640), unchecked((int) 0x15b0a848), unchecked((int) 0xe68b18cb), unchecked((int) 0x4caadeff),
        unchecked((int) 0x5f480a01), unchecked((int) 0x0412b2aa), unchecked((int) 0x259814fc), unchecked((int) 0x41d0efe2), unchecked((int) 0x4e40b48d), unchecked((int) 0x248eb6fb), unchecked((int) 0x8dba1cfe), unchecked((int) 0x41a99b02),
        unchecked((int) 0x1a550a04), unchecked((int) 0xba8f65cb), unchecked((int) 0x7251f4e7), unchecked((int) 0x95a51725), unchecked((int) 0xc106ecd7), unchecked((int) 0x97a5980a), unchecked((int) 0xc539b9aa), unchecked((int) 0x4d79fe6a),
        unchecked((int) 0xf2f3f763), unchecked((int) 0x68af8040), unchecked((int) 0xed0c9e56), unchecked((int) 0x11b4958b), unchecked((int) 0xe1eb5a88), unchecked((int) 0x8709e6b0), unchecked((int) 0xd7e07156), unchecked((int) 0x4e29fea7),
        unchecked((int) 0x6366e52d), unchecked((int) 0x02d1c000), unchecked((int) 0xc4ac8e05), unchecked((int) 0x9377f571), unchecked((int) 0x0c05372a), unchecked((int) 0x578535f2), unchecked((int) 0x2261be02), unchecked((int) 0xd642a0c9),
        unchecked((int) 0xdf13a280), unchecked((int) 0x74b55bd2), unchecked((int) 0x682199c0), unchecked((int) 0xd421e5ec), unchecked((int) 0x53fb3ce8), unchecked((int) 0xc8adedb3), unchecked((int) 0x28a87fc9), unchecked((int) 0x3d959981),
        unchecked((int) 0x5c1ff900), unchecked((int) 0xfe38d399), unchecked((int) 0x0c4eff0b), unchecked((int) 0x062407ea), unchecked((int) 0xaa2f4fb1), unchecked((int) 0x4fb96976), unchecked((int) 0x90c79505), unchecked((int) 0xb0a8a774),
        unchecked((int) 0xef55a1ff), unchecked((int) 0xe59ca2c2), unchecked((int) 0xa6b62d27), unchecked((int) 0xe66a4263), unchecked((int) 0xdf65001f), unchecked((int) 0x0ec50966), unchecked((int) 0xdfdd55bc), unchecked((int) 0x29de0655),
        unchecked((int) 0x911e739a), unchecked((int) 0x17af8975), unchecked((int) 0x32c7911c), unchecked((int) 0x89f89468), unchecked((int) 0x0d01e980), unchecked((int) 0x524755f4), unchecked((int) 0x03b63cc9), unchecked((int) 0x0cc844b2),
        unchecked((int) 0xbcf3f0aa), unchecked((int) 0x87ac36e9), unchecked((int) 0xe53a7426), unchecked((int) 0x01b3d82b), unchecked((int) 0x1a9e7449), unchecked((int) 0x64ee2d7e), unchecked((int) 0xcddbb1da), unchecked((int) 0x01c94910),
        unchecked((int) 0xb868bf80), unchecked((int) 0x0d26f3fd), unchecked((int) 0x9342ede7), unchecked((int) 0x04a5c284), unchecked((int) 0x636737b6), unchecked((int) 0x50f5b616), unchecked((int) 0xf24766e3), unchecked((int) 0x8eca36c1),
        unchecked((int) 0x136e05db), unchecked((int) 0xfef18391), unchecked((int) 0xfb887a37), unchecked((int) 0xd6e7f7d4), unchecked((int) 0xc7fb7dc9), unchecked((int) 0x3063fcdf), unchecked((int) 0xb6f589de), unchecked((int) 0xec2941da),
        unchecked((int) 0x26e46695), unchecked((int) 0xb7566419), unchecked((int) 0xf654efc5), unchecked((int) 0xd08d58b7), unchecked((int) 0x48925401), unchecked((int) 0xc1bacb7f), unchecked((int) 0xe5ff550f), unchecked((int) 0xb6083049),
        unchecked((int) 0x5bb5d0e8), unchecked((int) 0x87d72e5a), unchecked((int) 0xab6a6ee1), unchecked((int) 0x223a66ce), unchecked((int) 0xc62bf3cd), unchecked((int) 0x9e0885f9), unchecked((int) 0x68cb3e47), unchecked((int) 0x086c010f),
        unchecked((int) 0xa21de820), unchecked((int) 0xd18b69de), unchecked((int) 0xf3f65777), unchecked((int) 0xfa02c3f6), unchecked((int) 0x407edac3), unchecked((int) 0xcbb3d550), unchecked((int) 0x1793084d), unchecked((int) 0xb0d70eba),
        unchecked((int) 0x0ab378d5), unchecked((int) 0xd951fb0c), unchecked((int) 0xded7da56), unchecked((int) 0x4124bbe4), unchecked((int) 0x94ca0b56), unchecked((int) 0x0f5755d1), unchecked((int) 0xe0e1e56e), unchecked((int) 0x6184b5be),
        unchecked((int) 0x580a249f), unchecked((int) 0x94f74bc0), unchecked((int) 0xe327888e), unchecked((int) 0x9f7b5561), unchecked((int) 0xc3dc0280), unchecked((int) 0x05687715), unchecked((int) 0x646c6bd7), unchecked((int) 0x44904db3),
        unchecked((int) 0x66b4f0a3), unchecked((int) 0xc0f1648a), unchecked((int) 0x697ed5af), unchecked((int) 0x49e92ff6), unchecked((int) 0x309e374f), unchecked((int) 0x2cb6356a), unchecked((int) 0x85808573), unchecked((int) 0x4991f840),
        unchecked((int) 0x76f0ae02), unchecked((int) 0x083be84d), unchecked((int) 0x28421c9a), unchecked((int) 0x44489406), unchecked((int) 0x736e4cb8), unchecked((int) 0xc1092910), unchecked((int) 0x8bc95fc6), unchecked((int) 0x7d869cf4),
        unchecked((int) 0x134f616f), unchecked((int) 0x2e77118d), unchecked((int) 0xb31b2be1), unchecked((int) 0xaa90b472), unchecked((int) 0x3ca5d717), unchecked((int) 0x7d161bba), unchecked((int) 0x9cad9010), unchecked((int) 0xaf462ba2),
        unchecked((int) 0x9fe459d2), unchecked((int) 0x45d34559), unchecked((int) 0xd9f2da13), unchecked((int) 0xdbc65487), unchecked((int) 0xf3e4f94e), unchecked((int) 0x176d486f), unchecked((int) 0x097c13ea), unchecked((int) 0x631da5c7),
        unchecked((int) 0x445f7382), unchecked((int) 0x175683f4), unchecked((int) 0xcdc66a97), unchecked((int) 0x70be0288), unchecked((int) 0xb3cdcf72), unchecked((int) 0x6e5dd2f3), unchecked((int) 0x20936079), unchecked((int) 0x459b80a5),
        unchecked((int) 0xbe60e2db), unchecked((int) 0xa9c23101), unchecked((int) 0xeba5315c), unchecked((int) 0x224e42f2), unchecked((int) 0x1c5c1572), unchecked((int) 0xf6721b2c), unchecked((int) 0x1ad2fff3), unchecked((int) 0x8c25404e),
        unchecked((int) 0x324ed72f), unchecked((int) 0x4067b7fd), unchecked((int) 0x0523138e), unchecked((int) 0x5ca3bc78), unchecked((int) 0xdc0fd66e), unchecked((int) 0x75922283), unchecked((int) 0x784d6b17), unchecked((int) 0x58ebb16e),
        unchecked((int) 0x44094f85), unchecked((int) 0x3f481d87), unchecked((int) 0xfcfeae7b), unchecked((int) 0x77b5ff76), unchecked((int) 0x8c2302bf), unchecked((int) 0xaaf47556), unchecked((int) 0x5f46b02a), unchecked((int) 0x2b092801),
        unchecked((int) 0x3d38f5f7), unchecked((int) 0x0ca81f36), unchecked((int) 0x52af4a8a), unchecked((int) 0x66d5e7c0), unchecked((int) 0xdf3b0874), unchecked((int) 0x95055110), unchecked((int) 0x1b5ad7a8), unchecked((int) 0xf61ed5ad),
        unchecked((int) 0x6cf6e479), unchecked((int) 0x20758184), unchecked((int) 0xd0cefa65), unchecked((int) 0x88f7be58), unchecked((int) 0x4a046826), unchecked((int) 0x0ff6f8f3), unchecked((int) 0xa09c7f70), unchecked((int) 0x5346aba0),
        unchecked((int) 0x5ce96c28), unchecked((int) 0xe176eda3), unchecked((int) 0x6bac307f), unchecked((int) 0x376829d2), unchecked((int) 0x85360fa9), unchecked((int) 0x17e3fe2a), unchecked((int) 0x24b79767), unchecked((int) 0xf5a96b20),
        unchecked((int) 0xd6cd2595), unchecked((int) 0x68ff1ebf), unchecked((int) 0x7555442c), unchecked((int) 0xf19f06be), unchecked((int) 0xf9e0659a), unchecked((int) 0xeeb9491d), unchecked((int) 0x34010718), unchecked((int) 0xbb30cab8),
        unchecked((int) 0xe822fe15), unchecked((int) 0x88570983), unchecked((int) 0x750e6249), unchecked((int) 0xda627e55), unchecked((int) 0x5e76ffa8), unchecked((int) 0xb1534546), unchecked((int) 0x6d47de08), unchecked((int) 0xefe9e7d4)
    },
        S6 =
    {
        unchecked((int) 0xf6fa8f9d), unchecked((int) 0x2cac6ce1), unchecked((int) 0x4ca34867), unchecked((int) 0xe2337f7c), unchecked((int) 0x95db08e7), unchecked((int) 0x016843b4), unchecked((int) 0xeced5cbc), unchecked((int) 0x325553ac),
        unchecked((int) 0xbf9f0960), unchecked((int) 0xdfa1e2ed), unchecked((int) 0x83f0579d), unchecked((int) 0x63ed86b9), unchecked((int) 0x1ab6a6b8), unchecked((int) 0xde5ebe39), unchecked((int) 0xf38ff732), unchecked((int) 0x8989b138),
        unchecked((int) 0x33f14961), unchecked((int) 0xc01937bd), unchecked((int) 0xf506c6da), unchecked((int) 0xe4625e7e), unchecked((int) 0xa308ea99), unchecked((int) 0x4e23e33c), unchecked((int) 0x79cbd7cc), unchecked((int) 0x48a14367),
        unchecked((int) 0xa3149619), unchecked((int) 0xfec94bd5), unchecked((int) 0xa114174a), unchecked((int) 0xeaa01866), unchecked((int) 0xa084db2d), unchecked((int) 0x09a8486f), unchecked((int) 0xa888614a), unchecked((int) 0x2900af98),
        unchecked((int) 0x01665991), unchecked((int) 0xe1992863), unchecked((int) 0xc8f30c60), unchecked((int) 0x2e78ef3c), unchecked((int) 0xd0d51932), unchecked((int) 0xcf0fec14), unchecked((int) 0xf7ca07d2), unchecked((int) 0xd0a82072),
        unchecked((int) 0xfd41197e), unchecked((int) 0x9305a6b0), unchecked((int) 0xe86be3da), unchecked((int) 0x74bed3cd), unchecked((int) 0x372da53c), unchecked((int) 0x4c7f4448), unchecked((int) 0xdab5d440), unchecked((int) 0x6dba0ec3),
        unchecked((int) 0x083919a7), unchecked((int) 0x9fbaeed9), unchecked((int) 0x49dbcfb0), unchecked((int) 0x4e670c53), unchecked((int) 0x5c3d9c01), unchecked((int) 0x64bdb941), unchecked((int) 0x2c0e636a), unchecked((int) 0xba7dd9cd),
        unchecked((int) 0xea6f7388), unchecked((int) 0xe70bc762), unchecked((int) 0x35f29adb), unchecked((int) 0x5c4cdd8d), unchecked((int) 0xf0d48d8c), unchecked((int) 0xb88153e2), unchecked((int) 0x08a19866), unchecked((int) 0x1ae2eac8),
        unchecked((int) 0x284caf89), unchecked((int) 0xaa928223), unchecked((int) 0x9334be53), unchecked((int) 0x3b3a21bf), unchecked((int) 0x16434be3), unchecked((int) 0x9aea3906), unchecked((int) 0xefe8c36e), unchecked((int) 0xf890cdd9),
        unchecked((int) 0x80226dae), unchecked((int) 0xc340a4a3), unchecked((int) 0xdf7e9c09), unchecked((int) 0xa694a807), unchecked((int) 0x5b7c5ecc), unchecked((int) 0x221db3a6), unchecked((int) 0x9a69a02f), unchecked((int) 0x68818a54),
        unchecked((int) 0xceb2296f), unchecked((int) 0x53c0843a), unchecked((int) 0xfe893655), unchecked((int) 0x25bfe68a), unchecked((int) 0xb4628abc), unchecked((int) 0xcf222ebf), unchecked((int) 0x25ac6f48), unchecked((int) 0xa9a99387),
        unchecked((int) 0x53bddb65), unchecked((int) 0xe76ffbe7), unchecked((int) 0xe967fd78), unchecked((int) 0x0ba93563), unchecked((int) 0x8e342bc1), unchecked((int) 0xe8a11be9), unchecked((int) 0x4980740d), unchecked((int) 0xc8087dfc),
        unchecked((int) 0x8de4bf99), unchecked((int) 0xa11101a0), unchecked((int) 0x7fd37975), unchecked((int) 0xda5a26c0), unchecked((int) 0xe81f994f), unchecked((int) 0x9528cd89), unchecked((int) 0xfd339fed), unchecked((int) 0xb87834bf),
        unchecked((int) 0x5f04456d), unchecked((int) 0x22258698), unchecked((int) 0xc9c4c83b), unchecked((int) 0x2dc156be), unchecked((int) 0x4f628daa), unchecked((int) 0x57f55ec5), unchecked((int) 0xe2220abe), unchecked((int) 0xd2916ebf),
        unchecked((int) 0x4ec75b95), unchecked((int) 0x24f2c3c0), unchecked((int) 0x42d15d99), unchecked((int) 0xcd0d7fa0), unchecked((int) 0x7b6e27ff), unchecked((int) 0xa8dc8af0), unchecked((int) 0x7345c106), unchecked((int) 0xf41e232f),
        unchecked((int) 0x35162386), unchecked((int) 0xe6ea8926), unchecked((int) 0x3333b094), unchecked((int) 0x157ec6f2), unchecked((int) 0x372b74af), unchecked((int) 0x692573e4), unchecked((int) 0xe9a9d848), unchecked((int) 0xf3160289),
        unchecked((int) 0x3a62ef1d), unchecked((int) 0xa787e238), unchecked((int) 0xf3a5f676), unchecked((int) 0x74364853), unchecked((int) 0x20951063), unchecked((int) 0x4576698d), unchecked((int) 0xb6fad407), unchecked((int) 0x592af950),
        unchecked((int) 0x36f73523), unchecked((int) 0x4cfb6e87), unchecked((int) 0x7da4cec0), unchecked((int) 0x6c152daa), unchecked((int) 0xcb0396a8), unchecked((int) 0xc50dfe5d), unchecked((int) 0xfcd707ab), unchecked((int) 0x0921c42f),
        unchecked((int) 0x89dff0bb), unchecked((int) 0x5fe2be78), unchecked((int) 0x448f4f33), unchecked((int) 0x754613c9), unchecked((int) 0x2b05d08d), unchecked((int) 0x48b9d585), unchecked((int) 0xdc049441), unchecked((int) 0xc8098f9b),
        unchecked((int) 0x7dede786), unchecked((int) 0xc39a3373), unchecked((int) 0x42410005), unchecked((int) 0x6a091751), unchecked((int) 0x0ef3c8a6), unchecked((int) 0x890072d6), unchecked((int) 0x28207682), unchecked((int) 0xa9a9f7be),
        unchecked((int) 0xbf32679d), unchecked((int) 0xd45b5b75), unchecked((int) 0xb353fd00), unchecked((int) 0xcbb0e358), unchecked((int) 0x830f220a), unchecked((int) 0x1f8fb214), unchecked((int) 0xd372cf08), unchecked((int) 0xcc3c4a13),
        unchecked((int) 0x8cf63166), unchecked((int) 0x061c87be), unchecked((int) 0x88c98f88), unchecked((int) 0x6062e397), unchecked((int) 0x47cf8e7a), unchecked((int) 0xb6c85283), unchecked((int) 0x3cc2acfb), unchecked((int) 0x3fc06976),
        unchecked((int) 0x4e8f0252), unchecked((int) 0x64d8314d), unchecked((int) 0xda3870e3), unchecked((int) 0x1e665459), unchecked((int) 0xc10908f0), unchecked((int) 0x513021a5), unchecked((int) 0x6c5b68b7), unchecked((int) 0x822f8aa0),
        unchecked((int) 0x3007cd3e), unchecked((int) 0x74719eef), unchecked((int) 0xdc872681), unchecked((int) 0x073340d4), unchecked((int) 0x7e432fd9), unchecked((int) 0x0c5ec241), unchecked((int) 0x8809286c), unchecked((int) 0xf592d891),
        unchecked((int) 0x08a930f6), unchecked((int) 0x957ef305), unchecked((int) 0xb7fbffbd), unchecked((int) 0xc266e96f), unchecked((int) 0x6fe4ac98), unchecked((int) 0xb173ecc0), unchecked((int) 0xbc60b42a), unchecked((int) 0x953498da),
        unchecked((int) 0xfba1ae12), unchecked((int) 0x2d4bd736), unchecked((int) 0x0f25faab), unchecked((int) 0xa4f3fceb), unchecked((int) 0xe2969123), unchecked((int) 0x257f0c3d), unchecked((int) 0x9348af49), unchecked((int) 0x361400bc),
        unchecked((int) 0xe8816f4a), unchecked((int) 0x3814f200), unchecked((int) 0xa3f94043), unchecked((int) 0x9c7a54c2), unchecked((int) 0xbc704f57), unchecked((int) 0xda41e7f9), unchecked((int) 0xc25ad33a), unchecked((int) 0x54f4a084),
        unchecked((int) 0xb17f5505), unchecked((int) 0x59357cbe), unchecked((int) 0xedbd15c8), unchecked((int) 0x7f97c5ab), unchecked((int) 0xba5ac7b5), unchecked((int) 0xb6f6deaf), unchecked((int) 0x3a479c3a), unchecked((int) 0x5302da25),
        unchecked((int) 0x653d7e6a), unchecked((int) 0x54268d49), unchecked((int) 0x51a477ea), unchecked((int) 0x5017d55b), unchecked((int) 0xd7d25d88), unchecked((int) 0x44136c76), unchecked((int) 0x0404a8c8), unchecked((int) 0xb8e5a121),
        unchecked((int) 0xb81a928a), unchecked((int) 0x60ed5869), unchecked((int) 0x97c55b96), unchecked((int) 0xeaec991b), unchecked((int) 0x29935913), unchecked((int) 0x01fdb7f1), unchecked((int) 0x088e8dfa), unchecked((int) 0x9ab6f6f5),
        unchecked((int) 0x3b4cbf9f), unchecked((int) 0x4a5de3ab), unchecked((int) 0xe6051d35), unchecked((int) 0xa0e1d855), unchecked((int) 0xd36b4cf1), unchecked((int) 0xf544edeb), unchecked((int) 0xb0e93524), unchecked((int) 0xbebb8fbd),
        unchecked((int) 0xa2d762cf), unchecked((int) 0x49c92f54), unchecked((int) 0x38b5f331), unchecked((int) 0x7128a454), unchecked((int) 0x48392905), unchecked((int) 0xa65b1db8), unchecked((int) 0x851c97bd), unchecked((int) 0xd675cf2f)
    },
        S7 =
    {
        unchecked((int) 0x85e04019), unchecked((int) 0x332bf567), unchecked((int) 0x662dbfff), unchecked((int) 0xcfc65693), unchecked((int) 0x2a8d7f6f), unchecked((int) 0xab9bc912), unchecked((int) 0xde6008a1), unchecked((int) 0x2028da1f),
        unchecked((int) 0x0227bce7), unchecked((int) 0x4d642916), unchecked((int) 0x18fac300), unchecked((int) 0x50f18b82), unchecked((int) 0x2cb2cb11), unchecked((int) 0xb232e75c), unchecked((int) 0x4b3695f2), unchecked((int) 0xb28707de),
        unchecked((int) 0xa05fbcf6), unchecked((int) 0xcd4181e9), unchecked((int) 0xe150210c), unchecked((int) 0xe24ef1bd), unchecked((int) 0xb168c381), unchecked((int) 0xfde4e789), unchecked((int) 0x5c79b0d8), unchecked((int) 0x1e8bfd43),
        unchecked((int) 0x4d495001), unchecked((int) 0x38be4341), unchecked((int) 0x913cee1d), unchecked((int) 0x92a79c3f), unchecked((int) 0x089766be), unchecked((int) 0xbaeeadf4), unchecked((int) 0x1286becf), unchecked((int) 0xb6eacb19),
        unchecked((int) 0x2660c200), unchecked((int) 0x7565bde4), unchecked((int) 0x64241f7a), unchecked((int) 0x8248dca9), unchecked((int) 0xc3b3ad66), unchecked((int) 0x28136086), unchecked((int) 0x0bd8dfa8), unchecked((int) 0x356d1cf2),
        unchecked((int) 0x107789be), unchecked((int) 0xb3b2e9ce), unchecked((int) 0x0502aa8f), unchecked((int) 0x0bc0351e), unchecked((int) 0x166bf52a), unchecked((int) 0xeb12ff82), unchecked((int) 0xe3486911), unchecked((int) 0xd34d7516),
        unchecked((int) 0x4e7b3aff), unchecked((int) 0x5f43671b), unchecked((int) 0x9cf6e037), unchecked((int) 0x4981ac83), unchecked((int) 0x334266ce), unchecked((int) 0x8c9341b7), unchecked((int) 0xd0d854c0), unchecked((int) 0xcb3a6c88),
        unchecked((int) 0x47bc2829), unchecked((int) 0x4725ba37), unchecked((int) 0xa66ad22b), unchecked((int) 0x7ad61f1e), unchecked((int) 0x0c5cbafa), unchecked((int) 0x4437f107), unchecked((int) 0xb6e79962), unchecked((int) 0x42d2d816),
        unchecked((int) 0x0a961288), unchecked((int) 0xe1a5c06e), unchecked((int) 0x13749e67), unchecked((int) 0x72fc081a), unchecked((int) 0xb1d139f7), unchecked((int) 0xf9583745), unchecked((int) 0xcf19df58), unchecked((int) 0xbec3f756),
        unchecked((int) 0xc06eba30), unchecked((int) 0x07211b24), unchecked((int) 0x45c28829), unchecked((int) 0xc95e317f), unchecked((int) 0xbc8ec511), unchecked((int) 0x38bc46e9), unchecked((int) 0xc6e6fa14), unchecked((int) 0xbae8584a),
        unchecked((int) 0xad4ebc46), unchecked((int) 0x468f508b), unchecked((int) 0x7829435f), unchecked((int) 0xf124183b), unchecked((int) 0x821dba9f), unchecked((int) 0xaff60ff4), unchecked((int) 0xea2c4e6d), unchecked((int) 0x16e39264),
        unchecked((int) 0x92544a8b), unchecked((int) 0x009b4fc3), unchecked((int) 0xaba68ced), unchecked((int) 0x9ac96f78), unchecked((int) 0x06a5b79a), unchecked((int) 0xb2856e6e), unchecked((int) 0x1aec3ca9), unchecked((int) 0xbe838688),
        unchecked((int) 0x0e0804e9), unchecked((int) 0x55f1be56), unchecked((int) 0xe7e5363b), unchecked((int) 0xb3a1f25d), unchecked((int) 0xf7debb85), unchecked((int) 0x61fe033c), unchecked((int) 0x16746233), unchecked((int) 0x3c034c28),
        unchecked((int) 0xda6d0c74), unchecked((int) 0x79aac56c), unchecked((int) 0x3ce4e1ad), unchecked((int) 0x51f0c802), unchecked((int) 0x98f8f35a), unchecked((int) 0x1626a49f), unchecked((int) 0xeed82b29), unchecked((int) 0x1d382fe3),
        unchecked((int) 0x0c4fb99a), unchecked((int) 0xbb325778), unchecked((int) 0x3ec6d97b), unchecked((int) 0x6e77a6a9), unchecked((int) 0xcb658b5c), unchecked((int) 0xd45230c7), unchecked((int) 0x2bd1408b), unchecked((int) 0x60c03eb7),
        unchecked((int) 0xb9068d78), unchecked((int) 0xa33754f4), unchecked((int) 0xf430c87d), unchecked((int) 0xc8a71302), unchecked((int) 0xb96d8c32), unchecked((int) 0xebd4e7be), unchecked((int) 0xbe8b9d2d), unchecked((int) 0x7979fb06),
        unchecked((int) 0xe7225308), unchecked((int) 0x8b75cf77), unchecked((int) 0x11ef8da4), unchecked((int) 0xe083c858), unchecked((int) 0x8d6b786f), unchecked((int) 0x5a6317a6), unchecked((int) 0xfa5cf7a0), unchecked((int) 0x5dda0033),
        unchecked((int) 0xf28ebfb0), unchecked((int) 0xf5b9c310), unchecked((int) 0xa0eac280), unchecked((int) 0x08b9767a), unchecked((int) 0xa3d9d2b0), unchecked((int) 0x79d34217), unchecked((int) 0x021a718d), unchecked((int) 0x9ac6336a),
        unchecked((int) 0x2711fd60), unchecked((int) 0x438050e3), unchecked((int) 0x069908a8), unchecked((int) 0x3d7fedc4), unchecked((int) 0x826d2bef), unchecked((int) 0x4eeb8476), unchecked((int) 0x488dcf25), unchecked((int) 0x36c9d566),
        unchecked((int) 0x28e74e41), unchecked((int) 0xc2610aca), unchecked((int) 0x3d49a9cf), unchecked((int) 0xbae3b9df), unchecked((int) 0xb65f8de6), unchecked((int) 0x92aeaf64), unchecked((int) 0x3ac7d5e6), unchecked((int) 0x9ea80509),
        unchecked((int) 0xf22b017d), unchecked((int) 0xa4173f70), unchecked((int) 0xdd1e16c3), unchecked((int) 0x15e0d7f9), unchecked((int) 0x50b1b887), unchecked((int) 0x2b9f4fd5), unchecked((int) 0x625aba82), unchecked((int) 0x6a017962),
        unchecked((int) 0x2ec01b9c), unchecked((int) 0x15488aa9), unchecked((int) 0xd716e740), unchecked((int) 0x40055a2c), unchecked((int) 0x93d29a22), unchecked((int) 0xe32dbf9a), unchecked((int) 0x058745b9), unchecked((int) 0x3453dc1e),
        unchecked((int) 0xd699296e), unchecked((int) 0x496cff6f), unchecked((int) 0x1c9f4986), unchecked((int) 0xdfe2ed07), unchecked((int) 0xb87242d1), unchecked((int) 0x19de7eae), unchecked((int) 0x053e561a), unchecked((int) 0x15ad6f8c),
        unchecked((int) 0x66626c1c), unchecked((int) 0x7154c24c), unchecked((int) 0xea082b2a), unchecked((int) 0x93eb2939), unchecked((int) 0x17dcb0f0), unchecked((int) 0x58d4f2ae), unchecked((int) 0x9ea294fb), unchecked((int) 0x52cf564c),
        unchecked((int) 0x9883fe66), unchecked((int) 0x2ec40581), unchecked((int) 0x763953c3), unchecked((int) 0x01d6692e), unchecked((int) 0xd3a0c108), unchecked((int) 0xa1e7160e), unchecked((int) 0xe4f2dfa6), unchecked((int) 0x693ed285),
        unchecked((int) 0x74904698), unchecked((int) 0x4c2b0edd), unchecked((int) 0x4f757656), unchecked((int) 0x5d393378), unchecked((int) 0xa132234f), unchecked((int) 0x3d321c5d), unchecked((int) 0xc3f5e194), unchecked((int) 0x4b269301),
        unchecked((int) 0xc79f022f), unchecked((int) 0x3c997e7e), unchecked((int) 0x5e4f9504), unchecked((int) 0x3ffafbbd), unchecked((int) 0x76f7ad0e), unchecked((int) 0x296693f4), unchecked((int) 0x3d1fce6f), unchecked((int) 0xc61e45be),
        unchecked((int) 0xd3b5ab34), unchecked((int) 0xf72bf9b7), unchecked((int) 0x1b0434c0), unchecked((int) 0x4e72b567), unchecked((int) 0x5592a33d), unchecked((int) 0xb5229301), unchecked((int) 0xcfd2a87f), unchecked((int) 0x60aeb767),
        unchecked((int) 0x1814386b), unchecked((int) 0x30bcc33d), unchecked((int) 0x38a0c07d), unchecked((int) 0xfd1606f2), unchecked((int) 0xc363519b), unchecked((int) 0x589dd390), unchecked((int) 0x5479f8e6), unchecked((int) 0x1cb8d647),
        unchecked((int) 0x97fd61a9), unchecked((int) 0xea7759f4), unchecked((int) 0x2d57539d), unchecked((int) 0x569a58cf), unchecked((int) 0xe84e63ad), unchecked((int) 0x462e1b78), unchecked((int) 0x6580f87e), unchecked((int) 0xf3817914),
        unchecked((int) 0x91da55f4), unchecked((int) 0x40a230f3), unchecked((int) 0xd1988f35), unchecked((int) 0xb6e318d2), unchecked((int) 0x3ffa50bc), unchecked((int) 0x3d40f021), unchecked((int) 0xc3c0bdae), unchecked((int) 0x4958c24c),
        unchecked((int) 0x518f36b2), unchecked((int) 0x84b1d370), unchecked((int) 0x0fedce83), unchecked((int) 0x878ddada), unchecked((int) 0xf2a279c7), unchecked((int) 0x94e01be8), unchecked((int) 0x90716f4b), unchecked((int) 0x954b8aa3)
    },
        S8 =
    {
        unchecked((int) 0xe216300d), unchecked((int) 0xbbddfffc), unchecked((int) 0xa7ebdabd), unchecked((int) 0x35648095), unchecked((int) 0x7789f8b7), unchecked((int) 0xe6c1121b), unchecked((int) 0x0e241600), unchecked((int) 0x052ce8b5),
        unchecked((int) 0x11a9cfb0), unchecked((int) 0xe5952f11), unchecked((int) 0xece7990a), unchecked((int) 0x9386d174), unchecked((int) 0x2a42931c), unchecked((int) 0x76e38111), unchecked((int) 0xb12def3a), unchecked((int) 0x37ddddfc),
        unchecked((int) 0xde9adeb1), unchecked((int) 0x0a0cc32c), unchecked((int) 0xbe197029), unchecked((int) 0x84a00940), unchecked((int) 0xbb243a0f), unchecked((int) 0xb4d137cf), unchecked((int) 0xb44e79f0), unchecked((int) 0x049eedfd),
        unchecked((int) 0x0b15a15d), unchecked((int) 0x480d3168), unchecked((int) 0x8bbbde5a), unchecked((int) 0x669ded42), unchecked((int) 0xc7ece831), unchecked((int) 0x3f8f95e7), unchecked((int) 0x72df191b), unchecked((int) 0x7580330d),
        unchecked((int) 0x94074251), unchecked((int) 0x5c7dcdfa), unchecked((int) 0xabbe6d63), unchecked((int) 0xaa402164), unchecked((int) 0xb301d40a), unchecked((int) 0x02e7d1ca), unchecked((int) 0x53571dae), unchecked((int) 0x7a3182a2),
        unchecked((int) 0x12a8ddec), unchecked((int) 0xfdaa335d), unchecked((int) 0x176f43e8), unchecked((int) 0x71fb46d4), unchecked((int) 0x38129022), unchecked((int) 0xce949ad4), unchecked((int) 0xb84769ad), unchecked((int) 0x965bd862),
        unchecked((int) 0x82f3d055), unchecked((int) 0x66fb9767), unchecked((int) 0x15b80b4e), unchecked((int) 0x1d5b47a0), unchecked((int) 0x4cfde06f), unchecked((int) 0xc28ec4b8), unchecked((int) 0x57e8726e), unchecked((int) 0x647a78fc),
        unchecked((int) 0x99865d44), unchecked((int) 0x608bd593), unchecked((int) 0x6c200e03), unchecked((int) 0x39dc5ff6), unchecked((int) 0x5d0b00a3), unchecked((int) 0xae63aff2), unchecked((int) 0x7e8bd632), unchecked((int) 0x70108c0c),
        unchecked((int) 0xbbd35049), unchecked((int) 0x2998df04), unchecked((int) 0x980cf42a), unchecked((int) 0x9b6df491), unchecked((int) 0x9e7edd53), unchecked((int) 0x06918548), unchecked((int) 0x58cb7e07), unchecked((int) 0x3b74ef2e),
        unchecked((int) 0x522fffb1), unchecked((int) 0xd24708cc), unchecked((int) 0x1c7e27cd), unchecked((int) 0xa4eb215b), unchecked((int) 0x3cf1d2e2), unchecked((int) 0x19b47a38), unchecked((int) 0x424f7618), unchecked((int) 0x35856039),
        unchecked((int) 0x9d17dee7), unchecked((int) 0x27eb35e6), unchecked((int) 0xc9aff67b), unchecked((int) 0x36baf5b8), unchecked((int) 0x09c467cd), unchecked((int) 0xc18910b1), unchecked((int) 0xe11dbf7b), unchecked((int) 0x06cd1af8),
        unchecked((int) 0x7170c608), unchecked((int) 0x2d5e3354), unchecked((int) 0xd4de495a), unchecked((int) 0x64c6d006), unchecked((int) 0xbcc0c62c), unchecked((int) 0x3dd00db3), unchecked((int) 0x708f8f34), unchecked((int) 0x77d51b42),
        unchecked((int) 0x264f620f), unchecked((int) 0x24b8d2bf), unchecked((int) 0x15c1b79e), unchecked((int) 0x46a52564), unchecked((int) 0xf8d7e54e), unchecked((int) 0x3e378160), unchecked((int) 0x7895cda5), unchecked((int) 0x859c15a5),
        unchecked((int) 0xe6459788), unchecked((int) 0xc37bc75f), unchecked((int) 0xdb07ba0c), unchecked((int) 0x0676a3ab), unchecked((int) 0x7f229b1e), unchecked((int) 0x31842e7b), unchecked((int) 0x24259fd7), unchecked((int) 0xf8bef472),
        unchecked((int) 0x835ffcb8), unchecked((int) 0x6df4c1f2), unchecked((int) 0x96f5b195), unchecked((int) 0xfd0af0fc), unchecked((int) 0xb0fe134c), unchecked((int) 0xe2506d3d), unchecked((int) 0x4f9b12ea), unchecked((int) 0xf215f225),
        unchecked((int) 0xa223736f), unchecked((int) 0x9fb4c428), unchecked((int) 0x25d04979), unchecked((int) 0x34c713f8), unchecked((int) 0xc4618187), unchecked((int) 0xea7a6e98), unchecked((int) 0x7cd16efc), unchecked((int) 0x1436876c),
        unchecked((int) 0xf1544107), unchecked((int) 0xbedeee14), unchecked((int) 0x56e9af27), unchecked((int) 0xa04aa441), unchecked((int) 0x3cf7c899), unchecked((int) 0x92ecbae6), unchecked((int) 0xdd67016d), unchecked((int) 0x151682eb),
        unchecked((int) 0xa842eedf), unchecked((int) 0xfdba60b4), unchecked((int) 0xf1907b75), unchecked((int) 0x20e3030f), unchecked((int) 0x24d8c29e), unchecked((int) 0xe139673b), unchecked((int) 0xefa63fb8), unchecked((int) 0x71873054),
        unchecked((int) 0xb6f2cf3b), unchecked((int) 0x9f326442), unchecked((int) 0xcb15a4cc), unchecked((int) 0xb01a4504), unchecked((int) 0xf1e47d8d), unchecked((int) 0x844a1be5), unchecked((int) 0xbae7dfdc), unchecked((int) 0x42cbda70),
        unchecked((int) 0xcd7dae0a), unchecked((int) 0x57e85b7a), unchecked((int) 0xd53f5af6), unchecked((int) 0x20cf4d8c), unchecked((int) 0xcea4d428), unchecked((int) 0x79d130a4), unchecked((int) 0x3486ebfb), unchecked((int) 0x33d3cddc),
        unchecked((int) 0x77853b53), unchecked((int) 0x37effcb5), unchecked((int) 0xc5068778), unchecked((int) 0xe580b3e6), unchecked((int) 0x4e68b8f4), unchecked((int) 0xc5c8b37e), unchecked((int) 0x0d809ea2), unchecked((int) 0x398feb7c),
        unchecked((int) 0x132a4f94), unchecked((int) 0x43b7950e), unchecked((int) 0x2fee7d1c), unchecked((int) 0x223613bd), unchecked((int) 0xdd06caa2), unchecked((int) 0x37df932b), unchecked((int) 0xc4248289), unchecked((int) 0xacf3ebc3),
        unchecked((int) 0x5715f6b7), unchecked((int) 0xef3478dd), unchecked((int) 0xf267616f), unchecked((int) 0xc148cbe4), unchecked((int) 0x9052815e), unchecked((int) 0x5e410fab), unchecked((int) 0xb48a2465), unchecked((int) 0x2eda7fa4),
        unchecked((int) 0xe87b40e4), unchecked((int) 0xe98ea084), unchecked((int) 0x5889e9e1), unchecked((int) 0xefd390fc), unchecked((int) 0xdd07d35b), unchecked((int) 0xdb485694), unchecked((int) 0x38d7e5b2), unchecked((int) 0x57720101),
        unchecked((int) 0x730edebc), unchecked((int) 0x5b643113), unchecked((int) 0x94917e4f), unchecked((int) 0x503c2fba), unchecked((int) 0x646f1282), unchecked((int) 0x7523d24a), unchecked((int) 0xe0779695), unchecked((int) 0xf9c17a8f),
        unchecked((int) 0x7a5b2121), unchecked((int) 0xd187b896), unchecked((int) 0x29263a4d), unchecked((int) 0xba510cdf), unchecked((int) 0x81f47c9f), unchecked((int) 0xad1163ed), unchecked((int) 0xea7b5965), unchecked((int) 0x1a00726e),
        unchecked((int) 0x11403092), unchecked((int) 0x00da6d77), unchecked((int) 0x4a0cdd61), unchecked((int) 0xad1f4603), unchecked((int) 0x605bdfb0), unchecked((int) 0x9eedc364), unchecked((int) 0x22ebe6a8), unchecked((int) 0xcee7d28a),
        unchecked((int) 0xa0e736a0), unchecked((int) 0x5564a6b9), unchecked((int) 0x10853209), unchecked((int) 0xc7eb8f37), unchecked((int) 0x2de705ca), unchecked((int) 0x8951570f), unchecked((int) 0xdf09822b), unchecked((int) 0xbd691a6c),
        unchecked((int) 0xaa12e4f2), unchecked((int) 0x87451c0f), unchecked((int) 0xe0f6a27a), unchecked((int) 0x3ada4819), unchecked((int) 0x4cf1764f), unchecked((int) 0x0d771c2b), unchecked((int) 0x67cdb156), unchecked((int) 0x350d8384),
        unchecked((int) 0x5938fa0f), unchecked((int) 0x42399ef3), unchecked((int) 0x36997b07), unchecked((int) 0x0e84093d), unchecked((int) 0x4aa93e61), unchecked((int) 0x8360d87b), unchecked((int) 0x1fa98b0c), unchecked((int) 0x1149382c),
        unchecked((int) 0xe97625a5), unchecked((int) 0x0614d1b7), unchecked((int) 0x0e25244b), unchecked((int) 0x0c768347), unchecked((int) 0x589e8d82), unchecked((int) 0x0d2059d1), unchecked((int) 0xa466bb1e), unchecked((int) 0xf8da0a82),
        unchecked((int) 0x04f19130), unchecked((int) 0xba6e4ec0), unchecked((int) 0x99265164), unchecked((int) 0x1ee7230d), unchecked((int) 0x50b2ad80), unchecked((int) 0xeaee6801), unchecked((int) 0x8db2a283), unchecked((int) 0xea8bf59e)
    };

        //====================================
        // Useful constants
        //====================================

        internal static readonly int MAX_ROUNDS = 16;
        internal static readonly int RED_ROUNDS = 12;

        private const int BLOCK_SIZE = 8;  // bytes = 64 bits

        private int [] _Kr = new int[17];        // the rotating round key
        private int [] _Km = new int[17];        // the masking round key

        private bool _encrypting;

        private byte[] _workingKey;
        private int _rounds = MAX_ROUNDS;

        public Cast5Engine()
        {
        }

        /**
        * initialise a CAST cipher.
        *
        * @param forEncryption whether or not we are for encryption.
        * @param parameters the parameters required to set up the cipher.
        * @exception ArgumentException if the parameters argument is
        * inappropriate.
        */
        public void Init(
            bool				forEncryption,
            ICipherParameters	parameters)
        {
            if (!(parameters is KeyParameter))
				throw new ArgumentException("Invalid parameter passed to "+ AlgorithmName +" init - " + parameters.GetType().ToString());

			_encrypting = forEncryption;
			_workingKey = ((KeyParameter)parameters).GetKey();
			SetKey(_workingKey);
        }

		public virtual string AlgorithmName
        {
            get { return "CAST5"; }
        }

		public virtual bool IsPartialBlockOkay
		{
			get { return false; }
		}

		public virtual int ProcessBlock(
            byte[]	input,
            int		inOff,
            byte[]	output,
            int		outOff)
        {
			int blockSize = GetBlockSize();
            if (_workingKey == null)
                throw new InvalidOperationException(AlgorithmName + " not initialised");
            if ((inOff + blockSize) > input.Length)
                throw new DataLengthException("Input buffer too short");
            if ((outOff + blockSize) > output.Length)
                throw new DataLengthException("Output buffer too short");

			if (_encrypting)
            {
                return EncryptBlock(input, inOff, output, outOff);
            }
            else
            {
                return DecryptBlock(input, inOff, output, outOff);
            }
        }

        public virtual void Reset()
        {
        }

        public virtual int GetBlockSize()
        {
            return BLOCK_SIZE;
        }

        //==================================
        // Private Implementation
        //==================================

        /*
        * Creates the subkeys using the same nomenclature
        * as described in RFC2144.
        *
        * See section 2.4
        */
        internal virtual void SetKey(byte[] key)
        {
            /*
            * Determine the key size here, if required
            *
            * if keysize <= 80bits, use 12 rounds instead of 16
            * if keysize < 128bits, pad with 0
            *
            * Typical key sizes => 40, 64, 80, 128
            */

            if (key.Length < 11)
            {
                _rounds = RED_ROUNDS;
            }

            int [] z = new int[16];
            int [] x = new int[16];

            int z03, z47, z8B, zCF;
            int x03, x47, x8B, xCF;

            /* copy the key into x */
            for (int i=0; i< key.Length; i++)
            {
                x[i] = (int)(key[i] & 0xff);
            }

            /*
            * This will look different because the selection of
            * bytes from the input key I've already chosen the
            * correct int.
            */
            x03 = IntsTo32bits(x, 0x0);
            x47 = IntsTo32bits(x, 0x4);
            x8B = IntsTo32bits(x, 0x8);
            xCF = IntsTo32bits(x, 0xC);

            z03 = x03 ^S5[x[0xD]] ^S6[x[0xF]] ^S7[x[0xC]] ^S8[x[0xE]] ^S7[x[0x8]];

            Bits32ToInts(z03, z, 0x0);
            z47 = x8B ^S5[z[0x0]] ^S6[z[0x2]] ^S7[z[0x1]] ^S8[z[0x3]] ^S8[x[0xA]];
            Bits32ToInts(z47, z, 0x4);
            z8B = xCF ^S5[z[0x7]] ^S6[z[0x6]] ^S7[z[0x5]] ^S8[z[0x4]] ^S5[x[0x9]];
            Bits32ToInts(z8B, z, 0x8);
            zCF = x47 ^S5[z[0xA]] ^S6[z[0x9]] ^S7[z[0xB]] ^S8[z[0x8]] ^S6[x[0xB]];
            Bits32ToInts(zCF, z, 0xC);
            _Km[ 1]= S5[z[0x8]] ^ S6[z[0x9]] ^ S7[z[0x7]] ^ S8[z[0x6]] ^ S5[z[0x2]];
            _Km[ 2]= S5[z[0xA]] ^ S6[z[0xB]] ^ S7[z[0x5]] ^ S8[z[0x4]] ^ S6[z[0x6]];
            _Km[ 3]= S5[z[0xC]] ^ S6[z[0xD]] ^ S7[z[0x3]] ^ S8[z[0x2]] ^ S7[z[0x9]];
            _Km[ 4]= S5[z[0xE]] ^ S6[z[0xF]] ^ S7[z[0x1]] ^ S8[z[0x0]] ^ S8[z[0xC]];

            z03 = IntsTo32bits(z, 0x0);
            z47 = IntsTo32bits(z, 0x4);
            z8B = IntsTo32bits(z, 0x8);
            zCF = IntsTo32bits(z, 0xC);
            x03 = z8B ^S5[z[0x5]] ^S6[z[0x7]] ^S7[z[0x4]] ^S8[z[0x6]] ^S7[z[0x0]];
            Bits32ToInts(x03, x, 0x0);
            x47 = z03 ^S5[x[0x0]] ^S6[x[0x2]] ^S7[x[0x1]] ^S8[x[0x3]] ^S8[z[0x2]];
            Bits32ToInts(x47, x, 0x4);
            x8B = z47 ^S5[x[0x7]] ^S6[x[0x6]] ^S7[x[0x5]] ^S8[x[0x4]] ^S5[z[0x1]];
            Bits32ToInts(x8B, x, 0x8);
            xCF = zCF ^S5[x[0xA]] ^S6[x[0x9]] ^S7[x[0xB]] ^S8[x[0x8]] ^S6[z[0x3]];
            Bits32ToInts(xCF, x, 0xC);
            _Km[ 5]= S5[x[0x3]] ^ S6[x[0x2]] ^ S7[x[0xC]] ^ S8[x[0xD]] ^ S5[x[0x8]];
            _Km[ 6]= S5[x[0x1]] ^ S6[x[0x0]] ^ S7[x[0xE]] ^ S8[x[0xF]] ^ S6[x[0xD]];
            _Km[ 7]= S5[x[0x7]] ^ S6[x[0x6]] ^ S7[x[0x8]] ^ S8[x[0x9]] ^ S7[x[0x3]];
            _Km[ 8]= S5[x[0x5]] ^ S6[x[0x4]] ^ S7[x[0xA]] ^ S8[x[0xB]] ^ S8[x[0x7]];

            x03 = IntsTo32bits(x, 0x0);
            x47 = IntsTo32bits(x, 0x4);
            x8B = IntsTo32bits(x, 0x8);
            xCF = IntsTo32bits(x, 0xC);
            z03 = x03 ^S5[x[0xD]] ^S6[x[0xF]] ^S7[x[0xC]] ^S8[x[0xE]] ^S7[x[0x8]];
            Bits32ToInts(z03, z, 0x0);
            z47 = x8B ^S5[z[0x0]] ^S6[z[0x2]] ^S7[z[0x1]] ^S8[z[0x3]] ^S8[x[0xA]];
            Bits32ToInts(z47, z, 0x4);
            z8B = xCF ^S5[z[0x7]] ^S6[z[0x6]] ^S7[z[0x5]] ^S8[z[0x4]] ^S5[x[0x9]];
            Bits32ToInts(z8B, z, 0x8);
            zCF = x47 ^S5[z[0xA]] ^S6[z[0x9]] ^S7[z[0xB]] ^S8[z[0x8]] ^S6[x[0xB]];
            Bits32ToInts(zCF, z, 0xC);
            _Km[ 9]= S5[z[0x3]] ^ S6[z[0x2]] ^ S7[z[0xC]] ^ S8[z[0xD]] ^ S5[z[0x9]];
            _Km[10]= S5[z[0x1]] ^ S6[z[0x0]] ^ S7[z[0xE]] ^ S8[z[0xF]] ^ S6[z[0xc]];
            _Km[11]= S5[z[0x7]] ^ S6[z[0x6]] ^ S7[z[0x8]] ^ S8[z[0x9]] ^ S7[z[0x2]];
            _Km[12]= S5[z[0x5]] ^ S6[z[0x4]] ^ S7[z[0xA]] ^ S8[z[0xB]] ^ S8[z[0x6]];

            z03 = IntsTo32bits(z, 0x0);
            z47 = IntsTo32bits(z, 0x4);
            z8B = IntsTo32bits(z, 0x8);
            zCF = IntsTo32bits(z, 0xC);
            x03 = z8B ^S5[z[0x5]] ^S6[z[0x7]] ^S7[z[0x4]] ^S8[z[0x6]] ^S7[z[0x0]];
            Bits32ToInts(x03, x, 0x0);
            x47 = z03 ^S5[x[0x0]] ^S6[x[0x2]] ^S7[x[0x1]] ^S8[x[0x3]] ^S8[z[0x2]];
            Bits32ToInts(x47, x, 0x4);
            x8B = z47 ^S5[x[0x7]] ^S6[x[0x6]] ^S7[x[0x5]] ^S8[x[0x4]] ^S5[z[0x1]];
            Bits32ToInts(x8B, x, 0x8);
            xCF = zCF ^S5[x[0xA]] ^S6[x[0x9]] ^S7[x[0xB]] ^S8[x[0x8]] ^S6[z[0x3]];
            Bits32ToInts(xCF, x, 0xC);
            _Km[13]= S5[x[0x8]] ^ S6[x[0x9]] ^ S7[x[0x7]] ^ S8[x[0x6]] ^ S5[x[0x3]];
            _Km[14]= S5[x[0xA]] ^ S6[x[0xB]] ^ S7[x[0x5]] ^ S8[x[0x4]] ^ S6[x[0x7]];
            _Km[15]= S5[x[0xC]] ^ S6[x[0xD]] ^ S7[x[0x3]] ^ S8[x[0x2]] ^ S7[x[0x8]];
            _Km[16]= S5[x[0xE]] ^ S6[x[0xF]] ^ S7[x[0x1]] ^ S8[x[0x0]] ^ S8[x[0xD]];

            x03 = IntsTo32bits(x, 0x0);
            x47 = IntsTo32bits(x, 0x4);
            x8B = IntsTo32bits(x, 0x8);
            xCF = IntsTo32bits(x, 0xC);
            z03 = x03 ^S5[x[0xD]] ^S6[x[0xF]] ^S7[x[0xC]] ^S8[x[0xE]] ^S7[x[0x8]];
            Bits32ToInts(z03, z, 0x0);
            z47 = x8B ^S5[z[0x0]] ^S6[z[0x2]] ^S7[z[0x1]] ^S8[z[0x3]] ^S8[x[0xA]];
            Bits32ToInts(z47, z, 0x4);
            z8B = xCF ^S5[z[0x7]] ^S6[z[0x6]] ^S7[z[0x5]] ^S8[z[0x4]] ^S5[x[0x9]];
            Bits32ToInts(z8B, z, 0x8);
            zCF = x47 ^S5[z[0xA]] ^S6[z[0x9]] ^S7[z[0xB]] ^S8[z[0x8]] ^S6[x[0xB]];
            Bits32ToInts(zCF, z, 0xC);
            _Kr[ 1]=(S5[z[0x8]]^S6[z[0x9]]^S7[z[0x7]]^S8[z[0x6]] ^ S5[z[0x2]])&0x1f;
            _Kr[ 2]=(S5[z[0xA]]^S6[z[0xB]]^S7[z[0x5]]^S8[z[0x4]] ^ S6[z[0x6]])&0x1f;
            _Kr[ 3]=(S5[z[0xC]]^S6[z[0xD]]^S7[z[0x3]]^S8[z[0x2]] ^ S7[z[0x9]])&0x1f;
            _Kr[ 4]=(S5[z[0xE]]^S6[z[0xF]]^S7[z[0x1]]^S8[z[0x0]] ^ S8[z[0xC]])&0x1f;

            z03 = IntsTo32bits(z, 0x0);
            z47 = IntsTo32bits(z, 0x4);
            z8B = IntsTo32bits(z, 0x8);
            zCF = IntsTo32bits(z, 0xC);
            x03 = z8B ^S5[z[0x5]] ^S6[z[0x7]] ^S7[z[0x4]] ^S8[z[0x6]] ^S7[z[0x0]];
            Bits32ToInts(x03, x, 0x0);
            x47 = z03 ^S5[x[0x0]] ^S6[x[0x2]] ^S7[x[0x1]] ^S8[x[0x3]] ^S8[z[0x2]];
            Bits32ToInts(x47, x, 0x4);
            x8B = z47 ^S5[x[0x7]] ^S6[x[0x6]] ^S7[x[0x5]] ^S8[x[0x4]] ^S5[z[0x1]];
            Bits32ToInts(x8B, x, 0x8);
            xCF = zCF ^S5[x[0xA]] ^S6[x[0x9]] ^S7[x[0xB]] ^S8[x[0x8]] ^S6[z[0x3]];
            Bits32ToInts(xCF, x, 0xC);
            _Kr[ 5]=(S5[x[0x3]]^S6[x[0x2]]^S7[x[0xC]]^S8[x[0xD]]^S5[x[0x8]])&0x1f;
            _Kr[ 6]=(S5[x[0x1]]^S6[x[0x0]]^S7[x[0xE]]^S8[x[0xF]]^S6[x[0xD]])&0x1f;
            _Kr[ 7]=(S5[x[0x7]]^S6[x[0x6]]^S7[x[0x8]]^S8[x[0x9]]^S7[x[0x3]])&0x1f;
            _Kr[ 8]=(S5[x[0x5]]^S6[x[0x4]]^S7[x[0xA]]^S8[x[0xB]]^S8[x[0x7]])&0x1f;

            x03 = IntsTo32bits(x, 0x0);
            x47 = IntsTo32bits(x, 0x4);
            x8B = IntsTo32bits(x, 0x8);
            xCF = IntsTo32bits(x, 0xC);
            z03 = x03 ^S5[x[0xD]] ^S6[x[0xF]] ^S7[x[0xC]] ^S8[x[0xE]] ^S7[x[0x8]];
            Bits32ToInts(z03, z, 0x0);
            z47 = x8B ^S5[z[0x0]] ^S6[z[0x2]] ^S7[z[0x1]] ^S8[z[0x3]] ^S8[x[0xA]];
            Bits32ToInts(z47, z, 0x4);
            z8B = xCF ^S5[z[0x7]] ^S6[z[0x6]] ^S7[z[0x5]] ^S8[z[0x4]] ^S5[x[0x9]];
            Bits32ToInts(z8B, z, 0x8);
            zCF = x47 ^S5[z[0xA]] ^S6[z[0x9]] ^S7[z[0xB]] ^S8[z[0x8]] ^S6[x[0xB]];
            Bits32ToInts(zCF, z, 0xC);
            _Kr[ 9]=(S5[z[0x3]]^S6[z[0x2]]^S7[z[0xC]]^S8[z[0xD]]^S5[z[0x9]])&0x1f;
            _Kr[10]=(S5[z[0x1]]^S6[z[0x0]]^S7[z[0xE]]^S8[z[0xF]]^S6[z[0xc]])&0x1f;
            _Kr[11]=(S5[z[0x7]]^S6[z[0x6]]^S7[z[0x8]]^S8[z[0x9]]^S7[z[0x2]])&0x1f;
            _Kr[12]=(S5[z[0x5]]^S6[z[0x4]]^S7[z[0xA]]^S8[z[0xB]]^S8[z[0x6]])&0x1f;

            z03 = IntsTo32bits(z, 0x0);
            z47 = IntsTo32bits(z, 0x4);
            z8B = IntsTo32bits(z, 0x8);
            zCF = IntsTo32bits(z, 0xC);
            x03 = z8B ^S5[z[0x5]] ^S6[z[0x7]] ^S7[z[0x4]] ^S8[z[0x6]] ^S7[z[0x0]];
            Bits32ToInts(x03, x, 0x0);
            x47 = z03 ^S5[x[0x0]] ^S6[x[0x2]] ^S7[x[0x1]] ^S8[x[0x3]] ^S8[z[0x2]];
            Bits32ToInts(x47, x, 0x4);
            x8B = z47 ^S5[x[0x7]] ^S6[x[0x6]] ^S7[x[0x5]] ^S8[x[0x4]] ^S5[z[0x1]];
            Bits32ToInts(x8B, x, 0x8);
            xCF = zCF ^S5[x[0xA]] ^S6[x[0x9]] ^S7[x[0xB]] ^S8[x[0x8]] ^S6[z[0x3]];
            Bits32ToInts(xCF, x, 0xC);
            _Kr[13]=(S5[x[0x8]]^S6[x[0x9]]^S7[x[0x7]]^S8[x[0x6]]^S5[x[0x3]])&0x1f;
            _Kr[14]=(S5[x[0xA]]^S6[x[0xB]]^S7[x[0x5]]^S8[x[0x4]]^S6[x[0x7]])&0x1f;
            _Kr[15]=(S5[x[0xC]]^S6[x[0xD]]^S7[x[0x3]]^S8[x[0x2]]^S7[x[0x8]])&0x1f;
            _Kr[16]=(S5[x[0xE]]^S6[x[0xF]]^S7[x[0x1]]^S8[x[0x0]]^S8[x[0xD]])&0x1f;
        }

        /**
        * Encrypt the given input starting at the given offset and place
        * the result in the provided buffer starting at the given offset.
        *
        * @param src        The plaintext buffer
        * @param srcIndex    An offset into src
        * @param dst        The ciphertext buffer
        * @param dstIndex    An offset into dst
        */
        internal virtual int EncryptBlock(
            byte[] src,
            int srcIndex,
            byte[] dst,
            int dstIndex)
        {
            int  [] result = new int[2];

            // process the input block
            // batch the units up into a 32 bit chunk and go for it
            // the array is in bytes, the increment is 8x8 bits = 64

            int L0 = BytesTo32bits(src, srcIndex);
            int R0 = BytesTo32bits(src, srcIndex + 4);

            CAST_Encipher(L0, R0, result);

            // now stuff them into the destination block
            Bits32ToBytes(result[0], dst, dstIndex);
            Bits32ToBytes(result[1], dst, dstIndex + 4);

            return BLOCK_SIZE;
        }

        /**
        * Decrypt the given input starting at the given offset and place
        * the result in the provided buffer starting at the given offset.
        *
        * @param src        The plaintext buffer
        * @param srcIndex    An offset into src
        * @param dst        The ciphertext buffer
        * @param dstIndex    An offset into dst
        */
        internal virtual int DecryptBlock(
            byte[] src,
            int srcIndex,
            byte[] dst,
            int dstIndex)
        {
            int  [] result = new int[2];

            // process the input block
            // batch the units up into a 32 bit chunk and go for it
            // the array is in bytes, the increment is 8x8 bits = 64
            int L16 = BytesTo32bits(src, srcIndex);
            int R16 = BytesTo32bits(src, srcIndex+4);

            CAST_Decipher(L16, R16, result);

            // now stuff them into the destination block
            Bits32ToBytes(result[0], dst, dstIndex);
            Bits32ToBytes(result[1], dst, dstIndex+4);

            return BLOCK_SIZE;
        }

        /**
        * The first of the three processing functions for the
        * encryption and decryption.
        *
        * @param D            the input to be processed
        * @param Kmi        the mask to be used from Km[n]
        * @param Kri        the rotation value to be used
        *
        */
        internal static int F1(int D, int Kmi, int Kri)
        {
            int I = Kmi + D;
            I = I << Kri | (int) ((uint) I >> (32-Kri));
            return ((S1[((uint) I >>24)&0xff]^S2[((uint)I>>16)&0xff])-S3[((uint)I>> 8)&0xff])+
                    S4[(I     )&0xff];
        }

        /**
        * The second of the three processing functions for the
        * encryption and decryption.
        *
        * @param D            the input to be processed
        * @param Kmi        the mask to be used from Km[n]
        * @param Kri        the rotation value to be used
        *
        */
        internal static  int F2(int D, int Kmi, int Kri)
        {
            int I = Kmi ^ D;
            I = I << Kri | (int) ((uint)I >> (32-Kri));
            return ((S1[((uint)I>>24)&0xff]-S2[((uint)I>>16)&0xff])+S3[((uint)I>> 8)&0xff])^
                    S4[(I     )&0xff];
        }

        /**
        * The third of the three processing functions for the
        * encryption and decryption.
        *
        * @param D            the input to be processed
        * @param Kmi        the mask to be used from Km[n]
        * @param Kri        the rotation value to be used
        *
        */
        internal static int F3(int D, int Kmi, int Kri)
        {
            int I = Kmi - D;
            I = I << Kri | (int) ((uint)I >> (32-Kri));
            return ((S1[((uint)I>>24)&0xff]+S2[((uint)I>>16)&0xff])^S3[((uint)I>> 8)&0xff])-
                    S4[(I     )&0xff];
        }

        /**
        * Does the 16 rounds to encrypt the block.
        *
        * @param L0    the LH-32bits of the plaintext block
        * @param R0    the RH-32bits of the plaintext block
        */
        internal  void CAST_Encipher(int L0, int R0, int [] result)
        {
            int Lp = L0;        // the previous value, equiv to L[i-1]
            int Rp = R0;        // equivalent to R[i-1]

            /*
            * numbering consistent with paper to make
            * checking and validating easier
            */
            int Li = L0, Ri = R0;

            for (int i = 1; i<=_rounds ; i++)
            {
                Lp = Li;
                Rp = Ri;

                Li = Rp;
                switch (i)
                {
                    case  1:
                    case  4:
                    case  7:
                    case 10:
                    case 13:
                    case 16:
                        Ri = Lp ^ F1(Rp, _Km[i], _Kr[i]);
                        break;
                    case  2:
                    case  5:
                    case  8:
                    case 11:
                    case 14:
                        Ri = Lp ^ F2(Rp, _Km[i], _Kr[i]);
                        break;
                    case  3:
                    case  6:
                    case  9:
                    case 12:
                    case 15:
                        Ri = Lp ^ F3(Rp, _Km[i], _Kr[i]);
                        break;
                }
            }

            result[0] = Ri;
            result[1] = Li;

            return;
        }

        internal  void CAST_Decipher(int L16, int R16, int [] result)
        {
            int Lp = L16;        // the previous value, equiv to L[i-1]
            int Rp = R16;        // equivalent to R[i-1]

            /*
            * numbering consistent with paper to make
            * checking and validating easier
            */
            int Li = L16, Ri = R16;

            for (int i = _rounds; i > 0; i--)
            {
                Lp = Li;
                Rp = Ri;

                Li = Rp;
                switch (i)
                {
                    case  1:
                    case  4:
                    case  7:
                    case 10:
                    case 13:
                    case 16:
                        Ri = Lp ^ F1(Rp, _Km[i], _Kr[i]);
                        break;
                    case  2:
                    case  5:
                    case  8:
                    case 11:
                    case 14:
                        Ri = Lp ^ F2(Rp, _Km[i], _Kr[i]);
                        break;
                    case  3:
                    case  6:
                    case  9:
                    case 12:
                    case 15:
                        Ri = Lp ^ F3(Rp, _Km[i], _Kr[i]);
                        break;
                }
            }

            result[0] = Ri;
            result[1] = Li;

            return;
        }

        internal static  void Bits32ToInts(int inData,  int[] b, int offset)
        {
            b[offset + 3] = (inData & 0xff);
            b[offset + 2] = (int) (((uint) inData >> 8) & 0xff);
            b[offset + 1] = (int) (((uint)inData >> 16) & 0xff);
            b[offset]     = (int) (((uint)inData >> 24) & 0xff);
        }

        internal static int IntsTo32bits(int[] b, int i)
        {
            int rv = 0;

            rv = ((b[i]   & 0xff) << 24) |
                ((b[i+1] & 0xff) << 16) |
                ((b[i+2] & 0xff) << 8) |
                ((b[i+3] & 0xff));

            return rv;
        }

        internal static void Bits32ToBytes(int inData,  byte[] b, int offset)
        {
            b[offset + 3] = (byte)inData;
            b[offset + 2] = (byte)((uint)inData >> 8);
            b[offset + 1] = (byte)((uint)inData >> 16);
            b[offset]     = (byte)((uint)inData >> 24);
        }

        internal static int BytesTo32bits(byte[] b, int i)
        {
            return ((b[i]   & 0xff) << 24) |
                ((b[i+1] & 0xff) << 16) |
                ((b[i+2] & 0xff) << 8) |
                ((b[i+3] & 0xff));
        }
    }

}
