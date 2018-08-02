import { fetchUtils } from 'react-admin';

import {
    GET_LIST,
    GET_ONE,
    GET_MANY,
    GET_MANY_REFERENCE,
    CREATE,
    UPDATE,
    DELETE,
} from './types';

const { queryParameters, fetchJson } = fetchUtils;

/**
 * Maps react-admin queries to sailsjs REST API
 *
 * @example
 * GET_LIST     => GET http://my.api.url/posts?sort='title ASC'&skip=20&limit=10&where={userId:34}
 * GET_ONE      => GET http://my.api.url/posts/123
 * GET_MANY     => GET http://my.api.url/posts?where={or: [{id:12}, {id:46}]}
 * UPDATE       => PATCH http://my.api.url/poCardTextsts/123
 * CREATE       => POST http://my.api.url/posts/123
 * DELETE       => DELETE http://my.api.url/posts/123
 */
export default (apiUrl, httpClient = fetchJson) => {
    /**
     * @param {String} type One of the constants appearing at the top if this file, e.g. 'UPDATE'
     * @param {String} resource Name of the resource to fetch, e.g. 'posts'
     * @param {Object} params The REST request params, depending on the type
     * @returns {Object} { url, options } The HTTP request parameters
     */
    const convertRESTRequestToHTTP = (type, resource, params) => {
        let url = '';
        const options = { };
        options.credentials = 'include';
        switch (type) {
        case GET_LIST: {
            const { page, perPage } = params.pagination;
            const { field, order } = params.sort;
            const query = {
                sort: `${field} ${order}`,
                skip: JSON.stringify((page - 1) * perPage),
                limit: JSON.stringify(perPage),
                where: JSON.stringify(params.filter),
            };
            url = `${apiUrl}/${resource}?${queryParameters(query)}`;
            break;
        }
        case GET_ONE:
            url = `${apiUrl}/${resource}/${params.id}`;
            break;
        case GET_MANY: {
            const ids = params.ids.map(id => ({ id }));
            const query = {
                where: JSON.stringify({ or: ids }),
            };
            url = `${apiUrl}/${resource}?${queryParameters(query)}`;
            break;
        }
        case GET_MANY_REFERENCE: {
            const { page, perPage } = params.pagination;
            const { field, order } = params.sort;
            const query = {
                sort: `${field} ${order}`,
                skip: JSON.stringify((page - 1) * perPage),
                limit: JSON.stringify(perPage),
                where: JSON.stringify({
                  ...params.filter,
                  [params.target]: params.id,
                }),
            };
            url = `${apiUrl}/${resource}?${queryParameters(query)}`;
            break;
        }
        case UPDATE:
            url = `${apiUrl}/${resource}/${params.id}`;
            options.method = 'PATCH';
            options.body = JSON.stringify(params.data);
            break;
        case CREATE:
            url = `${apiUrl}/${resource}`;
            options.method = 'POST';
            options.body = JSON.stringify(params.data);
            break;
        case DELETE:
            url = `${apiUrl}/${resource}/${params.id}`;
            options.method = 'DELETE';
            break;
        default:
            throw new Error(`Unsupported fetch action type ${type}`);
        }
        console.log('url: ', url, ' options: ', options);
        return { url, options };
    };

    /**
     * @param {Object} response HTTP response from fetch()
     * @param {String} type One of the constants appearing at the top if this file, e.g. 'UPDATE'
     * @param {String} resource Name of the resource to fetch, e.g. 'posts'
     * @param {Object} params The REST request params, depending on the type
     * @returns {Object} REST response
     */
    const convertHTTPResponseToREST = (response, type, resource, params) => {
        // const { headers, json } = response;
        const { json } = response;

        switch (type) {
        case GET_LIST:
        case GET_MANY_REFERENCE:
            return {
                data: json.data,
                total: json.total,
            };
        case CREATE:
            return { data: { ...params.data, id: json.id } };
        case GET_MANY:
            return {
              data: json.data,
            };
        default:
            return { data: json };
        }
    };

    /**
     * @param {string} type Request type, e.g GET_LIST
     * @param {string} resource Resource name, e.g. "posts"
     * @param {Object} payload Request parameters. Depends on the request type
     * @returns {Promise} the Promise for a REST response
     */
    return (type, resource, params) => {
        const { url, options } = convertRESTRequestToHTTP(type, resource, params);
        // console.log(options);
        return httpClient(url, options)
            .then(response => {
              // console.log(`response type: ${type} - resource: ${resource} - res: - params:`);
              // console.log(response);
              // console.log(params);
              return convertHTTPResponseToREST(response, type, resource, params)
            });
    };
};