## Coords To Taiwanese City Country
* This is a C# ASP.NET Core Web API project, mainly in Traditional Chinese.
* Its purpose: Convert lat/long coordinates into nearest Taiwanese city and country.

## 座標轉台灣本島縣市鄉鎮市區
* 這是一個 C# ASP.NET Core Web API，主要功能是傳入經緯度，把座標資訊轉換成台灣本島的縣市、鄉鎮市區用。
* 它基於[政府資料開放平臺的 TWD97](https://data.nat.gov.tw/dataset/7441)，不依賴於其他外部 API，可用於內部自行建置服務用等情況。

## DEMO 網頁
* 您可以[在這裡試用](https://coords-to-taiwanese-city-country.teamol-developing.net/swagger/index.html)。

## 如何使用
#### 1. 先使用 `/api/auth/register` 註冊帳號。

![image](https://github.com/user-attachments/assets/d625da0f-e504-4b82-a286-b2495cf0f659)

#### 2. 使用 `/api/auth/login` 取得 access token。

![image](https://github.com/user-attachments/assets/07f3e416-d73d-4ae8-85e1-dc8a133906ca)

![image](https://github.com/user-attachments/assets/a815e1b4-4f35-47e7-90de-77cded1adb67)

>  Response 中的 `result.expiresAt` 會提示這組權杖有效到什麼時候。

#### 3. 使用 `/api/locating/coords-to-taiwanese-city-country` 即可開始轉換經緯度。

![image](https://github.com/user-attachments/assets/c7856cf4-2b5d-418a-9bec-1e0efc4e5604)

## 專案架構說明

![image](https://github.com/user-attachments/assets/59badf1e-1a9a-42c5-9262-eec67035b0a4)

## 如何建置
### Docker Compose
```yml
services:
  coords-to-tw-city:
    container_name: coords-to-tw-city
    image: teamolhuang/coords-to-tw-city:latest
    build:
      context: .
      dockerfile: Dockerfile
    volumes:
      - ./coords-to-tw-city-vol:/app/db
    ports:
      # 在 container 中 (api) 使用預設的 8080
      # 在 host 預設使用 32001 
      - "32001:8080"
    environment:
      # JWT 簽署用私鑰。建置者應在使用本 yml 時自行修改此私鑰。
      Jwt__SigningKey: "change-this-example-key-or-i-will-be-angry"
      # API 執行限制的寬容期間。預設 1 代表每次執行會保持在記錄中 1 秒
      Throttling__WindowDurationSeconds: 1
      # 在寬容期間最多可以執行幾次。預設 100 代表 100 次
      Throttling__MaxExecutionCount: 100
```

```
docker-compose up -d
```

* 建置以後，於本機連入 `https://localhost:32001/swagger/index.html` 即可存取 swagger。
* api 會在 `https://localhost:32001/api/xxx` 。

## json 回傳範例與說明
```jsonc
{
  // 通用欄位 - 如果 API 有實際回傳物件，會包裝在這個欄位中。
  "result": {
    // 各 API 的自訂回傳物件規格，可參照 Swagger 中的 schema 說明。
    "city": "基隆市",
    "country": "中山區"
  },
  
  // 通用欄位 - 表示此次商業邏輯是否成功。
  "isSuccessful": true,
  
  // 通用欄位 - 若執行失敗，此欄位會包含錯誤訊息，以供排查原因。
  "message": null
}
```
