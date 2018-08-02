/**
 * Module dependencies
 */
const actionUtil = require('sails/lib/hooks/blueprints/actionUtil');

/**
 * Find Records
 *
 * http://sailsjs.com/docs/reference/blueprint-api/find
 *
 * An API call to find and return model instances from the data adapter
 * using the specified criteria.  If an id was specified, just the instance
 * with that unique id will be returned.
 *
 */

module.exports = async function findRecords (req, res) {
  sails.log.debug('Find');

  var parseBlueprintOptions = req.options.parseBlueprintOptions || req._sails.config.blueprints.parseBlueprintOptions;

  // Set the blueprint action for parseBlueprintOptions.
  req.options.blueprintAction = 'find';

  var queryOptions = parseBlueprintOptions(req);
  var Model = req._sails.models[queryOptions.using];
  const modelName = queryOptions.using;

  Model
  .count()
  .exec(async (err, numberOfRecords) => {
    if (err) {
      switch (err.name) {
        case 'UsageError': return res.badRequest(err);
        default: return res.serverError(err);
      }
    }

    if (!numberOfRecords) {
      return res.json({
        total: 0,
        data: [],
      });
    }

    var generalQueryOptionsCriteria = queryOptions.criteria.where ?
    {
      where: queryOptions.criteria.where,
    } : {
      where: {},
    };
    // sails.log.debug('Find criteria:', queryOptions);
    sails.log.debug('Find options:', generalQueryOptionsCriteria);

    Model
      .find(generalQueryOptionsCriteria)
      .exec((err, allRecords) => {
        if (err) {
          switch (err.name) {
            case 'UsageError': return res.badRequest(err);
            default: return res.serverError(err);
          }
        }

        Model
        .find(queryOptions.criteria, queryOptions.populates)
        .meta(queryOptions.meta)
        .exec((err, matchingRecords) => {
          if (err) {
            switch (err.name) {
              case 'UsageError': return res.badRequest(err);
              default: return res.serverError(err);
            }
          }

          if (req._sails.hooks.pubsub && req.isSocket) {
            Model.subscribe(req, _.pluck(matchingRecords, Model.primaryKey));
            // Only `._watch()` for new instances of the model if
            // `autoWatch` is enabled.
            if (req.options.autoWatch) { Model._watch(req); }
            // Also subscribe to instances of all associated models
            _.each(matchingRecords, function (record) {
              actionUtil.subscribeDeep(req, record);
            });
          }

          const matchingRecordsNew = matchingRecords.map((record) => {
            Object.keys(queryOptions.populates).forEach((reference) => {
              // If reference is model not collection
              if(typeof queryOptions.populates[reference] === 'boolean' && record[reference]){
                record[reference] = record[reference].id;
              } else if (typeof queryOptions.populates[reference] === 'object') {
                const listId = record[reference].map((obj) => {
                  return obj.id;
                });
                record[reference] = listId;
              }
            });
            return record;
          });

          return res.json({
            total: allRecords.length,
            data: matchingRecordsNew,
          });
        });
      });
  });
};