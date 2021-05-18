import { Component, OnInit } from '@angular/core';
import { GuiCellView, GuiColumn, GuiColumnAlign, GuiDataType, GuiSorting } from '@generic-ui/ngx-grid';
import { GuiGridComponent } from '@generic-ui/ngx-grid';
import { UrlSearchResult } from '../models/url-search-result.model';
import { WebScraper } from '../services/web-scraper.service';

@Component({
  selector: 'app-web-scraper',
  templateUrl: './web-scraper.component.html',
  styleUrls: ['./web-scraper.component.css']
})
export class WebScraperComponent implements OnInit {

  constructor(private webScraper: WebScraper) { }

  columns: Array<GuiColumn> = [
    {
      header: 'Rank',
      field: 'index',
      type: GuiDataType.NUMBER,
      view: GuiCellView.CHIP,
      width: 100,
      align: GuiColumnAlign.CENTER
    },
    {
      header: 'Referring url',
      field: 'url',
      view: GuiCellView.LINK
    },
    {
      header: 'Found In',
      field: 'matchedIn',
      width: 100,
      align: GuiColumnAlign.CENTER
    },
    {
      header: 'Content',
      field: 'content',
      view: (value: string) => {
        return `<div style="white-space:pre-wrap;white-space: -moz-pre-wrap;word-wrap: break-word;">${value}</div>`
      }
    }
  ];

  private readonly url: string = "https://www.google.com";
  keyword: string = "";
  pagesize: number = 100;
  search: string = "";
  submitted: boolean = false;
  source: Array<UrlSearchResult> = new Array<UrlSearchResult>();

  sorting: GuiSorting = {
    enabled: true,
    multiSorting: true
  };

  ngOnInit(): void {

  }

  get itemCount(): number {
    return this.source.length;
  }

  onSubmit() {
    this.submitted = true;
    console.log("submitted");
    this.webScraper.urlSearch(this.url, this.keyword, this.search, this.pagesize).subscribe(records => {
      this.source = records;
      this.submitted = false;
    },
      err => {
        alert(err.message);
        this.submitted = false;
      });
  }

}
