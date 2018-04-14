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

export function updateSample(labId, sampleId, obj) {
    return HttpClient.put(`${baseUrl}/${labId}/samples/${sampleId}`, obj);
}

export function removeSample(labId, sampleId) {
    return HttpClient.del(`${baseUrl}/${labId}/removeSample/${sampleId}`)
}

export function postComment(labId, sampleId, commentObj) {
    return HttpClient.post(`${baseUrl}/${labId}/samples/${sampleId}/comment`, commentObj);
}

export function reagentsList(labId) {
    return HttpClient.get(`${baseUrl}/${labId}/reagents`);
}

export function addReagent(labId, reagentId, quantity) {
    return HttpClient.post(`${baseUrl}/${labId}/addReagent/${reagentId}?quantity=${quantity}`);
}

export function removeReagent(labId, usedReagentId, returnQuantity) {
    return HttpClient.post(`${baseUrl}/${labId}/removeReagent/${usedReagentId}?returnQuantity=${returnQuantity}`);
}
