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
├── angular/                            # Angular 前端應用程式
├── src/
│   ├── MyCompany.MyApp.Application/            # 應用層：業務邏輯與服務
│   ├── MyCompany.MyApp.Application.Contracts/  # 應用層介面與 DTO 定義
│   ├── MyCompany.MyApp.DbMigrator/             # 資料庫遷移與初始資料植入工具
│   ├── MyCompany.MyApp.Domain/                 # 領域層：核心業務模型
│   ├── MyCompany.MyApp.Domain.Shared/          # 領域共用常數與列舉
│   ├── MyCompany.MyApp.EntityFrameworkCore/    # EF Core 資料存取層
│   ├── MyCompany.MyApp.HttpApi/                # HTTP API 控制器定義
│   ├── MyCompany.MyApp.HttpApi.Client/         # HTTP API 用戶端代理
│   └── MyCompany.MyApp.HttpApi.Host/           # ASP.NET Core API 主機入口
└── test/                               # 單元測試與整合測試專案
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
