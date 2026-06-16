import { Component, OnInit } from '@angular/core';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
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
