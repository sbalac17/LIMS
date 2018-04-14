import * as HttpClient from './HttpClient';
import Config from '../Config';

const baseUrl = Config.apiServer + "api/labs";

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

export function read(labId) {
    return HttpClient.get(`${baseUrl}/${labId}`);
}

export function update(labId, values) {
    return HttpClient.put(`${baseUrl}/${labId}`, values);
}

export function remove(labId) {
    return HttpClient.del(`${baseUrl}/${labId}`);
}

export function samplesList(labId, query) {
    if (query) {
        return HttpClient.get(`${baseUrl}/${labId}/samples?query=${encodeURIComponent(query)}`);
    } else {
        return HttpClient.get(`${baseUrl}/${labId}/samples`);
    }
}

export function sampleDetails(labId, sampleId) {
    return HttpClient.get(`${baseUrl}/${labId}/samples/${sampleId}`);
}

export function postComment(labId, sampleId, commentObj) {
    return HttpClient.post(`${baseUrl}/${labId}/samples/${sampleId}/comment`, commentObj);
}
