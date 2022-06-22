using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Nethermind.Core;
using Nethermind.Core.Crypto;
using Nethermind.Core.Extensions;
using Nethermind.Db;
using Nethermind.Logging;
using Nethermind.State;
using Nethermind.State.Snap;
using Nethermind.Trie;
using Nethermind.Trie.Pruning;

namespace Nethermind.Synchronization.SnapSync
{
    public class SnapProvider : ISnapProvider
    {
        private readonly ITrieStore _store;
        private readonly IDbProvider _dbProvider;
        private readonly ILogManager _logManager;
        private readonly ILogger _logger;

        private readonly ProgressTracker _progressTracker;
        private readonly Dictionary<Keccak, string> _ahs;

        public SnapProvider(ProgressTracker progressTracker, IDbProvider dbProvider, ILogManager logManager)
        {
            _dbProvider = dbProvider ?? throw new ArgumentNullException(nameof(dbProvider));
            _progressTracker = progressTracker ?? throw new ArgumentNullException(nameof(progressTracker));

            _store = new TrieStore(
                _dbProvider.StateDb,
                Trie.Pruning.No.Pruning,
                Persist.EveryBlock,
                logManager);

            _logManager = logManager ?? throw new ArgumentNullException(nameof(logManager));
            _logger = logManager.GetClassLogger();
            _ahs = new string[] {
"0xdac17f958d2ee523a2206206994597c13d831ec7",
"0xa0b86991c6218b36c1d19d4a2e9eb0ce3606eb48",
"0x7be8076f4ea4a4ad08075c2508e481d6c946d12b",
"0x7f268357a8c2552623316e2562d90e642bb538e5",
"0xc02aaa39b223fe8d0a0e5c4f27ead9083c756cc2",
"0x514910771af9ca656af840dff83e8264ecf986ca",
"0x95ad61b0a150d79219dcf64e1e6cc01f0b64c4ce",
"0x06012c8cf97bead5deae237070f9587f8e7a266d",
"0x6b175474e89094c44da98b954eedeac495271d0f",
"0x7d1afa7b718fb893db30a3abc0cfc608aacfebb0",
"0x0d8775f648430679a709e98d2b0cb6250d2887ef",
"0xd26114cd6ee289accf82350c8d8487fedb8a0c07",
"0x9b9647431632af44be02ddd22477ed94d14aacaa",
"0x1f9840a85d5af5bf1d1762f925bdaddc4201f984",
"0x8e870d67f660d95d5be530380d0ec0bd388289e1",
"0xe41d2489571d322189246dafa5ebde1f4699f498",
"0xb64ef51c888972c908cfacf59b47c1afbc0ab8ac",
"0x2b591e99afe9f32eaa6214f7b7629768c40eeb39",
"0x0f5d2fb29fb7d3cfee444a200298f468908cc942",
"0x8e766f57f7d16ca50b4a0b90b88f6468a09b0439",
"0xa15c7ebe1f07caf6bff097d8a589fb8ac49ae5b3",
"0xf629cbd94d3791c9250152bd8dfbdf380e2a3b9c",
"0x21ab6c9fac80c59d401b37cb43f81ea9dde7fe34",
"0xa0b73e1ff0b80914ab6fe0444e65848c4c34450b",
"0x0000000000085d4780b73119b644ae5ecd22b376",
"0xcc8fa225d80b9c7d42f96e9570156c65d6caaa25",
"0x3506424f91fd33084466f402d5d97f05f8e3b4af",
"0xc011a73ee8576fb46f5e1c5751ca3b9fe0af2a6f",
"0x744d70fdbe2ba4cf95131626614a1763df805b9e",
"0x15d4c048f83bd7e37d49ea4c83a07267ec4203da",
"0xc00e94cb662c3520282e6f5717214004a7f26888",
"0xdd974d5c2e2928dea5f71b9825b8b646686bd200",
"0x4fabb145d64652a948d72533023f6e7a623c7c53",
"0x653430560be843c4a3d143d0110e896c2ab8ac0d",
"0x6b3595068778dd592e39a122f4f5a5cf09c90fe2",
"0xb8c77482e45f1f44de1745f52c74426c631bdd52",
"0x74fd51a98a4a1ecbef8cc43be801cce630e260bd",
"0x7fc66500c84a76ad7e9c93437bfc5ac33e2ddae9",
"0x6c6ee5e31d828de241282b9606c8e98ea48526e2",
"0x8a88f04e0c905054d2f33b26bb3a46d7091a039a",
"0x0ba45a8b5d5575935b8158a88c631e9f9c95a2e5",
"0xd533a949740bb3306d119cc777fa900ba034cd52",
"0x3845badade8e6dff049820680d1f14bd3903a5d0",
"0x41e5560054824ea6b0732e656e3ad64e20e94e45",
"0xc944e90c64b2c07662a292be6244bdf05cda44a7",
"0xc36442b4a4522e871399cd717abdd847ab11fe88",
"0x495f947276749ce646f68ac8c248420045cb7b5e",
"0x06a6a7af298129e3a2ab396c9c06f91d3c54aba8",
"0x0e69d0a2bbb30abcb7e5cfea0e4fde19c00a8d47",
"0x58959e0c71080434f237bd42d07cd84b74cef438",
"0x2260fac5e5542a773aa44fbcfedf7c193bc2c599",
"0x4e15361fd6b4bb609fa63c81a2be19d873717870",
"0x89d24a6b4ccb1b6faa2625fe562bdd9a23260359",
"0xaaaebe6fe48e54f431b0c390cfaf0b017d09d42d",
"0x6f259637dcd74c767781e37bc6133cd6a68aa161",
"0x990f341946a3fdb507ae7e52d17851b87168017c",
"0xbb0e17ef65f82ab018d8edd776e8dd940327b28b",
"0x9f8f72aa9304c8b593d555f12ef6589cc3a579a2",
"0x111111111117dc0aa78b770fa6a738034120c302",
"0x0bc529c00c6401aef6d220be8c6ea1667f6ad93e",
"0xbbbbca6a901c926f240b89eacb641d8aec7aeafd",
"0x3b484b82567a09e2588a13d54d032153f0c0aee0",
"0x2e65e12b5f0fd1d58738c6f38da7d57f5f183d1c",
"0x3c7b464376db7c9927930cf50eefdea2eff3a66a",
"0xb62132e35a6c13ee1ee0f84dc5d40bad8d815206",
"0x05f4a42e251f2d52b8ed15e9fedaacfcef1fad27",
"0xf411903cbc70a74d22900a5de66a2dda66507255",
"0x3597bfd533a99c9aa083587b074434e61eb0a258",
"0x408e41876cccdc0f92210600ef50372656052a38",
"0xd07dc4262bcdbf85190c01c996b4c06a461d2430",
"0x419d0d8bdd9af5e606ae2232ed285aff190e711b",
"0xb97048628db6b661d4c2aa833e95dbe1a905b280",
"0x8971f9fd7196e5cee2c1032b50f656855af7dd26",
"0xb4a81261b16b92af0b9f7c4a83f1e885132d81e4",
"0x1f573d6fb3f13d689ff844b4ce37794d79a7ff1c",
"0x4d224452801aced8b2f0aebe155379bb5d594381",
"0x4156d3342d5c385a87d264f90653733592000581",
"0x4dc3643dbc642b72c158e7f3d2ff232df61cb6ce",
"0x4a220e6096b25eadb88358cb44068a3248254675",
"0xb63b606ac810a52cca15e44bb630fd42d8d1d83d",
"0x39aa39c021dfbae8fac545936693ac917d5e7563",
"0x4092678e4e78230f46a1534c0fbc8fa39780892b",
"0x9992ec3cf6a55b00978cddf2b27bc6882d88d1ec",
"0xc5d105e63711398af9bbff092d4b6769c82f793d",
"0x8290333cef9e6d528dd5618fb97a76f268f3edd4",
"0x58b6a8a3302369daec383334672404ee733ab239",
"0x1776e1f26f98b1a5df9cd347953a26dd3cb46671",
"0xf4d2888d29d722226fafa5d9b24f9164c092421e",
"0x8ce9137d39326ad0cd6491fb5cc0cba0e089b6a9",
"0x0abdace70d3790235af448c88547603b945604ea",
"0x80fb784b7ed66730e8b1dbd9820afd29931aab03",
"0xc36cf0cfcb5d905b8b513860db0cfe63f6cf9f5c",
"0xbf2179859fc6d5bee9bf9158632dc51678a4100e",
"0xc18360217d8f7ab5e7c516566761ea12ce7f9d72",
"0xc77b230f31b517f1ef362e59c173c2be6540b5e8",
"0x60f80121c31a0d46b5279700f9df786054aa5ee5",
"0xf1f3ca6268f330fda08418db12171c3173ee39c9",
"0xff20817765cb7f73d4bde2e66e067e58d11095c2",
"0x8f8221afbb33998d8584a2b05749ba73c37a938a",
"0xa47c8bf37f92abed4a126bda807a7b7498661acd",
"0x4a8f44be523580a11cdb20e2c7c470adf44ec9bb",
"0x08f5a9235b08173b7569f83645d2c7fb55e8ccd8",
"0xd4fa1460f537bb9085d22c7bccb5dd450ef28e3a",
"0x58a4884182d9e835597f405e5f258290e46ae7c2",
"0xcb97e65f07da24d46bcdd078ebebd7c6e6e3d750",
"0x9a642d6b3368ddc662ca244badf32cda716005bc",
"0x08d32b0da63e2c3bcf8019c9c5d849d7a9d791e6",
"0x761d38e5ddf6ccf6cf7c55759d5210750b5d60f3",
"0xb6ed7644c69416d67b522e20bc294a9a9b405b31",
"0xb5fe099475d3030dde498c3bb6f3854f762a48ad",
"0xe530441f4f73bdb6dc2fa5af7c3fc5fd551ec838",
"0x08ceed1e8db59acbb687a5752f0a7db815cfda5e",
"0x4575f41308ec1483f3d399aa9a2826d74da13deb",
"0xa2b4c0af19cc16a6cfacce81f192b024d625817d",
"0xf1ca9cb74685755965c7458528a36934df52a3ef",
"0xc5bbae50781be1669306b9e001eff57a2957b09d",
"0x12d79c345cac7b050a5ff0797b5a607e254c73f5",
"0x0000000000b3f879cb30fe243b4dfee438691c04",
"0x4ddc2d193948926d02f9b1fe9e1daa0718270ed5",
"0x57f1887a8bf19b14fc0df6fd9b2acc9af147ea85",
"0x607f4c5bb672230e8672085532f7e901544a7375",
"0x1985365e9f78359a9b6ad760e32412f4a445e862",
"0xf433089366899d83a9f26a773d59ec7ecf30355e",
"0x8207c1ffc5b6804f6024322ccf34f29c3541ae26",
"0x6267873a5a0a1647a8fbb25541605a0815e04d74",
"0x419c4db4b9e25d6db2ad9691ccb832c8d9fda05e",
"0xba100000625a3754423978a60c9317c58a424e3d",
"0xd46ba6d942050d489dbd938a2c909a5d5039a161",
"0x5af2be193a6abca9c8817001f45744777db30756",
"0xba11d00c5f74255f56a5e366f4f77f5a186d7f55",
"0x55296f69f40ea6d20e478533c15a6b08b654e758",
"0xa4e8c3ec456107ea67d3075bf9e3df3a75823db0",
"0x50d1c9771902476076ecfc8b2a83ad6b9355a4c9",
"0x10086399dd8c1e3de736724af52587a2044c9fa2",
"0x383518188c0c6d7730d91b2c03a03c837814a899",
"0x006699d34aa3013605d468d2755a2fe59a16b12b",
"0x476c5e26a75bd202a9683ffd34359c0cc15be0ff",
"0x4d31200e6d7854c2f664af7fc38a21600960f74d",
"0x83e6f1e41cdd28eaceb20cb649155049fac3d5aa",
"0xb7cb1c96db6b22b0d3d9536e0108d062bd488f74",
"0xd2877702675e6ceb975b4a1dff9fb7baf4c91ea9",
"0x9ab165d795019b6d8b3e971dda91071421305e5a",
"0x445f51299ef3307dbd75036dd896565f5b4bf7a5",
"0x1519aff03b3e23722511d2576c769a77baf09580",
"0xf970b8e36e23f7fc3fd752eea86f8be8d83375a6",
"0x967da4048cd07ab37855c090aaf366e4ce1b9f48",
"0x1014613e2b3cbc4d575054d4982e580d9b99d7b1",
"0xb47e3cd837ddf8e4c57f05d70ab865de6e193bbb",
"0xf0f8b0b8dbb1124261fc8d778e2287e3fd2cf4f5",
"0x5d3a536e4d6dbd6114cc1ead35777bab948e3643",
"0x8eb24319393716668d768dcec29356ae9cffe285",
"0x5edc1a266e8b2c5e8086d373725df0690af7e3ea",
"0xdf574c24545e5ffecb9a659c229253d4111d87e1",
"0x04fa0d235c4abf4bcf4787af4cf447de572ef828",
"0xddb3422497e61e13543bea06989c0789117555c5",
"0x809826cceab68c387726af962713b64cb5cb3cca",
"0x9d9223436ddd466fc247e9dbbd20207e640fef58",
"0xbb97e381f1d1e94ffa2a5844f6875e6146981009",
"0x4fe83213d56308330ec302a8bd641f1d0113a4cc",
"0xc5807256e2e2fe85ca94c3617c4bc5ff2bd9cfb6",
"0x8713d26637cf49e1b6b4a7ce57106aabc9325343",
"0xa701122c1b67220a8b6883d03c8ad67896b12466",
"0x056fd409e1d7a124bd7017459dfea2f387b6d5cd",
"0xdf2c7238198ad8b389666574f2d8bc411a4b7428",
"0x0000000000095413afc295d19edeb1ad7b71c952",
"0x466912baa9430a4a460b141ee8c580d817441449",
"0x3883f5e181fccaf8410fa61e12b59bad963fb645",
"0x8f3470a7388c05ee4e7af3d01d8c722b0ff52374",
"0x115e29615d1c10c8ddf826e4faec64ef6c1e3357",
"0x4ecdb6385f3db3847f9c4a9bf3f9917bb27a5452",
"0x68d57c9a1c35f63e2c83ee8e49a64e9d70528d25",
"0x467bccd9d29f223bce8043b84e8c8b282827790f",
"0x3b3ee1931dc30c1957379fac9aba94d1c48a5405",
"0xd0a4b8946cb52f0661273bfbc6fd0e0c75fc6433",
"0x7420b4b9a0110cdc71fb720908340c03f9bc03ec",
"0x40e45890dff79e7d533797d964e64a2c0121f49a",
"0x115ec79f1de567ec68b7ae7eda501b406626478e",
"0xbb9bc244d798123fde783fcc1c72d3bb8c189413",
"0x4dd672e77c795844fe3a464ef8ef0faae617c8fb",
"0xea26c4ac16d4a5a106820bc8aee85fd0b7b2b664",
"0x0f51bb10119727a7e5ea3538074fb341f56b09ad",
"0x090185f2135308bad17527004364ebcc2d37e5f6",
"0xfaff15c6cdaca61a4f87d329689293e07c98f578",
"0xf0ee6b27b759c9893ce4f094b49ad28fd15a23e4",
"0xae7ab96520de3a18e5e111b5eaab095312d7fe84",
"0x0cf0ee63788a0849fe5297f3407f701e122cc023",
"0x45804880de22913dafe09f4980848ece6ecbaf78",
"0xf9e5af7b42d31d51677c75bbbd37c1986ec79aee",
"0xb3dd5dce850dca7519e74a943568b69f958df52c",
"0x09a3ecafa817268f77be1283176b946c4ff2e608",
"0xd8698a985b89650d0a70f99ad2909bd0c0b4b51c",
"0xd48b633045af65ff636f3c6edd744748351e020d",
"0x88d50b466be55222019d71f9e8fae17f5f45fca1",
"0xbe9375c6a420d2eeb258962efb95551a5b722803",
"0x3472a5a71965499acd81997a54bba8d852c6e53d",
"0x27c70cd1946795b66be9d954418546998b546634",
"0x5121e348e897daef1eef23959ab290e5557cf274",
"0xff56cc6b1e6ded347aa0b7676c85ab0b3d08b0fa",
"0x34d85c9cdeb23fa97cb08333b511ac86e1c4e258",
"0xf7920b0768ecb20a123fac32311d07d193381d6f",
"0xaa7a9ca87d3694b5755f213b5d04094b8d0f0a6f",
"0x54f0e3b0d7ccbb65e56d166350aa86f7e71ce20b",
"0x7e9e431a0b8c4d532c745b1043c7fa29a48d4fba",
"0xe53ec727dbdeb9e2d5456c3be40cff031ab40a55",
"0x04f2694c8fcee23e8fd0dfea1d4f5bb8c352111f",
"0x6c3be406174349cfa4501654313d97e6a31072e1",
"0x6fb3e0a217407efff7ca062d46c26e5d60a14d69",
"0x6810e776880c02933d47db1b9fc05908e5386b96",
"0x8400d94a5cb0fa0d041a3788e395285d61c9ee5e",
"0xb98d4c97425d9908e66e53a6fdf673acca0be986",
"0x4ccc3759eb48faf1c6cfadad2619e7038db6b212",
"0x1dfe7ca09e99d10835bf73044a23b73fc20623df",
"0x3301ee63fb29f863f2333bd4466acb46cd8323e6",
"0x4f9254c83eb525f9fcf346490bbb3ed28a81c667",
"0x888666ca69e0f178ded6d75b5726cee99a87d698",
"0x92d6c1e31e14520e676a687f0a93788b716beff5",
"0x46b9ad944d1059450da1163511069c718f699d31",
"0xf5b0a3efb8e8e4c201e2a935f110eaaf3ffecb8d",
"0x27054b13b1b798b345b591a4d22e6562d47ea75a",
"0x8798249c2e607446efb7ad49ec89dd1865ff4272",
"0x6bba316c48b49bd1eac44573c5c871ff02958469",
"0x72dd4b6bd852a3aa172be4d6c5a6dbec588cf131",
"0xfe5f141bf94fe84bc28ded0ab966c16b17490657",
"0x9813037ee2218799597d83d4a5b6f3b6778218d9",
"0x2bf91c18cd4ae9c2f2858ef9fe518180f7b5096d",
"0x516e5436bafdc11083654de7bb9b95382d08d5de",
"0x5ca381bbfb58f0092df149bd3d243b08b9a8386e",
"0xfca59cd816ab1ead66534d82bc21e7515ce441cf",
"0x5f5b176553e51171826d1a62e540bc30422c7717",
"0x39bb259f66e1c59d5abef88375979b4d20d98022",
"0x4ceda7906a5ed2179785cd3a40a69ee8bc99c466",
"0x9eec65e5b998db6845321baa915ec3338b1a469b",
"0x177d39ac676ed1c67a2b268ad7f1e58826e5b0af",
"0xfe3e6a25e6b192a42a44ecddcd13796471735acf",
"0x286bda1413a2df81731d4930ce2f862a35a609fe",
"0x255aa6df07540cb5d3d297f0d0d4d84cb52bc8e6",
"0x8ab7404063ec4dbcfd4598215992dc3f8ec853d7",
"0xba50933c268f567bdc86e1ac131be072c6b0b71a",
"0x6ea53dfc58c5cbf68a799edd208cb3a905db5939",
"0x4cf488387f035ff08c371515562cba712f9015d4",
"0x0000000000004946c0e9f43f4dee607b0ef1fa1c",
"0xe3818504c1b32bf1557b16c238b2e01fd3149c17",
"0x77fba179c79de5b7653f68b5039af940ada60ce0",
"0xbe428c3867f05dea2a89fc76a102b544eac7f772",
"0xaea46a60368a7bd060eec7df8cba43b7ef41ad85",
"0x2ab05b915c30093679165bcdba9c26d8cd8bee99",
"0x8f136cc8bef1fea4a7b71aa2301ff1a52f084384",
"0x943ed852dadb5c3938ecdc6883718df8142de4c8",
"0x9064c91e51d7021a85ad96817e1432abf6624470",
"0xb9bb08ab7e9fa0a1356bd4a39ec0ca267e03b0b3",
"0x558ec3152e2eb2174905cd19aea4e34a23de9ad6",
"0x667088b212ce3d06a1b553a7221e1fd19000d9af",
"0x618e75ac90b12c6049ba3b27f5d5f8651b0037f6",
"0x0b38210ea11411557c13457d4da7dc6ea731b88a",
"0xfd43d1da000558473822302e1d44d81da2e4cc0d",
"0x752ff65b884b9c260d212c804e0b7aceea012473",
"0x74faab6986560fd1140508e4266d8a7b87274ffd",
"0x2c4e8f2d746113d0696ce89b35f0d8bf88e0aeca",
"0xf57e7e7c23978c3caec3c3548e3d615c346e79ff",
"0xd780ae2bf04cd96e577d3d014762f831d97129d0",
"0xee4458e052b533b1aabd493b5f8c4d85d7b263dc",
"0x767fe9edc9e0df98e07454847909b5e959d7ca0e",
"0x7fdcd2a1e52f10c28cb7732f46393e297ecadda1",
"0xf2ddae89449b7d26309a5d54614b1fc99c608af5",
"0x4b317864a05c91225ab8f401ec7be0aeb87e9c12",
"0x5b2e4a700dfbc560061e957edec8f6eeeb74a320",
"0x3893b9422cd5d70a81edeffe3d5a1c6a978310bb",
"0x4691937a7508860f876c9c0a2a617e7d9e945d4b",
"0x6c5ba91642f10282b576d91922ae6448c9d52f4e",
"0x3dd98c8a089dbcff7e8fc8d4f532bd493501ab7f",
"0x491604c0fdf08347dd1fa4ee062a822a5dd06b5d",
"0x0258f474786ddfd37abce6df6bbb1dd5dfc4434a",
"0x4f878c0852722b0976a955d68b376e4cd4ae99e5",
"0xdefa4e8a7bcba345f687a2f1456f5edd9ce97202",
"0x07bf5f95851ef2b2996f192569e406a6fea2a95a",
"0x2f141ce366a2462f02cea3d12cf93e4dca49e4fd",
"0x4e0603e2a27a30480e5e3a4fe548e29ef12f64be",
"0x75231f58b43240c9718dd58b4967c5114342a86c",
"0xbd13e53255ef917da7557db1b7d2d5c38a2efe24",
"0xa1faa113cbe53436df28ff0aee54275c13b40975",
"0x389999216860ab8e0175387a0c90e5c52522c945",
"0x84ca8bc7997272c7cfb4d0cd3d55cd942b3c9419",
"0x3cc5eb07e0e1227613f1df58f38b549823d11cb9",
"0xde7d85157d9714eadf595045cc12ca4a5f3e2adb",
"0xbbc2ae13b23d715c30720f079fcd9b4a74093505",
"0x3e9bc21c9b189c09df3ef1b824798658d5011937",
"0x28472a58a490c5e09a238847f66a68a47cc76f0f",
"0xe5dada80aa6477e85d09747f2842f7993d0df71c",
"0x7b00ae36c7485b678fe945c2dd9349eb5baf7b6b",
"0x0e0989b1f9b8a38983c2ba8053269ca62ec9b195",
"0xa7d8d9ef8d8ce8992df33d8b8cf4aebabd5bd270",
"0x23352036e911a22cfc692b5e2e196692658aded9",
"0xa37adde3ba20a396338364e2ddb5e0897d11a91d",
"0x0affa06e7fbe5bc9a764c979aa66e8256a631f02",
"0x006bea43baa3f7a6f765f14f10a1a1b08334ef45",
"0xe4815ae53b124e7263f08dcdbbb757d41ed658c6",
"0x2a46f2ffd99e19a89476e2f62270e0a35bbf0756"
            }.ToDictionary(a => Keccak.Compute(new Address(a).Bytes), a => a);           
        }

