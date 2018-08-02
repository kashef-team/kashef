/**
 * Pilgrim.js
 *
 * @description :: A model definition.  Represents a database table/collection/etc.
 * @docs        :: https://sailsjs.com/docs/concepts/models-and-orm/models
 */

const validator = require('validator');

const isUrlsArray = (value) => {
  for(let i=0; i<value.length; i+=1) {
    if (!validator.isURL(value[i].url)) {
      return false;
    }
  }
  return true;
};

module.exports = {

  attributes: {

    //  ╔═╗╦═╗╦╔╦╗╦╔╦╗╦╦  ╦╔═╗╔═╗
    //  ╠═╝╠╦╝║║║║║ ║ ║╚╗╔╝║╣ ╚═╗
    //  ╩  ╩╚═╩╩ ╩╩ ╩ ╩ ╚╝ ╚═╝╚═╝
    name: {
      type: 'string',
      required: true,
      example: 'Mohamed Mayla',
    },

    images: {
      type: 'json',
      custom: (value) => {
        return _.isArray(value) && value.length >= 1 && isUrlsArray(value);
      },
      required: true,
    },

    gender: {
      type: 'string',
      isIn: ['male', 'female'],
      required: true,
    },

    birthdate: {
      type: 'string',
      example: '681696000',
    },

    nationality: {
      type: 'string',
      example: 'Egypt',
    },

    type: {
      type: 'string',
      isIn: ['pilgrim', 'police', 'blacklisted'],
      defaultsTo: 'pilgrim',
    },

    isMissing: {
      type: 'boolean',
      defaultsTo: false,
    },

    found: {
      type: 'boolean',
      defaultsTo: false,
    },

    //  ╔═╗╔╦╗╔╗ ╔═╗╔╦╗╔═╗
    //  ║╣ ║║║╠╩╗║╣  ║║╚═╗
    //  ╚═╝╩ ╩╚═╝╚═╝═╩╝╚═╝


    //  ╔═╗╔═╗╔═╗╔═╗╔═╗╦╔═╗╔╦╗╦╔═╗╔╗╔╔═╗
    //  ╠═╣╚═╗╚═╗║ ║║  ║╠═╣ ║ ║║ ║║║║╚═╗
    //  ╩ ╩╚═╝╚═╝╚═╝╚═╝╩╩ ╩ ╩ ╩╚═╝╝╚╝╚═╝

  },

};

