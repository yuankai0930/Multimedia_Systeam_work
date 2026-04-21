# 更新日誌

本專案的所有重要變更皆記錄於此檔案。
格式依循 [Keep a Changelog](https://keepachangelog.com/zh-TW/1.1.0/) 規範。

---

## [1.1.0] - 2026-04-21

### 新增
- **Domain 層**：新增 `ApodImage` 領域實體（`src/MyCompany.MyApp.Domain/Apod/ApodImage.cs`），定義 NASA APOD 資料欄位（Date、Title、Explanation、Url）
- **Application.Contracts 層**：新增 `ApodImageDto` 資料傳輸物件與 `IApodAppService` 服務介面
- **Application 層**：新增 `ApodAppService`，實作呼叫 NASA APOD API、去重判斷、寫入資料庫邏輯
- **EntityFrameworkCore 層**：在 `MyAppDbContext` 註冊 `AppApodImages` 資料表，並產生 Migration `AddApodTable`
- **Angular 前端**：新增 `ApodModule`，包含圖片展示頁面、歷史清單側欄及呼叫後端的 `ApodService`
- **導航選單**：在左側選單新增「每日天文圖片」項目，路由為 `/apod`
- **設定**：`appsettings.json` 加入 `Nasa:ApiKey` 設定項目
- **HttpClient 註冊**：`MyAppHttpApiHostModule` 加入 `AddHttpClient()` 供應用層使用

### 修改
- 更新 `README.md` 專案結構說明，新增各檔案的詳細用途說明
- 新增 `CHANGELOG.md` 版本更新記錄檔

---

## [1.0.0] - 2026-04-21

### 新增
- 初始版本發布
- 建立 ABP Framework 分層架構專案（DDD）
- 整合 Angular 前端應用程式
- 整合 Entity Framework Core 資料存取層
- 整合 OpenIddict 身份驗證與授權
- 新增 DbMigrator 資料庫遷移工具
- 新增 Serilog 日誌記錄（支援檔案與主控台輸出）
- 新增中文 README.md 專案說明文件
