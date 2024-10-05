'use strict';
const MANIFEST = 'flutter-app-manifest';
const TEMP = 'flutter-temp-cache';
const CACHE_NAME = 'flutter-app-cache';

const RESOURCES = {"@gradio/client/index.d.ts": "fb08c5d6d9bc73ed5fe9153905aca587",
"@gradio/client/index.d.ts.map": "be9e5dfe372a2ec810104b513cea1f01",
"@gradio/client/index.js": "5587b30c59aca8199cb58d34f905e331",
"assets/AssetManifest.bin": "207016f3300caa6b18f284b43309fc8e",
"assets/AssetManifest.bin.json": "f0b3a17d0fb8c6b294d1f7c43066d51f",
"assets/AssetManifest.json": "97bc7b3abace3cbb6af8d7712a8a76b8",
"assets/assets/audio/camera.mp3": "eef2ae9cdcdc7e4daa3d2c08b4421dd7",
"assets/assets/backgrounds/blue_circle.png": "85a58493a130ef225984d32543bfb2ea",
"assets/assets/backgrounds/circle_object.png": "c3fc60d7fbd918163a755d008dc1b339",
"assets/assets/backgrounds/landing_background.jpg": "449b7c9488a77a9a94e6444c0dc3203a",
"assets/assets/backgrounds/photobooth_background.jpg": "83d1484d0879a224e4534908146dbb3b",
"assets/assets/backgrounds/photobooth_background.webp": "5e4435d5d1b7f1d19f4e5ecb8192da1b",
"assets/assets/backgrounds/photobooth_background_old.jpg": "83d1484d0879a224e4534908146dbb3b",
"assets/assets/backgrounds/red_box.png": "7c63437873ed4d2719260ea71e9ceffc",
"assets/assets/backgrounds/yellow_bar.png": "8add88e180c01adf792e6464d7a37bc3",
"assets/assets/backgrounds/yellow_plus.png": "1447836449833441a890d2d66900f8bd",
"assets/assets/file/prompts.txt": "ee7db68f78c4f093c480c807a6716da8",
"assets/assets/fonts/PlayfairDisplay-Italic.ttf": "c6bd3951c4797106c1dac753a795534a",
"assets/assets/fonts/PlayfairDisplay.ttf": "5c26cb0a6cef324c460754822591bd93",
"assets/assets/icons/android_icon.png": "7a959290a5b1131dc1774cffc5064efe",
"assets/assets/icons/camera_button_icon.png": "d57aedc4200924996a11686d1ed27b8c",
"assets/assets/icons/custom_icon.png": "d35ce7ee8e5b8107dfe5bb31e43b038a",
"assets/assets/icons/dash_icon.png": "32f278d505765547c000486d22dfe1f7",
"assets/assets/icons/delete_icon.png": "89e830952c4e0115deacc9ee7f7d4553",
"assets/assets/icons/dino_icon.png": "d876fbd329ceb41719c8e3fb800bf09d",
"assets/assets/icons/eyewear_icon.png": "2af05ce331c7a302b4272df945959e25",
"assets/assets/icons/firebase_icon.png": "b9c992f2013dbe31c03b7c2ea07fdf47",
"assets/assets/icons/flutter_icon.png": "d49b7036e39b0272562db977d7afb357",
"assets/assets/icons/food_icon.png": "8024439402f89fc233a34eb8cb25ad81",
"assets/assets/icons/google_icon.png": "1aa99c190fb74a4ba6d9985283292b54",
"assets/assets/icons/go_next_button_icon.png": "64c6ffb16ddfa36b90c4b18a637bc031",
"assets/assets/icons/hats_icon.png": "5c00b92a73e466f2303a84ac78b17252",
"assets/assets/icons/neon_flower.png": "7e88c6d5a46df03632c2ba4a2e673187",
"assets/assets/icons/neon_flower_2.png": "591cb4b0f795600e7ec2ea83159216af",
"assets/assets/icons/retake_button_icon.png": "44bfab882c6b556afb34fe07ba7b50b4",
"assets/assets/icons/shapes_icon.png": "b2b84a04e93b75285c9bb720a73daac8",
"assets/assets/icons/sparky_icon.png": "8bdd9793ea6752d028ec221972ff82aa",
"assets/assets/icons/stickers_button_icon.png": "7b64efc9b64afe668a62cede7275ffb1",
"assets/assets/icons/stickers_button_icon_stars.png": "5e5a317db3bc05177908afb9af53be87",
"assets/assets/images/android.png": "be514af38d2ecce15b150ca124a70499",
"assets/assets/images/android_spritesheet.png": "d9e7470f57e6c793098d21fdc8fe8b3f",
"assets/assets/images/dash.png": "cbb0d6bb1baef4a67f88f2a5bb65e1ff",
"assets/assets/images/dash_spritesheet.png": "f0412af469b2fea1f34dacd1125a564b",
"assets/assets/images/dino.png": "52579b8f3946ea76354e0f3f3afb08ef",
"assets/assets/images/dino_spritesheet.png": "aee0d9da6e571df2ddd6c59b98e45fab",
"assets/assets/images/error_photo_desktop.png": "a8de7cba1f1a5922a62726cd4118677d",
"assets/assets/images/error_photo_mobile.png": "a8de7cba1f1a5922a62726cd4118677d",
"assets/assets/images/photo_frame_download.png": "80067e13b55164d17c6a177d8797b9d7",
"assets/assets/images/photo_frame_mobile_download.png": "f259c576c152cdf4817c86fc7654de80",
"assets/assets/images/photo_frame_spritesheet_landscape.jpg": "19bbd9a5ad23baff841a19bc0c586b21",
"assets/assets/images/photo_frame_spritesheet_portrait.png": "5402d46af894214ec52980bd620a7730",
"assets/assets/images/photo_indicator_spritesheet.png": "5eb589e14bc9b29b9515e85d717e7715",
"assets/assets/images/photo_placeholder.png": "dbd2926d6282e086d9772a94fbade956",
"assets/assets/images/sparky.png": "96b3a85907caa64b783432f94ea6ce2c",
"assets/assets/images/sparky_spritesheet.png": "c8e571e51d490933db6783597cdc95f9",
"assets/assets/images/train_smooch.jpg": "29ee77e21f885b9a9c68f69d800bc89b",
"assets/assets/images/train_smooch2.jpg": "449b7c9488a77a9a94e6444c0dc3203a",
"assets/assets/props/custom/photo0092.png": "5ea8b2e31c2852c78ffdc352d1ab7c4e",
"assets/assets/props/custom/photo0093.png": "9475f9779b9d64990d727d5723fd50aa",
"assets/assets/props/custom/photo0094.png": "5339cf6481cce06aab0b3d215157e2d7",
"assets/assets/props/custom/photo0095.png": "24c1e09b9fc0d859cb0c06baea57f015",
"assets/assets/props/custom/photo0096.png": "ee697a141e84e5d567c1c1f7a89c88bb",
"assets/assets/props/custom/photo0097.png": "8d4b72e42672750af4a3fe7774f6347d",
"assets/assets/props/custom/photo0098.png": "80bb58499866e4f2d446f3669c074682",
"assets/assets/props/custom/photo0099.png": "f5240d4413ecda268f23ed9d7c0d6e6a",
"assets/assets/props/custom/photo0100.png": "450868948fc3a94df704cdfda1f1938c",
"assets/assets/props/custom/photo0101.png": "9ba19cc0221a95fac4512114ebac11c5",
"assets/assets/props/custom/photo0102.png": "7c971737a8f8b4b18ea1d150c7926729",
"assets/assets/props/custom/photo0103.png": "8c254df2ed18cb4107696959a12af9b4",
"assets/assets/props/custom/photo0104.png": "50943d45703492ca2f5156f36b9f4e82",
"assets/assets/props/custom/photo0105.png": "d726242542038d87d9d7c8060d750487",
"assets/assets/props/custom/photo0106.png": "c56c361521e4e54de71a2ae44568c49e",
"assets/assets/props/custom/photo0107.png": "8baa56f25026d53aadf36955595c102f",
"assets/assets/props/custom/photo0108.png": "e5a6df3067ffbc367f5563ce593c8471",
"assets/assets/props/custom/photo0109.png": "1f9c5fa1e049e1fad7820f31a6b78cdc",
"assets/assets/props/custom/photo0110.png": "623adb492d9e5e7a49347894f00611b8",
"assets/assets/props/custom/photo0111.png": "822e32a1a8b0af2f2381b341a02cbcdb",
"assets/assets/props/custom/photo0112.png": "4b5db1ae4f4fd488eff06de3bab54a7e",
"assets/assets/props/custom/photo0113.png": "3976be4d093320f5ebb3d586105e8390",
"assets/assets/props/custom/photo0114.png": "044d4d003a8477fdb5abcdbd3401765a",
"assets/assets/props/custom/photo0115.png": "b94eeb6b3162599a31d93fe2d4c52cbc",
"assets/assets/props/custom/photo0116.png": "2dda932b13b4c77ed1ffac28e8069950",
"assets/assets/props/custom/photo0117.png": "544e9788eb38a5cf9d080aef60c073d7",
"assets/assets/props/custom/photo0118.png": "0fce167fc4fc79c461df7e37ae5ce3cb",
"assets/assets/props/custom/photo0119.png": "1608f7e2583ecbc0a66b400646bda1ff",
"assets/assets/props/custom/photo0120.png": "491d0cdfb95fdef2a105014eda395459",
"assets/assets/props/custom/photo0121.png": "11003f6c8b448670ad5088e6be356255",
"assets/assets/props/custom/photo0122.png": "34c3a5c90de8e9c7806290982f5df9dc",
"assets/assets/props/custom/photo0123.png": "c3c03dda48be154b1fb9f5820c316cef",
"assets/assets/props/custom/photo0124.png": "58e8507e328e9859c40d1be65be3381a",
"assets/assets/props/custom/photo0125.png": "2f91e594b2bd8392be704e2e1df93e60",
"assets/assets/props/custom/photo0126.png": "d01b13f764f6cc19cd88fdeefae46b6d",
"assets/assets/props/custom/photo0127.png": "9cd1ec464ce59a4057697dc11d98f48a",
"assets/assets/props/custom/photo0128.png": "51c2ad5076554b147476e1dc93d99141",
"assets/assets/props/custom/photo0129.png": "ebe74c4a4f35c2d74b826978bfd7c30c",
"assets/assets/props/custom/photo0130.png": "7bb6e56bebc40f818617e8db44de118e",
"assets/assets/props/custom/photo0131.png": "d537ece492cdead0dee75484595bc610",
"assets/assets/props/custom/photo0132.png": "c92d19599c8ee8175b928bb3561641d9",
"assets/assets/props/custom/photo0133.png": "a8caf9d8bbcb97f864b2dc95fa861128",
"assets/assets/props/custom/photo0134.png": "f6f811fea6eaab0122ade95f57470c87",
"assets/assets/props/custom/photo0135.png": "34a84bafb5c6dd5d36e6e0a8cadfb2bb",
"assets/assets/props/custom/photo0136.png": "470b9bdae8ba2842a6e3f1549a07cfcf",
"assets/assets/props/custom/photo0137.png": "8c634277dab8a99073f5e028723ef401",
"assets/assets/props/custom/photo0138.png": "c2e34aede85d6b1f55dca95a4c13c36a",
"assets/assets/props/custom/photo0139.png": "c5ab1cde3c531b4de54dba0e7a4cc4ff",
"assets/assets/props/custom/photo0140.png": "9475f9779b9d64990d727d5723fd50aa",
"assets/assets/props/custom/photo0141.png": "3fd0e15173a6ac31f310b8d2e4717df2",
"assets/assets/props/custom/photo0142.png": "34c3a5c90de8e9c7806290982f5df9dc",
"assets/assets/props/custom/photo0143.png": "fc31bf71eca054c22a1d5fe6c4d41bda",
"assets/assets/props/custom/photo0144.png": "45f0bec065566552845dc07b360d9136",
"assets/assets/props/custom/photo0145.png": "a18f33670412812a0ee6b9bea3b2d380",
"assets/assets/props/custom/photo0146.png": "8e3d3673e4979cff842f976b823d7082",
"assets/assets/props/custom/photo0147.png": "a9d87de3aacca5f87b25faed0f7b288a",
"assets/assets/props/custom/photo0148.png": "f0a1b695e53bf77f4155aeba070636e2",
"assets/assets/props/custom/photo0149.png": "2af295ccfffb46fcae42275ded718610",
"assets/assets/props/custom/photo0150.png": "059dbde26e428a9172a9c17e78dc1a7c",
"assets/assets/props/custom/photo0151.png": "5d3dc7d25887cdcf579030eb332f7f20",
"assets/assets/props/custom/photo0152.png": "c5ab1cde3c531b4de54dba0e7a4cc4ff",
"assets/assets/props/custom/photo0153.png": "a3bb0ff2a89d1145b4f628a9154728f4",
"assets/assets/props/custom/photo0154.png": "ee697a141e84e5d567c1c1f7a89c88bb",
"assets/assets/props/custom/photo0155.png": "2adde6f801dd1a7b2ae1e7cc891baffa",
"assets/assets/props/custom/photo0156.png": "49c00bb1032f55d6e91f1de05b8b40db",
"assets/assets/props/custom/photo0157.png": "49c00bb1032f55d6e91f1de05b8b40db",
"assets/assets/props/custom/photo0158.png": "774999401120b23ce724fe600090b65d",
"assets/assets/props/custom/photo0159.png": "89a6f4521823a27796e76e263d3a9c76",
"assets/assets/props/custom/photo0160.png": "b849a37d3f6a3e859315b60b99f7a4fe",
"assets/assets/props/custom/photo0161.png": "b30f26719822ecb35f74ca5b80b6edb5",
"assets/assets/props/custom/photo0162.png": "c4f3c61bbcdccc6197a15a23a83507d0",
"assets/assets/props/custom/photo0163.png": "f0fb148186fd89ef05ff1e0703e9308b",
"assets/assets/props/custom/photo0164.png": "7f4b8f5e477e1268f7b5b80f65b2bc80",
"assets/assets/props/custom/photo0165.png": "ef273408622a73cf3aedd8429375431c",
"assets/assets/props/custom/photo0166.png": "e4c628a2f55db9c9e5c1b7b633fe537e",
"assets/assets/props/custom/photo0167.png": "fd179760b8cceaf24f130549b8c0b62c",
"assets/assets/props/custom/photo0168.png": "97c60fb4dca3e8a2fd762660d78391f1",
"assets/assets/props/custom/photo0169.png": "2f91e594b2bd8392be704e2e1df93e60",
"assets/assets/props/custom/photo0170.png": "91794c45fd596c8f785797ff5bb144f8",
"assets/assets/props/custom/photo0171.png": "b23f0c600042d7ea6fcf6e260ca0fd7b",
"assets/assets/props/custom/photo0172.png": "6a7d68e428b18ce5fdc8c27025e27b47",
"assets/assets/props/custom/photo0173.png": "76c6a176fe2266b5ea858fca842e2430",
"assets/assets/props/custom/photo0174.png": "7a41552f56a251e5adb06f3f53be0963",
"assets/assets/props/custom/photo0175.png": "b386f23124ad314eb3c8a6780438390b",
"assets/assets/props/custom/photo0176.png": "07a7024e9c3262de91c6b127ae9ba82b",
"assets/assets/props/custom/photo0177.png": "6440a45369cf5db441f72fc0767416b1",
"assets/assets/props/custom/photo0178.png": "abe5046fe7e6018588b24d36f3192388",
"assets/assets/props/custom/photo0179.png": "81414da261d2daf33e905172b4d3896f",
"assets/assets/props/custom/photo0180.png": "363e3806a8b6db0434e36a20070ad63f",
"assets/assets/props/custom/photo0181.png": "8d4b72e42672750af4a3fe7774f6347d",
"assets/assets/props/custom/photo0182.png": "92576cf85275bab55f8675138a39c3f2",
"assets/assets/props/custom/photo0183.png": "8cf3dcae231e80e5d72471e932386155",
"assets/assets/props/eyewear/01_eyewear_v1.png": "aeb7f3069137cc1c7ea6086dda12c6a2",
"assets/assets/props/eyewear/02_eyewear_v1.png": "c059500cf21cb90d3700994f5eaac3a3",
"assets/assets/props/eyewear/03_eyewear_v1.png": "943fcb8ab12844d0eb2e1d96a176db67",
"assets/assets/props/eyewear/04_eyewear_v1.png": "ccd8fa844e8673c1bdb7b06f482411c1",
"assets/assets/props/eyewear/05_eyewear_v1.png": "20a935e7a36deb724991cdd430ea4398",
"assets/assets/props/eyewear/06_eyewear_v1.png": "5e5a317db3bc05177908afb9af53be87",
"assets/assets/props/eyewear/07_eyewear_v1.png": "e0f1857d6c9646d150f260d18ecd9557",
"assets/assets/props/eyewear/08_eyewear_v1.png": "d6f29369905e8d5de288ceda4c23ac39",
"assets/assets/props/eyewear/09_eyewear_v1.png": "ba1c6b165a59559447e9b302c31d0ea4",
"assets/assets/props/eyewear/11_eyewear_v1.png": "1a6901f83c9d95006aeb0c7408e8f87b",
"assets/assets/props/eyewear/12_eyewear_v1.png": "bdc2b88ed05e4356b70f4ac497f1b86e",
"assets/assets/props/eyewear/13_eyewear_v1.png": "c465f4185d06692fa916d0780e1829e0",
"assets/assets/props/eyewear/15_eyewear_v1.png": "ebb4b6fc53e79a48d44c1e05d6fa4d69",
"assets/assets/props/eyewear/16_eyewear_v1.png": "9fb6d9358431bcf7c7b2b06008103342",
"assets/assets/props/food/03_food_v1.png": "a71f5042d92b8eb5a13d7a175d9ceac2",
"assets/assets/props/food/05_food_v1.png": "14b1fc2000273d3f8027c3b51a6c26f9",
"assets/assets/props/food/06_food_v1.png": "f9a895fead9735eee3ec95e6890a72ea",
"assets/assets/props/food/07_food_v1.png": "e09bae81b79c048549526bcd63292b84",
"assets/assets/props/food/09_food_v1.png": "25ab228fc4ac72e7e690b9964aac08fa",
"assets/assets/props/food/10_food_v1.png": "f922250f5d6be489a048c82c7e22887f",
"assets/assets/props/food/12_food_v1.png": "1548486ca954e88b99a9957abc2c74fe",
"assets/assets/props/food/13_food_v1.png": "7179565d58b3a3209d23f3a9a4aa63f4",
"assets/assets/props/food/14_food_v1.png": "b4acb89201f26cca8c3a171f2c81f54f",
"assets/assets/props/food/15_food_v1.png": "94aa11070246e49a549ef431805e4023",
"assets/assets/props/food/17_food_v1.png": "d41be5fe373ff0dd5b16ab63bc375fe2",
"assets/assets/props/food/19_food_v1.png": "e18323b969ea2358fa257ffaedb114eb",
"assets/assets/props/food/20_food_v1.png": "1cfec8dc072cd080d9d355cc924bad9f",
"assets/assets/props/food/21_food_v1.png": "5378510a1be848d4c1e12609812e1c41",
"assets/assets/props/food/23_food_v1.png": "629b228c897f8a605d5145957373f568",
"assets/assets/props/food/25_food_v1.png": "7866df8472f5d9d973138bda0cec6cfc",
"assets/assets/props/hats/02_hats_v1.png": "6a143aea091f233b723fa158d612b33a",
"assets/assets/props/hats/04_hats_v1.png": "2f75bd80ca497702124b0f75af74ff16",
"assets/assets/props/hats/05_hats_v1.png": "98cc69051bd2988ee89763d515659d1a",
"assets/assets/props/hats/06_hats_v1.png": "2b49c53755244085c738a3c8340af49a",
"assets/assets/props/hats/07_hats_v1.png": "82eb8ad04ea08419eba6ef92aadb1495",
"assets/assets/props/hats/08_hats_v1.png": "364e1cd7f017a7083132785b399236f8",
"assets/assets/props/hats/09_hats_v1.png": "d1214cb5816bd733a1fcef44c777404f",
"assets/assets/props/hats/10_hats_v1.png": "845932332017f0e2894738368306a85a",
"assets/assets/props/hats/11_hats_v1.png": "4188259680a5c94655ce38c70d7a8a9d",
"assets/assets/props/hats/12_hats_v1.png": "e3e2da1cd7b86f1bf02d3ebb2d5060a7",
"assets/assets/props/hats/13_hats_v1.png": "a8ab56c7443ad99b9a7f45fdc6dc555c",
"assets/assets/props/hats/14_hats_v1.png": "df93c77e2c091e62869c3e3230c5f77e",
"assets/assets/props/hats/15_hats_v1.png": "f9b19c9cf3980cebc602a95e7c163dc5",
"assets/assets/props/hats/16_hats_v1.png": "b0a0aa8ae836ef5be52afeadbb3bd138",
"assets/assets/props/hats/17_hats_v1.png": "cbb7a45fecb27a3787ecffd24a9e5ba0",
"assets/assets/props/hats/18_hats_v1.png": "cee544f47354d535ade57dc8ed183f76",
"assets/assets/props/hats/19_hats_v1.png": "df78c87c22acce55205e65f0c053b071",
"assets/assets/props/hats/20_hats_v1.png": "797303861d6c6e3d56085634b00f9136",
"assets/assets/props/hats/23_hats_v1.png": "5b338763f17a7e14c1ce364d0b1e4ba0",
"assets/assets/props/hats/24_hats_v1.png": "b11e6a1650c69ca0bfaed8680ce8573a",
"assets/assets/props/hats/27_hats_v1.png": "6eb91d9a029360bd4398998bc1ff8d57",
"assets/assets/props/shapes/01_shapes_v1.png": "04d8a86eada42219bdafbf143ca44d8b",
"assets/assets/props/shapes/03_shapes_v1.png": "e8aa971b4ab033b1c1cbfba6fe9f8676",
"assets/assets/props/shapes/04_shapes_v1.png": "2495b65b38b84779d1c6efe072b4a424",
"assets/assets/props/shapes/05_shapes_v1.png": "afd3f1140728c567091c15d43531544f",
"assets/assets/props/shapes/06_shapes_v1.png": "87dc57707264564e371bcd0138d3d326",
"assets/assets/props/shapes/07_shapes_v1.png": "2538282caebfb002725f2058afa9bada",
"assets/assets/props/shapes/08_shapes_v1.png": "ae20eb5d71dd94fd459eb804aac86ca3",
"assets/assets/props/shapes/09_shapes_v1.png": "c8d641f50d2d5800230a13ff89148a70",
"assets/assets/props/shapes/10_shapes_v1.png": "aa92b5dfeeee205e39a3cffc32a96c67",
"assets/assets/props/shapes/11_shapes_v1.png": "b79f8c2dbfb56e6669ac47fac25fe9f2",
"assets/assets/props/shapes/12_shapes_v1.png": "fb7bbfcc2a649c586e5ade6c7c998cda",
"assets/assets/props/shapes/13_shapes_v1.png": "7d99f0acc8f86f1b2d5ae4c9db3653db",
"assets/assets/props/shapes/14_shapes_v1.png": "c762c1e588b39b19e523748cb2d55962",
"assets/assets/props/shapes/16_shapes_v1.png": "8724ca8bd5e00bb9cbf305e1290c8583",
"assets/assets/props/shapes/17_shapes_v1.png": "1bd57d3d20885591120c7d5cb6927f6d",
"assets/assets/props/shapes/18_shapes_v1.png": "897117c59c141339ab75c593422fa00f",
"assets/assets/props/shapes/19_shapes_v1.png": "60fa08eca69dce8821e171dfee872e71",
"assets/assets/props/shapes/20_shapes_v1.png": "6146a360c76daaac9b1d7a8d4eee1ef8",
"assets/assets/props/shapes/21_shapes_v1.png": "2d8628f405ca209e3ad220859d79b50e",
"assets/assets/props/shapes/22_shapes_v1.png": "afbd1d12f958dd9bdfb3e734845d90b6",
"assets/assets/props/shapes/23_shapes_v1.png": "788bc961907e558ab6f33eb6c3a217ff",
"assets/assets/props/shapes/24_shapes_v1.png": "679726a5f43233bc5befda9e316dec0d",
"assets/assets/props/shapes/25_shapes_v1.png": "03a8c63656738d271371c6676fd4e4db",
"assets/FontManifest.json": "cf83ab9e01a71db6cc85b652972d40b4",
"assets/fonts/MaterialIcons-Regular.otf": "497e1f4ae94c441ce34c96c218b92a6c",
"assets/NOTICES": "54463f9d7e71efd97701c66356e6cc2c",
"assets/packages/fluttertoast/assets/toastify.css": "a85675050054f179444bc5ad70ffc635",
"assets/packages/fluttertoast/assets/toastify.js": "56e2c9cedd97f10e7e5f1cebd85d53e3",
"assets/shaders/ink_sparkle.frag": "ecc85a2e95f5e9f53123dcaf8cb9b6ce",
"canvaskit/canvaskit.js": "66177750aff65a66cb07bb44b8c6422b",
"canvaskit/canvaskit.js.symbols": "48c83a2ce573d9692e8d970e288d75f7",
"canvaskit/canvaskit.wasm": "1f237a213d7370cf95f443d896176460",
"canvaskit/chromium/canvaskit.js": "671c6b4f8fcc199dcc551c7bb125f239",
"canvaskit/chromium/canvaskit.js.symbols": "a012ed99ccba193cf96bb2643003f6fc",
"canvaskit/chromium/canvaskit.wasm": "b1ac05b29c127d86df4bcfbf50dd902a",
"canvaskit/skwasm.js": "694fda5704053957c2594de355805228",
"canvaskit/skwasm.js.symbols": "262f4827a1317abb59d71d6c587a93e2",
"canvaskit/skwasm.wasm": "9f0c0c02b82a910d12ce0543ec130e60",
"canvaskit/skwasm.worker.js": "89990e8c92bcb123999aa81f7e203b1c",
"favicon.png": "22b5a749979fc0d9b8d3d510dca40a38",
"favicon64.png": "62ca48240116946063f9d0dba02fefd9",
"flutter.js": "f393d3c16b631f36852323de8e583132",
"flutter_bootstrap.js": "19ed02d6dd32eb06e6ca1cabe30295ba",
"icons/Icon-192.png": "8b55c1719ca10c0091ed0caff3420061",
"icons/Icon-512.png": "60079887e479a84f734bbccf865af40b",
"icons/Icon-maskable-192.png": "359a233825b5c46e6cbbc22837ac9bf0",
"icons/Icon-maskable-512.png": "2d49db6482764a8c4a9bbd2242d5b0c1",
"index.html": "dccbf9692b3c263c7a0c361c813304ab",
"/": "dccbf9692b3c263c7a0c361c813304ab",
"index.html.template": "46547a4f8d56b7469237574e6c91221a",
"js/@gradio/client/dist/index.js": "9b2ceafe68c570662b54b15801d3b4c7",
"js/@gradio/client/index.d.ts": "fb08c5d6d9bc73ed5fe9153905aca587",
"js/@gradio/client/index.d.ts.map": "be9e5dfe372a2ec810104b513cea1f01",
"js/@gradio/client/index.js": "9b2ceafe68c570662b54b15801d3b4c7",
"js/gradio.d.ts": "152ae1f09bc6a3e7d50173f6c51b366b",
"js/gradio.js": "4b95a2f37d446247f950d8e17a0a28f6",
"js/gradio.js.map": "23b0eb12543dbe8574a6d610e440f2d0",
"js/tsconfig.tsbuildinfo": "56884e6586485e0e4a302ec707fbe6f8",
"main.dart.js": "77ce6a6546bfb7960fc34871b3c611e9",
"manifest.json": "25ab75dfbaf3dca2d0e5491a9dc2c245",
"tsconfig.tsbuildinfo": "5175392d242b15a84b3a3b8959e6204f",
"version.json": "45b2add39f4e14ceef3e908c3e04a7f0",
"__/firebase/8.4.1/firebase-app.js": "919709b4639c4e4a3f1e98de6fdee033",
"__/firebase/8.4.1/firebase-auth.js": "d3ea03773990125ee237ef37e0c29043",
"__/firebase/8.4.1/firebase-performance.js": "7ed1e155e9d6993063968f562ef866db",
"__/firebase/8.4.1/firebase-storage.js": "a571da14943032d36d3145d980ded5e1",
"__/firebase/init.js": "2621e4dc25da30dcffb2e221e0bb0b8b"};
// The application shell files that are downloaded before a service worker can
// start.
const CORE = ["main.dart.js",
"index.html",
"flutter_bootstrap.js",
"assets/AssetManifest.bin.json",
"assets/FontManifest.json"];

