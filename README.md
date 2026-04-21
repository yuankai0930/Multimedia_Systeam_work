# MyCompany.MyApp

## 關於此專案

本專案是一個基於[領域驅動設計（DDD）](https://docs.abp.io/en/abp/latest/Domain-Driven-Design)實踐的分層啟動方案，所有基礎 ABP 模組均已安裝。更多資訊請參閱 [Application Startup Template](https://abp.io/docs/latest/startup-templates/application/index) 文件。

### 環境需求

* [.NET 9.0+ SDK](https://dotnet.microsoft.com/download/dotnet)
* [Node v18 或 v20](https://nodejs.org/en)

### 設定說明

本方案提供開箱即用的預設設定，但在執行前請考慮調整以下設定：

* 檢查 `MyCompany.MyApp.HttpApi.Host` 與 `MyCompany.MyApp.DbMigrator` 專案下 `appsettings.json` 中的 `ConnectionStrings`，並依需求修改。

### 執行前準備

* 在方案根目錄執行 `abp install-libs` 以安裝前端套件相依性。建立新方案時會自動執行此步驟（除非特別停用）。若從版本控制複製此方案，或新增了前端套件相依性，需手動執行。
* 執行 `MyCompany.MyApp.DbMigrator` 以建立初始資料庫。建立新方案時亦會自動執行（除非停用）。第一次執行時需要此步驟，之後新增資料庫遷移時同樣需要執行。

#### 產生簽署憑證

在正式環境中，需要使用正式的簽署憑證。ABP Framework 會在應用程式中設定簽署與加密憑證，並預期在應用程式中存在 `openiddict.pfx` 檔案。

可使用以下指令產生簽署憑證：

```bash
dotnet dev-certs https -v -ep openiddict.pfx -p 95042188-0074-4ddd-9468-244fce02aa35
```

> `95042188-0074-4ddd-9468-244fce02aa35` 為憑證密碼，可自行更改。

建議使用**兩張** RSA 憑證，與 HTTPS 憑證分開：一張用於加密，一張用於簽署。

更多資訊請參閱：[https://documentation.openiddict.com/configuration/encryption-and-signing-credentials.html#registering-a-certificate-recommended-for-production-ready-scenarios](https://documentation.openiddict.com/configuration/encryption-and-signing-credentials.html#registering-a-certificate-recommended-for-production-ready-scenarios)

> 另請參閱 [Configuring OpenIddict](https://docs.abp.io/en/abp/latest/Deployment/Configuring-OpenIddict#production-environment) 文件。

### 方案結構

本方案為分層單體應用程式，包含以下子應用：

* `MyCompany.MyApp.DbMigrator`：主控台應用程式，用於套用資料庫遷移並植入初始資料，可用於開發與正式環境。
* `MyCompany.MyApp.HttpApi.Host`：ASP.NET Core API 應用程式，負責對外公開 API。
* `angular`：Angular 前端應用程式。


## 部署應用程式

部署 ABP 應用程式與部署一般 .NET 或 ASP.NET Core 應用程式並無差異，但部署時需注意若干事項。部署前請參閱 ABP 的[部署文件](https://docs.abp.io/en/abp/latest/Deployment/Index)與 ABP Commercial 的[部署文件](https://abp.io/docs/latest/startup-templates/application/deployment?UI=MVC&DB=EF&Tiered=No)。

### 其他資源

#### 內部資源

以下為本方案的詳細設定指南：

* [Angular 前端](./angular/README.md)
* [更新日誌](./CHANGELOG.md)

#### 外部資源

以下資源可協助您深入了解本方案與 ABP Framework：

* [Web 應用程式開發教學](https://abp.io/docs/latest/tutorials/book-store/part-1)
* [Application Startup Template](https://abp.io/docs/latest/startup-templates/application/index)
