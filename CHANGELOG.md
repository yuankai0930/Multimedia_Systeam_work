# 更新日誌

本文件記錄每次推送至 GitHub 的詳細變更內容，依時間倒序排列。

---

## [v0.2.0] - 2026-04-21

### 文件

- **README.md（根目錄）**：將全文由英文翻譯為繁體中文，包含環境需求、設定說明、執行前準備、憑證產生教學、方案結構說明及部署說明；新增更新日誌連結。
- **angular/README.md**：將全文由英文翻譯為繁體中文，包含開發伺服器、程式碼鷹架、建置、單元測試、端對端測試說明。
- **CHANGELOG.md（本檔案）**：新增更新日誌，用於追蹤每次推送的詳細變更。

---

## [v0.1.0] - 2026-04-21

### 初始提交

- 初始化 Git 儲存庫。
- 上傳完整 ABP Framework 分層方案，包含以下專案：
  - `MyCompany.MyApp.Application`：應用層，含 AutoMapper Profile 與應用模組設定。
  - `MyCompany.MyApp.Application.Contracts`：應用合約層，含 DTO 擴充與權限定義。
  - `MyCompany.MyApp.DbMigrator`：資料庫遷移主控台工具。
  - `MyCompany.MyApp.Domain`：領域層，含領域常數、資料植入、OpenIddict 及設定。
  - `MyCompany.MyApp.Domain.Shared`：領域共用層，含錯誤碼、全域功能及模組擴充設定。
  - `MyCompany.MyApp.EntityFrameworkCore`：Entity Framework Core 資料存取層。
  - `MyCompany.MyApp.HttpApi`：HTTP API 控制器層。
  - `MyCompany.MyApp.HttpApi.Client`：HTTP API 用戶端代理層。
  - `MyCompany.MyApp.HttpApi.Host`：ASP.NET Core Web API 主機專案（含 Serilog 日誌設定）。
  - `angular/`：Angular 前端應用程式。
  - `test/`：單元測試與整合測試專案。
