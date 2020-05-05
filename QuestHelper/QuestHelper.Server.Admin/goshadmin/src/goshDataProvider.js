import { fetchUtils } from 'react-admin';
import { stringify } from 'query-string';

//const apiUrl = 'http://localhost:31192/api/v2';
const apiUrl = process.env.REACT_APP_API_URL;
//const apiUrl = 'http://igosh.pro/api';

const httpClient = (url, options = {}) => {
    if (!options.headers) {
        options.headers = new Headers({ Accept: 'application/json' });
    }
    options.user = {
        authenticated: true,
        token: localStorage.getItem('token')
    };
    options.headers.set('Access-Control-Expose-Headers', 'x-total-count');
    return fetchUtils.fetchJson(url, options);
};

function getMappingByResource(json, resourceType){
    switch(resourceType)
    {
        case "routes":{
            return json.map(resource => ({id:resource.routeId, 
                name:resource.name,
                createDate:resource.createDate,
                creatorId:resource.creatorId,
                description:resource.description,
                version:resource.version,
                isDeleted:resource.isDeleted,
                isShared:resource.isShared,
                isPublished:resource.isPublished,
                imgFilename:resource.imgFilename,
                versionsList:resource.versionsList,
                coverImgBase64:resource.coverImgBase64
            }));
        };
        case "routepoints":{
            return json.map(resource => ({id:resource.routePointId,
                 name:resource.name,
                 createDate:resource.createDate,
                 updateDate:resource.updateDate,
                 updatedUserId:resource.updatedUserId,
                 routeId:resource.routeId,
                 latitude:resource.latitude,
                 longitude:resource.longitude,
                 address:resource.address,
                 version:resource.version,
                 isDeleted:resource.isDeleted,
                 description:resource.description
                }));
        };
        case "pois":{
            return json.map(resource => ({id:resource.id,
                 name:resource.name,
                 createDate:resource.createDate,
                 latitude:resource.latitude,
                 longitude:resource.longitude,
                 address:resource.address,
                 version:resource.version,
                 isDeleted:resource.isDeleted,
                 isPublished:resource.isPublished,
                 description:resource.description
                }));
        };
        break;
    }
}

function addIdByResourceType(paramsData, resourceType){
    switch(resourceType)
    {
        case "routes":{
            paramsData.routeId = paramsData.id;
            return paramsData;
        };
        case "routepoints":{
            paramsData.routePointId = paramsData.id;
            return paramsData;
        };
        break;
    }
    return paramsData;
}
export default {
    getList: (resource, params) => {
        const { page, perPage } = params.pagination;
        const { field, order } = params.sort;
        const query = {
            sort: JSON.stringify([field, order]),
            range: JSON.stringify([(page - 1) * perPage, page * perPage - 1]),
            filter: JSON.stringify(params.filter),
            pageSize: perPage
        };
        const url = `${apiUrl}/${resource}?${stringify(query)}`;
        return httpClient(url).then(({ headers, json }) => ({
            data: getMappingByResource(json, resource),
            total: parseInt(headers.get('x-total-count'), 10),
        }));
    },

    getOne: (resource, params) =>{
        return httpClient(`${apiUrl}/${resource}/${params.id}`).then(({ json }) => ({
            data: json,
        }));
    },

    getMany: (resource, params) => {
        const query = {
            filter: JSON.stringify({ id: params.ids }),
        };
        const url = `${apiUrl}/${resource}?${stringify(query)}`;
        return httpClient(url).then(({ json }) => ({ data: json }));
    },

    getManyReference: (resource, params) => {
        const { page, perPage } = params.pagination;
        const { field, order } = params.sort;
        const query = {
            sort: JSON.stringify([field, order]),
            range: JSON.stringify([(page - 1) * perPage, page * perPage - 1]),
            filter: JSON.stringify({
                ...params.filter,
                [params.target]: params.id,
            }),
        };
        const url = `${apiUrl}/${resource}?${stringify(query)}`;

        return httpClient(url).then(({ headers, json }) => ({
            data: json,
            total: parseInt(headers.get('content-range').split('/').pop(), 10),
        }));
    },

    update: (resource, params) => {        
        addIdByResourceType(params.data, resource);
        return httpClient(`${apiUrl}/${resource}/${params.id}`, {
            method: 'PUT',
            body: JSON.stringify(params.data),
        }).then(({ json }) => ({ data: '' }));
    },

    updateMany: (resource, params) => {
        const query = {
            filter: JSON.stringify({ id: params.ids}),
        };
        return httpClient(`${apiUrl}/${resource}?${stringify(query)}`, {
            method: 'PUT',
            body: JSON.stringify(params.data),
        }).then(({ json }) => ({ data: json }));
    },

    create: (resource, params) =>
        httpClient(`${apiUrl}/${resource}`, {
            method: 'POST',
            body: JSON.stringify(params.data),
        }).then(({ json }) => ({
            data:  ''
            //data: { ...params.data, id: json.id },
        })),

    delete: (resource, params) =>
        httpClient(`${apiUrl}/${resource}/${params.id}`, {
            method: 'DELETE',
        }).then(({ json }) => ({ data: json })),

    deleteMany: (resource, params) => {
        const query = {
            filter: JSON.stringify({ id: params.ids}),
        };
        return httpClient(`${apiUrl}/${resource}?${stringify(query)}`, {
            method: 'DELETE',
            body: JSON.stringify(params.data),
        }).then(({ json }) => ({ data: json }));
    }
};