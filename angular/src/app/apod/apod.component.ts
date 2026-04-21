import { Component, OnInit } from '@angular/core';
import { ApodService, ApodImageDto } from './apod.service';

@Component({
  selector: 'app-apod',
  templateUrl: './apod.component.html',
  styleUrls: ['./apod.component.scss'],
})
export class ApodComponent implements OnInit {
  apodData: ApodImageDto | null = null;
  savedList: ApodImageDto[] = [];
  isLoading = false;
  isFetching = false;
  errorMessage = '';

  constructor(private apodService: ApodService) {}

  ngOnInit(): void {
    this.loadList();
  }

  /** 呼叫後端，從 NASA 抓取今日圖片並存入資料庫 */
  fetchToday(): void {
    this.isFetching = true;
    this.errorMessage = '';
    this.apodService.fetchAndSave().subscribe({
      next: (data) => {
        this.apodData = data;
        this.isFetching = false;
        this.loadList();
      },
      error: (err) => {
        this.errorMessage = '抓取失敗，請確認後端服務是否啟動。';
        this.isFetching = false;
        console.error(err);
      },
    });
  }

  /** 載入資料庫中所有已儲存的圖片 */
  loadList(): void {
    this.isLoading = true;
    this.apodService.getList().subscribe({
      next: (list) => {
        this.savedList = list;
        if (list.length > 0) {
          this.apodData = list[list.length - 1];
        }
        this.isLoading = false;
      },
      error: (err) => {
        this.isLoading = false;
        console.error(err);
      },
    });
  }

  /** 點選歷史清單中的圖片 */
  selectImage(item: ApodImageDto): void {
    this.apodData = item;
  }
}
