const path = require("path");
const webpack = require('webpack');
const HtmlWebpackPlugin = require("html-webpack-plugin");
const { CleanWebpackPlugin } = require("clean-webpack-plugin");
const MiniCssExtractPlugin = require("mini-css-extract-plugin");
const CssMinimizerPlugin = require("css-minimizer-webpack-plugin");
const TerserPlugin = require("terser-webpack-plugin");

function getTypes(list) {
    if (list === 'all')
        list = "styles,scripts,content";
    return list.split(',');
}

module.exports = {
    entry: {
        infiniteButts: path.join(__dirname, "Scripts/infinitebutts/index.ts"),
        input: path.join(__dirname, "Scripts/prompts/index.ts"),
        gradio: path.join(__dirname, "Scripts/prompts/gradio.ts"),
        prompt: path.join(__dirname, "Scripts/prompts/prompt.ts"),
        generate: path.join(__dirname, "Scripts/prompts/generate.ts"),
    },
    devtool: 'source-map',
    output: {
        path: path.resolve(__dirname, "wwwroot"),
        chunkFilename: "js/[name].js?ver=[contenthash]",
        filename: "js/[name].js?ver=[contenthash]",
        publicPath: "/",
        library: ["butts"],
        libraryTarget: "window",
    },
    experiments: {
        outputModule: false
    },
    watchOptions: {
        ignored: ['**/*.cshtml'],
    },
    resolve: {
        extensions: [".js", ".cshtml.js", ".ts", ".cshtml.ts", ".razor.js", ".razor.ts"],
        extensionAlias: {
            '.js': ['.ts', '.js']
        },
        modules: [
            path.resolve("./"),
            path.resolve(__dirname, "Scripts"),
            path.resolve(__dirname, "Pages"),
            path.resolve(__dirname, "node_modules"),
            path.resolve(__dirname, "Pages/Shared")
        ],
        fallback: {
            "zlib": require.resolve("browserify-zlib"),
//            "assert": require.resolve("assert/"),
            //            "buffer": require.resolve("buffer/"),
            "buffer": false,
            "util": require.resolve("util/"),
            //            "crypto": require.resolve(""),
            "crypto": false,
            "https": require.resolve("https-browserify"),
            "http": require.resolve("stream-http"),
            "url": require.resolve("url/"),
            "stream": require.resolve("stream-browserify"),
            // fixes proxy-agent dependencies
            net: false,
            dns: false,
            tls: false,
            assert: false,
            // fixes next-i18next dependencies
            path: false,
            fs: false,
            // fixes mapbox dependencies
            events: false,
            // fixes sentry dependencies
            process: false,
            react: false
        }
    },
    externals: {
        DotNet: '@microsoft/dotnet-js-interop',
    },
    optimization: {
        runtimeChunk: "single",
        splitChunks: {
            cacheGroups: {
                bootstrap: {
                    test: /[\\/]bootstrap[\\/]/,
                    name: "bootstrap",
                    reuseExistingChunk: true,
                    chunks: "all"
                },
                angular: {
                    test: /[\\/]node_modules[\\/]jquery/,
                    name: "jquery",
                    reuseExistingChunk: true,
                    chunks: "all"
                },
                vendors: {
                    test: /[\\/]node_modules[\\/]/,
                    name: "vendors",
                    reuseExistingChunk: true,
                    chunks: "initial",
                    minSize: 0,
                    priority: -20
                },
                default: {
                    reuseExistingChunk: true,
                },
                defaultVendors: false
            }
        },
        minimize: true,
        minimizer: [
            new TerserPlugin(),
            new CssMinimizerPlugin(),
        ],
    },
    module: {
        rules: [
            { test: /\.cshtml$/, use: 'ignore-loader' },
            {
                test: /\.ts$/,
                use: "ts-loader",
            },
            {
                test: /\.css$/,
                use: [{
                    loader: MiniCssExtractPlugin.loader,
                    options: {
                        esModule: false
                    }
                },
                {
                    loader: 'css-loader',
                    options: {
                        esModule: false
                    }
                }
                ],
            },
            {
                test: /\.(png|jp?g|gif|svg)(\?v=\d+\.\d+\.\d+)?$/,
                use: [
                    {
                        loader: "file-loader",
                        options: {
                            name: "[name].[ext]",
                            outputPath: (url, resourcePath, context) => {
                                // `resourcePath` is original absolute path to asset
                                // `context` is directory where stored asset (`rootContext`) or `context` option
                                // To get relative path you can use
                                const relativePath = path.relative(context, resourcePath);
                                var match = relativePath.match(/(node_modules|lib)[\\/](.*?)([\\/]|$)/);
                                if (match)
                                    return `img/${match[2]}/${url}`;
                                match = relativePath.match(/(Views)[\\/](.*?)([\\/]|$)/);
                                if (match)
                                    return `img/views/${match[2]}/${url}`;
                                else
                                    return `img/${url}`;
                            },

                        }
                    }
                ]
            },
        ]
    },
    plugins: [
        new CleanWebpackPlugin({
            cleanOnceBeforeBuildPatterns: [
                'css/**',
                'js/**',
                'img/**',
            ]
        }),
        new webpack.NormalModuleReplacementPlugin(/node:/, (resource) => {
            resource.request = resource.request.replace(/^node:/, "");
        }),
        new webpack.ProvidePlugin({
            $: 'jquery',
            jQuery: 'jquery',
        }),
        new MiniCssExtractPlugin({
            filename: "css/[name].css?[contenthash]",
            chunkFilename: "css/[name].css?[contenthash]",
            ignoreOrder: false
        }),
        new HtmlWebpackPlugin({
            inject: false,
            template: 'Pages/Shared/_HeadTemplate.html',
            filename: '_head.html'
        }),
        new HtmlWebpackPlugin({
            inject: false,
            template: 'Pages/Shared/_FootTemplate.html',
            filename: '_foot.html'
        })
    ],
};