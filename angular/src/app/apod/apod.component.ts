import { Component, OnInit } from '@angular/core';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { CdkDragDrop } from '@angular/cdk/drag-drop';
import { ApodService, ApodImageDto, ApodQueryHistoryDto } from './apod.service';

@Component({
  selector: 'app-apod',
  templateUrl: './apod.component.html',
  styleUrls: ['./apod.component.scss'],
})
export class ApodComponent implements OnInit {
  apodData: ApodImageDto | null = null;
  historyList: ApodQueryHistoryDto[] = [];
  selectedDate = this.getTodayString();
  maxDate = this.getTodayString();
  isLoading = false;
  isHistoryLoading = false;
  isFetching = false;
  isDeleting: { [key: string]: boolean } = {};
  isTogglingstar: { [key: string]: boolean } = {};
  errorMessage = '';

  constructor(
    private apodService: ApodService,
    private sanitizer: DomSanitizer
  ) {}

  ngOnInit(): void {
    this.loadHistory();
  }

  /** 呼叫後端，從 NASA 抓取今日圖片並存入資料庫 */
  fetchToday(): void {
    this.selectedDate = this.maxDate;
    this.searchBySelectedDate();
  }

  searchBySelectedDate(): void {
    if (!this.selectedDate) {
      this.errorMessage = '請先選擇日期。';
      return;
    }

    this.isFetching = true;
    this.isLoading = true;
    this.errorMessage = '';
    this.apodService.getByDate(this.selectedDate).subscribe({
      next: (data) => {
        this.apodData = data;
        this.isFetching = false;
        this.isLoading = false;
        this.loadHistory();
      },
      error: (err) => {
        const serverMessage = err?.error?.error?.message;
        this.errorMessage =
          typeof serverMessage === 'string' && serverMessage.trim().length > 0
            ? serverMessage
            : '查詢失敗，請確認日期格式正確且後端服務已啟動。';

        if (this.errorMessage.includes('沒有 APOD 資料')) {
          this.apodData = null;
        }

        this.isFetching = false;
        this.isLoading = false;
        console.error(err);
      },
    });
  }

  /** 載入目前登入使用者的查詢歷史 */
  loadHistory(): void {
    this.isHistoryLoading = true;
    this.apodService.getMyHistory().subscribe({
      next: (list) => {
        this.historyList = list;
        if (!this.apodData && list.length > 0) {
          this.apodData = this.toApodImage(list[0]);
          this.selectedDate = list[0].date;
        }
        this.isHistoryLoading = false;
      },
      error: (err) => {
        this.isHistoryLoading = false;
        console.error(err);
      },
    });
  }

  /** 點選歷史清單中的圖片 */
  selectHistory(item: ApodQueryHistoryDto): void {
    this.apodData = this.toApodImage(item);
    this.selectedDate = item.date;
  }

  /** 刪除歷史項目 */
  deleteHistoryItem(item: ApodQueryHistoryDto, event: Event): void {
    event.stopPropagation();
    if (!confirm(`確定要刪除 ${item.date} 的查詢紀錄嗎？`)) {
      return;
    }

    this.isDeleting[item.id] = true;
    this.apodService.deleteHistory(item.id).subscribe({
      next: () => {
        this.historyList = this.historyList.filter(x => x.id !== item.id);
        if (this.apodData?.date === item.date) {
          this.apodData = null;
        }
        this.isDeleting[item.id] = false;
      },
      error: (err) => {
        alert('刪除失敗，請稍後再試');
        this.isDeleting[item.id] = false;
        console.error(err);
      },
    });
  }

  /** 切換星號狀態 */
  toggleStar(item: ApodQueryHistoryDto, event: Event): void {
    event.stopPropagation();
    this.isTogglingstar[item.id] = true;
    this.apodService.toggleStarred(item.id).subscribe({
      next: (updated) => {
        const index = this.historyList.findIndex(x => x.id === item.id);
        if (index >= 0) {
          this.historyList[index] = updated;
          this.historyList = this.historyList.sort((a, b) => {
            if (a.isStarred !== b.isStarred) return b.isStarred ? 1 : -1;
            if (a.isStarred && a.pinnedOrder !== null && b.pinnedOrder !== null) {
              return a.pinnedOrder - b.pinnedOrder;
            }
            return new Date(b.queryTime).getTime() - new Date(a.queryTime).getTime();
          });
        }
        this.isTogglingstar[item.id] = false;
      },
      error: (err) => {
        alert('切換失敗，請稍後再試');
        this.isTogglingstar[item.id] = false;
        console.error(err);
      },
    });
  }

