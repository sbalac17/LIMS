import * as HttpClient from './HttpClient';
import Config from '../Config';

const baseUrl = Config.apiServer + "api/tests";

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

export function read(testId) {
    return HttpClient.get(`${baseUrl}/${testId}`);
}

export function update(testId, values) {
    return HttpClient.put(`${baseUrl}/${testId}`, values);
}

export function remove(testId) {
    return HttpClient.del(`${baseUrl}/${testId}`);
}
