import { Component, OnInit } from '@angular/core';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
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

  constructor(
    private apodService: ApodService,
    private sanitizer: DomSanitizer
  ) {}

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
