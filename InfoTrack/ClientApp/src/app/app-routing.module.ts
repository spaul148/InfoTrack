import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { WebScraperComponent } from './web-scraper/web-scraper.component';

const routes: Routes = [
  { path: '', component: WebScraperComponent, pathMatch: 'full' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
