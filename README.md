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

### 資料庫連線字串

請分別在以下兩個專案的 `appsettings.json` 中確認並修改 `ConnectionStrings`：

- `src/MyCompany.MyApp.HttpApi.Host/appsettings.json`
- `src/MyCompany.MyApp.DbMigrator/appsettings.json`

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