  /** 拖曳星號區完成事件 */
  onStarredDropped(event: CdkDragDrop<ApodQueryHistoryDto[]>): void {
    if (event.previousIndex === event.currentIndex) {
      return;
    }

    const starredItems = this.getStarredHistories();
    const reorderedIds = starredItems.map(x => x.id);
    
    this.apodService.reorderStarredHistories(reorderedIds).subscribe({
      next: () => {
        // 重新載入歷史確保順序一致
        this.loadHistory();
      },
      error: (err) => {
        alert('重排失敗，請稍後再試');
        this.loadHistory();
        console.error(err);
      },
    });
  }

  /** 取得所有星號項目 */
  getStarredHistories(): ApodQueryHistoryDto[] {
    return this.historyList.filter(x => x.isStarred).sort((a, b) => {
      const aOrder = a.pinnedOrder ?? 9999;
      const bOrder = b.pinnedOrder ?? 9999;
      return aOrder - bOrder;
    });
  }

  /** 取得所有非星號項目 */
  getNormalHistories(): ApodQueryHistoryDto[] {
    return this.historyList
      .filter(x => !x.isStarred)
      .sort((a, b) => new Date(b.queryTime).getTime() - new Date(a.queryTime).getTime());
  }

  isImage(item: ApodImageDto): boolean {
    return this.resolveMediaType(item) === 'image';
  }

  isVideo(item: ApodImageDto): boolean {
    return this.resolveMediaType(item) === 'video';
  }

  getVideoEmbedUrl(item: ApodImageDto): SafeResourceUrl | null {
    if (!this.isVideo(item)) {
      return null;
    }

    const embedUrl = this.toEmbedUrl(item.url);
    return embedUrl ? this.sanitizer.bypassSecurityTrustResourceUrl(embedUrl) : null;
  }

  private resolveMediaType(item: ApodImageDto): string {
    const mediaType = item.mediaType?.trim().toLowerCase();
    if (mediaType === 'image' || mediaType === 'video') {
      return mediaType;
    }

    return this.isKnownVideoUrl(item.url) ? 'video' : 'image';
  }

  private isKnownVideoUrl(url: string): boolean {
    return /youtube\.com|youtu\.be|vimeo\.com/i.test(url);
  }

  private toApodImage(item: ApodQueryHistoryDto): ApodImageDto {
    return {
      date: item.date,
      title: item.title,
      explanation: item.explanation,
      mediaType: item.mediaType,
      url: item.url,
    };
  }

  private getTodayString(): string {
    return new Date().toISOString().slice(0, 10);
  }

  private toEmbedUrl(url: string): string | null {
    try {
      const parsed = new URL(url);
      const host = parsed.hostname.toLowerCase();

      if (host.includes('youtu.be')) {
        const id = parsed.pathname.split('/').filter(Boolean)[0];
        return id ? `https://www.youtube.com/embed/${id}` : null;
      }

      if (host.includes('youtube.com')) {
        if (parsed.pathname === '/watch') {
          const id = parsed.searchParams.get('v');
          return id ? `https://www.youtube.com/embed/${id}` : null;
        }

        if (parsed.pathname.startsWith('/embed/')) {
          return `https://www.youtube.com${parsed.pathname}`;
        }

        if (parsed.pathname.startsWith('/shorts/')) {
          const id = parsed.pathname.split('/')[2];
          return id ? `https://www.youtube.com/embed/${id}` : null;
        }
      }

      if (host.includes('vimeo.com')) {
        const id = parsed.pathname.split('/').filter(Boolean)[0];
        return id ? `https://player.vimeo.com/video/${id}` : null;
      }

      return null;
    } catch {
      return null;
    }
  }
}
