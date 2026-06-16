import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { ApodRoutingModule } from './apod-routing.module';
import { ApodComponent } from './apod.component';

@NgModule({
  declarations: [ApodComponent],
  imports: [CommonModule, FormsModule, HttpClientModule, ApodRoutingModule],
})
export class ApodModule {}
