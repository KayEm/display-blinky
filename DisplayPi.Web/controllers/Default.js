'use strict';

var url = require('url');


var Default = require('./DefaultService');


module.exports.getMessages = function getMessages (req, res, next) {
  Default.getMessages(req.swagger.params, res, next);
};

module.exports.sendMessage = function sendMessage (req, res, next) {
  Default.sendMessage(req.swagger.params, res, next);
};
