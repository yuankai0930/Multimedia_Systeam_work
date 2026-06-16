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
  id: string;
  date: string;
  title: string;
  explanation: string;
  mediaType: string;
  url: string;
  queryTime: string;
  isStarred: boolean;
  pinnedOrder: number | null;
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

  /** 刪除指定歷史記錄 */
  deleteHistory(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiBase}/history/${id}`);
  }

  /** 切換指定歷史的星號狀態 */
  toggleStarred(id: string): Observable<ApodQueryHistoryDto> {
    return this.http.post<ApodQueryHistoryDto>(`${this.apiBase}/history/${id}/toggle-starred`, {});
  }

  /** 批次更新星號區的排序順序 */
  reorderStarredHistories(ids: string[]): Observable<void> {
    return this.http.post<void>(`${this.apiBase}/history/reorder-starred`, { starredHistoryIds: ids });
  }
}
