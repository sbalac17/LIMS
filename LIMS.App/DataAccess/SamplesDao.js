import * as HttpClient from './HttpClient';
import Config from '../Config';

const baseUrl = Config.apiServer + "api/samples";

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

export function read(sampleId) {
    return HttpClient.get(`${baseUrl}/${sampleId}`);
}

export function update(sampleId, values) {
    return HttpClient.put(`${baseUrl}/${sampleId}`, values);
}

export function remove(sampleId) {
    return HttpClient.del(`${baseUrl}/${sampleId}`);
}