        public bool CanSync() => _progressTracker.CanSync();

        public (SnapSyncBatch request, bool finished) GetNextRequest() => _progressTracker.GetNextRequest();

        public AddRangeResult AddAccountRange(AccountRange request, AccountsAndProofs response)
        {
            AddRangeResult result;

            if (response.PathAndAccounts.Length == 0 && response.Proofs.Length == 0)
            {
                _logger.Trace($"SNAP - GetAccountRange - requested expired RootHash:{request.RootHash}");

                result = AddRangeResult.ExpiredRootHash;
            }
            else
            {
                result = AddAccountRange(request.BlockNumber.Value, request.RootHash, request.StartingHash, response.PathAndAccounts, response.Proofs);

                if (result == AddRangeResult.OK)
                {
                    Interlocked.Add(ref Metrics.SnapSyncedAccounts, response.PathAndAccounts.Length);
                }
            }

            _progressTracker.ReportAccountRequestFinished();

            return result;
        }

        public AddRangeResult AddAccountRange(long blockNumber, Keccak expectedRootHash, Keccak startingHash, PathWithAccount[] accounts, byte[][] proofs = null)
        {
            StateTree tree = new(_store, _logManager);

            (AddRangeResult result, bool moreChildrenToRight, IList<PathWithAccount> accountsWithStorage, IList<Keccak> codeHashes) =
                SnapProviderHelper.AddAccountRange(tree, blockNumber, expectedRootHash, startingHash, accounts, proofs);

            if (result == AddRangeResult.OK)
            {
                foreach (var item in accountsWithStorage)
                {
                    _progressTracker.EnqueueAccountStorage(item);
                }

                _progressTracker.EnqueueCodeHashes(codeHashes);

                _progressTracker.NextAccountPath = accounts[accounts.Length - 1].Path;
                _progressTracker.MoreAccountsToRight = moreChildrenToRight;
            }
            else if(result == AddRangeResult.MissingRootHashInProofs)
            {
                _logger.Trace($"SNAP - AddAccountRange failed, missing root hash {tree.RootHash} in the proofs, startingHash:{startingHash}");
            }
            else if(result == AddRangeResult.DifferentRootHash)
            {
                _logger.Trace($"SNAP - AddAccountRange failed, expected {blockNumber}:{expectedRootHash} but was {tree.RootHash}, startingHash:{startingHash}");
            }

            return result;
        }

