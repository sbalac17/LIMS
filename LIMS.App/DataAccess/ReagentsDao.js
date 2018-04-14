import * as HttpClient from './HttpClient';
import Config from '../Config';

const baseUrl = Config.apiServer + "api/reagents/";

export function list(query) {
    if (query) {
        return HttpClient.get(`${baseUrl}?query=${encodeURIComponent(query)}`);
    } else {
        return HttpClient.get(baseUrl);
    }
}

export function create(values) {
    return HttpClient.post(baseUrl, values);
}

export function read(reagentId) {
    return HttpClient.get(`${baseUrl}/${reagentId}`);
}

export function update(reagentId, values) {
    return HttpClient.put(`${baseUrl}/${reagentId}`, values);
}

export function remove(reagentId) {
    return HttpClient.del(`${baseUrl}/${reagentId}`);
}
