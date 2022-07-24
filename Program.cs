
using System.Net.Http;
using MediaDataBaseModelShare;
using System.Text.Json;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("作品を収集するよ");

// メディアデータベース
Console.WriteLine("メディアDBの分類一覧だよ");

Dictionary<MediaType, string> mediaTypePairs = new Dictionary<MediaType, string>();
mediaTypePairs.Add(MediaType.Manga, "manga");
mediaTypePairs.Add(MediaType.Animation, "animation");
mediaTypePairs.Add(MediaType.Game, "game");
mediaTypePairs.Add(MediaType.MediaArt, "mediaart");
mediaTypePairs.Add(MediaType.Collection, "collection");

HttpClient client = new HttpClient();
HttpResponseMessage response;

List<MediaRecordRow> mediaRecordRows = new List<MediaRecordRow>();


foreach (KeyValuePair<MediaType, string> pair in mediaTypePairs) {
    Console.WriteLine($"{pair.Value}のカテゴリー一覧を取得する");
    Console.WriteLine($"https://mediaarts-db.bunka.go.jp/api/field?fieldId={pair.Value}");
    response = await client.GetAsync($"https://mediaarts-db.bunka.go.jp/api/field?fieldId={pair.Value}");
    response.EnsureSuccessStatusCode();
    string fieldResponseBody = await response.Content.ReadAsStringAsync();

    FieldResponse fieldResponse = JsonSerializer.Deserialize<FieldResponse>(fieldResponseBody);

   foreach (var fieldRow in fieldResponse.Fields) {
        foreach (var category in fieldRow.FieldCategories) {
            foreach (var subCategory in category.SubCategories)
            {
                var mediaRecordRow = new MediaRecordRow();
                mediaRecordRow.FieldId = pair.Value;
                mediaRecordRow.CategoryId = category.CategoryId;
                mediaRecordRow.SubCategoryId = subCategory.SubCategoryId;
                mediaRecordRows.Add(mediaRecordRow);
            }
        }
    }

}


foreach (MediaRecordRow mediaRecord in mediaRecordRows) {
    Console.WriteLine($"{mediaRecord.FieldId}_{mediaRecord.CategoryId}_{mediaRecord.SubCategoryId}の情報を収集するよ");
    client = new HttpClient();
    response = await client.GetAsync($"https://mediaarts-db.bunka.go.jp/api/search?fieldId={mediaRecord.FieldId}&categoryId={mediaRecord.CategoryId}&subcategoryId={mediaRecord.SubCategoryId}&sort=date&limit=1000");
    // 分類の新規予約による作品制作中の場合は収集しない
    if (response.StatusCode != System.Net.HttpStatusCode.OK) {
        continue;
    }
    response.EnsureSuccessStatusCode();

    string responseBody = await response.Content.ReadAsStringAsync();

    SearchResponse searchResponse = JsonSerializer.Deserialize<SearchResponse>(responseBody);
    DisplaySearchResponse(searchResponse);

}



void DisplaySearchResponse(SearchResponse searchResponse) {

    for (int i = 0; i < searchResponse.Records.Count; i++)
    {
        string lineString = "";
        // 検索結果
        if (searchResponse.Records[i].FieldId != null)
        {
            lineString += searchResponse.Records[i].FieldId;
        }
        if (searchResponse.Records[i].CategoryId != null)
        {
            lineString += searchResponse.Records[i].CategoryId;
        }
        if (searchResponse.Records[i].SubCategoryId != null)
        {
            lineString += searchResponse.Records[i].SubCategoryId;
        }
        if (searchResponse.Records[i].AipId != null)
        {
            lineString += searchResponse.Records[i].AipId;
        }
        if (searchResponse.Records[i].FreeWord != null)
        {
            lineString += searchResponse.Records[i].FreeWord;
        }
        lineString += searchResponse.Records[i].SearchMetaData.SchemaNames[0];
        lineString += searchResponse.Records[i].SearchMetaData.RdfTypes[0];
        if (searchResponse.Records[i].SearchMetaData.Creators != null)
        {
            lineString += searchResponse.Records[i].SearchMetaData.Creators[0];
        }
        // 権利元調整などで未定の場合はスキップ
        if (searchResponse.Records[i].SearchMetaData.Brands != null)
        {
            lineString += searchResponse.Records[i].SearchMetaData.Brands[0];
        }

        // 作品と場所が紐づいていたら出力する
        if (searchResponse.Records[i].SearchMetaData.Locations != null && searchResponse.Records[i].SearchMetaData.Locations != null)
        {
            lineString += searchResponse.Records[i].SearchMetaData.Locations[0];
        }
        // 出版日が分かる場合は取得する
        if (searchResponse.Records[i].SearchMetaData.DatePublished != null && searchResponse.Records[i].SearchMetaData.DatePublished != null)
        {
            lineString += searchResponse.Records[i].SearchMetaData.DatePublished[0];
        }
        Console.WriteLine(lineString);
    }

}