        public AddRangeResult AddStorageRange(StorageRange request, SlotsAndProofs response)
        {
            AddRangeResult result = AddRangeResult.OK;

            if (response.PathsAndSlots.Length == 0 && response.Proofs.Length == 0)
            {
                _logger.Trace($"SNAP - GetStorageRange - expired BlockNumber:{request.BlockNumber}, RootHash:{request.RootHash}, (Accounts:{request.Accounts.Count()}), {request.StartingHash}");

                _progressTracker.ReportStorageRangeRequestFinished(request);

                return AddRangeResult.ExpiredRootHash;
            }
            else
            {
                int slotCount = 0;

                int requestLength = request.Accounts.Length;
                int responseLength = response.PathsAndSlots.Length;

                for (int i = 0; i < responseLength; i++)
                {
                    // only the last can have proofs
                    byte[][] proofs = null;
                    if (i == responseLength - 1)
                    {
                        proofs = response.Proofs;
                    }
                    if (_ahs.ContainsKey(request.Accounts[i].Path))
                    {
                        _logger.Warn($"{_ahs[request.Accounts[i].Path]}, {response.PathsAndSlots[i].OrderByDescending(x=>x.SlotRlpValue.Length).FirstOrDefault()?.Path}");
                    }

                    result = AddStorageRange(request.BlockNumber.Value, request.Accounts[i], request.Accounts[i].Account.StorageRoot, request.StartingHash, response.PathsAndSlots[i], proofs);

                    slotCount += response.PathsAndSlots[i].Length;
                }

                if (requestLength > responseLength)
                {
                    _progressTracker.ReportFullStorageRequestFinished(request.Accounts[responseLength..requestLength]);
                }
                else
                {
                    _progressTracker.ReportFullStorageRequestFinished();
                }

                if (result == AddRangeResult.OK && slotCount > 0)
                {
                    Interlocked.Add(ref Metrics.SnapSyncedStorageSlots, slotCount);
                }
            }

            return result;
        }