// During install, the TEMP cache is populated with the application shell files.
self.addEventListener("install", (event) => {
  self.skipWaiting();
  return event.waitUntil(
    caches.open(TEMP).then((cache) => {
      return cache.addAll(
        CORE.map((value) => new Request(value, {'cache': 'reload'})));
    })
  );
});
// During activate, the cache is populated with the temp files downloaded in
// install. If this service worker is upgrading from one with a saved
// MANIFEST, then use this to retain unchanged resource files.
self.addEventListener("activate", function(event) {
  return event.waitUntil(async function() {
    try {
      var contentCache = await caches.open(CACHE_NAME);
      var tempCache = await caches.open(TEMP);
      var manifestCache = await caches.open(MANIFEST);
      var manifest = await manifestCache.match('manifest');
      // When there is no prior manifest, clear the entire cache.
      if (!manifest) {
        await caches.delete(CACHE_NAME);
        contentCache = await caches.open(CACHE_NAME);
        for (var request of await tempCache.keys()) {
          var response = await tempCache.match(request);
          await contentCache.put(request, response);
        }
        await caches.delete(TEMP);
        // Save the manifest to make future upgrades efficient.
        await manifestCache.put('manifest', new Response(JSON.stringify(RESOURCES)));
        // Claim client to enable caching on first launch
        self.clients.claim();
        return;
      }
      var oldManifest = await manifest.json();
      var origin = self.location.origin;
      for (var request of await contentCache.keys()) {
        var key = request.url.substring(origin.length + 1);
        if (key == "") {
          key = "/";
        }
        // If a resource from the old manifest is not in the new cache, or if
        // the MD5 sum has changed, delete it. Otherwise the resource is left
        // in the cache and can be reused by the new service worker.
        if (!RESOURCES[key] || RESOURCES[key] != oldManifest[key]) {
          await contentCache.delete(request);
        }
      }
      // Populate the cache with the app shell TEMP files, potentially overwriting
      // cache files preserved above.
      for (var request of await tempCache.keys()) {
        var response = await tempCache.match(request);
        await contentCache.put(request, response);
      }
      await caches.delete(TEMP);
      // Save the manifest to make future upgrades efficient.
      await manifestCache.put('manifest', new Response(JSON.stringify(RESOURCES)));
      // Claim client to enable caching on first launch
      self.clients.claim();
      return;
    } catch (err) {
      // On an unhandled exception the state of the cache cannot be guaranteed.
      console.error('Failed to upgrade service worker: ' + err);
      await caches.delete(CACHE_NAME);
      await caches.delete(TEMP);
      await caches.delete(MANIFEST);
    }
  }());
});
// The fetch handler redirects requests for RESOURCE files to the service
// worker cache.
self.addEventListener("fetch", (event) => {
  if (event.request.method !== 'GET') {
    return;
  }
  var origin = self.location.origin;
  var key = event.request.url.substring(origin.length + 1);
  // Redirect URLs to the index.html
  if (key.indexOf('?v=') != -1) {
    key = key.split('?v=')[0];
  }
  if (event.request.url == origin || event.request.url.startsWith(origin + '/#') || key == '') {
    key = '/';
  }
  // If the URL is not the RESOURCE list then return to signal that the
  // browser should take over.
  if (!RESOURCES[key]) {
    return;
  }
  // If the URL is the index.html, perform an online-first request.
  if (key == '/') {
    return onlineFirst(event);
  }
  event.respondWith(caches.open(CACHE_NAME)
    .then((cache) =>  {
      return cache.match(event.request).then((response) => {
        // Either respond with the cached resource, or perform a fetch and
        // lazily populate the cache only if the resource was successfully fetched.
        return response || fetch(event.request).then((response) => {
          if (response && Boolean(response.ok)) {
            cache.put(event.request, response.clone());
          }
          return response;
        });
      })
    })
  );
});
self.addEventListener('message', (event) => {
  // SkipWaiting can be used to immediately activate a waiting service worker.
  // This will also require a page refresh triggered by the main worker.
  if (event.data === 'skipWaiting') {
    self.skipWaiting();
    return;
  }
  if (event.data === 'downloadOffline') {
    downloadOffline();
    return;
  }
});
// Download offline will check the RESOURCES for all files not in the cache
// and populate them.
async function downloadOffline() {
  var resources = [];
  var contentCache = await caches.open(CACHE_NAME);
  var currentContent = {};
  for (var request of await contentCache.keys()) {
    var key = request.url.substring(origin.length + 1);
    if (key == "") {
      key = "/";
    }
    currentContent[key] = true;
  }
  for (var resourceKey of Object.keys(RESOURCES)) {
    if (!currentContent[resourceKey]) {
      resources.push(resourceKey);
    }
  }
  return contentCache.addAll(resources);
}
// Attempt to download the resource online before falling back to
// the offline cache.
function onlineFirst(event) {
  return event.respondWith(
    fetch(event.request).then((response) => {
      return caches.open(CACHE_NAME).then((cache) => {
        cache.put(event.request, response.clone());
        return response;
      });
    }).catch((error) => {
      return caches.open(CACHE_NAME).then((cache) => {
        return cache.match(event.request).then((response) => {
          if (response != null) {
            return response;
          }
          throw error;
        });
      });
    })
  );
}
