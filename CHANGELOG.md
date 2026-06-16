# 更新日誌

本專案的所有重要變更皆記錄於此檔案。
格式依循 [Keep a Changelog](https://keepachangelog.com/zh-TW/1.1.0/) 規範。

---

## [1.4.1] - 2026-06-16

### 修正
- **歷史操作路由**：將 APOD 歷史的刪除、標星、排序 API 路由移至 `HttpApi` 層專屬 controller，修正應用層直接引用 MVC attribute 導致的建置失敗
- **星號切換失敗**：修正 `toggle-starred` 端點在後端回傳 405 的問題，確保前端按星號時可正常呼叫 API
- **排序請求模型**：`reorder-starred` 改用輸入 DTO，避免模型繫結與路由處理不一致

### 維運
- **建置驗證**：重新確認 `ApodAppService`、`ApodHistoryController` 與相關合約可正常編譯

---

## [1.4.0] - 2026-06-16

### 新增
- **歷史管理 - 刪除**：新增 `DELETE /api/app/apod/history/{id}` 端點，使用者可刪除單筆查詢記錄（硬刪除，無法復原）
- **歷史管理 - 收藏**：新增 `POST /api/app/apod/history/{id}/toggle-starred` 端點，使用者可標星/取消標星記錄，標星項目自動移至清單頂部
- **歷史管理 - 排序**：新增 `POST /api/app/apod/history/reorder-starred` 端點，使用者可在「我的收藏」區拖曳手動調整標星項目順序
- **前端元件**：Angular 元件新增星號按鈕、刪除按鈕、拖曳排序功能（使用 @angular/cdk）
- **雙區域歷史檢視**：APOD 頁面新增「我的收藏」與「其他查詢」兩區，標星項目優先顯示
- **資料庫欄位**：`AppApodQueryHistories` 表新增 `IsStarred` (bit) 與 `PinnedOrder` (int?) 欄位，支援標星狀態與手動排序
- **複合索引**：優化資料庫索引從 (UserId, QueryTime) 改為 (UserId, IsStarred, QueryTime)，提升標星排序查詢效能

### 修改
- **授權強化**：所有歷史管理操作（刪除、標星、排序）在應用層加入 `[Authorize]` 屬性與使用者所有權驗證
- **排序邏輯**：`GetMyHistoryAsync` 回傳排序改為 IsStarred DESC → PinnedOrder ASC → QueryTime DESC，前端分區展示
- **前端路由保護**：APOD 模組路由加入 AuthGuard 確保未登入使用者無法進入

### 維運
- **Migration 套用**：新增並套用 `20260616120000_AddHistoryStarredAndPinnedOrder`，確保新欄位與索引可正常運作
- **GitIgnore 更新**：package.json 顯式聲明 `@angular/cdk` 版本 ~18.1.0

---

## [1.3.0] - 2026-06-16

### 新增
- **APOD 日期查詢**：新增 `GET /api/app/apod/by-date?date=yyyy-MM-dd`，支援使用者指定日期查詢 APOD
- **登入保護**：`/apod` 前端路由與後端 APOD 應用服務改為需登入後才能使用
- **個人歷史**：新增 `ApodQueryHistory`，紀錄每位使用者的查詢日期與時間，並提供「我的查詢歷史」列表
- **資料庫模型**：新增 APOD 查詢歷史資料表與索引，並為 APOD 日期建立唯一索引以避免重複寫入

### 修改
- **查詢流程優化（DB-first）**：查詢指定日期時，系統先讀本地資料庫，無資料才呼叫 NASA API，降低重複外部請求
- **Angular APOD 頁面**：新增日期選擇器、指定日期查詢按鈕、今天快捷按鈕與歷史清單點選回看

### 修正
- **錯誤可讀性**：針對「日期無資料 / 日期超出範圍 / NASA 暫時不可用 / 請求過多」回傳可理解原因，避免前端僅看到泛用 500 訊息
- **無資料顯示行為**：前端接收到無資料訊息時清空舊內容，避免誤以為查詢成功

### 維運
- **Migration 套用**：新增並套用 `AddApodQueryHistory`，確保查詢歷史功能與索引可正常運作

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