        public AddRangeResult AddStorageRange(long blockNumber, PathWithAccount pathWithAccount, Keccak expectedRootHash, Keccak? startingHash, PathWithStorageSlot[] slots, byte[][]? proofs = null)
        {
            StorageTree tree = new(_store, _logManager);
            (AddRangeResult result, bool moreChildrenToRight) = SnapProviderHelper.AddStorageRange(tree, blockNumber, startingHash, slots, expectedRootHash, proofs);

            if (result == AddRangeResult.OK)
            {
                if (moreChildrenToRight)
                {
                    StorageRange range = new()
                    {
                        Accounts = new[] { pathWithAccount },
                        StartingHash = slots.Last().Path
                    };

                    _progressTracker.EnqueueStorageRange(range);
                }
            }
            else if(result == AddRangeResult.MissingRootHashInProofs)
            {
                _logger.Trace($"SNAP - AddStorageRange failed, missing root hash {expectedRootHash} in the proofs, startingHash:{startingHash}");

                _progressTracker.EnqueueAccountRefresh(pathWithAccount, startingHash);
            }
            else if(result == AddRangeResult.DifferentRootHash)
            {
                _logger.Trace($"SNAP - AddStorageRange failed, expected storage root hash:{expectedRootHash} but was {tree.RootHash}, startingHash:{startingHash}");

                _progressTracker.EnqueueAccountRefresh(pathWithAccount, startingHash);
            }

            return result;
        }

