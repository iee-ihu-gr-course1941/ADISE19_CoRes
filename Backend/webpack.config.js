const path = require('path')
const webpack = require('webpack')

module.exports = {
  entry: ['babel-polyfill', './mock-client/src/index.js'],
  mode: 'development',
  module: {
    rules: [
      {
        test: /\.(js|jsx)$/,
        exclude: /(node_modules|bower_components)/,
        loader: 'babel-loader',
        options: { presets: ['@babel/env'] }
      },
      {
        test: /\.css$/,
        use: ['style-loader', 'css-loader']
      }
    ]
  },
  resolve: { extensions: ['*', '.js', '.jsx'] },
  output: {
    path: path.resolve(__dirname, 'mock-client/dist/'),
    filename: 'bundle.js'
  },
  devtool: 'source-map',
  devServer: {
    contentBase: path.resolve(__dirname, 'mock-client/'),
    port: 3001,
    hotOnly: true,
    publicPath: 'http://localhost:3001/dist/'
  },
  plugins: [new webpack.HotModuleReplacementPlugin()]
}
