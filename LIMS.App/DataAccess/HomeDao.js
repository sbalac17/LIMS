import * as HttpClient from './HttpClient';
import Config from '../Config';

const baseUrl = Config.apiServer + "api/home";

export async function getRecentLabs() {
    let home = await HttpClient.get(baseUrl);
    return home.RecentLabs;
}