        public void RefreshAccounts(AccountsToRefreshRequest request, byte[][] response)
        {
            int respLength = response.Length;

            for (int reqi = 0; reqi < request.Paths.Length; reqi++)
            {
                var requestedPath = request.Paths[reqi];

                if (reqi < respLength)
                {
                    byte[] nodeData = response[reqi];

                    if(nodeData.Length == 0)
                    {
                        RetryAccountRefresh(requestedPath);
                        _logger.Trace($"SNAP - Empty Account Refresh:{requestedPath.PathAndAccount.Path}");
                        continue;
                    }

                    try
                    {
                        var node = new TrieNode(NodeType.Unknown, nodeData, true);
                        node.ResolveNode(_store);
                        node.ResolveKey(_store, true);

                        requestedPath.PathAndAccount.Account = requestedPath.PathAndAccount.Account.WithChangedStorageRoot(node.Keccak);

                        if (requestedPath.StorageStartingHash > Keccak.Zero)
                        {
                            StorageRange range = new()
                            {
                                Accounts = new[] { requestedPath.PathAndAccount },
                                StartingHash = requestedPath.StorageStartingHash
                            };

                            _progressTracker.EnqueueStorageRange(range);
                        }
                        else
                        {
                            _progressTracker.EnqueueAccountStorage(requestedPath.PathAndAccount);
                        }
                    }
                    catch (Exception exc)
                    {
                        RetryAccountRefresh(requestedPath);
                        _logger.Warn($"SNAP - {exc.Message}:{requestedPath.PathAndAccount.Path}:{Bytes.ToHexString(nodeData)}");
                    }
                }
                else
                {
                    RetryAccountRefresh(requestedPath);
                }
            }

            _progressTracker.ReportAccountRefreshFinished();
        }

