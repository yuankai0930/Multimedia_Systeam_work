# 更新日誌

本專案的所有重要變更皆記錄於此檔案。
格式依循 [Keep a Changelog](https://keepachangelog.com/zh-TW/1.1.0/) 規範。

---

## [1.2.1] - 2026-04-22

### 修正
- **APOD 顯示問題**：修正 NASA APOD 回傳影片網址（如 YouTube）時前端以圖片渲染造成破圖/空白的問題，改為依 `MediaType` 分流顯示（image/video）
- **Application 層**：`ApodAppService` 新增 `media_type` 解析與回傳，並加入媒體型態正規化與影片網址 fallback 判斷
- **Domain / DTO**：`ApodImage` 與 `ApodImageDto` 新增 `MediaType` 欄位，確保前後端資料一致
- **EF Core**：`MyAppDbContext` 補上 `MediaType` 欄位映射，與既有 Migration `AddApodMediaType` 對齊
- **Angular 前端**：`apod.component` 新增影片嵌入顯示（YouTube/Vimeo 轉 embed URL）與不支援媒體提示，避免頁面空白

### 維運
- **資料庫升級**：確認需執行 `DbMigrator` 套用 `AddApodMediaType`，否則會出現 `Invalid column name 'MediaType'`

---

## [1.2.0] - 2026-04-21

### 新增
- **安全性**：新增 `.env.example` 範本檔，提供本機 API Key 設定指引
- **安全性**：`.gitignore` 補充排除 `.env`、`.env.*`（保留 `.env.example`），防止密鑰洩漏

### 修改
- **Application 層**：`ApodAppService` 改為優先讀取環境變數 `NASA_API_KEY`，若未設定則 fallback 至 `DEMO_KEY`，避免真實 Key 寫入版控
- **設定**：`appsettings.json`（HttpApi.Host、DbMigrator）中的 `Nasa:ApiKey` 改為預設值 `DEMO_KEY`
- **啟動流程**：`HttpApi.Host` 與 `DbMigrator` 的 `Program.cs` 加入 `.env` 自動載入邏輯（向上遍歷目錄搜尋）
- **README**：新增「NASA API Key（GitHub 安全做法）」設定說明，說明 DEMO_KEY fallback 機制與 `.env` 使用方式
- **CHANGELOG**：新增本版本更新記錄

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
