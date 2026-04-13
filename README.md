# MyWorkItem

最簡版 ASP.NET Core MVC 面試題實作（不使用 DTO / ViewModel、帳密明碼、SQL Server localhost）。

## 環境需求

- .NET SDK 10
- SQL Server（localhost）

## 連線設定

`appsettings.json` 已設定：

`Server=localhost;Database=MyWorkItemDb;Trusted_Connection=True;TrustServerCertificate=True;`

## 啟動方式

1. 進入專案資料夾：
   - `cd c:\project\MyWorkItem`
2. 還原與建置：
   - `dotnet restore`
   - `dotnet build`
3. 啟動：
   - `dotnet run`

首次啟動會自動執行 Migration 建立資料庫與資料表。

## Demo 路徑

- 登入頁：`/Auth/Login`
- 前台列表：`/WorkItems`
- 前台詳情：`/WorkItems/Details/{id}`
- 後台管理：`/AdminWorkItems`（需 Admin）

## 測試帳號

- Admin：`admin / admin123`
- User：`user1 / user123`
- User：`user2 / user123`

## 手動驗收步驟

1. 以 `user1` 登入，進入 `/WorkItems`。
2. 勾選 1~2 筆資料，按「確認勾選項目」，狀態應變為「已確認」。
3. 對已確認項目按「撤銷確認」，狀態應變回「待確認」。
4. 重新整理或重新登入 `user1`，狀態應維持上次操作結果。
5. 登出改用 `user2`，同一筆資料狀態應獨立（不受 `user1` 影響）。
6. 以 `admin` 登入，進入 `/AdminWorkItems`，測試新增/編輯/刪除。
7. 回到 `/WorkItems`，確認後台修改有反映到前台列表。