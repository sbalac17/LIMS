import { tryGetSavedToken, clearSavedToken } from '../Authentication/Token';
import { navigate }  from '../NavigationService';
import { flatMap } from 'lodash';

export function get(url, body) {
    return doRequest('GET', url, body);
}

export function post(url, body) {
    return doRequest('POST', url, body);
}

export function put(url, body) {
    return doRequest('PUT', url, body);
}

export function del(url, body) {
    return doRequest('DELETE', url, body);
}

async function doRequest(method, url, body) {
    let options = {
        method: method,
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        }
    };

    // automatically inject the access token
    let token = await tryGetSavedToken();
    if (token) {
        options.headers['Authorization'] = 'Bearer ' + token.accessToken;
    } else {
        navigate('Auth');
        throw new HttpError('Not authenticated');
    }

    if (body) {
        options.body = JSON.stringify(body);
    }

    let response = await fetch(url, options);
    if (response.status == 401) { // unauthorized
        await clearSavedToken();
        navigate('Auth');
        throw new HttpError('Authentication expired');
    }

    let obj = null;
    try {
        obj = await response.json();
    } catch(e) { }

    console.log("response", obj);

    if (!obj) {
        // no json
        console.error('Server did not send valid JSON');
        throw new HttpError('Internal server error');
    } else if (!response.ok) {
        // server exception
        if (response.status == 500) {
            throw new HttpError(obj.ExceptionMessage || obj.Message);
        }

        // form validation errors
        let modelState = obj.ModelState;
        if (response.status == 400 && modelState) {
            let errors = flatMap(Object.keys(modelState), key => modelState[key]);
            throw new HttpError(...errors);
        }

        // unknown error message
        console.error(obj);
        throw new HttpError();
    }

    return obj;
}

const unknownError = [ 'Unknown error occurred' ];

class HttpError extends Error {
    constructor(...errorMessages) {
        if (!Array.isArray(errorMessages) || errorMessages.length == 0) {
            errorMessages = unknownError;
        }

        super('One or more server errors occurred');

        this.errorMessages = errorMessages;
    }
}

export function extractErrorMessages(exception) {
    let errorMessages = exception.errorMessages;
    if (errorMessages && Array.isArray(errorMessages)) {
        return errorMessages;
    }

    console.log('unknown exception', exception);
    return unknownError;
}
