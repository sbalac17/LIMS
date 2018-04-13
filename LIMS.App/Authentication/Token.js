import { AsyncStorage } from 'react-native';
import Config from '../Config';

const tokenKey = "CenCol-LIMS.Token";
const loginUrl = Config.apiServer + "Token";

let cachedToken = null;

export async function createAccessToken(username, password) {
    let response = await fetch(loginUrl, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/x-form-urlencoded',
            'Accept': 'application/json'
        },
        body: `grant_type=password&username=${encodeURIComponent(username)}&password=${encodeURIComponent(password)}`
    });

    let responseObj = await response.json();

    if (responseObj.error) {
        throw (responseObj.error_description || responseObj.error);
    }

    let token = {
        accessToken: responseObj.access_token,
        expiresIn: responseObj.expires_in,
        userName: responseObj.userName
    };

    try {
        cachedToken = token; // in-memory cache
        AsyncStorage.setItem(tokenKey, JSON.stringify(token));
    } catch (e) {
        console.error(e);
    }

    return token;
}

export async function tryGetSavedToken() {
    try {
        if (cachedToken) { // TODO: check expiry
            return cachedToken;
        }

        let tokenJson = await AsyncStorage.getItem(tokenKey);
        let token = JSON.parse(tokenJson);

        // TODO: check expiry

        return token;
    } catch (e) {
        console.error(e);
        return null;
    }
}

export async function clearSavedToken() {
    cachedToken = null;

    try {
        await AsyncStorage.removeItem(tokenKey);
    } catch (e) { }
}
