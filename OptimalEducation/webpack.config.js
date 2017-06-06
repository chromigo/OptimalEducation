'use strict';
const path = require('path');
const NODE_ENV = process.env.NODE_ENV || 'development';
const webpack = require('webpack');
const node_dir = path.resolve(__dirname, 'node_modules');
const scripts_dir = path.resolve(__dirname, 'Scripts');
const ExtractTextPlugin = require("extract-text-webpack-plugin");
const rimraf = require('rimraf');

module.exports = {
  context: path.resolve(__dirname, 'frontend/develop'),

  entry: {
	common: './common',
	validation: './validation',
  },

  output: {
    path:     path.resolve(__dirname,'frontend/bundles'),
    filename: "[name].js"
  },

  watch: NODE_ENV == 'development',

  watchOptions: {
    aggregateTimeout: 100
  },

  devtool: NODE_ENV == 'development' ? "cheap-inline-module-source-map" : null,

  plugins: [
	{
      apply: (compiler) => {
        rimraf.sync(compiler.options.output.path);
      }
    },
    new webpack.NoErrorsPlugin(),
    new webpack.DefinePlugin({
      NODE_ENV: JSON.stringify(NODE_ENV)
    }),
    new webpack.optimize.CommonsChunkPlugin({
      name: "common"
    }),
	new webpack.ProvidePlugin({
            jQuery: "jquery",
			$: 'jquery'
        }),
	new ExtractTextPlugin('[name].css', {allChunks: true})
  ],

  resolve: {
    modulesDirectories: ['node_modules'],
    extensions:         ['', '.js'],
	alias: {
            jquery: path.resolve(node_dir, "jquery/src/jquery"), 
        }
  },  

  resolveLoader: {
    modulesDirectories: ['node_modules'],
    moduleTemplates:    ['*-loader', '*'],
    extensions:         ['', '.js']
  },


  module: {
    loaders: [{
      test:   /\.js$/,
      loader: 'babel',
	  query: {
             presets: ['es2015']
         },
	 include: path.resolve(__dirname,'frontend/develop')
    },{
		test: /\.css$/,
		loader: ExtractTextPlugin.extract('css')
    }, {
      test:   /\.(png|jpg|svg|ttf|eot|woff|woff2)$/,
      loader: 'file?name=[path][name].[ext]'
    }]
  }
};


if (NODE_ENV == 'production') {
  module.exports.plugins.push(
      new webpack.optimize.UglifyJsPlugin({
        compress: {
          // don't show unreachable variables etc
          warnings:     false,
          drop_console: true,
          unsafe:       true
        }
      })
  );
}
