import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface ApodImageDto {
  date: string;
  title: string;
  explanation: string;
  mediaType: string;
  url: string;
}

export interface ApodQueryHistoryDto {
  date: string;
  title: string;
  explanation: string;
  mediaType: string;
  url: string;
  queryTime: string;
}

@Injectable({
  providedIn: 'root',
})
export class ApodService {
  private apiBase = `${environment.apis.default.url}/api/app/apod`;

  constructor(private http: HttpClient) {}

  /** 從 NASA API 抓取今日圖片並儲存至資料庫 */
  fetchAndSave(): Observable<ApodImageDto> {
    return this.http.post<ApodImageDto>(`${this.apiBase}/fetch-and-save`, {});
  }

  /** 依指定日期取得 APOD，若本地無資料則由後端抓取後保存 */
  getByDate(date: string): Observable<ApodImageDto> {
    return this.http.get<ApodImageDto>(`${this.apiBase}/by-date`, {
      params: { date },
    });
  }

  /** 取得資料庫中所有已儲存的天文圖片 */
  getList(): Observable<ApodImageDto[]> {
    return this.http.get<ApodImageDto[]>(this.apiBase);
  }

  /** 取得目前登入使用者的查詢歷史 */
  getMyHistory(): Observable<ApodQueryHistoryDto[]> {
    return this.http.get<ApodQueryHistoryDto[]>(`${this.apiBase}/my-history`);
  }
}
