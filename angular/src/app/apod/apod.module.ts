import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { DragDropModule } from '@angular/cdk/drag-drop';
import { ApodRoutingModule } from './apod-routing.module';
import { ApodComponent } from './apod.component';

@NgModule({
  declarations: [ApodComponent],
  imports: [CommonModule, FormsModule, HttpClientModule, DragDropModule, ApodRoutingModule],
})
export class ApodModule {}
