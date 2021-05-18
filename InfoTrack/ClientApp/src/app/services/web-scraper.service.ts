
import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { UrlSearchResult } from "../models/url-search-result.model";
import { BaseHttpService } from "./base-http-service";

@Injectable({ providedIn: "root" })
export class WebScraper extends BaseHttpService {

    constructor(private readonly http: HttpClient) {
        super();
    }

    public urlSearch(url: string, keyword: string, toSearch: string, pageSize: number) {
        const endpoint = this.baseUrl + `/Webscrap/Search?url=${url}&keyword=${keyword}&toSearch=${toSearch}&pageSize=${pageSize}`;
        return this.http.get<UrlSearchResult[]>(endpoint, { responseType: "json" });
    }

}