        private void RetryAccountRefresh(AccountWithStorageStartingHash requestedPath)
        {
            _progressTracker.EnqueueAccountRefresh(requestedPath.PathAndAccount, requestedPath.StorageStartingHash);
        }

        public void AddCodes(Keccak[] requestedHashes, byte[][] codes)
        {
            HashSet<Keccak> set = requestedHashes.ToHashSet();

            for (int i = 0; i < codes.Length; i++)
            {
                byte[] code = codes[i];
                Keccak codeHash = Keccak.Compute(code);

                if (set.Remove(codeHash))
                {
                    Interlocked.Add(ref Metrics.SnapStateSynced, code.Length);
                    _dbProvider.CodeDb.Set(codeHash, code);
                }
            }

            Interlocked.Add(ref Metrics.SnapSyncedCodes, codes.Length);

            _progressTracker.ReportCodeRequestFinished(set);
        }

        public void RetryRequest(SnapSyncBatch batch)
        {
            if (batch.AccountRangeRequest != null)
            {
                _progressTracker.ReportAccountRequestFinished();
            }
            else if (batch.StorageRangeRequest != null)
            {
                _progressTracker.ReportStorageRangeRequestFinished(batch.StorageRangeRequest);
            }
            else if (batch.CodesRequest != null)
            {
                _progressTracker.ReportCodeRequestFinished(batch.CodesRequest);
            }
            else if (batch.AccountsToRefreshRequest != null)
            {
                _progressTracker.ReportAccountRefreshFinished(batch.AccountsToRefreshRequest);
            }
        }

        public bool IsSnapGetRangesFinished() => _progressTracker.IsSnapGetRangesFinished();

        public void UpdatePivot()
        {
            _progressTracker.UpdatePivot();
        }
    }
}
