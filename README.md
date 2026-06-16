# MyCompany.MyApp

## 專案簡介

本專案是基於 [領域驅動設計（DDD）](https://docs.abp.io/en/abp/latest/Domain-Driven-Design) 原則所建立的分層架構啟動方案，使用 [ABP Framework](https://abp.io/) 開發，並已預先安裝所有基礎 ABP 模組。

---

## 環境需求

在執行本專案前，請確認已安裝以下工具：

- [.NET 9.0+ SDK](https://dotnet.microsoft.com/download/dotnet)
- [Node.js v18 或 v20](https://nodejs.org/en)
- SQL Server 或其他支援的資料庫（依 `appsettings.json` 設定）

---

## 專案結構

```
work01/
├── angular/                                    # Angular 前端應用程式
│   └── src/app/
│       ├── apod/                               # NASA 每日天文圖片功能模組
│       │   ├── apod.component.ts               # 元件邏輯：抓取、載入、切換圖片
│       │   ├── apod.component.html             # 畫面：圖片展示 + 歷史清單
│       │   ├── apod.component.scss             # 元件樣式
│       │   ├── apod.module.ts                  # Angular 功能模組
│       │   ├── apod-routing.module.ts          # /apod 路由設定
│       │   └── apod.service.ts                 # 呼叫後端 API 的服務
│       ├── home/                               # 首頁模組
│       ├── shared/                             # 共用元件與服務
│       ├── app.module.ts                       # 根模組
│       ├── app-routing.module.ts               # 全域路由設定
│       └── route.provider.ts                   # 左側選單項目設定
│
├── src/
│   ├── MyCompany.MyApp.Application/            # 應用層：實作業務邏輯與服務
│   │   └── Apod/
│   │       └── ApodAppService.cs               # 呼叫 NASA API、寫入資料庫的核心邏輯
│   │
│   ├── MyCompany.MyApp.Application.Contracts/  # 應用層合約：介面與 DTO 定義（供前端或外部使用）
│   │   └── Apod/
│   │       ├── ApodImageDto.cs                 # APOD 資料傳輸物件
│   │       └── IApodAppService.cs              # APOD 服務介面
│   │
│   ├── MyCompany.MyApp.DbMigrator/             # 主控台工具：執行 EF Core Migration、植入初始資料
│   │   └── appsettings.json                    # 資料庫連線字串、NASA API Key
│   │
│   ├── MyCompany.MyApp.Domain/                 # 領域層：核心業務實體與規則（不依賴任何框架）
│   │   └── Apod/
│   │       └── ApodImage.cs                    # APOD 領域實體（繼承 ABP Entity<Guid>）
│   │
│   ├── MyCompany.MyApp.Domain.Shared/          # 領域共用層：常數、列舉、錯誤碼（前後端皆可引用）
│   │
│   ├── MyCompany.MyApp.EntityFrameworkCore/    # 資料存取層：EF Core DbContext 與 Migration
│   │   ├── EntityFrameworkCore/
│   │   │   └── MyAppDbContext.cs               # 資料庫上下文（含 AppApodImages 資料表設定）
│   │   └── Migrations/
│   │       └── 20260421063106_AddApodTable.cs  # 建立 AppApodImages 資料表的 Migration
│   │
│   ├── MyCompany.MyApp.HttpApi/                # API 層：HTTP 控制器定義（ABP 自動產生）
│   ├── MyCompany.MyApp.HttpApi.Client/         # HTTP API 用戶端代理（供其他服務呼叫）
│   └── MyCompany.MyApp.HttpApi.Host/           # 主機入口：ASP.NET Core Web API 啟動設定
│       ├── appsettings.json                    # 伺服器設定、資料庫連線、NASA API Key
│       ├── MyAppHttpApiHostModule.cs           # 模組設定（CORS、Swagger、HttpClient 等）
│       └── Program.cs                          # 應用程式進入點
│
└── test/                                       # 測試專案
    ├── MyCompany.MyApp.Application.Tests/      # 應用層單元測試
    ├── MyCompany.MyApp.Domain.Tests/           # 領域層單元測試
    └── MyCompany.MyApp.EntityFrameworkCore.Tests/ # EF Core 整合測試
```

---

## 設定說明

## APOD 功能說明

NASA 每日天文圖片（APOD）功能目前支援以下流程：

1. 使用者需先登入後，才能進入 APOD 頁面與執行查詢。
2. 使用者可依指定日期查詢 APOD。
3. 系統會先查詢本地資料庫，若該日期資料已存在，直接回傳本地資料，不再呼叫 NASA API。
4. 若本地尚無資料，系統才會向 NASA APOD API 抓取，並寫入資料庫供後續重用。
5. 每位使用者的查詢歷史會個別保存，可於 APOD 頁面查看自己的歷史紀錄。

### APOD 錯誤與無資料提示

當使用者選擇的日期無法顯示時，系統會回傳可理解原因，而非僅顯示泛用錯誤：

- 日期格式錯誤：提示使用 `yyyy-MM-dd`
- 日期超出範圍：提示 APOD 可查詢區間（1995-06-16 到今天）
- 該日期尚未發布或查無資料：提示「該日期沒有 APOD 資料」
- NASA 服務暫時不可用（503）：提示稍後再試
- API 請求過多（429）：提示稍後再試

前端會直接顯示後端回傳原因，若為無資料情境會清空舊內容，避免誤判為查詢成功。

### APOD 歷史管理

本版本新增完整的歷史管理功能，使用者可在 APOD 頁面管理自己的查詢記錄：

#### 主要功能

1. **刪除記錄**：點擊歷史項目旁的紅色 trash 按鈕，可永久刪除該筆查詢記錄（無法復原）
2. **標星/收藏**：點擊歷史項目旁的星號按鈕，標星項目自動移至「我的收藏」區頂部（按鈕顯示黃色）
3. **手動排序**：在「我的收藏」區，使用者可直接拖曳標星項目調整順序，新的順序會自動保存
4. **自動分區**：
   - **我的收藏**：顯示所有標星項目，按手動排序順序展示
   - **其他查詢**：顯示未標星項目，按查詢時間倒序（最新在上）

#### 授權隔離

- 每位使用者只能查看、管理自己的查詢歷史
- 後端 API 驗證使用者所有權，防止跨用戶存取
- 刪除/標星/排序操作必須登入後才能執行

#### 實作位置

- 查詢與歷史資料處理：`src/MyCompany.MyApp.Application/Apod/ApodAppService.cs`
- 歷史操作路由：`src/MyCompany.MyApp.HttpApi/Controllers/ApodHistoryController.cs`
- 前端呼叫：`angular/src/app/apod/apod.service.ts`

> 如果你在修改後看到「切換失敗請稍後再試」，先確認後端已重新啟動，避免舊執行個體還在提供 API。

### APOD 主要 API

- `GET /api/app/apod/by-date?date=yyyy-MM-dd`：查詢指定日期 APOD（需登入）
- `GET /api/app/apod/my-history`：取得目前登入使用者的查詢歷史（已排序）
- `DELETE /api/app/apod/history/{id}`：刪除單筆查詢記錄（需登入且驗證所有權）
- `POST /api/app/apod/history/{id}/toggle-starred`：標星/取消標星（需登入且驗證所有權）
- `POST /api/app/apod/history/reorder-starred`：調整標星項目順序（需登入且驗證所有權）
- `POST /api/app/apod/fetch-and-save`：查詢今天 APOD（內部改走指定日期查詢流程）
- `GET /api/app/apod`：取得資料庫已儲存的 APOD 清單

---

### 資料庫連線字串

請分別在以下兩個專案的 `appsettings.json` 中確認並修改 `ConnectionStrings`：

- `src/MyCompany.MyApp.HttpApi.Host/appsettings.json`
- `src/MyCompany.MyApp.DbMigrator/appsettings.json`

### NASA API Key（GitHub 安全做法）

本專案為了方便執行，預設使用 NASA 的 `DEMO_KEY`，無需任何設定即可直接啟動。
若因流量限制（DEMO_KEY 每小時約 30 次）導致抓取失敗，請依下列步驟設定自己的 API Key：

1. 前往 [https://api.nasa.gov/](https://api.nasa.gov/) 免費申請 API Key

2. 在**專案根目錄**複製範本檔：

```bash
# Windows
copy .env.example .env

# macOS / Linux
cp .env.example .env
```

3. 編輯 `.env`，將 `your_nasa_api_key_here` 替換為你的 Key：

```env
NASA_API_KEY=your_nasa_api_key_here
```

4. **重新啟動後端**，設定即生效（`.env` 在啟動時載入）。

> 程式讀取優先順序：系統環境變數 `NASA_API_KEY` → `.env` 檔案 → 預設 `DEMO_KEY`
> `.env` 已加入 `.gitignore`，**不會被提交到 GitHub**。
> `.env.example` 為公開範本，不含真實 Key，**可以**入版控。

---

## 首次執行步驟

### 1. 安裝前端相依套件

在方案根目錄執行：

```bash
abp install-libs
```

> 若為首次從版本控制系統複製專案，或新增了前端套件相依性，需手動執行此指令。

### 2. 建立資料庫與初始資料

執行 `MyCompany.MyApp.DbMigrator` 專案以自動建立資料庫結構並植入初始資料：

```bash
cd src/MyCompany.MyApp.DbMigrator
dotnet run
```

> 每次新增資料庫 Migration 後也需重新執行此步驟。

### 3. 啟動後端 API

```bash
cd src/MyCompany.MyApp.HttpApi.Host
dotnet run
```

### 4. 啟動 Angular 前端

```bash
cd angular
npm install
npm start
```

---

## 任務管理

本專案已建立專用任務區，供後續新功能、修復項目與維運工作集中管理。

### 任務區位置

- [tasks/README.md](tasks/README.md)：任務區說明與使用規則
- [tasks/todo-list.md](tasks/todo-list.md)：待辦任務總覽
- [tasks/done-list.md](tasks/done-list.md)：已完成任務總覽
- [tasks/todo](tasks/todo)：待辦任務明細檔
- [tasks/done](tasks/done)：已完成任務明細檔
- [tasks/_template.md](tasks/_template.md)：新任務模板

### 使用方式

1. 有新功能或修復需求時，在 `tasks/todo/` 建立一份任務檔。
2. 建議檔名格式使用 `yyyymmdd-功能名稱.md`。
3. 同步在 [tasks/todo-list.md](tasks/todo-list.md) 新增一筆連結。
4. 任務完成後，將任務檔移到 `tasks/done/`，並更新 [tasks/todo-list.md](tasks/todo-list.md) 與 [tasks/done-list.md](tasks/done-list.md)。

> `CHANGELOG.md` 只保留版本變更紀錄，不作為待辦任務區使用。

---

## 產生簽章憑證（正式環境）

正式環境需使用 RSA 簽章憑證，ABP 框架預設讀取 `openiddict.pfx` 檔案。

執行以下指令產生憑證：

```bash
dotnet dev-certs https -v -ep openiddict.pfx -p 請替換為您的密碼
```

建議使用**兩份**獨立的 RSA 憑證，分別用於：
- **加密（Encryption）**
- **簽章（Signing）**

詳細說明請參考：
- [OpenIddict 憑證設定文件](https://documentation.openiddict.com/configuration/encryption-and-signing-credentials.html#registering-a-certificate-recommended-for-production-ready-scenarios)
- [ABP OpenIddict 正式環境設定](https://docs.abp.io/en/abp/latest/Deployment/Configuring-OpenIddict#production-environment)

---

## 部署說明

ABP 應用程式的部署方式與一般 ASP.NET Core 應用程式相同。部署前請參閱：

- [ABP 部署文件](https://docs.abp.io/en/abp/latest/Deployment/Index)
- [ABP 應用程式啟動範本部署指南](https://abp.io/docs/latest/startup-templates/application/deployment?UI=Angular&DB=EF&Tiered=No)

---

## 相關資源

### 內部文件

- [Angular 前端說明](./angular/README.md)

### 外部學習資源

- [ABP Web 應用程式開發教學](https://abp.io/docs/latest/tutorials/book-store/part-1)
- [ABP 應用程式啟動範本說明](https://abp.io/docs/latest/startup-templates/application/index)
