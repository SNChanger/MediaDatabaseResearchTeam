
using System.Net.Http;
using MediaDataBaseModelShare;
using System.Text.Json;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("作品を収集するよ");

Console.WriteLine("アニメーションの情報を収集するよ");
HttpClient client = new HttpClient();
HttpResponseMessage response = await client.GetAsync("https://mediaarts-db.bunka.go.jp/api/search?fieldId=manga&categoryId=cm-item&subcategoryId=cm101&sort=date&limit=1000");
response.EnsureSuccessStatusCode();

string responseBody = await response.Content.ReadAsStringAsync();

SearchResponse searchResponse = JsonSerializer.Deserialize<SearchResponse>(responseBody);
DisplaySearchResponse(searchResponse);

Console.WriteLine("アニメーション関連の情報を収集するよ");

response = await client.GetAsync("https://mediaarts-db.bunka.go.jp/api/search?fieldId=animation&sort=date&limit=1000");
response.EnsureSuccessStatusCode();

responseBody = await response.Content.ReadAsStringAsync();
Console.WriteLine(responseBody);
searchResponse = JsonSerializer.Deserialize<SearchResponse>(responseBody);
DisplaySearchResponse(searchResponse);


void DisplaySearchResponse(SearchResponse searchResponse) {

    for (int i = 0; i < searchResponse.Records.Count; i++)
    {
        // 検索結果
        Console.WriteLine($"ヒット {i} 件目");
        if (searchResponse.Records[i].FieldId != null)
        {
            Console.WriteLine(searchResponse.Records[i].FieldId);
        }
        if (searchResponse.Records[i].CategoryId != null)
        {
            Console.WriteLine(searchResponse.Records[i].CategoryId);
        }
        if (searchResponse.Records[i].SubCategoryId != null)
        {
            Console.WriteLine(searchResponse.Records[i].SubCategoryId);
        }
        if (searchResponse.Records[i].AipId != null)
        {
            Console.WriteLine(searchResponse.Records[i].AipId);
        }
        if (searchResponse.Records[i].FreeWord != null)
        {
            Console.WriteLine(searchResponse.Records[i].FreeWord);
        }
        Console.WriteLine(searchResponse.Records[i].SearchMetaData.SchemaNames[0]);
        Console.WriteLine(searchResponse.Records[i].SearchMetaData.RdfTypes[0]);
        if (searchResponse.Records[i].SearchMetaData.Creators != null)
        {
            Console.WriteLine(searchResponse.Records[i].SearchMetaData.Creators[0]);
        }
        // 権利元調整などで未定の場合はスキップ
        if (searchResponse.Records[i].SearchMetaData.Brands != null)
        {
            Console.WriteLine(searchResponse.Records[i].SearchMetaData.Brands[0]);
        }

        // 作品と場所が紐づいていたら出力する
        if (searchResponse.Records[i].SearchMetaData.Locations != null && searchResponse.Records[i].SearchMetaData.Locations != null)
        {
            Console.WriteLine(searchResponse.Records[i].SearchMetaData.Locations[0]);
        }
        // 出版日が分かる場合は取得する
        if (searchResponse.Records[i].SearchMetaData.DatePublished != null && searchResponse.Records[i].SearchMetaData.DatePublished != null)
        {
            Console.WriteLine(searchResponse.Records[i].SearchMetaData.DatePublished[0]);
        }
    }

}