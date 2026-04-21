import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ApodComponent } from './apod.component';

const routes: Routes = [
  { path: '', component: ApodComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class ApodRoutingModule {}
